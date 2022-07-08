namespace ZeeDash.API.Abstractions.Domains.Identity;

public class User {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
