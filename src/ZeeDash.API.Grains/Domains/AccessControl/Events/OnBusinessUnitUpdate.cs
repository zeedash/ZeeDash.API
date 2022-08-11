namespace ZeeDash.API.Grains.Domains.AccessControl.Events;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class OnBusinessUnitUpdate {

    public OnBusinessUnitUpdate(TenantId tenantId, BusinessUnitId businessUnitId, UserId userId, AccessLevel level) {
        this.TenantId = tenantId;
        this.BusinessUnitId = businessUnitId;
        this.UserId = userId;
        this.Level = level;
    }

    public TenantId TenantId { get; init; }
    public BusinessUnitId BusinessUnitId { get; init; }

    public UserId UserId { get; init; }

    public AccessLevel Level { get; init; }
}
