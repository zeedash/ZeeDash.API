namespace ZeeDash.API.Abstractions.Domains.Dashboards;

/// <summary>
/// Types of block content that will be displayed
/// </summary>
public enum BlockContentTypes {

    /// <summary>
    /// List of application links (viewable in cards or columns)
    /// </summary>
    BookMarks = 0,

    /// <summary>
    /// A formated text
    /// </summary>
    Text = 1,

    /// <summary>
    /// List of bookmarks organized by tree
    /// </summary>
    OrganizedBookmarks = 2,

    /// <summary>
    /// Data charts
    /// </summary>
    Charts = 3,
}
