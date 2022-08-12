namespace ZeeDash.API.Grains.Domains.AccessControl;

using System.Collections.Generic;
using ZeeDash.API.Abstractions.Domains.IAM;

public interface IMembershipViewState {
    MembershipViewId Id { get; set; }
    List<Membership> Members { get; set; }
}

public class MembershipViewState
    : IMembershipViewState {
    public MembershipViewId Id { get; set; } = MembershipViewId.Empty;
    public List<Membership> Members { get; set; } = new();
}
