namespace ZeeDash.API.Abstractions.Domains.Dashboards;

public class Dashboard {
    public DashboardId Id { get; set; } = DashboardId.Empty;

    public string Name { get; set; } = string.Empty;
}
