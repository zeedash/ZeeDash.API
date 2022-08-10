namespace ZeeDash.API.Abstractions.Domains.Dashboards;

/// <summary>
/// Layouts allowed by a content block
/// </summary>
[Flags]
public enum BlockContentLayouts {

    /// <summary>
    /// Not viewable by default
    /// </summary>
    Hidden = 0,

    /// <summary>
    /// View as a grid
    /// </summary>
    GridView = 1,

    /// <summary>
    /// View as columns, by categories
    /// </summary>
    ColumnsView = 2,

    /// <summary>
    /// Grid or Column, we don't care
    /// </summary>
    All = GridView | ColumnsView,
}
