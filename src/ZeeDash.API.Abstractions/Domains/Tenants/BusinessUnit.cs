namespace ZeeDash.API.Abstractions.Domains.Tenants;

using ZeeDash.API.Abstractions.Domains.Identity;

public class BusinessUnit {
    public BusinessUnitId Id { get; set; } = BusinessUnitId.Empty;
    public string Name { get; set; } = string.Empty;
    public List<User> Owners { get; set; } = new List<User>();
}
