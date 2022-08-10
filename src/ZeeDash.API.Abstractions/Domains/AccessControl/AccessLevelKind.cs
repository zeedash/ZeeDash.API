namespace ZeeDash.API.Abstractions.Domains.AccessControl;

/// <summary>
/// The kind of access level
/// </summary>
public enum AccessLevelKind : byte {

    /// <summary>
    /// UNspecified kind of access level kind
    /// </summary>
    None = 0,

    /// <summary>
    /// Direct access level
    /// </summary>
    Direct = 1,

    /// <summary>
    /// Inherited access level
    /// </summary>
    Inherited = 2,
}
