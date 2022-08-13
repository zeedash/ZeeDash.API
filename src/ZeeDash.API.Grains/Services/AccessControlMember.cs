namespace ZeeDash.API.Grains.Services;

using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.IAM;

public class AccessControlMember {

    /// <summary>
    /// The identifier of the member
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// Get if the memberId is related to a user (else, for a group of user)
    /// </summary>
    public bool IsUser => this.MemberId.Contains(URNs.UserTemplate, StringComparison.Ordinal);

    /// <summary>
    /// The access level of the member
    /// </summary>

    public AccessLevel Level { get; set; } = AccessLevel.None;

    /// <summary>
    /// The kind of access level of the member
    /// </summary>
    public AccessLevelKind Kind { get; set; } = AccessLevelKind.None;
}
