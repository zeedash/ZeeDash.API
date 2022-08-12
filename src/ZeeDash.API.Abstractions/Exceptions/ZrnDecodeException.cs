namespace ZeeDash.API.Abstractions.Exceptions;

using System;

public class ZrnDecodeException
    : Exception {

    public ZrnDecodeException(string zrn)
        : base($"The ZRN '{zrn} 'isn't correctly formatted. It can't be decoded.") {
        this.Zrn = zrn;
    }

    public string Zrn { get; init; }
}
