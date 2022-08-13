namespace ZeeDash.API.Grains.Domains.AccessControl;

using System.Collections.Generic;
using ZeeDash.API.Abstractions.Domains.IAM;

public class MembershipViewState {
    public MembershipId Id { get; set; } = MembershipId.Empty;
    public List<Membership> Members { get; set; } = new();
}
