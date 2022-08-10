namespace ZeeDash.API.Abstractions.Grains.Legacy;

using Orleans;

public interface IHelloGrain : IGrainWithGuidKey {

    ValueTask<string> SayHelloAsync(string name);
}
