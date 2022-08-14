namespace ZeeDash.API.Client;

using Bogus;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Xunit;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Abstractions.Grains.Legacy;

public static class Program {

    public static async Task<int> Main() {
        try {
            var tee = new TenantId();
            var bee = new BusinessUnitId(tee);
            var dee = new DashboardId(bee);
            var doo = DashboardId.Parse(dee.Value);

            var clusterClient = CreateClientBuilder().Build();
            await clusterClient.Connect().ConfigureAwait(false);

            // Set a trace ID, so that requests can be identified.
            RequestContext.Set("TraceId", Guid.NewGuid());

            var faker = new Faker();

            var ownerId = new UserId();
            var contributorsIds = new UserId[3] {
                new UserId(), new UserId(), new UserId()
            };
            var readerId = new UserId();


            var owner = clusterClient.GetGrain<IUserGrain>(ownerId.Value);
            await owner.CreateAsync(faker.Person.FullName, faker.Person.Email);

            var tenantId = new TenantId();
            var tenantGrain = clusterClient.GetGrain<ITenantGrain>(tenantId.Value);
            await tenantGrain.CreateAsync(faker.Company.CompanyName(), TenantTypes.Corporate, ownerId);
            var tenant = await tenantGrain.GetAsync();

            var tenantMembershipId = new MembershipId(tenantId);
            var tenantMembership = clusterClient.GetGrain<IMembershipGrain>(tenantMembershipId.Value);
            var tenantMembers = await tenantMembership.GetMembersAsync();
            Assert.Equal(1, tenantMembers.Count);

            var contributors = contributorsIds.Select(id => clusterClient.GetGrain<IUserGrain>(id.Value));
            foreach (var contributor in contributors) {
                await contributor.CreateAsync(faker.Person.FullName, faker.Person.Email);
            }

            var reader = clusterClient.GetGrain<IUserGrain>(readerId.Value);
            await reader.CreateAsync(faker.Person.FullName, faker.Person.Email);

            var groupId = new GroupId(tenantId);
            var group = clusterClient.GetGrain<IGroupGrain>(groupId.Value);
            await group.CreateAsync("Contributors");
            foreach (var contributorId in contributorsIds) {
                await group.SetReaderAsync(contributorId);
            }

            var businessUnitId = new BusinessUnitId(tenantId);
            var businessUnitGrain = clusterClient.GetGrain<IBusinessUnitGrain>(businessUnitId.Value);
            var businessUnit = await businessUnitGrain.CreateAsync(faker.Commerce.ProductName());
            await businessUnitGrain.SetContributorAsync(groupId);

            var businessUnitMembershipId = new MembershipId(businessUnitId);
            var businessUnitMembership = clusterClient.GetGrain<IMembershipGrain>(businessUnitMembershipId.Value);
            var businessUnitMembers = await businessUnitMembership.GetMembersAsync();
            Assert.Equal(2, businessUnitMembers.Count);

            var tenantDashboardId = new DashboardId(tenantId);
            var tenantDashboardGrain = clusterClient.GetGrain<IDashboardGrain>(tenantDashboardId.Value);
            var tenantDashboard = await tenantDashboardGrain.CreateAsync(faker.Commerce.ProductName());
            await tenantDashboardGrain.SetReaderAsync(readerId);

            var tenantDashboardMembershipId = new MembershipId(tenantDashboardId);
            var tenantDashboardMembership = clusterClient.GetGrain<IMembershipGrain>(tenantDashboardMembershipId.Value);
            var tenantDashboardMembers = await tenantDashboardMembership.GetMembersAsync();
            Assert.Equal(2, tenantDashboardMembers.Count);

            var businessUnitDashboardId = new DashboardId(businessUnitId);
            var businessUnitDashboardGrain = clusterClient.GetGrain<IDashboardGrain>(businessUnitDashboardId.Value);
            var businessUnitDashboard = await businessUnitDashboardGrain.CreateAsync(faker.Commerce.ProductName());
            await businessUnitDashboardGrain.SetReaderAsync(readerId);

            var businessUnitDashboardMembershipId = new MembershipId(businessUnitDashboardId);
            var businessUnitDashboardMembership = clusterClient.GetGrain<IMembershipGrain>(businessUnitDashboardMembershipId.Value);
            var businessUnitDashboardMembers = await businessUnitDashboardMembership.GetMembersAsync();
            Assert.Equal(3, businessUnitDashboardMembers.Count);

            //var reminderGrain = clusterClient.GetGrain<IReminderGrain>(Guid.Empty);
            //await reminderGrain.SetReminderAsync("Don't forget to say hello!").ConfigureAwait(false);

            //var streamProvider = clusterClient.GetStreamProvider(StreamProviderName.Default);
            //var saidHelloStream = streamProvider.GetStream<string>(Guid.Empty, StreamName.SaidHello);
            //var saidHelloSubscription = await saidHelloStream.SubscribeAsync(OnSaidHelloAsync).ConfigureAwait(false);
            //var reminderStream = streamProvider.GetStream<string>(Guid.Empty, StreamName.Reminder);
            //var reminderSubscription = await reminderStream.SubscribeAsync(OnReminderAsync).ConfigureAwait(false);

            //Console.WriteLine("What is your name?");
            //var name = Console.ReadLine() ?? "Rehan";

            //var exit = false;
            //while (!exit) {
            //    var helloGrain = clusterClient.GetGrain<IHelloGrain>(Guid.NewGuid());
            //    Console.WriteLine(await helloGrain.SayHelloAsync(name).ConfigureAwait(false));
            //}

            //await saidHelloSubscription.UnsubscribeAsync().ConfigureAwait(false);
            //await reminderSubscription.UnsubscribeAsync().ConfigureAwait(false);
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

    //private static Task OnSaidHelloAsync(string name, StreamSequenceToken token) {
    //    Console.WriteLine($"{name} said hello.");
    //    return Task.CompletedTask;
    //}

    //private static Task OnReminderAsync(string reminder, StreamSequenceToken token) {
    //    Console.WriteLine(reminder);
    //    return Task.CompletedTask;
    //}

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
            .AddSimpleMessageStreamProvider(StreamProviderName.Membership);
}
