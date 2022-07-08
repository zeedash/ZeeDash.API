namespace ZeeDash.API.Grains.Domains.Tenants;

using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Tenants;

public class TenantGrain : Grain<TenantState>, ITenantGrain {

    public ValueTask<Tenant> GetAsync() {
        return new ValueTask<Tenant>(new Tenant { Name = "Emilien" });
    }

    public ValueTask<List<BusinessUnit>> GetBusinessUnitsAsync() {
        throw new NotImplementedException();
    }

    public ValueTask<List<Dashboard>> GetDashboardAsync() {
        throw new NotImplementedException();
    }
}
