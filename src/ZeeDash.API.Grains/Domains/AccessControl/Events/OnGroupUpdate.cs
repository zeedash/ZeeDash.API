namespace ZeeDash.API.Grains.Domains.AccessControl.Events;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class OnGroupUpdate {

    public OnGroupUpdate(GroupId groupId, UserId userId, AccessLevel level) {
        this.TenantId = groupId.TenantId;
        this.UserId = userId;
        this.GroupId = groupId;
        this.Level = level;
    }

    public TenantId TenantId { get; init; }
    public GroupId GroupId { get; init; }
    public UserId UserId { get; init; }

    public string MemberId => this.UserId?.Value ?? UserId.Empty.Value;

    public AccessLevel Level { get; init; }
}
