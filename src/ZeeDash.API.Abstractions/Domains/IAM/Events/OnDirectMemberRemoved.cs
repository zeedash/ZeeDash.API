namespace ZeeDash.API.Abstractions.Domains.IAM.Events;

using ZeeDash.API.Abstractions.Domains.Identity;

public class OnDirectMemberRemoved {
    public string Id { get; set; } = string.Empty;
    public UserId UserId { get; set; } = UserId.Empty;
}
