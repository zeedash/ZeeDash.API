namespace ZeeDash.API.Grains.Domains.Tenants;

using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Abstractions.Grains.Common;
using ZeeDash.API.Grains.Domains.AccessControl;
using ZeeDash.API.Grains.Domains.AccessControl.Events;
using ZeeDash.API.Grains.Services;

public class DashboardGrain
    : Grain<DashboardState>
    , IDashboardGrain
    , IIncomingGrainCallFilter {

    #region Private Fields

    private readonly IAccessControlService accessControlService;
    private readonly IMembershipService membershipService;

    #endregion Private Fields

    #region Ctor.Dtor

    public DashboardGrain(
        IAccessControlService accessControlService,
        IMembershipService membershipService) {
        this.accessControlService = accessControlService;
        this.membershipService = membershipService;
    }

    #endregion Ctor.Dtor

    #region Private Methods

    private Dashboard MapStateToTenant() {
        return new Dashboard {
            Id = this.State.Id,
            Name = this.State.Name
        };
    }

    #endregion Private Methods

    #region IIncomingGrainCallFilter

    async Task IIncomingGrainCallFilter.Invoke(IIncomingGrainCallContext context) {
        var isCreated = !this.State.Id.IsEmpty;
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

    #endregion IIncomingGrainCallFilter

    #region IDashboardGrain

    /// <inheritdoc/>
    async Task<Dashboard> IDashboardGrain.CreateAsync(string name) {
        // TODO : Input validation

        this.State.Id = DashboardId.Parse(this.GetPrimaryKeyString());
        this.State.Name = name;
        await this.WriteStateAsync();

        await this.accessControlService.CreateDashboardAsync(this.State.Id);

        return this.MapStateToTenant();
    }

    /// <inheritdoc/>
    Task<Dashboard> IDashboardGrain.GetAsync() {
        return Task.FromResult(this.MapStateToTenant());
    }

    #endregion IDashboardGrain

    #region IGrainMembership<UserId>

    /// <inheritdoc/>
    Task<Member> IGrainMembership<UserId>.SetContributorAsync(UserId userId) {
        return this.SetMembershipAsync(userId, AccessLevel.Contributor);
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership<UserId>.SetOwnerAsync(UserId userId) {
        return this.SetMembershipAsync(userId, AccessLevel.Owner);
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership<UserId>.SetReaderAsync(UserId userId) {
        return this.SetMembershipAsync(userId, AccessLevel.Reader);
    }

    /// <inheritdoc/>
    async Task<Member> IGrainMembership<UserId>.RemoveMemberAsync(UserId userId) {
        var member = this.membershipService.RemoveMember(this.State, userId);

        var membershipId = new MembershipId(this.State.Id);
        await this.accessControlService.RemoveMembershipAsync(membershipId, userId);
        await this.WriteStateAsync();

        return member;
    }

    /// <summary>
    /// Apply membership to a user on the tenant
    /// </summary>
    /// <param name="userId">The userId to manage</param>
    /// <param name="level">The level of the user on the tenant</param>
    /// <returns>The user as member</returns>
    private async Task<Member> SetMembershipAsync(UserId userId, AccessLevel level) {
        var membershipId = new MembershipId(this.State.Id);

        var member = this.membershipService.SetMember(this.State, userId, level);
        await this.WriteStateAsync();

        await this.accessControlService.AddMembershipAsync(membershipId, userId, level);

        await this.GetStreamProvider(StreamProviderName.Membership)
            .GetStream<OnDashboardUpdate>(this.State.Id.AsGuid(), StreamName.Membership.OnDashboardUpdate)
            .OnNextAsync(new OnDashboardUpdate(this.State.Id, userId, level));

        return member;
    }

    #endregion IGrainMembership<UserId>

    #region IGrainMembership<GroupId>

    /// <inheritdoc/>
    Task<Member> IGrainMembership<GroupId>.SetContributorAsync(GroupId groupId) {
        return this.SetMembershipAsync(groupId, AccessLevel.Contributor);
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership<GroupId>.SetOwnerAsync(GroupId groupId) {
        return this.SetMembershipAsync(groupId, AccessLevel.Owner);
    }

    /// <inheritdoc/>
    Task<Member> IGrainMembership<GroupId>.SetReaderAsync(GroupId groupId) {
        return this.SetMembershipAsync(groupId, AccessLevel.Reader);
    }

    /// <inheritdoc/>
    async Task<Member> IGrainMembership<GroupId>.RemoveMemberAsync(GroupId groupId) {
        var member = this.membershipService.RemoveMember(this.State, groupId);

        var membershipId = new MembershipId(this.State.Id);
        await this.accessControlService.RemoveMembershipAsync(membershipId, groupId);
        await this.WriteStateAsync();

        return member;
    }

    /// <summary>
    /// Apply membership to a Group on the tenant
    /// </summary>
    /// <param name="groupId">The GroupId to manage</param>
    /// <param name="level">The level of the Group on the tenant</param>
    /// <returns>The Group as member</returns>
    private async Task<Member> SetMembershipAsync(GroupId groupId, AccessLevel level) {
        var membershipId = new MembershipId(this.State.Id);

        var member = this.membershipService.SetMember(this.State, groupId, level);
        await this.WriteStateAsync();

        await this.accessControlService.AddMembershipAsync(membershipId, groupId, level);

        await this.GetStreamProvider(StreamProviderName.Membership)
            .GetStream<OnDashboardUpdate>(this.State.Id.AsGuid(), StreamName.Membership.OnDashboardUpdate)
            .OnNextAsync(new OnDashboardUpdate(this.State.Id, groupId, level));

        return member;
    }

    #endregion IGrainMembership<GroupId>
}
