namespace ZeeDash.API.Abstractions.Domains.IAM;

using ZeeDash.API.Abstractions.Domains.Identity;

public class Membership {

    /// <summary>
    /// The User represented by the membership
    /// </summary>
    public User User { get; set; } = new();

    /// <summary>
    /// The access level of the member
    /// </summary>
    public AccessLevel Level { get; set; } = AccessLevel.None;

    /// <summary>
    /// The access level of the member
    /// </summary>
    public AccessLevelKind Kind { get; set; } = AccessLevelKind.None;
}
