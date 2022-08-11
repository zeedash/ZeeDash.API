namespace ZeeDash.API.Abstractions.Domains.Identity;

public class User {
    public UserId Id { get; set; } = UserId.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
