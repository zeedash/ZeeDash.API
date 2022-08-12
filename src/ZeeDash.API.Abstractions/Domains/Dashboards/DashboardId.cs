namespace ZeeDash.API.Abstractions.Domains.Dashboards;

using NUlid;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Exceptions;

public class DashboardId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public DashboardId(TenantId tenantId)
        : this(tenantId.AsUlid(), Ulid.Empty, Ulid.NewUlid()) { }

    public DashboardId(Ulid tenantValue, Ulid dashboardValue)
        : this(tenantValue, Ulid.Empty, dashboardValue) { }

    public DashboardId(Ulid tenantValue, Ulid businessUnitValue, Ulid dashboardValue)
        : base(string.Format(URNs.DashboardZRN, tenantValue, dashboardValue)) {
        this.IsEmpty = tenantValue == Ulid.Empty || dashboardValue == Ulid.Empty;
        this.TenantId = new TenantId(tenantValue);
        this.BusinessUnitId = new BusinessUnitId(tenantValue, businessUnitValue);
        this.idValue = dashboardValue;
    }

    public static DashboardId Empty => new(Ulid.Empty, Ulid.Empty, Ulid.Empty);

    public bool IsEmpty { get; init; }

    public TenantId TenantId { get; init; }
    public BusinessUnitId BusinessUnitId { get; init; }

    public Guid AsGuid() {
        return this.idValue.ToGuid();
    }

    public Ulid AsUlid() {
        return this.idValue;
    }

    public static DashboardId Parse(string identityString) {
        var dashboardIdIndex = identityString.IndexOf(URNs.DashboardTemplate, StringComparison.OrdinalIgnoreCase);
        var textSize = identityString.Length - dashboardIdIndex - URNs.TenantTemplate.Length;
        if (textSize != 26) { // 26 == Ulid.ToString().Length
            throw new ZrnFormatException(identityString, nameof(TenantId));
        }

        var tenantIdIndex = identityString.IndexOf(URNs.TenantTemplate, StringComparison.OrdinalIgnoreCase);
        var tenantUlidString = identityString.AsSpan(tenantIdIndex + URNs.TenantTemplate.Length, 26);
        if (Ulid.TryParse(tenantUlidString, out var tenantUlidValue)) {
            var businessUnitUlidValue = Ulid.Empty;
            var businessUnitIdIndex = identityString.IndexOf(URNs.BusinessUnitTemplate, StringComparison.OrdinalIgnoreCase);
            if (businessUnitIdIndex > -1) {
                var businessUnitUlidString = identityString.AsSpan(businessUnitIdIndex + URNs.BusinessUnitTemplate.Length, 26);
                _ = Ulid.TryParse(businessUnitUlidString, out businessUnitUlidValue);
            }

            var dashboardUlidString = identityString.AsSpan(dashboardIdIndex + URNs.DashboardTemplate.Length);
            if (Ulid.TryParse(dashboardUlidString, out var dashboardUlidValue)) {
                return new DashboardId(tenantUlidValue, businessUnitUlidValue, dashboardUlidValue);
            }
        }

        throw new ZrnFormatException(identityString, nameof(BusinessUnitId));
    }
}
