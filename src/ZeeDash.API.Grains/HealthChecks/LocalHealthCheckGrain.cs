namespace ZeeDash.API.Grains;

using Orleans;
using Orleans.Concurrency;
using ZeeDash.API.Abstractions.Grains.HealthChecks;

[StatelessWorker(1)]
public class LocalHealthCheckGrain : Grain, ILocalHealthCheckGrain {

    public ValueTask CheckAsync() => ValueTask.CompletedTask;
}
