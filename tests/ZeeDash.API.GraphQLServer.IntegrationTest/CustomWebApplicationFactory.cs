namespace ZeeDash.API.GraphQLServer.IntegrationTest;

using System;
using System.Net.Http;
using ZeeDash.API.GraphQLServer.Options;
using ZeeDash.API.GraphQLServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    public CustomWebApplicationFactory()
    {
        this.ClientOptions.AllowAutoRedirect = false;
        this.ClientOptions.BaseAddress = new Uri("https://localhost");
    }

    public ApplicationOptions ApplicationOptions { get; private set; } = default!;

    public Mock<IClockService> ClockServiceMock { get; } = new Mock<IClockService>(MockBehavior.Strict);

    public void VerifyAllMocks() => Mock.VerifyAll(this.ClockServiceMock);

    protected override void ConfigureClient(HttpClient client)
    {
        using (var serviceScope = this.Services.CreateScope())
        {
            var serviceProvider = serviceScope.ServiceProvider;
            this.ApplicationOptions = serviceProvider.GetRequiredService<ApplicationOptions>();
        }

        base.ConfigureClient(client);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder
            .UseEnvironment(Constants.EnvironmentName.Test)
            .ConfigureServices(this.ConfigureServices);

    protected virtual void ConfigureServices(IServiceCollection services) =>
        services
            .AddDistributedMemoryCache()
            .AddSingleton(this.ClockServiceMock.Object);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.VerifyAllMocks();
        }

        base.Dispose(disposing);
    }
}
