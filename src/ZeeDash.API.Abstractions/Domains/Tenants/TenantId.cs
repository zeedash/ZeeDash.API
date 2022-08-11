namespace ZeeDash.API.Abstractions.Domains.Tenants;

using NUlid;

/// <summary>
/// Identifier of the <see cref="Tenant"/>
/// </summary>
public class TenantId
    : Commons.Identities.Identity {
    private readonly Ulid idValue;

    public TenantId()
        : this(Ulid.NewUlid()) {
        this.IsEmpty = false;
    }

    //public TenantId(string value)
    //    : base(value) {
    //    this.IsEmpty = false;
    //}

    public TenantId(Ulid value)
        : base(string.Format(Constants.URNs.TenantZRN, value)) {
        this.IsEmpty = value == Ulid.Empty;
        this.idValue = value;
    }

    public static TenantId Empty => new(Ulid.Empty);

    //public static TenantId Parse(string value) => new(Ulid.Empty);

    public bool IsEmpty { get; init; }

    public Guid AsGuid() => this.idValue.ToGuid();

    public Ulid AsUlid() => this.idValue;

    public static TenantId Parse(string identityString) {
        throw new NotImplementedException();
    }
}
