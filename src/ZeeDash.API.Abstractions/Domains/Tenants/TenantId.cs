namespace ZeeDash.API.Abstractions.Domains.Tenants;

using NUlid;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Exceptions;

/// <summary>
/// Identifier of the <see cref="Tenant"/>
/// </summary>
public class TenantId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public TenantId()
        : this(Ulid.NewUlid()) { }

    public TenantId(Ulid value)
        : base(string.Format(URNs.TenantZRN, value)) {
        this.IsEmpty = value == Ulid.Empty;
        this.idValue = value;
    }

    public bool IsEmpty { get; init; }

    public Guid AsGuid() => this.idValue.ToGuid();

    public Ulid AsUlid() => this.idValue;

    public static TenantId Empty => new(Ulid.Empty);

    public static TenantId Parse(string identityString) {
        var index = identityString.IndexOf(URNs.TenantTemplate, StringComparison.OrdinalIgnoreCase);
        var textSize = identityString.Length - index - URNs.TenantTemplate.Length;
        if (textSize != 26) { // 26 == Ulid.ToString().Length
            throw new ZrnFormatException(identityString, nameof(TenantId));
        }

        var tenantUlid = identityString.AsSpan(index + URNs.TenantTemplate.Length);
        if (Ulid.TryParse(tenantUlid, out var value)) {
            return new TenantId(value);
        }

        throw new ZrnFormatException(identityString, nameof(TenantId));
    }
}
