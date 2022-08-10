namespace ZeeDash.API.SiloServer;

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Statistics;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Grains;
using ZeeDash.API.SiloServer.Options;

#pragma warning disable RCS1102 // Make class static.

public class Program
#pragma warning restore RCS1102 // Make class static.
{
    public static async Task<int> Main(string[] args) {
        IHost? host = null;

        try {
            var hostBuilder = CreateHostBuilder(args);
            host = hostBuilder.Build();

            host.LogApplicationStarted();
            await host.RunAsync().ConfigureAwait(false);
            host.LogApplicationStopped();

            return 0;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            host!.LogApplicationTerminatedUnexpectedly(exception);

            return 1;
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        new HostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureHostConfiguration(
                configurationBuilder => configurationBuilder.AddCustomBootstrapConfiguration(args))
            .ConfigureAppConfiguration(
                (hostingContext, configurationBuilder) => {
                    hostingContext.HostingEnvironment.ApplicationName = AssemblyInformation.Current.Product;
                    configurationBuilder.AddCustomConfiguration(hostingContext.HostingEnvironment, args);
                })
            .UseDefaultServiceProvider(
                (context, options) => {
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })
            .UseOrleans(ConfigureSiloBuilder)
            .ConfigureWebHost(ConfigureWebHostBuilder)
            .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
            .UseConsoleLifetime();

    private static void ConfigureSiloBuilder(
        Microsoft.Extensions.Hosting.HostBuilderContext context,
        ISiloBuilder siloBuilder) =>
        siloBuilder
            .ConfigureServices(
                (context, services) => {
                    services.Configure<ApplicationOptions>(context.Configuration);
                    services.Configure<ClusterOptions>(context.Configuration.GetSection(nameof(ApplicationOptions.Cluster)));
                    services.Configure<StorageOptions>(context.Configuration.GetSection(nameof(ApplicationOptions.Storage)));
                })
            .UseSiloUnobservedExceptionsHandler()
            .UseRedisClustering(opt => {
                opt.ConnectionString = GetClusteringOptions(context.Configuration).ConnectionString;
                opt.Database = 0;
            })
            //.UseAzureStorageClustering(
            //    options => options.ConfigureTableServiceClient(GetStorageOptions(context.Configuration).ConnectionString))
            .ConfigureEndpoints(
                GetFreeSiloPort(EndpointOptions.DEFAULT_SILO_PORT),
                EndpointOptions.DEFAULT_GATEWAY_PORT,
                listenOnAnyHostAddress: true) //!context.HostingEnvironment.IsDevelopment())
            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
            .AddAdoNetGrainStorageAsDefault(
                options => {
                    options.Invariant = "Npgsql";
                    options.ConnectionString = GetStorageOptions(context.Configuration).ConnectionString;
                    options.ConfigureJsonSerializerSettings = ConfigureJsonSerializerSettings;
                    options.UseJsonFormat = true;
                })
            //.AddAzureTableGrainStorageAsDefault(
            //    options =>
            //    {
            //        options.ConfigureTableServiceClient(GetStorageOptions(context.Configuration).ConnectionString);
            //        options.ConfigureJsonSerializerSettings = ConfigureJsonSerializerSettings;
            //        options.UseJson = true;
            //    })
            .UseAdoNetReminderService(
                options => {
                    options.ConnectionString = GetStorageOptions(context.Configuration).ConnectionString;
                    options.Invariant = "Npgsql";
                })
            //.UseAzureTableReminderService(
            //    options => options.ConfigureTableServiceClient(GetStorageOptions(context.Configuration).ConnectionString))
            //.UseTransactions(withStatisticsReporter: true)
            ////.AddAzureTableTransactionalStateStorageAsDefault(
            ////    options => options.ConfigureTableServiceClient(GetStorageOptions(context.Configuration).ConnectionString))
            //.AddAdoNetTransactionalStateStorageAsDefault(cfg => {
            //    cfg.DbConnector = DbConnectors.PostgreSQL;
            //    cfg.ConnectionString = GetStorageOptions(context.Configuration).ConnectionString;
            //})
            .AddSimpleMessageStreamProvider(StreamProviderName.Default)
            //.AddAzureTableGrainStorage(
            //    "PubSubStore",
            //    options =>
            //    {
            //        options.ConfigureTableServiceClient(GetStorageOptions(context.Configuration).ConnectionString);
            //        options.ConfigureJsonSerializerSettings = ConfigureJsonSerializerSettings;
            //        options.UseJson = true;
            //    })
            .AddAdoNetGrainStorage(
                "PubSubStore",
                options => {
                    options.Invariant = "Npgsql";
                    options.ConnectionString = GetStorageOptions(context.Configuration).ConnectionString;
                    options.ConfigureJsonSerializerSettings = ConfigureJsonSerializerSettings;
                    options.UseJsonFormat = true;
                })
            .UseIf(
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
                x => x.UseLinuxEnvironmentStatistics())
            .UseIf(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                x => x.UsePerfCounterEnvironmentStatistics())
            .UseDashboard();

    private static int GetFreeSiloPort(int port) {
        var ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
        var current = port;

        while (true) {
            try {
                var tcpListener = new TcpListener(ipAddress, 666);
                tcpListener.Start();
                tcpListener.Stop();
                break;
            } catch { current++; }
        }

        return current;
    }

    private static void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) =>
        webHostBuilder
            .UseKestrel(
                (builderContext, options) => {
                    options.AddServerHeader = false;
                    options.Configure(
                        builderContext.Configuration.GetSection(nameof(ApplicationOptions.Kestrel)),
                        reloadOnChange: false);
                })
            .UseStartup<Startup>();

    private static void ConfigureJsonSerializerSettings(JsonSerializerSettings jsonSerializerSettings) {
        jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        jsonSerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
    }

    private static StorageOptions GetStorageOptions(IConfiguration configuration) =>
        configuration.GetSection(nameof(ApplicationOptions.Storage)).Get<StorageOptions>();

    private static ClusteringOptions GetClusteringOptions(IConfiguration configuration) =>
        configuration.GetSection(nameof(ApplicationOptions.Clustering)).Get<ClusteringOptions>();
}
