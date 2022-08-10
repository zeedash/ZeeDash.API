namespace ZeeDash.API.GraphQLServer.Resolvers;

using ZeeDash.API.GraphQLServer.DataLoaders;
using ZeeDash.API.GraphQLServer.Models;
using ZeeDash.API.GraphQLServer.Repositories;
using HotChocolate;

public class DroidResolver
{
    public Task<Droid> GetDroidAsync(IDroidDataLoader droidDataLoader, Guid id, CancellationToken cancellationToken) =>
        droidDataLoader.LoadAsync(id, cancellationToken);

    public Task<List<Character>> GetFriendsAsync(
        [Service] IDroidRepository droidRepository,
        [Parent] Droid droid,
        CancellationToken cancellationToken) =>
        droidRepository.GetFriendsAsync(droid, cancellationToken);
}
