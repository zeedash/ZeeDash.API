namespace ZeeDash.API.Grains.Domains.AccessControl.Events;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class OnTenantUpdate {

    public OnTenantUpdate(TenantId tenantId, UserId userId, AccessLevel level) {
        this.TenantId = tenantId;
        this.UserId = userId;
        this.Level = level;
    }

    public TenantId TenantId { get; init; }

    public UserId UserId { get; init; }

    public AccessLevel Level { get; init; }
}
