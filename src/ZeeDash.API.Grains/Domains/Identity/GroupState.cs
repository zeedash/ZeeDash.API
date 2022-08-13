namespace ZeeDash.API.Grains.Domains.Identity;

using ZeeDash.API.Abstractions.Domains.Identity;

public class GroupState {

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
    /// List of all users' ids belonging to the group
    /// </summary>
    public List<UserId> Users { get; set; } = new();
}
