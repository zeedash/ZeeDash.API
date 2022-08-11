namespace ZeeDash.API.Grains.Domains.AccessControl.Events;

using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class OnDashboardUpdate {

    public OnDashboardUpdate(TenantId tenantId, DashboardId dashboardId, UserId userId, AccessLevel level)
         : this(tenantId, null, dashboardId, userId, level) { }

    public OnDashboardUpdate(TenantId tenantId, BusinessUnitId? businessUnitId, DashboardId dashboardId, UserId userId, AccessLevel level) {
        this.TenantId = tenantId;
        this.BusinessUnitId = businessUnitId;
        this.DashboardId = dashboardId;
        this.UserId = userId;
        this.Level = level;
    }

    public TenantId TenantId { get; init; }
    public BusinessUnitId? BusinessUnitId { get; init; }
    public DashboardId DashboardId { get; init; }

    public UserId UserId { get; init; }

    public AccessLevel Level { get; init; }
}
