namespace ZeeDash.API.Abstractions.Domains.Identity;

using ZeeDash.API.Abstractions.Domains.Dashboards;

public class User {
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Dashboard> Dashboards { get; set; } = new List<Dashboard>();
}
