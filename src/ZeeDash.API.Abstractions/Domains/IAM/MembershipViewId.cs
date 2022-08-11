namespace ZeeDash.API.Abstractions.Domains.IAM;

using System;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Tenants;

/// <summary>
/// Identifier of a <see cref="MembershipView"/>
/// </summary>
public class MembershipViewId
    : Commons.Identities.Identity {

    public MembershipViewId(TenantId tenantId)
        : base(string.Format(Constants.URNs.MembershipZRN, tenantId.Value)) {
        this.IsEmpty = tenantId.IsEmpty;
        this.TenantId = tenantId;
        this.BusinessUnitId = BusinessUnitId.Empty;
        this.DashboardId = DashboardId.Empty;
    }

    public MembershipViewId(BusinessUnitId businessUnitId)
        : base(string.Format(Constants.URNs.MembershipZRN, businessUnitId.Value)) {
        this.IsEmpty = businessUnitId.IsEmpty;
        this.TenantId = businessUnitId.TenantId;
        this.BusinessUnitId = businessUnitId;
        this.DashboardId = DashboardId.Empty;
    }

    public MembershipViewId(DashboardId dashboardId)
        : base(string.Format(Constants.URNs.MembershipZRN, dashboardId.Value)) {
        this.IsEmpty = dashboardId.IsEmpty;
        this.TenantId = dashboardId.TenantId;
        this.BusinessUnitId = dashboardId.BusinessUnitId;
        this.DashboardId = dashboardId;
    }

    public static MembershipViewId Empty => new(TenantId.Empty);

    public bool IsEmpty { get; init; }
    public TenantId TenantId { get; init; }
    public BusinessUnitId BusinessUnitId { get; init; }
    public DashboardId DashboardId { get; init; }

    public static MembershipViewId Parse(string identityString) {
        throw new NotImplementedException();
    }
}
