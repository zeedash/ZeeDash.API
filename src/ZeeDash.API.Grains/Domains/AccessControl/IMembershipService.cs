namespace ZeeDash.API.Grains.Domains.AccessControl;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Exceptions;
using ZeeDash.API.Grains.Domains.Tenants;

public interface IMembershipService
    : IMembershipService<UserId>
    , IMembershipService<GroupId> {

    /// <summary>
    /// Get if the current user is the last owner in the direct membership of the grain
    /// </summary>
    /// <param name="state">The membership state to alter</param>
    /// <param name="memberId">The identifier of the member targeted by lookup</param>
    /// <returns><see cref="true"/> if he is the last owner on the direct membership list, otherwise <see cref="false"/></returns>
    /// <remarks>Only usefull on higher level of membership (e.g. <see cref="TenantGrain"/>)</remarks>
    bool IsLastOwner(IHaveMembers state, string memberId);
}

public interface IMembershipService<TIdentity> {

    /// <summary>
    /// Set a user to be managed on the grain
    /// </summary>
    /// <param name="state">The membership state to alter</param>
    /// <param name="memberId">The identifier of the member to affect</param>
    /// <param name="level">The access level of the member on the grain</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Member SetMember(IHaveMembers state, TIdentity memberId, AccessLevel level);

    /// <summary>
    /// Remove a <see cref="Member"/> to the grain
    /// </summary>
    /// <param name="state">The membership state to alter</param>
    /// <param name="memberId">The identifier of the member to affect</param>
    /// <returns>The <see cref="Member"/> removed, or null if it wasn't managed on this grain</returns>
    Member RemoveMember(IHaveMembers state, TIdentity memberId);
}

public class MembershipService
    : IMembershipService {

    ///<inheritdoc/>
    Member IMembershipService<UserId>.RemoveMember(IHaveMembers state, UserId userId) {
        var member = state.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == default) {
            throw new MemberNotFoundException(userId);
        }

        state.Members.Remove(member);

        return member;
    }

    ///<inheritdoc/>
    Member IMembershipService<UserId>.SetMember(IHaveMembers state, UserId userId, AccessLevel level) {
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
    Member IMembershipService<GroupId>.RemoveMember(IHaveMembers state, GroupId groupId) {
        var member = state.Members.FirstOrDefault(m => m.GroupId == groupId);
        if (member == default) {
            throw new MemberNotFoundException(groupId);
        }

        state.Members.Remove(member);

        return member;
    }

    ///<inheritdoc/>
    Member IMembershipService<GroupId>.SetMember(IHaveMembers state, GroupId groupId, AccessLevel level) {
        var member = state.Members.FirstOrDefault(m => m.GroupId == groupId);
        if (member == null) {
            member = new Member {
                GroupId = groupId,
                Level = level
            };
            state.Members.Add(member);
        } else {
            member.Level = level;
        }

        return member;
    }

    ///<inheritdoc/>
    bool IMembershipService.IsLastOwner(IHaveMembers state, string memberId) {
        var member = state.Members.FirstOrDefault(m => m.MemberId == memberId);
        if (member == null) {
            return false;
        }

        if (member.Level == AccessLevel.Owner) {
            return false;
        }

        if (!state.Members.Any(m => m.Level == AccessLevel.Owner && m.MemberId != memberId)) {
            return true;
        }

        return false;
    }
}
