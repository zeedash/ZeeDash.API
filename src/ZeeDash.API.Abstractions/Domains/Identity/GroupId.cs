namespace ZeeDash.API.Abstractions.Domains.Identity;

using NUlid;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Exceptions;

public class GroupId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public GroupId()
        : this(Ulid.NewUlid()) { }

    private GroupId(Ulid value)
        : base(string.Format(URNs.GroupZRN, value.ToString())) {
        this.IsEmpty = value == Ulid.Empty;
        this.idValue = value;
    }

    public static GroupId Empty => new(Ulid.Empty);

    public bool IsEmpty { get; init; }

    public Guid AsGuid() => this.idValue.ToGuid();

    public Ulid AsUlid() => this.idValue;

    public static GroupId Parse(string identityString) {
        var index = identityString.IndexOf(URNs.GroupTemplate, StringComparison.OrdinalIgnoreCase);
        var textSize = identityString.Length - index - URNs.GroupTemplate.Length;
        if (textSize != 26) { // 26 == Ulid.ToString().Length
            throw new ZrnFormatException(identityString, nameof(GroupId));
        }

        var groupUlid = identityString.AsSpan(index + URNs.GroupTemplate.Length);
        if (Ulid.TryParse(groupUlid, out var value)) {
            return new GroupId(value);
        }

        throw new ZrnFormatException(identityString, nameof(GroupId));
    }
}
