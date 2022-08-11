namespace ZeeDash.API.Abstractions.Constants;

public static class URNs {
    public const string ZRN = "zrn:zeedash:";
    public const string TenantZRN = ZRN + "tenant/{0}";
    public const string MembershipZRN = "{0}/membership";
    public const string BusinessUnitZRN = TenantZRN + ":business-unit/{1}";
    public const string BusinessUnitTemplateZRN = "{0}:business-unit/{1}";
    public const string DashboardZRN = TenantZRN + ":dashboard/{1}";
    public const string UserZRN = ZRN + "user/{0}";
    public const string UserViewZRN = UserZRN + ":view/{1}";
}
