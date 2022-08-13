namespace ZeeDash.API.Grains.Domains.Identity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Grains;

public partial class GroupGrain
    : Grain<GroupState>
    , IGroupGrain
    , IIncomingGrainCallFilter {

    #region Private Methods

    private Group MapStateToGroup() {
        return new Group {
            Id = this.State.Id,
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

        return this.MapStateToGroup();
    }

    /// <inheritdoc/>
    Task<Group> IGroupGrain.GetAsync() {
        return Task.FromResult(this.MapStateToGroup());
    }

    /// <inheritdoc/>
    Task<bool> IGroupGrain.HasUsersAsync(UserId userId) {
        return Task.FromResult(this.State.Users.Contains(userId));
    }

    /// <inheritdoc/>
    async Task<List<User>> IGroupGrain.GetUsersAsync() {
        var getUsersTasks = this.State.Users
            .Select(id => this.GrainFactory.GetGrain<IUserGrain>(id.Value).GetAsync());
        await Task.WhenAll(getUsersTasks);
        return getUsersTasks
            .Select(t => t.Result)
            .Where(v => v is not null)
            .ToList();
    }

    /// <inheritdoc/>
    async Task IGroupGrain.AddUserAsync(UserId userId) {
        this.State.Users.Add(userId);
        await this.WriteStateAsync();
    }

    /// <inheritdoc/>
    async Task IGroupGrain.RemoveUserAsync(UserId userId) {
        var user = this.State.Users.FirstOrDefault(u => u == userId);
        if (user is null) {
            return;
        }
        this.State.Users.Remove(user);

        await this.WriteStateAsync();
    }

    #endregion IGroupGrain
}
