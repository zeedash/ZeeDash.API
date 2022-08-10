namespace ZeeDash.API.Abstractions.Domains.Dashboards;

using System;
using System.Collections.Generic;

public class BlockContent {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public BlockContentTypes Type { get; set; }
    public BlockContentLayouts Layouts { get; set; } = BlockContentLayouts.All;
    public List<ItemContent> Items { get; set; } = new List<ItemContent>();
}
