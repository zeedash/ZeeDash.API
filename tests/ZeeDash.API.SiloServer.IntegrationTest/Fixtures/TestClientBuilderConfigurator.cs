namespace ZeeDash.API.SiloServer.IntegrationTest.Fixtures;

using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using Orleans.TestingHost;
using ZeeDash.API.Abstractions.Constants;

public class TestClientBuilderConfigurator : IClientBuilderConfigurator {

    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder) =>
        clientBuilder.AddSimpleMessageStreamProvider(StreamProviderName.Default);
}
