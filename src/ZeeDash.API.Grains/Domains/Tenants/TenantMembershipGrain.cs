namespace ZeeDash.API.Grains.Domains.Tenants;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Abstractions.Grains.Common;
using ZeeDash.API.Grains.Domains.AccessControl;
using ZeeDash.API.Grains.Domains.AccessControl.Events;
using ZeeDash.API.Grains.Services;

public partial class TenantMembershipViewGrain
    : Grain<MembershipViewState>
    , ITenantMembershipViewGrain {

    #region Private Fields

    private readonly ILogger<TenantMembershipViewGrain> logger;
    private readonly IAccessControlService accessControlService;
    private StreamSubscriptionHandle<OnTenantUpdate>? tenantUpdateSubscription;
    private StreamSubscriptionHandle<OnBusinessUnitUpdate>? businessUnitUpdateSubscription;
    private StreamSubscriptionHandle<OnDashboardUpdate>? dashboardUpdateSubscription;

    #endregion Private Fields

    #region Ctor.Dtor

    public TenantMembershipViewGrain(
        ILogger<TenantMembershipViewGrain> logger,
        IAccessControlService accessControlService) {
        this.logger = logger;
        this.accessControlService = accessControlService;
    }

    #endregion Ctor.Dtor

    #region Grain

    public override async Task OnActivateAsync() {
        if (this.State.Id.IsEmpty) {
            this.State.Id = MembershipViewId.Parse(this.IdentityString);
        }

        await ((IMembershipView)this).RefreshAsync();

        var streamProvider = this.GetStreamProvider(StreamProviderName.Membership);
        this.tenantUpdateSubscription = await streamProvider
            .GetStream<OnTenantUpdate>(this.State.Id.TenantId.AsGuid(), StreamName.Membership.OnTenantUpdate)
            .SubscribeAsync(this.OnTenantUpdateAsync)
            .ConfigureAwait(false);

        if (!this.State.Id.BusinessUnitId.IsEmpty) {
            this.businessUnitUpdateSubscription = await streamProvider
                .GetStream<OnBusinessUnitUpdate>(this.State.Id.BusinessUnitId.AsGuid(), StreamName.Membership.OnBusinessUnitUpdate)
                .SubscribeAsync(this.OnBusinessUnitUpdateAsync)
                .ConfigureAwait(false);
        }

        if (!this.State.Id.DashboardId.IsEmpty) {
            this.dashboardUpdateSubscription = await streamProvider
                .GetStream<OnDashboardUpdate>(this.State.Id.DashboardId.AsGuid(), StreamName.Membership.OnDashboardUpdate)
                .SubscribeAsync(this.OnDashboardUpdateAsync)
                .ConfigureAwait(false);
        }

        await base.OnActivateAsync();
    }

    public override async Task OnDeactivateAsync() {
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
        await ((IMembershipView)this).RefreshAsync();
    }

    private async Task OnBusinessUnitUpdateAsync(OnBusinessUnitUpdate onUpdate, StreamSequenceToken token) {
        this.logger.OnBusinessUnitUpdate(onUpdate.BusinessUnitId.Value);
        await ((IMembershipView)this).RefreshAsync();
    }

    private async Task OnDashboardUpdateAsync(OnDashboardUpdate onUpdate, StreamSequenceToken token) {
        this.logger.OnDashboardUpdate(onUpdate.DashboardId.Value);
        await ((IMembershipView)this).RefreshAsync();
    }

    #endregion Subscription Handlers

    #region IMembershipView

    /// <inheritdoc/>
    async Task<List<Membership>> IMembershipView.GetMembersAsync(AccessLevel? level) {
        // Query local state
        var query = this.State.Members.AsQueryable();

        if (level is not null) {
            query = query.Where(m => m.Level == level.Value);
        }

        var members = query.ToList();

        // Retreive user infos
        var tasks = members.Select(member => this.GrainFactory.GetGrain<IUserGrain>(member.UserId.Value).GetAsync());
        await Task.WhenAll(tasks);
        var users = tasks.Select(t => t.Result).ToDictionary(user => user.Id, user => user);

        // Return complete membership
        return members
            .Select(m => new Membership {
                Kind = m.Kind,
                Level = m.Level,
                User = users[m.UserId]
            })
            .ToList();
    }

    /// <inheritdoc/>
    async Task IMembershipView.RefreshAsync() {
        this.State.Members = await this.accessControlService.GetMembersAsync(this.State.Id);
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
