namespace ZeeDash.API.Abstractions.Domains.Tenants;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Abstractions.Grains.Common;

public interface ITenantGrain : IGrainWithStringKey, IGrainMembership, IGrainWithDashboards {

    Task<Tenant> CreateAsync(string name, TenantTypes type, UserId ownerId);

    Task<Tenant> GetAsync();

    Task<BusinessUnitId> AddBusinessUnitsAsync(BusinessUnitId businessUnitId);

    Task RemoveBusinessUnitsAsync(BusinessUnitId businessUnitId);

    Task<List<BusinessUnitId>> GetBusinessUnitsAsync();
}
