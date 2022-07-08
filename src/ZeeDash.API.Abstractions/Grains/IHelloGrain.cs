namespace ZeeDash.API.Abstractions.Grains;

using Orleans;

public interface IHelloGrain : IGrainWithGuidKey {

    ValueTask<string> SayHelloAsync(string name);
}
