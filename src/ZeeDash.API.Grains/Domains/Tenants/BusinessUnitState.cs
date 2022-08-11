namespace ZeeDash.API.Grains.Domains.Tenants;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Grains.Domains.AccessControl;

public class BusinessUnitState
    : IMembershipState {

    /// <summary>
    /// Name of the business unit
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the parent tenant
    /// </summary>
    public TenantId Tenant { get; set; } = TenantId.Empty;

    /// <summary>
    /// List of dashboard that belong to this business units
    /// </summary>
    public List<DashboardId> Dashboards { get; set; } = new();

    /// <summary>
    /// List of all direct members of the business unit
    /// </summary>
    public List<Member> Members { get; set; } = new();
}
