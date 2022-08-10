namespace ZeeDash.API.Abstractions.Exceptions;

using ZeeDash.API.Abstractions.Domains.Identity;

public class MemberNotFoundException
    : Exception {

    public MemberNotFoundException(UserId userId) {
        this.UserId = userId;
    }

    public UserId UserId { get; init; }
}
