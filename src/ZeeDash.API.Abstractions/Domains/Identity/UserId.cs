namespace ZeeDash.API.Abstractions.Domains.Identity;

using NUlid;
using ZeeDash.API.Abstractions.Constants;

public class UserId
    : Commons.Identities.Identity {

    public UserId()
        : base(string.Format(URNs.UserZRN, Ulid.NewUlid().ToString())) { }

    public UserId(string value)
        : base(value) { }

    public static UserId Empty => new();
}
