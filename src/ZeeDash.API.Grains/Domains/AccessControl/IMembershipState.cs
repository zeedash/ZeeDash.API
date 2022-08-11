namespace ZeeDash.API.Grains.Domains.AccessControl;

using System.Collections.Generic;
using ZeeDash.API.Abstractions.Domains.IAM;

public interface IMembershipState {
    List<Member> Members { get; set; }
}
