namespace ZeeDash.API.Abstractions.Constants;

public static class URNs {
    public const string ZRN = "zrn:zeedash";

    public const string TenantTemplate = ":tenant/";
    public const string TenantZRN = ZRN + TenantTemplate + "{0}";

    public const string BusinessUnitTemplate = ":business-unit/";
    public const string BusinessUnitZRN = TenantZRN + BusinessUnitTemplate + "{1}";

    public const string DashboardTemplate = ":dashboard/";
    public const string DashboardZRN = TenantZRN + DashboardTemplate + "{1}";

    public const string MembershipTemplate = "/membership";
    public const string MembershipZRN = "{0}" + MembershipTemplate;

    public const string UserZRN = ZRN + "user/{0}";
    public const string UserViewZRN = UserZRN + ":view/{1}";
}
