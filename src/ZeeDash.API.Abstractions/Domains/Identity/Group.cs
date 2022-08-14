namespace ZeeDash.API.Abstractions.Domains.Identity;

using ZeeDash.API.Abstractions.Domains.Tenants;

/// <summary>
/// A group of users
/// </summary>
public class Group {

    /// <summary>
    /// Identifier of the group
    /// </summary>
    public GroupId Id { get; set; } = GroupId.Empty;

    /// <summary>
    /// Unique identifier of the parent <see cref="Tenant"/>
    /// </summary>
    public TenantId TenantId { get; set; } = TenantId.Empty;

    /// <summary>
    /// Name of the group
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
