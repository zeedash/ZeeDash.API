namespace ZeeDash.API.Grains.Domains.Tenants;

using Orleans;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Grains.Domains.AccessControl;

public class DashboardState
    : IHaveMembers {

    /// <summary>
    /// Dashboard grain identifier
    /// </summary>
    /// <remarks>
    /// Its value is based on the <see cref="Grain"/> Primary Key String ಠ_ಠ
    /// </remarks>
    public DashboardId Id { get; set; } = DashboardId.Empty;

    /// <summary>
    /// Name of the dashboard
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of all direct members of the dashboard
    /// </summary>
    public List<Member> Members { get; set; } = new();
}
