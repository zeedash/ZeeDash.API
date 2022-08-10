namespace ZeeDash.API.SiloServer.IntegrationTest;

using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using ZeeDash.API.Abstractions.Grains.Legacy;
using ZeeDash.API.SiloServer.IntegrationTest.Fixtures;

public class CounterStatelessGrainTest : ClusterFixture {

    public CounterStatelessGrainTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper) {
    }

    [Fact]
    public async Task Increment_Default_EventuallyIncrementsTotalCountAsync() {
        var grain = this.Cluster.GrainFactory.GetGrain<ICounterStatelessGrain>(0L);
        var counterGrain = this.Cluster.GrainFactory.GetGrain<ICounterGrain>(Guid.Empty);

        await grain.IncrementAsync().ConfigureAwait(false);
        var countBefore = await counterGrain.GetCountAsync().ConfigureAwait(false);

        Assert.Equal(0L, countBefore);

        await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);

        var countAfter = await counterGrain.GetCountAsync().ConfigureAwait(false);

        Assert.Equal(1L, countAfter);
    }
}
