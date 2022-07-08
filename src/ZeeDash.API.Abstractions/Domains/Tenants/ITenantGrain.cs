namespace ZeeDash.API.Abstractions.Domains.Tenants;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Dashboards;

public interface ITenantGrain : IGrainWithGuidKey {

    ValueTask<Tenant> GetAsync();

    ValueTask<List<BusinessUnit>> GetBusinessUnitsAsync();

    ValueTask<List<Dashboard>> GetDashboardAsync();
}
