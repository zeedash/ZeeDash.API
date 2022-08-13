namespace ZeeDash.API.Abstractions.Grains.Legacy;

using Orleans;

/// <summary>
/// Holds the total count on a given silo and then feeds this forward to the <see cref="ICounterGrain"/>.
/// </summary>
/// <remarks>Implemented using the 'Reduce' pattern (See https://github.com/OrleansContrib/DesignPatterns/blob/master/Reduce.md).</remarks>
/// <seealso cref="IGrain" />
public interface ICounterStatelessGrain : IGrainWithIntegerKey {

    ValueTask IncrementAsync();
}
