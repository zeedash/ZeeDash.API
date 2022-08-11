namespace ZeeDash.API.Abstractions.Domains.Identity;

using NUlid;
using ZeeDash.API.Abstractions.Constants;

public class UserId
    : Commons.Identities.Identity {

    public UserId()
        : this(Ulid.NewUlid()) { }

    public UserId(string value)
        : base(value) { }

    private UserId(Ulid userId)
        : base(string.Format(URNs.UserZRN, userId.ToString())) { }

    public static UserId Empty => new();
}
