namespace ZeeDash.API.Abstractions.Domains.IAM;

using System;
using ZeeDash.API.Abstractions.Constants;
using ZeeDash.API.Abstractions.Domains.Dashboards;
using ZeeDash.API.Abstractions.Domains.Tenants;
using ZeeDash.API.Abstractions.Exceptions;

/// <summary>
/// Identifier of a <see cref="MembershipView"/>
/// </summary>
public class MembershipViewId
    : Commons.Identities.Identity {

    public MembershipViewId(TenantId tenantId)
        : base(string.Format(URNs.MembershipZRN, tenantId.Value)) {
        this.IsEmpty = tenantId.IsEmpty;
        this.TenantId = tenantId;
        this.BusinessUnitId = BusinessUnitId.Empty;
        this.DashboardId = DashboardId.Empty;
    }

    public MembershipViewId(BusinessUnitId businessUnitId)
        : base(string.Format(URNs.MembershipZRN, businessUnitId.Value)) {
        this.IsEmpty = businessUnitId.IsEmpty;
        this.TenantId = businessUnitId.TenantId;
        this.BusinessUnitId = businessUnitId;
        this.DashboardId = DashboardId.Empty;
    }

    public MembershipViewId(DashboardId dashboardId)
        : base(string.Format(URNs.MembershipZRN, dashboardId.Value)) {
        this.IsEmpty = dashboardId.IsEmpty;
        this.TenantId = dashboardId.TenantId;
        this.BusinessUnitId = dashboardId.BusinessUnitId;
        this.DashboardId = dashboardId;
    }

    public static MembershipViewId Empty => new(TenantId.Empty);

    public bool IsEmpty { get; init; }
    public TenantId TenantId { get; init; }
    public BusinessUnitId BusinessUnitId { get; init; }
    public DashboardId DashboardId { get; init; }

    public static MembershipViewId Parse(string identityString) {
        var containsTemplate = identityString.Contains(URNs.MembershipTemplate, StringComparison.OrdinalIgnoreCase);
        if (!containsTemplate) {
            throw new ZrnFormatException(identityString, nameof(MembershipViewId));
        }

        // Skip the lasts "URNs.MembershipTemplate.Length"th character
        var identifier = identityString[..^URNs.MembershipTemplate.Length];

        if (identityString.Contains(URNs.DashboardTemplate)) {
            return new MembershipViewId(DashboardId.Parse(identifier));
        }

        if (identityString.Contains(URNs.BusinessUnitTemplate)) {
            return new MembershipViewId(BusinessUnitId.Parse(identifier));
        }

        if (identityString.Contains(URNs.TenantTemplate)) {
            return new MembershipViewId(TenantId.Parse(identifier));
        }

        throw new ZrnDecodeException(identityString);
    }
}
