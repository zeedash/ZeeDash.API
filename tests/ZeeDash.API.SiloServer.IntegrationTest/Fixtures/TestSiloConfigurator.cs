namespace ZeeDash.API.SiloServer.IntegrationTest.Fixtures;

using Orleans.Hosting;
using Orleans.TestingHost;
using ZeeDash.API.Abstractions.Constants;

public class TestSiloConfigurator : ISiloConfigurator {

    public void Configure(ISiloBuilder siloBuilder) =>
        siloBuilder
            .AddMemoryGrainStorageAsDefault()
            .AddMemoryGrainStorage("PubSubStore")
            .AddSimpleMessageStreamProvider(StreamProviderName.Default);
}
