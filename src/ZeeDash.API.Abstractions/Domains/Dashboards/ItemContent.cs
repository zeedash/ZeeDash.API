namespace ZeeDash.API.Abstractions.Domains.Dashboards;

public class ItemContent {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ItemData Data { get; set; } = new ItemData();
}
