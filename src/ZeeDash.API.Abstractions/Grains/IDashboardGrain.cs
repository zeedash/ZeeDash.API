namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Grains.Common;

public interface IDashboardGrain
    : IGrainWithStringKey
    , IGrainMembership {

    Task<Dashboard> CreateAsync(string name);

    Task<Dashboard> GetAsync();
}
