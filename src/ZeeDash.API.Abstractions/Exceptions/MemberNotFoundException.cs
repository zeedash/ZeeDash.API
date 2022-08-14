namespace ZeeDash.API.Abstractions.Exceptions;

using ZeeDash.API.Abstractions.Domains.Identity;

public class MemberNotFoundException
    : Exception {

    public MemberNotFoundException(UserId userId) {
        this.MemberId = userId.Value;
    }

    public MemberNotFoundException(GroupId groupId) {
        this.MemberId = groupId.Value;
    }

    public string MemberId { get; init; }
}
