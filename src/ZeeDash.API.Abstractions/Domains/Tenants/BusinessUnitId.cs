namespace ZeeDash.API.Abstractions.Domains.Tenants;

using NUlid;

/// <summary>
/// Identifier of the <see cref="BusinessUnit"/>
/// </summary>
public class BusinessUnitId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public BusinessUnitId(TenantId tenantId)
        : this(tenantId.AsUlid(), Ulid.NewUlid()) { }

    public BusinessUnitId(Ulid tenantValue, Ulid businessValue)
        : base(string.Format(Constants.URNs.BusinessUnitTemplateZRN, tenantValue, businessValue)) {
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
}
