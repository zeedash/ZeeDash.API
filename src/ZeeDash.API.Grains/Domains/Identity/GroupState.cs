namespace ZeeDash.API.Grains.Domains.Identity;

using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Grains.Domains.AccessControl;

public class GroupState
    : IHaveMembers {

    /// <summary>
    /// Group grain identifier
    /// </summary>
    /// <remarks>
    /// Its value is based on the <see cref="Grain"/> Primary Key String ಠ_ಠ
    /// </remarks>
    public GroupId Id { get; set; } = GroupId.Empty;

    /// <summary>
    /// Name of the group
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of all direct members of the business unit
    /// </summary>
    public List<Member> Members { get; set; } = new();
}
