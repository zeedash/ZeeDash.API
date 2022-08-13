namespace ZeeDash.API.Grains.Domains.AccessControl;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Grains.Domains.AccessControl.Events;
using ZeeDash.API.Grains.Services;

public partial class MembershipGrain
    : Grain<MembershipViewState>
    , IMembershipGrain {

    #region Private Fields

    private readonly ILogger<MembershipGrain> logger;
    private readonly IAccessControlService accessControlService;
    private StreamSubscriptionHandle<OnTenantUpdate>? tenantUpdateSubscription;
    private StreamSubscriptionHandle<OnBusinessUnitUpdate>? businessUnitUpdateSubscription;
    private StreamSubscriptionHandle<OnDashboardUpdate>? dashboardUpdateSubscription;

    #endregion Private Fields

    #region Ctor.Dtor

    public MembershipGrain(
        ILogger<MembershipGrain> logger,
        IAccessControlService accessControlService) {
        this.logger = logger;
        this.accessControlService = accessControlService;
    }

    #endregion Ctor.Dtor

    #region Grain

    public override async Task OnActivateAsync() {
        if (this.State.Id.IsEmpty) {
            this.State.Id = MembershipId.Parse(this.GetPrimaryKeyString());
        }

        await ((IMembershipGrain)this).RefreshAsync();

        // Subscribe to parents activities
        var streamProvider = this.GetStreamProvider(StreamProviderName.Membership);
        this.tenantUpdateSubscription = await streamProvider
            .GetStream<OnTenantUpdate>(this.State.Id.TenantId.AsGuid(), StreamName.Membership.OnTenantUpdate)
            .SubscribeAsync(this.OnTenantUpdateAsync);

        if (!this.State.Id.BusinessUnitId.IsEmpty) {
            this.businessUnitUpdateSubscription = await streamProvider
                .GetStream<OnBusinessUnitUpdate>(this.State.Id.BusinessUnitId.AsGuid(), StreamName.Membership.OnBusinessUnitUpdate)
                .SubscribeAsync(this.OnBusinessUnitUpdateAsync);
        }

        if (!this.State.Id.DashboardId.IsEmpty) {
            this.dashboardUpdateSubscription = await streamProvider
                .GetStream<OnDashboardUpdate>(this.State.Id.DashboardId.AsGuid(), StreamName.Membership.OnDashboardUpdate)
                .SubscribeAsync(this.OnDashboardUpdateAsync);
        }

        await base.OnActivateAsync();
    }

    public override async Task OnDeactivateAsync() {
        // Unsubscribe from all active subscription
        if (this.tenantUpdateSubscription != null) {
            await this.tenantUpdateSubscription.UnsubscribeAsync().ConfigureAwait(false);
        }
        if (this.businessUnitUpdateSubscription != null) {
            await this.businessUnitUpdateSubscription.UnsubscribeAsync().ConfigureAwait(false);
        }
        if (this.dashboardUpdateSubscription != null) {
            await this.dashboardUpdateSubscription.UnsubscribeAsync().ConfigureAwait(false);
        }

        await base.OnDeactivateAsync();
    }

    #endregion Grain

    #region Subscription Handlers

    private async Task OnTenantUpdateAsync(OnTenantUpdate onUpdate, StreamSequenceToken token) {
        this.logger.OnTenantUpdate(onUpdate.TenantId.Value);
        await ((IMembershipGrain)this).RefreshAsync();
    }

    private async Task OnBusinessUnitUpdateAsync(OnBusinessUnitUpdate onUpdate, StreamSequenceToken token) {
        this.logger.OnBusinessUnitUpdate(onUpdate.BusinessUnitId.Value);
        await ((IMembershipGrain)this).RefreshAsync();
    }

    private async Task OnDashboardUpdateAsync(OnDashboardUpdate onUpdate, StreamSequenceToken token) {
        this.logger.OnDashboardUpdate(onUpdate.DashboardId.Value);
        await ((IMembershipGrain)this).RefreshAsync();
    }

    #endregion Subscription Handlers

    #region IMembershipView

    /// <inheritdoc/>
    Task<List<Membership>> IMembershipGrain.GetMembersAsync(AccessLevel? level, AccessLevelKind? kind) {
        // Query local state
        var query = this.State.Members.AsQueryable();

        if (level is not null) {
            query = query.Where(m => m.Level == level.Value);
        }

        if (kind is not null) {
            query = query.Where(m => m.Kind == kind.Value);
        }

        var members = query.ToList();

        // Return complete membership
        return Task.FromResult(members);
    }

    /// <inheritdoc/>
    async Task IMembershipGrain.RefreshAsync() {
        var members = await this.accessControlService.GetMembersAsync(this.State.Id);

        // TODO : There is a deadlock here, find a way to kill it (design session ?)

        // Retreive user infos
        var userIds = members
            .Where(member => member.IsUser)
            .Select(member => UserId.Parse(member.MemberId));

        var getUserTasks = userIds
            .Select(id => this.GrainFactory.GetGrain<IUserGrain>(id.Value).GetAsync())
            .ToList();
        await Task.WhenAll(getUserTasks);
        var users = getUserTasks.Select(t => t.Result).ToDictionary(user => user.Id.Value, user => user);

        this.State.Members.AddRange(
            members
                .Where(member => member.IsUser)
                .Select(m => new Membership {
                    Level = m.Level,
                    Kind = m.Kind,
                    User = users[m.MemberId],
                    Group = new Group()
                })
                .ToList()
        );

        // Retreive group infos
        var groupIds = members
            .Where(member => !member.IsUser)
            .Select(member => GroupId.Parse(member.MemberId));

        var getGroupTasks = userIds.Select(id => this.GrainFactory.GetGrain<IGroupGrain>(id.Value).GetAsync());
        await Task.WhenAll(getGroupTasks);
        var groups = getGroupTasks.Select(t => t.Result).ToDictionary(group => group.Id.Value, group => group);

        this.State.Members.AddRange(
            members
                .Where(member => !member.IsUser)
                .Select(m => new Membership {
                    Level = m.Level,
                    Kind = m.Kind,
                    User = new User(),
                    Group = groups[m.MemberId]
                })
                .ToList()
        );

        await this.WriteStateAsync();
    }

    #endregion IMembershipView
}

/// <summary>
/// <see cref="ILogger"/> extension methods. Helps log messages using strongly typing and source generators.
/// </summary>
internal static partial class LoggerExtensions {

    [LoggerMessage(
        EventId = 5100,
        Level = LogLevel.Information,
        Message = "Tenant '{tenantZRN}' has been updated. Membership view must be updated")]
    public static partial void OnTenantUpdate(
        this ILogger logger,
        string tenantZRN);

    [LoggerMessage(
        EventId = 5101,
        Level = LogLevel.Information,
        Message = "Business unit '{businessUnitZRN}' has been updated. Membership view must be updated")]
    public static partial void OnBusinessUnitUpdate(
        this ILogger logger,
        string businessUnitZRN);

    [LoggerMessage(
        EventId = 5102,
        Level = LogLevel.Information,
        Message = "Dashboard '{dashboardZRN}' has been updated. Membership view must be updated")]
    public static partial void OnDashboardUpdate(
        this ILogger logger,
        string dashboardZRN);
}
