namespace ZeeDash.API.GraphQLServer;

using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Grains.Legacy;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods add project services.
/// </summary>
/// <remarks>
/// AddSingleton - Only one instance is ever created and returned.
/// AddScoped - A new instance is created and returned for each request/response cycle.
/// AddTransient - A new instance is created and returned each time.
/// </remarks>
internal static class OrleansServiceCollectionExtensions {

    public static IServiceCollection AddOrleansClusterClient(this IServiceCollection services, IConfiguration configuration) {
        var builder = new ClientBuilder();

        builder.UseRedisClustering(opt => {
            opt.ConnectionString = configuration.GetSection("Redis").GetValue<string>("ConnectionString");
            opt.Database = configuration.GetSection("Redis").GetValue<int>("Database");
        });
        var clusterOptions = new Action<ClusterOptions>(options => {
            options.ClusterId = Cluster.ClusterId;
            options.ServiceId = Cluster.ServiceId;
        });

        builder.Configure(clusterOptions);
        builder.ConfigureApplicationParts(
                parts => parts
                    .AddApplicationPart(typeof(ICounterGrain).Assembly)
                    .WithReferences());
        builder.AddSimpleMessageStreamProvider(StreamProviderName.Default);

        var client = builder.Build();

        return services
            .AddSingleton(client)
            .AddSingleton(sp => {
                var options = new ClusterOptions();
                clusterOptions(options);
                return options;
            });
    }
}
