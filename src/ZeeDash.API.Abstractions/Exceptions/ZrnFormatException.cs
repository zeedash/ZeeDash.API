namespace ZeeDash.API.Abstractions.Exceptions;

using System;

public class ZrnFormatException
    : Exception {

    public ZrnFormatException(string zrn, string entityType)
        : base($"The ZRN '{zrn} 'isn't correctly formatted and can't be parsed as '{entityType}.") {
        this.Zrn = zrn;
    }

    public string Zrn { get; init; }
}
