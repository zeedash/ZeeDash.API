namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;

public interface IBusinessUnitGrain : IGrainWithStringKey, IManageableGrain, IGrainWithDashboards {

    Task<BusinessUnit> CreateAsync(string name, TenantId tenantId, UserId ownerId);

    Task<BusinessUnit> GetAsync();

    Task<BusinessUnitId> AddBusinessUnitsAsync(BusinessUnitId businessUnitId);

    Task RemoveBusinessUnitsAsync();

    Task<List<BusinessUnitId>> GetBusinessUnitsAsync();
}
