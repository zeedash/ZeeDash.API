namespace ZeeDash.API.Abstractions.Commons.Identities;

using ZeeDash.API.Abstractions.Commons.ValueObjects;

public class Identity
    : SingleValueObject<string>
#pragma warning disable CS8767 // La nullabilité des types référence dans le type du paramètre ne correspond pas au membre implémenté implicitement (probablement en raison des attributs de nullabilité).
    , IIdentity {
#pragma warning restore CS8767 // La nullabilité des types référence dans le type du paramètre ne correspond pas au membre implémenté implicitement (probablement en raison des attributs de nullabilité).

    #region Ctor.Dtor

    public Identity()
        : base(Guid.NewGuid().ToString()) { }

    public Identity(string value)
        : base(value ?? Guid.NewGuid().ToString()) { }

    public Identity(Guid value)
        : base(value.ToString()) { }

    #endregion Ctor.Dtor

    #region Properties

#pragma warning disable S2292 // Trivial properties should be auto-implemented

    public new string Value {
#pragma warning restore S2292 // Trivial properties should be auto-implemented
        get => this.value;
        set => this.value = value;
    }

    #endregion Properties

    #region IComparable

    public int CompareTo(string other) {
#pragma warning disable CA1310 // Spécifier StringComparison à des fins de précision
        return this.Value.CompareTo(other);
#pragma warning restore CA1310 // Spécifier StringComparison à des fins de précision
    }

    #endregion IComparable

    #region IEquatable

    public bool Equals(string other) {
        return this.Value.Equals(other, StringComparison.Ordinal);
    }

    #endregion IEquatable

    #region Overides

    public override string ToString() => this.Value;

    #endregion Overides
}