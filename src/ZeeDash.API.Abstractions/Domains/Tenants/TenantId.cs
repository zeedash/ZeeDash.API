namespace ZeeDash.API.Abstractions.Domains.Tenants;

using NUlid;

/// <summary>
/// Identifier of the <see cref="Tenant"/>
/// </summary>
public class TenantId
    : Commons.Identities.Identity {

    public TenantId()
        : base(value: string.Format(Constants.URNs.TenantZRN, Ulid.NewUlid().ToString())) {
    }

    public TenantId(string value)
        : base(value) { }

    public static TenantId Empty => new(string.Format(Constants.URNs.TenantZRN, Guid.Empty));
}
