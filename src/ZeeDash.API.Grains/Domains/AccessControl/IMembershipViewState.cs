namespace ZeeDash.API.Grains.Domains.AccessControl;

using System.Collections.Generic;
using ZeeDash.API.Abstractions.Domains.IAM;

public interface IMembershipViewState {
    MembershipViewId Id { get; set; }
    List<Member> Members { get; set; }
}

public class MembershipViewState
    : IMembershipViewState {
    public MembershipViewId Id { get; set; } = MembershipViewId.Empty;
    public List<Member> Members { get; set; } = new();
}
