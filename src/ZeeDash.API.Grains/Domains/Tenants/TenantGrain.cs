namespace ZeeDash.API.Grains.Domains.Tenants;

using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains.Common;
using ZeeDash.API.Grains.Domains.AccessControl;
using ZeeDash.API.Grains.Domains.AccessControl.Events;
using ZeeDash.API.Grains.Services;

public partial class TenantGrain
    : Grain<TenantState>
    , ITenantGrain
    , IIncomingGrainCallFilter {

    #region Private Fields

    private readonly IAccessControlService accessControlService;
    private readonly IMembershipService manageableService;

    #endregion Private Fields

    #region Ctor.Dtor

    public TenantGrain(
        IAccessControlService accessControlService,
        IMembershipService manageableService) {
        this.accessControlService = accessControlService;
        this.manageableService = manageableService;
    }

    #endregion Ctor.Dtor

    #region Private Methods

    private Tenant MapStateToTenant() {
        return new Tenant {
            Id = this.State.Id,
            Name = this.State.Name,
            Type = this.State.Type,
            Owners = this.State.Members.Where(m => m.Level == AccessLevel.Owner).Select(m => m.UserId).ToList(),
            Contributors = this.State.Members.Where(m => m.Level == AccessLevel.Contributor).Select(m => m.UserId).ToList(),
            Readers = this.State.Members.Where(m => m.Level == AccessLevel.Reader).Select(m => m.UserId).ToList(),
        };
    }

    #endregion Private Methods

    #region IIncomingGrainCallFilter

    async Task IIncomingGrainCallFilter.Invoke(IIncomingGrainCallContext context) {
        var isCreated = this.State.IsCreated;
        if (!string.Equals(context.InterfaceMethod.Name, nameof(ITenantGrain.CreateAsync), StringComparison.Ordinal)) {
            if (!isCreated) {
                throw new UnauthorizedAccessException();
            }
        } else {
            if (isCreated) {
                throw new UnauthorizedAccessException();
            }
        }

        if (this.State.Id.IsEmpty) {
            this.State.Id = TenantId.Parse(this.IdentityString);
        }

        await context.Invoke();
    }

    #endregion IIncomingGrainCallFilter

    #region IGrainMembership

    /// <inheritdoc/>
    Task<Member> IGrainMembership.SetContributorAsync(UserId userId) {
        return this.SetMembershipAsync(userId, AccessLevel.Contributor);
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership.SetOwnerAsync(UserId userId) {
        return this.SetMembershipAsync(userId, AccessLevel.Owner);
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership.SetReaderAsync(UserId userId) {
        return this.SetMembershipAsync(userId, AccessLevel.Reader);
    }

    /// <inheritdoc/>
    async Task<Member> IGrainMembership.RemoveMemberAsync(UserId userId) {
        var member = this.manageableService.RemoveMember(this.State, userId);

        var membershipId = new MembershipViewId(this.State.Id);
        await this.accessControlService.RemoveMembershipAsync(membershipId, userId);
        await this.WriteStateAsync();
        await this.RefreshAccessControlViewAsync();

        return member;
    }

    /// <summary>
    /// Apply membership to a user on the tenant
    /// </summary>
    /// <param name="userId">The userId to manage</param>
    /// <param name="level">The level of the user on the tenant</param>
    /// <returns>The user as member</returns>
    private async Task<Member> SetMembershipAsync(UserId userId, AccessLevel level) {
        var member = this.manageableService.SetMember(this.State, userId, level);

        var membershipId = new MembershipViewId(this.State.Id);
        switch (level) {
            case AccessLevel.Reader:
                await this.accessControlService.AddReaderMembershipAsync(membershipId, userId, level);
                break;

            case AccessLevel.Contributor:
                await this.accessControlService.AddContributorMembershipAsync(membershipId, userId, level);
                break;

            case AccessLevel.Owner:
                await this.accessControlService.AddOwnerMembershipAsync(membershipId, userId, level);
                break;

            case AccessLevel.None:
            default:
                break;
        }

        await this.WriteStateAsync();
        await this.RefreshAccessControlViewAsync();

        await this.GetStreamProvider(StreamProviderName.Membership)
            .GetStream<OnTenantUpdate>(this.State.Id.AsGuid(), StreamName.Membership.OnTenantUpdate)
            .OnNextAsync(new OnTenantUpdate(this.State.Id, userId, level));

        return member;
    }

    private async Task RefreshAccessControlViewAsync() {
        var membershipId = new MembershipViewId(this.State.Id);
        var membership = this.GrainFactory.GetGrain<ITenantMembershipViewGrain>(membershipId.Value);
        await membership.RefreshAsync();
        await Task.CompletedTask;
    }

    #endregion IGrainMembership

    #region IGrainWithDashboards

    /// <inheritdoc/>
    Task<DashboardId> IGrainWithDashboards.AddDashboardAsync(DashboardId dashboardId) {
        return Task.FromResult(dashboardId);
    }

    /// <inheritdoc/>
    Task IGrainWithDashboards.RemoveDashboardAsync(DashboardId dashboardId) {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    Task<List<DashboardId>> IGrainWithDashboards.GetDashboardAsync() {
        return Task.FromResult(new List<DashboardId>());
    }

    #endregion IGrainWithDashboards

    #region ITenantGrain

    /// <inheritdoc/>
    Task<BusinessUnitId> ITenantGrain.AddBusinessUnitsAsync(BusinessUnitId businessUnitId) {
        this.State.BusinessUnits.Add(businessUnitId);
        return Task.FromResult(businessUnitId);
    }

    /// <inheritdoc/>
    async Task<Tenant> ITenantGrain.CreateAsync(string name, TenantTypes type, UserId ownerId) {
        // TODO : Input validation

        this.State.IsCreated = true;
        this.State.Name = name;
        this.State.Type = type;
        await this.WriteStateAsync();

        _ = await ((IGrainMembership)this).SetOwnerAsync(ownerId);

        return this.MapStateToTenant();
    }

    /// <inheritdoc/>
    Task<Tenant> ITenantGrain.GetAsync() {
        return Task.FromResult(this.MapStateToTenant());
    }

    /// <inheritdoc/>
    Task<BusinessUnitId> ITenantGrain.AddBusinessUnitsAsync(BusinessUnitId businessUnitId) {
        this.State.BusinessUnits.Add(businessUnitId);
        return Task.FromResult(businessUnitId);
    }

    /// <inheritdoc/>
    Task<List<BusinessUnitId>> ITenantGrain.GetBusinessUnitsAsync() {
        return Task.FromResult(this.State.BusinessUnits);
    }

    /// <inheritdoc/>
    Task ITenantGrain.RemoveBusinessUnitsAsync(BusinessUnitId businessUnitId) {
        this.State.BusinessUnits.Add(businessUnitId);
        return Task.CompletedTask;
    }

    #endregion ITenantGrain
}
