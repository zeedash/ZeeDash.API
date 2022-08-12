namespace ZeeDash.API.Abstractions.Domains.Tenants;

using NUlid;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Exceptions;

/// <summary>
/// Identifier of the <see cref="BusinessUnit"/>
/// </summary>
public class BusinessUnitId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public BusinessUnitId(TenantId tenantId)
        : this(tenantId.AsUlid(), Ulid.NewUlid()) { }

    public BusinessUnitId(Ulid tenantValue, Ulid businessValue)
        : base(string.Format(URNs.BusinessUnitZRN, tenantValue, businessValue)) {
        this.IsEmpty = tenantValue == Ulid.Empty || businessValue == Ulid.Empty;
        this.TenantId = new TenantId(tenantValue);
        this.idValue = businessValue;
    }

    public static BusinessUnitId Empty => new(Ulid.Empty, Ulid.Empty);

    public bool IsEmpty { get; init; }
    public TenantId TenantId { get; init; }

    public Guid AsGuid() {
        return this.idValue.ToGuid();
    }

    public Ulid AsUlid() {
        return this.idValue;
    }

    public static BusinessUnitId Parse(string identityString) {
        var index = identityString.IndexOf(URNs.BusinessUnitTemplate, StringComparison.OrdinalIgnoreCase);
        var textSize = identityString.Length - index - URNs.TenantTemplate.Length;
        if (textSize != 26) { // 26 == Ulid.ToString().Length
            throw new ZrnFormatException(identityString, nameof(TenantId));
        }

        var tenantUlidString = identityString.AsSpan(index - 26, 26);
        if (Ulid.TryParse(tenantUlidString, out var tenantUlidValue)) {
            var businessUnitUlidString = identityString.AsSpan(index + URNs.BusinessUnitTemplate.Length);
            if (Ulid.TryParse(businessUnitUlidString, out var businessUnitUlidValue)) {
                return new BusinessUnitId(tenantUlidValue, businessUnitUlidValue);
            }
        }

        throw new ZrnFormatException(identityString, nameof(BusinessUnitId));
    }
}
