namespace ZeeDash.API.Abstractions.Domains.Identity;

/// <summary>
/// A group of users
/// </summary>
public class Group {

    /// <summary>
    /// Identifier of the group
    /// </summary>
    public GroupId Id { get; set; } = GroupId.Empty;

    /// <summary>
    /// Name of the group
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
