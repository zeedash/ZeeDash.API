namespace ZeeDash.API.Grains.Services;

using System.Threading.Tasks;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;

public interface IAccessControlService {

    Task AddContributorMembershipAsync(string identityString, UserId userId, AccessLevel level);

    Task AddOwnerMembershipAsync(string identityString, UserId userId, AccessLevel level);

    Task AddReaderMembershipAsync(string identityString, UserId userId, AccessLevel level);

    Task RemoveMembershipAsync(string identityString, UserId userId);
}

public class AccessControlService
    : IAccessControlService {

    Task IAccessControlService.AddContributorMembershipAsync(string identityString, UserId userId, AccessLevel level) {
        return Task.CompletedTask;
    }

    Task IAccessControlService.AddOwnerMembershipAsync(string identityString, UserId userId, AccessLevel level) {
        return Task.CompletedTask;
    }

    Task IAccessControlService.AddReaderMembershipAsync(string identityString, UserId userId, AccessLevel level) {
        return Task.CompletedTask;
    }

    Task IAccessControlService.RemoveMembershipAsync(string identityString, UserId userId) {
        return Task.CompletedTask;
    }
}
