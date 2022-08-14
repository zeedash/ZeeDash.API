namespace ZeeDash.API.Grains.Domains.AccessControl.Events;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class OnTenantUpdate {

    public OnTenantUpdate(TenantId tenantId, UserId userId, AccessLevel level) {
        this.TenantId = tenantId;
        this.UserId = userId;
        this.GroupId = GroupId.Empty;
        this.Level = level;
    }

    public OnTenantUpdate(TenantId tenantId, GroupId groupId, AccessLevel level) {
        this.TenantId = tenantId;
        this.UserId = UserId.Empty;
        this.GroupId = groupId;
        this.Level = level;
    }

    public TenantId TenantId { get; init; }

    public UserId UserId { get; init; }
    public GroupId GroupId { get; init; }

    public string MemberId {
        get {
            if (this.UserId?.IsEmpty == true) {
                return this.GroupId?.Value ?? GroupId.Empty.Value;
            } else {
                return this.UserId?.Value ?? UserId.Empty.Value;
            }
        }
    }

    public AccessLevel Level { get; init; }
}
