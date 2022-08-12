namespace ZeeDash.API.Grains.Domains.AccessControl;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Exceptions;
using ZeeDash.API.Grains.Domains.Tenants;

public interface IMembershipService {

    /// <summary>
    /// Set a user to be managed on the grain
    /// </summary>
    /// <param name="state">The membership state to alter</param>
    /// <param name="userId">The identifier of the user to affect</param>
    /// <param name="level">The access level of the user on the grain</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Member SetMember(IMembershipState state, UserId userId, AccessLevel level);

    /// <summary>
    /// Remove a <see cref="Member"/> to the grain
    /// </summary>
    /// <param name="state">The membership state to alter</param>
    /// <param name="userId">The identifier of the user to affect</param>
    /// <returns>The <see cref="Member"/> removed, or null if it wasn't managed on this grain</returns>
    Member RemoveMember(IMembershipState state, UserId userId);

    /// <summary>
    /// Get if the current user is the last owner in the direct membership of the grain
    /// </summary>
    /// <param name="state">The membership state to alter</param>
    /// <param name="userId">The identifier of the user targeted by lookup</param>
    /// <returns><see cref="true"/> if he is the last owner on the direct membership list, otherwise <see cref="false"/></returns>
    /// <remarks>Only usefull on higher level of membership (e.g. <see cref="TenantGrain"/>)</remarks>
    bool IsLastOwner(IMembershipState state, UserId userId);
}

public class MembershipService
    : IMembershipService {

    ///<inheritdoc/>
    Member IMembershipService.RemoveMember(IMembershipState state, UserId userId) {
        var member = state.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == default) {
            throw new MemberNotFoundException(userId);
        }

        state.Members.Remove(member);

        return member;
    }

    ///<inheritdoc/>
    Member IMembershipService.SetMember(IMembershipState state, UserId userId, AccessLevel level) {
        var member = state.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == null) {
            member = new Member {
                UserId = userId,
                Level = level
            };
            state.Members.Add(member);
        } else {
            member.Level = level;
        }

        return member;
    }

    ///<inheritdoc/>
    bool IMembershipService.IsLastOwner(IMembershipState state, UserId userId) {
        var member = state.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == null) {
            return false;
        }

        if (member.Level == AccessLevel.Owner) {
            return false;
        }

        if (!state.Members.Any(m => m.Level == AccessLevel.Owner && m.UserId != userId)) {
            return true;
        }

        return false;
    }
}
