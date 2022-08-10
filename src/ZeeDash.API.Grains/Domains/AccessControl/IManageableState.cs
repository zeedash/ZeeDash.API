namespace ZeeDash.API.Grains.Domains.AccessControl;

using System.Collections.Generic;
using ZeeDash.API.Abstractions.Domains.IAM;

public interface IManageableState {
    List<Member> Members { get; set; }
}

public class Membership
    : IManageableState {
    public List<Member> Members { get; set; } = new();
}
