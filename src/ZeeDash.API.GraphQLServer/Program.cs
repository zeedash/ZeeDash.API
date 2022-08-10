namespace ZeeDash.API.GraphQLServer;

using Orleans;
using ZeeDash.API.GraphQLServer.Options;

public sealed class Program {

    public static async Task<int> Main(string[] args) {
        IHost? host = null;

        try {
            host = CreateHostBuilder(args).Build();

            var clusterClient = host.Services.GetService<IClusterClient>()!;
            host.LogSiloClusterConnection();
            await clusterClient.Connect().ConfigureAwait(false);
            host.LogSiloClusterConnected();

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

    public static IHostBuilder CreateHostBuilder(string[] args) =>
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
            .ConfigureWebHost(ConfigureWebHostBuilder)
            .UseConsoleLifetime();

    private static void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) =>
        webHostBuilder
            .UseKestrel(
                (builderContext, options) => {
                    options.AddServerHeader = false;
                    options.Configure(
                        builderContext.Configuration.GetRequiredSection(nameof(ApplicationOptions.Kestrel)),
                        reloadOnChange: false);
                })
            // Used for IIS and IIS Express for in-process hosting. Use UseIISIntegration for out-of-process hosting.
            .UseIIS()
            .UseStartup<Startup>();
}
