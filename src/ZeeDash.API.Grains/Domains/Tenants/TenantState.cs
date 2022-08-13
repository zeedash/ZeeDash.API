namespace ZeeDash.API.Grains.Domains.Tenants;

using Orleans;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Grains.Domains.AccessControl;

public class TenantState
    : IHaveMembers {

    /// <summary>
    /// Tenant grain identifier
    /// </summary>
    /// <remarks>
    /// Its value is based on the <see cref="Grain"/> Primary Key String ಠ_ಠ
    /// </remarks>
    public TenantId Id { get; set; } = TenantId.Empty;

    /// <summary>
    /// Name of the tenant
    /// </summary>
    /// <remarks>
    /// Represented by an organization name when <see cref="Type"/> is
    /// <see cref="TenantTypes.Corporate"/> and the email address of the owner when
    /// <see cref="Type"/> is <see cref="TenantTypes.Personal"/>
    /// </remarks>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of tenant
    /// </summary>
    public TenantTypes Type { get; set; } = TenantTypes.Personal;

    /// <summary>
    /// List of business units that belong to this tenant
    /// </summary>
    public List<BusinessUnitId> BusinessUnits { get; set; } = new();

    /// <summary>
    /// List of dashboard that belong to this tenant
    /// </summary>
    public List<DashboardId> Dashboards { get; set; } = new();

    /// <summary>
    /// List of all direct members of the tenant
    /// </summary>
    public List<Member> Members { get; set; } = new();
}
