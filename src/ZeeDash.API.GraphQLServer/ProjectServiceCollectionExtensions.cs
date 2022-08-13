namespace ZeeDash.API.GraphQLServer;

using Boxed.Mapping;
using ZeeDash.API.GraphQLServer.Mappers;
using ZeeDash.API.GraphQLServer.Models;
using ZeeDash.API.GraphQLServer.Repositories;
using ZeeDash.API.GraphQLServer.Services;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods add project services.
/// </summary>
/// <remarks>
/// AddSingleton - Only one instance is ever created and returned.
/// AddScoped - A new instance is created and returned for each request/response cycle.
/// AddTransient - A new instance is created and returned each time.
/// </remarks>
internal static class ProjectServiceCollectionExtensions
{
    public static IServiceCollection AddProjectMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IImmutableMapper<HumanInput, Human>, HumanInputToHumanMapper>();

    public static IServiceCollection AddProjectServices(this IServiceCollection services) =>
        services
            .AddSingleton<IClockService, ClockService>();

    public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        services
            .AddSingleton<IDroidRepository, DroidRepository>()
            .AddSingleton<IHumanRepository, HumanRepository>();
}
