namespace ZeeDash.API.Abstractions.Grains.HealthChecks;

using Orleans;

public interface ILocalHealthCheckGrain : IGrainWithGuidKey {

    ValueTask CheckAsync();
}
