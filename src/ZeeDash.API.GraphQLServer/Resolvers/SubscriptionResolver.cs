namespace ZeeDash.API.GraphQLServer.Resolvers;

using ZeeDash.API.GraphQLServer.DataLoaders;
using ZeeDash.API.GraphQLServer.Models;
using HotChocolate;

public class SubscriptionResolver
{
    public Task<Human> OnHumanCreatedAsync(
        IHumanDataLoader humanDataLoader,
        [EventMessage] Guid id,
        CancellationToken cancellationToken) =>
        humanDataLoader.LoadAsync(id, cancellationToken);
}
