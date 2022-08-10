namespace ZeeDash.API.Grains.Domains.Tenants;

using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class TenantState {

    /// <summary>
    /// Détermine si le modèle est correctement créé
    /// </summary>
    public bool IsCreated { get; set; }

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
}
