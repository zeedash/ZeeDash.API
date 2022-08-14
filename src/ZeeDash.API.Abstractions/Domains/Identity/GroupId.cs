namespace ZeeDash.API.Abstractions.Domains.Identity;

using NUlid;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Exceptions;

public class GroupId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public GroupId(TenantId tenantId)
        : this(tenantId.AsUlid(), Ulid.NewUlid()) { }

    private GroupId(Ulid tenantValue, Ulid value)
        : base(string.Format(URNs.GroupZRN, tenantValue.ToString(), value.ToString())) {
        this.IsEmpty = value == Ulid.Empty;
        this.TenantId = new TenantId(tenantValue);
        this.idValue = value;
    }

    public static GroupId Empty => new(Ulid.Empty, Ulid.Empty);

    public bool IsEmpty { get; init; }
    public TenantId TenantId { get; init; }

    public Guid AsGuid() => this.idValue.ToGuid();

    public Ulid AsUlid() => this.idValue;

    public static GroupId Parse(string identityString) {
        var index = identityString.IndexOf(URNs.GroupTemplate, StringComparison.OrdinalIgnoreCase);
        var textSize = identityString.Length - index - URNs.GroupTemplate.Length;
        if (textSize != 26) { // 26 == Ulid.ToString().Length
            throw new ZrnFormatException(identityString, nameof(GroupId));
        }

        var groupUlid = identityString.AsSpan(index + URNs.GroupTemplate.Length);
        if (Ulid.TryParse(groupUlid, out var groupUlidValue)) {
            var tenantIndex = identityString.IndexOf(URNs.TenantTemplate, StringComparison.OrdinalIgnoreCase);
            var tenantUlidString = identityString.AsSpan(tenantIndex + URNs.TenantTemplate.Length, 26);
            if (Ulid.TryParse(tenantUlidString, out var tenantUlidValue)) {
                return new GroupId(tenantUlidValue, groupUlidValue);
            }
        }

        throw new ZrnFormatException(identityString, nameof(GroupId));
    }
}
