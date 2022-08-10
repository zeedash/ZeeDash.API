namespace ZeeDash.API.Abstractions.Grains.Legacy;

using Orleans;

public interface IReminderGrain : IGrainWithGuidKey {

    ValueTask SetReminderAsync(string reminder);
}
