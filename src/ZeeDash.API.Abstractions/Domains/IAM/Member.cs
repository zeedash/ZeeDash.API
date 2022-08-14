namespace ZeeDash.API.Abstractions.Domains.IAM;

using ZeeDash.API.Abstractions.Domains.Identity;

public class Member {

    /// <summary>
    /// The UserId of the member
    /// </summary>
    public UserId UserId { get; set; } = UserId.Empty;

    /// <summary>
    /// The UserId of the member
    /// </summary>
    public GroupId GroupId { get; set; } = GroupId.Empty;

    /// <summary>
    /// The member raw identifier
    /// </summary>
    public string MemberId {
        get {
            if (this.UserId?.IsEmpty == true) {
                return this.GroupId?.Value ?? GroupId.Empty.Value;
            } else {
                return this.UserId?.Value ?? UserId.Empty.Value;
            }
        }
    }

    /// <summary>
    /// The access level of the member
    /// </summary>
    public AccessLevel Level { get; set; } = AccessLevel.None;
}
