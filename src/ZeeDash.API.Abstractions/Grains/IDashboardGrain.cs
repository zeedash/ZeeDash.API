namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains.Common;

public interface IDashboardGrain : IGrainWithStringKey, IGrainMembership {

    Task<Dashboard> CreateAsync(string name, TenantId tenantId, UserId ownerId, BusinessUnitId? businessUnitId);

    Task<Dashboard> GetAsync();
}
