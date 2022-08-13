namespace ZeeDash.API.Abstractions.Domains.IAM;

using ZeeDash.API.Abstractions.Domains.Identity;

public class Membership {

    /// <summary>
    /// The User represented by the membership
    /// </summary>
    public User User { get; set; } = new();

    /// <summary>
    /// Get if the current membership relate to a user
    /// </summary>
    public bool IsUser => this.User?.Id?.IsEmpty ?? false;

    /// <summary>
    /// The Group of users represented by the membership
    /// </summary>
    public Group Group { get; set; } = new();

    /// <summary>
    /// Get if the current membership relate to a group of users
    /// </summary>
    public bool IsGroup => this.Group?.Id?.IsEmpty ?? false;

    /// <summary>
    /// The access level of the member
    /// </summary>
    public AccessLevel Level { get; set; } = AccessLevel.None;

    /// <summary>
    /// The access level of the member
    /// </summary>
    public AccessLevelKind Kind { get; set; } = AccessLevelKind.None;
}
