namespace ZeeDash.API.Abstractions.Domains.AccessControl;

using ZeeDash.API.Abstractions.Domains.Identity;

public class Member {

    /// <summary>
    /// The UserId of the member
    /// </summary>
    public UserId UserId { get; set; } = UserId.Empty;

    /// <summary>
    /// The access level of the member
    /// </summary>
    public AccessLevel Level { get; set; } = AccessLevel.None;

    /// <summary>
    /// The access level of the member
    /// </summary>
    public AccessLevelKind Kind { get; set; } = AccessLevelKind.None;
}
