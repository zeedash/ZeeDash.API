namespace ZeeDash.API.Abstractions.Domains.Tenants;

using ZeeDash.API.Abstractions.Domains.Identity;

/// <summary>
/// Business unit of a tenant
/// </summary>
public class BusinessUnit {

    /// <summary>
    /// Unique identifier of the <see cref="BusinessUnit"/> in the system
    /// </summary>
    public BusinessUnitId Id { get; set; } = BusinessUnitId.Empty;

    /// <summary>
    /// Unique identifier of the parent <see cref="Tenant"/>
    /// </summary>
    public TenantId TenantId { get; set; } = TenantId.Empty;

    /// <summary>
    /// Name of the <see cref="BusinessUnit"/>
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of Reader members of the tenant
    /// </summary>
    public List<UserId> Readers { get; set; } = new List<UserId>();

    /// <summary>
    /// List of Contributor members of the tenant
    /// </summary>
    public List<UserId> Contributors { get; set; } = new List<UserId>();

    /// <summary>
    /// List of Owners members of the tenant
    /// </summary>
    public List<UserId> Owners { get; set; } = new List<UserId>();
}
