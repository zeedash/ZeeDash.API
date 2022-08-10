namespace ZeeDash.API.Client;

using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Streams;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains.Legacy;

public static class Program {

    public static async Task<int> Main() {
        try {
            var clusterClient = CreateClientBuilder().Build();
            await clusterClient.Connect().ConfigureAwait(false);

            // Set a trace ID, so that requests can be identified.
            RequestContext.Set("TraceId", Guid.NewGuid());

            var tenantId = new TenantId();// "zrn:zeedash:tenant/01G88YKQZVA87R0HKNC8J3ZDR0");
            var tenantGrain = clusterClient.GetGrain<ITenantGrain>(tenantId.Value);
            var readerId = new UserId("zrn:zeedash:user/01G88YV82PHHDBGTR4T00MNB2H");
            var ownerId = new UserId("zrn:zeedash:user/01G8TJJ5F6FSD2TXZQAWN2TBMB");
            await tenantGrain.CreateAsync("Salut ma poule !", TenantTypes.Personal, ownerId);
            await tenantGrain.SetReaderAsync(readerId);

            var tenants = Enumerable.Range(0, 100)
                .Select(i => new TenantId())
                .ToDictionary(
                    k => k.Value,
                    v => Enumerable.Range(0, 100)
                            .Select(i => new {
                                UserId = new UserId(),
                                Level = i % 20 == 0 ? AccessLevel.Owner : AccessLevel.Contributor
                            })
                );

            var sw = new Stopwatch();
            sw.Start();
            var tasks = tenants.SelectMany(tKvp => {
                var tenantGrain = clusterClient.GetGrain<ITenantGrain>(tKvp.Key);
                return tKvp.Value.Select(v => v.Level == AccessLevel.Owner ? tenantGrain.SetOwnerAsync(v.UserId) : tenantGrain.SetContributorAsync(v.UserId));
            }).ToList();

            //var evenTasks = new Task[100 * 100];
            //for (var k = 0; k < tenants.Keys.Count; k++) {
            //    var client = tenants[tenants.Keys.ElementAt(k)];
            //    for (var i = 0; i < 100; i++) {
            //        evenTasks[k + (i * 100)] = tenantGrain.SetOwnerAsync(new UserId());
            //    }
            //}

            try {
                await Task.WhenAll(tasks);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
#pragma warning disable IDE0120 // Simplifier l'expression LINQ
                var successfull = tasks.Where(t => t.IsCompletedSuccessfully).Count();
                var completed = tasks.Where(t => t.IsCompleted).Count();
                var faulted = tasks.Where(t => t.IsFaulted).Count();
                var canceled = tasks.Where(t => t.IsCanceled).Count();
#pragma warning restore IDE0120 // Simplifier l'expression LINQ
                Console.WriteLine($"{successfull} / {completed} / {faulted} / {canceled}");
            } finally { sw.Stop(); }

            Console.WriteLine(sw.Elapsed.ToString());

            var tenants2 = Enumerable.Range(0, 1000)
                .Select(i => new TenantId())
                .ToDictionary(
                    k => k,
                    v => Enumerable.Range(0, 10)
                            .Select(i => new {
                                UserId = new UserId(),
                                Level = i % 9 == 0 ? AccessLevel.Owner : AccessLevel.Contributor
                            })
                );

            sw.Reset();
            sw.Start();
            var tasks2 = tenants2.SelectMany(tKvp => {
                var tenantGrain = clusterClient.GetGrain<ITenantGrain>(tKvp.Key.Value);
                return tKvp.Value.Select(v => v.Level == AccessLevel.Owner ? tenantGrain.SetOwnerAsync(v.UserId) : tenantGrain.SetContributorAsync(v.UserId));
            }).ToArray();
            try {
                await Task.WhenAll(tasks2);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
#pragma warning disable IDE0120 // Simplifier l'expression LINQ
                var successfull = tasks2.Where(t => t.IsCompletedSuccessfully).Count();
                var completed = tasks2.Where(t => t.IsCompleted).Count();
                var faulted = tasks2.Where(t => t.IsFaulted).Count();
                var canceled = tasks2.Where(t => t.IsCanceled).Count();
#pragma warning restore IDE0120 // Simplifier l'expression LINQ
                Console.WriteLine($"{successfull} / {completed} / {faulted} / {canceled}");
            } finally { sw.Stop(); }

            Console.WriteLine(sw.Elapsed.ToString());

            var reminderGrain = clusterClient.GetGrain<IReminderGrain>(Guid.Empty);
            await reminderGrain.SetReminderAsync("Don't forget to say hello!").ConfigureAwait(false);

            var streamProvider = clusterClient.GetStreamProvider(StreamProviderName.Default);
            var saidHelloStream = streamProvider.GetStream<string>(Guid.Empty, StreamName.SaidHello);
            var saidHelloSubscription = await saidHelloStream.SubscribeAsync(OnSaidHelloAsync).ConfigureAwait(false);
            var reminderStream = streamProvider.GetStream<string>(Guid.Empty, StreamName.Reminder);
            var reminderSubscription = await reminderStream.SubscribeAsync(OnReminderAsync).ConfigureAwait(false);

#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.WriteLine("What is your name?");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            var name = Console.ReadLine() ?? "Rehan";

            var exit = false;
            while (!exit) {
                var helloGrain = clusterClient.GetGrain<IHelloGrain>(Guid.NewGuid());
                Console.WriteLine(await helloGrain.SayHelloAsync(name).ConfigureAwait(false));
            }

            await saidHelloSubscription.UnsubscribeAsync().ConfigureAwait(false);
            await reminderSubscription.UnsubscribeAsync().ConfigureAwait(false);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            Console.WriteLine(exception.ToString());
            return -1;
        }

        return 0;
    }

    private static Task OnSaidHelloAsync(string name, StreamSequenceToken token) {
        Console.WriteLine($"{name} said hello.");
        return Task.CompletedTask;
    }

    private static Task OnReminderAsync(string reminder, StreamSequenceToken token) {
        Console.WriteLine(reminder);
        return Task.CompletedTask;
    }

    private static IClientBuilder CreateClientBuilder() =>
        new ClientBuilder()
            .UseRedisClustering(opt => {
                opt.ConnectionString = "localhost:6379";
                opt.Database = 0;
            })
            .Configure<ClusterOptions>(
                options => {
                    options.ClusterId = Cluster.ClusterId;
                    options.ServiceId = Cluster.ServiceId;
                })
            .ConfigureApplicationParts(
                parts => parts
                    .AddApplicationPart(typeof(ICounterGrain).Assembly)
                    .WithReferences())
            .AddSimpleMessageStreamProvider(StreamProviderName.Default);
}
