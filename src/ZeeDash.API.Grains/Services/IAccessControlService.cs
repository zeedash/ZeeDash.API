namespace ZeeDash.API.Grains.Services;

using System.Threading.Tasks;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;

public interface IAccessControlService {

    Task<List<Member>> GetMembersAsync(MembershipViewId id);

    Task AddContributorMembershipAsync(MembershipViewId id, UserId userId, AccessLevel level);

    Task AddOwnerMembershipAsync(MembershipViewId id, UserId userId, AccessLevel level);

    Task AddReaderMembershipAsync(MembershipViewId id, UserId userId, AccessLevel level);

    Task RemoveMembershipAsync(MembershipViewId id, UserId userId);
}

public class AccessControlService
    : IAccessControlService {

    Task<List<Member>> IAccessControlService.GetMembersAsync(MembershipViewId id) {
        throw new NotImplementedException();
    }

    Task IAccessControlService.AddContributorMembershipAsync(MembershipViewId id, UserId userId, AccessLevel level) {
        return Task.CompletedTask;
    }

    Task IAccessControlService.AddOwnerMembershipAsync(MembershipViewId id, UserId userId, AccessLevel level) {
        return Task.CompletedTask;
    }

    Task IAccessControlService.AddReaderMembershipAsync(MembershipViewId id, UserId userId, AccessLevel level) {
        return Task.CompletedTask;
    }

    Task IAccessControlService.RemoveMembershipAsync(MembershipViewId id, UserId userId) {
        return Task.CompletedTask;
    }
}
