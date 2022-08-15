namespace ZeeDash.API.IdentityServer;

using ZeeDash.API.IdentityServer.ConfigureOptions;
using ZeeDash.API.IdentityServer.Constants;
using ZeeDash.API.IdentityServer.Options;
using Boxed.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods which extend ASP.NET Core services.
/// </summary>
internal static class CustomServiceCollectionExtensions {

    /// <summary>
    /// Configures the settings by binding the contents of the appsettings.json file to the specified Plain Old CLR
    /// Objects (POCO) and adding <see cref="IOptions{T}"/> objects to the services collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The services with caching services added.</returns>
    public static IServiceCollection AddCustomOptions(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            // ConfigureAndValidateSingleton registers IOptions<T> and also T as a singleton to the services collection.
            .ConfigureAndValidateSingleton<ApplicationOptions>(configuration)
            .ConfigureAndValidateSingleton<CacheProfileOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.CacheProfiles)))
            .ConfigureAndValidateSingleton<CompressionOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Compression)))
            .ConfigureAndValidateSingleton<ForwardedHeadersOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.ForwardedHeaders)))
            .Configure<ForwardedHeadersOptions>(
                options => {
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                })
            .ConfigureAndValidateSingleton<RedisOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Redis)))
            .ConfigureAndValidateSingleton<HostOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Host)))
            .ConfigureAndValidateSingleton<KestrelServerOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Kestrel)));

    public static IServiceCollection AddCustomConfigureOptions(this IServiceCollection services) =>
        services
            .ConfigureOptions<ConfigureAuthorizationOptions>()
            .ConfigureOptions<ConfigureCorsOptions>()
            .ConfigureOptions<ConfigureRedisCacheOptions>()
            .ConfigureOptions<ConfigureResponseCompressionOptions>()
            .ConfigureOptions<ConfigureRouteOptions>()
            .ConfigureOptions<ConfigureStaticFileOptions>();

    public static IServiceCollection AddCustomHealthChecks(
        this IServiceCollection services,
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration) =>
        services
            .AddHealthChecks()
            // Add health checks for external dependencies here. See https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
            .AddIf(
                !webHostEnvironment.IsEnvironment(EnvironmentName.Test),
                x => x.AddRedis(configuration.GetRequiredSection(nameof(ApplicationOptions.Redis)).Get<RedisOptions>().ConfigurationOptions.ToString()))
            .Services;

    public static IServiceCollection AddCustomRedis(
        this IServiceCollection services,
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration) =>
        services.AddIf(
            !webHostEnvironment.IsEnvironment(EnvironmentName.Test),
            x => x.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(
                     configuration
                        .GetRequiredSection(nameof(ApplicationOptions.Redis))
                        .Get<RedisOptions>()
                        .ConfigurationOptions)));
}
