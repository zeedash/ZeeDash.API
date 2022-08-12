namespace ZeeDash.API.Grains.Domains.Tenants;

using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Abstractions.Grains.Common;

public class BusinessUnitGrain : Grain<BusinessUnitState>, IBusinessUnitGrain {

    /// <inheritdoc/>
    Task<BusinessUnitId> IBusinessUnitGrain.AddBusinessUnitsAsync(BusinessUnitId businessUnitId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<DashboardId> IGrainWithDashboards.AddDashboardAsync(DashboardId dashboardId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<BusinessUnit> IBusinessUnitGrain.CreateAsync(string name, TenantId tenantId, UserId ownerId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<BusinessUnit> IBusinessUnitGrain.GetAsync() {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<List<BusinessUnitId>> IBusinessUnitGrain.GetBusinessUnitsAsync() {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<List<DashboardId>> IGrainWithDashboards.GetDashboardAsync() {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task IBusinessUnitGrain.RemoveBusinessUnitsAsync() {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task IGrainWithDashboards.RemoveDashboardAsync(DashboardId dashboardId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership.RemoveMemberAsync(UserId userId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership.SetContributorAsync(UserId userId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership.SetOwnerAsync(UserId userId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership.SetReaderAsync(UserId userId) {
        throw new NotImplementedException();
    }
}
