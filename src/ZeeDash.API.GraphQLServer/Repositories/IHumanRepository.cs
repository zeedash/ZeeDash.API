namespace ZeeDash.API.GraphQLServer.Repositories;

using ZeeDash.API.GraphQLServer.Models;

public interface IHumanRepository
{
    Task<Human> AddHumanAsync(Human human, CancellationToken cancellationToken);

    Task<List<Character>> GetFriendsAsync(Human human, CancellationToken cancellationToken);

    Task<IQueryable<Human>> GetHumansAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Human>> GetHumansAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}
