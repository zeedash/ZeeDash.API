namespace ZeeDash.API.GraphQLServer.Resolvers;

using Boxed.Mapping;
using ZeeDash.API.GraphQLServer.Models;
using ZeeDash.API.GraphQLServer.Repositories;
using HotChocolate;
using HotChocolate.Subscriptions;

public class MutationResolver
{
    public async Task<Human> CreateHumanAsync(
        [Service] IImmutableMapper<HumanInput, Human> humanInputToHumanMapper,
        [Service] IHumanRepository humanRepository,
        [Service] ITopicEventSender topicEventSender,
        HumanInput humanInput,
        CancellationToken cancellationToken)
    {
        var human = humanInputToHumanMapper.Map(humanInput);
        human = await humanRepository
            .AddHumanAsync(human, cancellationToken)
            .ConfigureAwait(false);
        await topicEventSender
            .SendAsync(nameof(SubscriptionResolver.OnHumanCreatedAsync), human.Id, CancellationToken.None)
            .ConfigureAwait(false);
        return human;
    }
}
