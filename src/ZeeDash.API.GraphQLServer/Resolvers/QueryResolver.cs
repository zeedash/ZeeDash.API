namespace ZeeDash.API.GraphQLServer.Resolvers;

using ZeeDash.API.GraphQLServer.DataLoaders;
using ZeeDash.API.GraphQLServer.Models;
using ZeeDash.API.GraphQLServer.Repositories;
using HotChocolate;

public class QueryResolver
{
    public Task<IQueryable<Droid>> GetDroidsAsync(
        [Service] IDroidRepository droidRepository,
        CancellationToken cancellationToken) =>
        droidRepository.GetDroidsAsync(cancellationToken);

    public Task<IReadOnlyList<Droid>> GetDroidByIdsAsync(
        IDroidDataLoader droidDataLoader,
        List<Guid> ids,
        CancellationToken cancellationToken) =>
        droidDataLoader.LoadAsync(ids, cancellationToken);

    public Task<IQueryable<Human>> GetHumansAsync(
        [Service] IHumanRepository humanRepository,
        CancellationToken cancellationToken) =>
        humanRepository.GetHumansAsync(cancellationToken);

    public Task<IReadOnlyList<Human>> GetHumansByIdsAsync(
        IHumanDataLoader humanDataLoader,
        List<Guid> ids,
        CancellationToken cancellationToken) =>
        humanDataLoader.LoadAsync(ids, cancellationToken);
}
