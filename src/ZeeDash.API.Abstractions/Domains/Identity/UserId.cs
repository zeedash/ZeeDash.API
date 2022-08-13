namespace ZeeDash.API.Abstractions.Domains.Identity;

using NUlid;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Exceptions;

public class UserId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public UserId()
        : this(Ulid.NewUlid()) { }

    private UserId(Ulid value)
        : base(string.Format(URNs.UserZRN, value.ToString())) {
        this.IsEmpty = value == Ulid.Empty;
        this.idValue = value;
    }

    public static UserId Empty => new(Ulid.Empty);

    public bool IsEmpty { get; init; }

    public Guid AsGuid() => this.idValue.ToGuid();

    public Ulid AsUlid() => this.idValue;

    public static UserId Parse(string identityString) {
        var index = identityString.IndexOf(URNs.UserTemplate, StringComparison.OrdinalIgnoreCase);
        var textSize = identityString.Length - index - URNs.UserTemplate.Length;
        if (textSize != 26) { // 26 == Ulid.ToString().Length
            throw new ZrnFormatException(identityString, nameof(UserId));
        }

        var userUlid = identityString.AsSpan(index + URNs.UserTemplate.Length);
        if (Ulid.TryParse(userUlid, out var value)) {
            return new UserId(value);
        }

        throw new ZrnFormatException(identityString, nameof(UserId));
    }
}
