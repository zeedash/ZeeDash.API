namespace ZeeDash.API.Abstractions.Domains.Tenants;

using ZeeDash.API.Abstractions.Domains.Identity;

public class Tenant {
    public TenantId Id { get; set; } = TenantId.Empty;
    public string Name { get; set; } = string.Empty;
    public TenantTypes Type { get; set; } = TenantTypes.Personal;
    public List<User> Owners { get; set; } = new List<User>();
}
