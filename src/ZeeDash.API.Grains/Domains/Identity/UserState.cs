namespace ZeeDash.API.Grains.Domains.Identity;

using ZeeDash.API.Abstractions.Domains.Identity;

public class UserState {

    /// <summary>
    /// User grain identifier
    /// </summary>
    /// <remarks>
    /// Its value is based on the <see cref="Grain"/> Primary Key String ಠ_ಠ
    /// </remarks>
    public UserId Id { get; set; } = UserId.Empty;

    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email of the user
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
