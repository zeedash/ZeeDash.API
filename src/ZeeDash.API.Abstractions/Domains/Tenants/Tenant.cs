namespace ZeeDash.API.Abstractions.Domains.Tenants;

using ZeeDash.API.Abstractions.Domains.Identity;

/// <summary>
/// Represent an organization in the system
/// </summary>
public class Tenant {

    /// <summary>
    /// Unique identifier of the <see cref="Tenant"/> in the system
    /// </summary>
    public TenantId Id { get; set; } = TenantId.Empty;

    /// <summary>
    /// Name of the <see cref="Tenant"/>
    /// </summary>
    /// <remarks>
    /// Represented by an organization name when <see cref="Type"/> is
    /// <see cref="TenantTypes.Corporate"/> and the email address of the owner when
    /// <see cref="Type"/> is <see cref="TenantTypes.Personal"/>
    /// </remarks>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of <see cref="Tenant"/>
    /// </summary>
    public TenantTypes Type { get; set; } = TenantTypes.Personal;

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
