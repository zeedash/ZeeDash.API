namespace ZeeDash.API.Abstractions.Grains;

using System.Collections.Generic;
using System.Threading.Tasks;
using ZeeDash.API.Abstractions.Domains.Dashboards;

/// <summary>
/// Methods for managing dashboards of the grain
/// </summary>
public interface IGrainWithDashboards {

    Task<DashboardId> AddDashboardAsync(DashboardId dashboardId);

    Task RemoveDashboardAsync(DashboardId dashboardId);

    Task<List<DashboardId>> GetDashboardAsync();
}
