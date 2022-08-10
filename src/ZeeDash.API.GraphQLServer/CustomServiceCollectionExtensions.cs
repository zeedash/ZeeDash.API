namespace ZeeDash.API.GraphQLServer;

using Boxed.AspNetCore;
using ZeeDash.API.GraphQLServer.ConfigureOptions;
using ZeeDash.API.GraphQLServer.Constants;
using ZeeDash.API.GraphQLServer.Directives;
using ZeeDash.API.GraphQLServer.Options;
using ZeeDash.API.GraphQLServer.Types;
using HotChocolate.Execution.Options;
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
            .ConfigureAndValidateSingleton<GraphQLOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.GraphQL)))
            .ConfigureAndValidateSingleton<RequestExecutorOptions>(
                configuration.GetRequiredSection(nameof(ApplicationOptions.GraphQL)).GetRequiredSection(nameof(GraphQLOptions.Request)))
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

    public static IServiceCollection AddCustomGraphQL(
        this IServiceCollection services,
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration) {
        var graphQLOptions = configuration.GetRequiredSection(nameof(ApplicationOptions.GraphQL)).Get<GraphQLOptions>();
        return services
            .AddGraphQLServer()
            .AddInstrumentation()
            .InitializeOnStartup()
            .AddFiltering()
            .AddSorting()
            .AddGlobalObjectIdentification()
            .AddQueryFieldToMutationPayloads()
            .AddApolloTracing()
            .AddAuthorization()
            .UseAutomaticPersistedQueryPipeline()
            .AddIfElse(
                webHostEnvironment.IsEnvironment(EnvironmentName.Test),
                x => x.AddInMemoryQueryStorage(),
                x => x.AddRedisQueryStorage())
            .AddIfElse(
                webHostEnvironment.IsEnvironment(EnvironmentName.Test),
                x => x.AddInMemorySubscriptions(),
                x => x.AddRedisSubscriptions())
            .SetSchema<MainSchema>()
            .AddDirectiveType<UpperDirectiveType>()
            .AddDirectiveType<LowerDirectiveType>()
            .AddDirectiveType<IncludeDirectiveType>()
            .AddDirectiveType<SkipDirectiveType>()
            .AddGraphQLServerTypes()
            .TrimTypes()
            .ModifyOptions(options => options.UseXmlDocumentation = false)
            .AddMaxExecutionDepthRule(graphQLOptions.MaxAllowedExecutionDepth)
            .SetPagingOptions(graphQLOptions.Paging)
            .SetRequestOptions(() => graphQLOptions.Request)
            .Services;
    }
}
