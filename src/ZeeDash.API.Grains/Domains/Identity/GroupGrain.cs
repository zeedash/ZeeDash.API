namespace ZeeDash.API.Grains.Domains.Identity;

using System;
using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Abstractions.Grains.Common;
using ZeeDash.API.Grains.Domains.AccessControl;
using ZeeDash.API.Grains.Domains.AccessControl.Events;
using ZeeDash.API.Grains.Services;

public partial class GroupGrain
    : Grain<GroupState>
    , IGroupGrain
    , IIncomingGrainCallFilter {

    #region Private Fields

    private readonly IAccessControlService accessControlService;
    private readonly IMembershipService membershipService;

    #endregion Private Fields

    #region Ctor.Dtor

    public GroupGrain(
        IAccessControlService accessControlService,
        IMembershipService membershipService) {
        this.accessControlService = accessControlService;
        this.membershipService = membershipService;
    }

    #endregion Ctor.Dtor

    #region Private Methods

    private Group MapStateToGroup() {
        return new Group {
            Id = this.State.Id,
            TenantId = this.State.Id.TenantId,
            Name = this.State.Name
        };
    }

    #endregion Private Methods

    #region IIncomingGrainCallFilter

    async Task IIncomingGrainCallFilter.Invoke(IIncomingGrainCallContext context) {
        var isCreated = !this.State.Id.IsEmpty;
        if (!string.Equals(context.InterfaceMethod.Name, nameof(IGroupGrain.CreateAsync), StringComparison.Ordinal)) {
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

    #region IGroupGrain

    /// <inheritdoc/>
    async Task<Group> IGroupGrain.CreateAsync(string name) {
        // TODO : Input validation

        this.State.Id = GroupId.Parse(this.GetPrimaryKeyString());
        this.State.Name = name;
        await this.WriteStateAsync();

        await this.accessControlService.CreateMemberAsync(this.State.Id);
        await this.accessControlService.AddBelongshipAsync(this.State.Id.TenantId, this.State.Id);

        return this.MapStateToGroup();
    }

    /// <inheritdoc/>
    Task<Group> IGroupGrain.GetAsync() {
        return Task.FromResult(this.MapStateToGroup());
    }

    #endregion IGroupGrain

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
            .GetStream<OnGroupUpdate>(this.State.Id.AsGuid(), StreamName.Membership.OnGroupUpdate)
            .OnNextAsync(new OnGroupUpdate(this.State.Id, userId, level));

        return member;
    }

    #endregion IGrainMembership<UserId>
}
