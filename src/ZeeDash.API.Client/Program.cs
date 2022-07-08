namespace ZeeDash.API.Client;

using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Streams;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Grains;

public static class Program {

    public static async Task<int> Main() {
        try {
            var clusterClient = CreateClientBuilder().Build();
            await clusterClient.Connect().ConfigureAwait(false);

            // Set a trace ID, so that requests can be identified.
            RequestContext.Set("TraceId", Guid.NewGuid());

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
