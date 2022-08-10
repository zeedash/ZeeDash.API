namespace ZeeDash.API.GraphQLServer.Repositories;

using ZeeDash.API.GraphQLServer.Models;

public interface IDroidRepository
{
    Task<IQueryable<Droid>> GetDroidsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Droid>> GetDroidsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<List<Character>> GetFriendsAsync(Droid droid, CancellationToken cancellationToken);
}
