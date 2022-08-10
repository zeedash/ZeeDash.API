namespace ZeeDash.API.Grains.Domains.Tenants;

using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Grains.Domains.AccessControl;
using Orleans.Runtime;
using ZeeDash.API.Grains.Services;

public partial class TenantGrain
    : Grain
    , ITenantGrain
    , IIncomingGrainCallFilter {

    #region Private Fields

    private readonly IPersistentState<TenantState> tenant;
    private readonly IPersistentState<Membership> membership;

    private readonly IAccessControlService accessControlService;
    private readonly IManageableService manageableService;

    #endregion Private Fields

    #region Ctor.Dtor

    public TenantGrain(
        [PersistentState("Tenant")] IPersistentState<TenantState> tenant,
        [PersistentState("TenantMembership")] IPersistentState<Membership> membership,
        IAccessControlService accessControlService,
        IManageableService manageableService) {
        this.tenant = tenant;
        this.membership = membership;
        this.accessControlService = accessControlService;
        this.manageableService = manageableService;
    }

    #endregion Ctor.Dtor

    #region Private Methods

    private Tenant MapStateToTenant() {
        return new Tenant {
            Id = new TenantId(this.IdentityString),
            Name = this.tenant.State.Name,
            Type = this.tenant.State.Type,
            Owners = this.membership.State.Members.Where(m => m.Level == AccessLevel.Owner).Select(m => m.UserId).ToList(),
            Contributors = this.membership.State.Members.Where(m => m.Level == AccessLevel.Contributor).Select(m => m.UserId).ToList(),
            Readers = this.membership.State.Members.Where(m => m.Level == AccessLevel.Reader).Select(m => m.UserId).ToList(),
        };
    }

    #endregion Private Methods

    #region IIncomingGrainCallFilter

    public override Task OnActivateAsync() {
        return base.OnActivateAsync();
    }

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods

    async Task IIncomingGrainCallFilter.Invoke(IIncomingGrainCallContext context) {
        var isCreated = this.tenant.State.IsCreated;
        if (!string.Equals(context.InterfaceMethod.Name, nameof(ITenantGrain.CreateAsync), StringComparison.Ordinal)) {
            if (!isCreated) {
                throw new UnauthorizedAccessException();
            }
        } else {
            if (isCreated) {
                throw new UnauthorizedAccessException();
            }
        }

        await context.Invoke();
    }

#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods

    #endregion IIncomingGrainCallFilter

    #region IManageable

    /// <inheritdoc/>
    Task<List<Member>> IManageableGrain.GetMembersAsync(AccessLevel? level, AccessLevelKind? kind) {
        var query = this.membership.State.Members.AsQueryable();

        if (level is not null) {
            query = query.Where(m => m.Level == level.Value);
        }

        if (kind is not null) {
            query = query.Where(m => m.Kind == kind.Value);
        }

        return Task.FromResult(query.ToList());
    }

    /// <inheritdoc/>
    async Task<Member> IManageableGrain.SetContributorAsync(UserId userId, AccessLevelKind? kind) {
        var level = AccessLevel.Contributor;
        // Note : An indirect access level is not possible on a tenant (since it's the highest element in struture tree)
        var member = this.manageableService.SetMember(this.membership.State, userId, level, AccessLevelKind.Direct);

        await this.accessControlService.AddContributorMembershipAsync(this.IdentityString, userId, level);
        await this.membership.WriteStateAsync();
        await this.RefreshAccessControlViewAsync();

        ////var setAsContributorOfBusinessUnitTasks = this.tenant.State.BusinessUnits
        ////    .Select(businessUnitId => this.GrainFactory.GetGrain<IBusinessUnitGrain>(businessUnitId.ToString()))
        ////    .Select(bu => bu.SetContributorAsync(userId, AccessLevelKind.Inherited));
        ////await Task.WhenAll(setAsContributorOfBusinessUnitTasks);

        ////var setAsContributorOfDashboardTasks = this.State.Dashboards
        ////    .Select(dashboardId => this.GrainFactory.GetGrain<IDashboardGrain>(dashboardId.ToString()))
        ////    .Select(dashboard => dashboard.SetContributorAsync(userId, AccessLevelKind.Inherited));
        ////await Task.WhenAll(setAsContributorOfDashboardTasks);

        return member;
    }

#pragma warning disable CA1822 // Marquer les membres comme étant static

    private async Task RefreshAccessControlViewAsync() {
        //var membership = this.GrainFactory.GetGrain<ITenantMembershipGrain>();
        //await membership.RefreshViewAsync();
        await Task.CompletedTask;
    }

#pragma warning restore CA1822 // Marquer les membres comme étant static

    /// <inheritdoc/>
    async Task<Member> IManageableGrain.SetOwnerAsync(UserId userId, AccessLevelKind? kind) {
        var level = AccessLevel.Owner;
        // Note : An indirect access level is not possible on a tenant (since it's the highest element in struture tree)
        var member = this.manageableService.SetMember(this.membership.State, userId, level, AccessLevelKind.Direct);

        await this.accessControlService.AddOwnerMembershipAsync(this.IdentityString, userId, level);
        await this.membership.WriteStateAsync();
        await this.RefreshAccessControlViewAsync();

        //var setAsOwnerOfBusinessUnitTasks = this.membership.State.BusinessUnits
        //    .Select(businessUnitId => this.GrainFactory.GetGrain<IBusinessUnitGrain>(businessUnitId.ToString()))
        //    .Select(bu => bu.SetOwnerAsync(userId, AccessLevelKind.Inherited));
        //await Task.WhenAll(setAsOwnerOfBusinessUnitTasks);

        //var setAsOwnerOfDashboardTasks = this.membership.State.Dashboards
        //    .Select(dashboardId => this.GrainFactory.GetGrain<IDashboardGrain>(dashboardId.ToString()))
        //    .Select(dashboard => dashboard.SetOwnerAsync(userId, AccessLevelKind.Inherited));
        //await Task.WhenAll(setAsOwnerOfDashboardTasks);

        return member;
    }

    /// <inheritdoc/>
    async Task<Member> IManageableGrain.SetReaderAsync(UserId userId, AccessLevelKind? kind) {
        var level = AccessLevel.Reader;
        // Note : An indirect access level is not possible on a tenant (since it's the highest element in struture tree)
        var member = this.manageableService.SetMember(this.membership.State, userId, level, AccessLevelKind.Direct);

        await this.accessControlService.AddReaderMembershipAsync(this.IdentityString, userId, level);
        await this.membership.WriteStateAsync();
        await this.RefreshAccessControlViewAsync();

        return member;
    }

    /// <inheritdoc/>
    async Task<Member> IManageableGrain.RemoveMemberAsync(UserId userId) {
        var member = this.manageableService.RemoveMember(this.membership.State, userId);

        await this.accessControlService.RemoveMembershipAsync(this.IdentityString, userId);
        await this.membership.WriteStateAsync();
        await this.RefreshAccessControlViewAsync();

        return member;
    }

    #endregion IManageable

    #region IGrainWithDashboards

    /// <inheritdoc/>
    Task<DashboardId> IGrainWithDashboards.AddDashboardAsync(DashboardId dashboardId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task IGrainWithDashboards.RemoveDashboardAsync(DashboardId dashboardId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    Task<List<DashboardId>> IGrainWithDashboards.GetDashboardAsync() {
        throw new NotImplementedException();
    }

    #endregion IGrainWithDashboards

    #region ITenantGrain

    /// <inheritdoc/>
    Task<BusinessUnitId> ITenantGrain.AddBusinessUnitsAsync(BusinessUnitId businessUnitId) {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    async Task<Tenant> ITenantGrain.CreateAsync(string name, TenantTypes type, UserId ownerId) {
        // Input validation

        this.tenant.State.IsCreated = true;
        this.tenant.State.Name = name;
        this.tenant.State.Type = type;
        await this.tenant.WriteStateAsync();

        await ((IManageableGrain)this).SetOwnerAsync(ownerId, AccessLevelKind.Direct);

        return this.MapStateToTenant();
    }

    /// <inheritdoc/>
    Task<Tenant> ITenantGrain.GetAsync() {
        return Task.FromResult(this.MapStateToTenant());
    }

    /// <inheritdoc/>
    Task<List<BusinessUnitId>> ITenantGrain.GetBusinessUnitsAsync() {
        return Task.FromResult(this.tenant.State.BusinessUnits);
    }

    /// <inheritdoc/>
    Task ITenantGrain.RemoveBusinessUnitsAsync() {
        throw new NotImplementedException();
    }

    #endregion ITenantGrain
}
