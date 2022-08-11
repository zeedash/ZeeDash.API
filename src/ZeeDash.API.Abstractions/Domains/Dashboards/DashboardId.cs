namespace ZeeDash.API.Abstractions.Domains.Dashboards;

using NUlid;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class DashboardId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public DashboardId(TenantId tenantId)
        : this(tenantId.AsUlid(), Ulid.Empty, Ulid.NewUlid()) { }

    public DashboardId(Ulid tenantValue, Ulid dashboardValue)
        : this(tenantValue, Ulid.Empty, dashboardValue) { }

    public DashboardId(Ulid tenantValue, Ulid businessUnitValue, Ulid dashboardValue)
        : base(string.Format(Constants.URNs.DashboardZRN, tenantValue, dashboardValue)) {
        this.IsEmpty = tenantValue == Ulid.Empty || dashboardValue == Ulid.Empty;
        this.TenantId = new TenantId(tenantValue);
        this.BusinessUnitId = new BusinessUnitId(tenantValue, businessUnitValue);
        this.idValue = dashboardValue;
    }

    public static DashboardId Empty => new(Ulid.Empty, Ulid.Empty, Ulid.Empty);

    public bool IsEmpty { get; init; }

    public TenantId TenantId { get; init; }
    public BusinessUnitId BusinessUnitId { get; init; }

    public Guid AsGuid() => this.idValue.ToGuid();

    public Ulid AsUlid() => this.idValue;
}
