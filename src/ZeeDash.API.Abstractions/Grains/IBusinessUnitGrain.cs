namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains.Common;

public interface IBusinessUnitGrain
    : IGrainWithStringKey
    , IGrainMembership
    , IGrainWithDashboards {

    Task<BusinessUnit> CreateAsync(string name);

    Task<BusinessUnit> GetAsync();
}
