namespace ZeeDash.API.Abstractions.Constants;

public static class StreamName {
    public const string Reminder = nameof(Reminder);
    public const string SaidHello = nameof(SaidHello);

    public static class Membership {
        public const string OnTenantUpdate = $"{nameof(Membership)}->{nameof(OnTenantUpdate)}";
        public const string OnBusinessUnitUpdate = $"{nameof(Membership)}->{nameof(OnBusinessUnitUpdate)}";
        public const string OnDashboardUpdate = $"{nameof(Membership)}->{nameof(OnDashboardUpdate)}";
    }
}
