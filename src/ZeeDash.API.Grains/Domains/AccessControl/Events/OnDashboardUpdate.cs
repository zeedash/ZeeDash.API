namespace ZeeDash.API.Grains.Domains.AccessControl.Events;

using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class OnDashboardUpdate {

    public OnDashboardUpdate(DashboardId dashboardId, UserId userId, AccessLevel level) {
        this.TenantId = dashboardId.TenantId;
        this.BusinessUnitId = dashboardId.BusinessUnitId;
        this.DashboardId = dashboardId;
        this.UserId = userId;
        this.GroupId = GroupId.Empty;
        this.Level = level;
    }

    public OnDashboardUpdate(DashboardId dashboardId, GroupId groupId, AccessLevel level) {
        this.TenantId = dashboardId.TenantId;
        this.BusinessUnitId = dashboardId.BusinessUnitId;
        this.DashboardId = dashboardId;
        this.UserId = UserId.Empty;
        this.GroupId = groupId;
        this.Level = level;
    }

    public TenantId TenantId { get; init; }
    public BusinessUnitId BusinessUnitId { get; init; }
    public DashboardId DashboardId { get; init; }

    public UserId UserId { get; init; }
    public GroupId GroupId { get; init; }

    public AccessLevel Level { get; init; }
}
