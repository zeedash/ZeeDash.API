namespace ZeeDash.API.Grains.Domains.Identity;

using System;
using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Grains;
using ZeeDash.API.Grains.Services;

public partial class UserGrain
    : Grain<UserState>
    , IUserGrain
    , IIncomingGrainCallFilter {

    #region Private Fields

    private readonly IAccessControlService accessControlService;

    #endregion Private Fields

    #region Ctor.Dtor

    public UserGrain(IAccessControlService accessControlService) {
        this.accessControlService = accessControlService;
    }

    #endregion Ctor.Dtor

    #region Private Methods

    private User MapStateToUser() {
        return new User {
            Id = this.State.Id,
            FullName = this.State.FullName,
            Email = this.State.Email
        };
    }

    #endregion Private Methods

    #region IIncomingGrainCallFilter

    async Task IIncomingGrainCallFilter.Invoke(IIncomingGrainCallContext context) {
        var isCreated = !this.State.Id.IsEmpty;
        if (!string.Equals(context.InterfaceMethod.Name, nameof(IUserGrain.CreateAsync), StringComparison.Ordinal)) {
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

    #region IUserGrain

    /// <inheritdoc/>
    async Task<User> IUserGrain.CreateAsync(string fullName, string email) {
        // TODO : Input validation

        this.State.Id = UserId.Parse(this.GetPrimaryKeyString());
        this.State.FullName = fullName;
        this.State.Email = email;
        await this.WriteStateAsync();

        await this.accessControlService.CreateMemberAsync(this.State.Id);

        var tenantId = new TenantId(this.State.Id);
        var tenant = this.GrainFactory.GetGrain<ITenantGrain>(tenantId.Value);

        await tenant.CreateAsync(fullName, TenantTypes.Personal, this.State.Id);

        return this.MapStateToUser();
    }

    /// <inheritdoc/>
    Task<User> IUserGrain.GetAsync() {
        return Task.FromResult(this.MapStateToUser());
    }

    /// <inheritdoc/>
    async Task<User> IUserGrain.ChangeEmailAsync(string email) {
        this.State.Email = email;
        await this.WriteStateAsync();

        return this.MapStateToUser();
    }

    /// <inheritdoc/>
    async Task<User> IUserGrain.ChangeFullNameAsync(string fullName) {
        this.State.FullName = fullName;
        await this.WriteStateAsync();

        return this.MapStateToUser();
    }

    #endregion IUserGrain
}
