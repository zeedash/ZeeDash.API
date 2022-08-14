namespace ZeeDash.API.Grains.Domains.Tenants;

using Orleans;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Grains.Domains.AccessControl;

public class BusinessUnitState
    : IHaveMembers {

    /// <summary>
    /// Tenant grain identifier
    /// </summary>
    /// <remarks>
    /// Its value is based on the <see cref="Grain"/> Primary Key String ಠ_ಠ
    /// </remarks>
    public BusinessUnitId Id { get; set; } = BusinessUnitId.Empty;

    /// <summary>
    /// Name of the business unit
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of dashboard that belong to this business units
    /// </summary>
    public List<DashboardId> Dashboards { get; set; } = new();

    /// <summary>
    /// List of all direct members of the business unit
    /// </summary>
    public List<Member> Members { get; set; } = new();
}
