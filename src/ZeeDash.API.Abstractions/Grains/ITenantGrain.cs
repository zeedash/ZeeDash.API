namespace ZeeDash.API.Abstractions.Domains.Tenants;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Grains;

public interface ITenantGrain : IGrainWithStringKey, IManageableGrain, IGrainWithDashboards {

    Task<Tenant> CreateAsync(string name, TenantTypes type, UserId ownerId);

    Task<Tenant> GetAsync();

    Task<BusinessUnitId> AddBusinessUnitsAsync(BusinessUnitId businessUnitId);

    Task RemoveBusinessUnitsAsync();

    Task<List<BusinessUnitId>> GetBusinessUnitsAsync();
}
