namespace ZeeDash.API.Grains.Domains.AccessControl;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Exceptions;

public interface IManageableService {

    /// <summary>
    /// Get the list of all members of the grain
    /// </summary>
    /// <param name="state">The manageable state to alter</param>
    /// <param name="level">The level to list</param>
    /// <param name="kind">The level to list</param>
    /// <returns>The list of all <see cref="Member"/> of the grain</returns>
    List<Member> GetMembers(IMembershipState state, AccessLevel? level = null, AccessLevelKind? kind = null);

    /// <summary>
    /// Set a user to be managed on the grain
    /// </summary>
    /// <param name="state">The manageable state to alter</param>
    /// <param name="userId">The identifier of the user to affect</param>
    /// <param name="level">The access level of the user on the grain</param>
    /// <param name="kind">The kind of level access to set. <see cref="AccessLevelKind.Direct"/> by default.</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Member SetMember(IMembershipState state, UserId userId, AccessLevel level, AccessLevelKind? kind = null);

    /// <summary>
    /// Remove a <see cref="Member"/> to the grain
    /// </summary>
    /// <param name="state">The manageable state to alter</param>
    /// <param name="userId">The identifier of the user to affect</param>
    /// <returns>The <see cref="Member"/> removed, or null if it wasn't managed on this grain</returns>
    Member RemoveMember(IMembershipState state, UserId userId);//, Func<Task> saveContext, Func<string, IStreamProvider> streamFactory);
}

public class ManageableService
    : IManageableService {

    List<Member> IManageableService.GetMembers(IMembershipState state, AccessLevel? level, AccessLevelKind? kind) {
        var query = state.Members.AsQueryable();

        if (level is not null) {
            query = query.Where(m => m.Level == level.Value);
        }

        if (kind is not null) {
            query = query.Where(m => m.Kind == kind.Value);
        }

        return query.ToList();
    }

    Member IManageableService.RemoveMember(IMembershipState state, UserId userId) {
        var member = state.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == default) {
            throw new MemberNotFoundException(userId);
        }

        state.Members.Remove(member);

        return member;
    }

    Member IManageableService.SetMember(IMembershipState state, UserId userId, AccessLevel level, AccessLevelKind? kind) {
        var member = state.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == null) {
            member = new Member {
                UserId = userId,
                Level = level,
                Kind = kind ?? AccessLevelKind.Inherited
            };
            state.Members.Add(member);
        } else {
            if (member.Level == AccessLevel.Owner && member.Kind == AccessLevelKind.Direct && member.Level != level) {
                if (!state.Members.Any(m => m.Level == AccessLevel.Owner && m.UserId != userId)) {
                    throw new InvalidMembershipException();
                }
            }

            member.Level = level;
            if (kind is not null) {
                member.Kind = kind.Value;
            }
        }

        return member;
    }
}
