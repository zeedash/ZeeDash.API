namespace ZeeDash.API.Abstractions.Domains.AccessControl;

public enum AccessLevel : byte {

    /// <summary>
    /// No access to item
    /// </summary>
    None = 0,

    /// <summary>
    /// Can view/read element items, including members list
    /// </summary>
    Reader = 1,

    /// <summary>
    /// Has Reader access level and can manage element' title and  items (but not members list)
    /// </summary>
    Contributor = 2,

    /// <summary>
    /// Can fully manage element
    /// </summary>
    Owner = 3,
}
