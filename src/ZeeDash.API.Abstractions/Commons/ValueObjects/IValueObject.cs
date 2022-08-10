namespace ZeeDash.API.Abstractions.Commons.ValueObjects;

using System.Diagnostics.CodeAnalysis;

public interface IValueObject
    : IEquatable<IValueObject> {

    IEnumerable<object> GetEqualityComponents();
}

public abstract class ValueObject
    : IValueObject
    , IEqualityComparer<ValueObject> {

    #region IEquatable

#pragma warning disable CS8767 // La nullabilité des types référence dans le type du paramètre ne correspond pas au membre implémenté implicitement (probablement en raison des attributs de nullabilité).

    public virtual bool Equals(IValueObject other) {
        if (other is null) {
            return false;
        }

        if (ReferenceEquals(this, other)) {
            return true;
        }

        return this.GetEqualityComponents()
            .SequenceEqual(
                other.GetEqualityComponents()
            );
    }

#pragma warning restore CS8767 // La nullabilité des types référence dans le type du paramètre ne correspond pas au membre implémenté implicitement (probablement en raison des attributs de nullabilité).

    #endregion IEquatable

    #region IEqualityComparer

#pragma warning disable CS8769 // La nullabilité des types référence dans le type du paramètre ne correspond pas au membre implémenté (probablement en raison des attributs de nullabilité).

    bool IEqualityComparer<ValueObject>.Equals(ValueObject x, ValueObject y)
        => x?.Equals(y) ?? false;

#pragma warning restore CS8769 // La nullabilité des types référence dans le type du paramètre ne correspond pas au membre implémenté (probablement en raison des attributs de nullabilité).

    int IEqualityComparer<ValueObject>.GetHashCode([DisallowNull] ValueObject obj)
        => obj.GetHashCode();

    #endregion IEqualityComparer

    #region Overrides

#pragma warning disable CS8765 // La nullabilité de type du paramètre ne correspond pas au membre substitué (probablement en raison des attributs de nullabilité).

    public override bool Equals(object obj) {
#pragma warning disable CS8604 // Existence possible d'un argument de référence null.
        return this.Equals(obj as ValueObject);
#pragma warning restore CS8604 // Existence possible d'un argument de référence null.
    }

#pragma warning restore CS8765 // La nullabilité de type du paramètre ne correspond pas au membre substitué (probablement en raison des attributs de nullabilité).

    public override int GetHashCode() {
        unchecked {
            return this.GetEqualityComponents()
                .Aggregate(
                    seed: 17,
                    (current, obj) => (current * 23) + (obj?.GetHashCode() ?? 0)
                );
        }
    }

    public override string ToString() {
        return string.Join(
            " // ",
            this.GetEqualityComponents()
                .Select(p => $"{p}")
        );
    }

    #endregion Overrides

    #region Operators

    public static bool operator ==(ValueObject left, ValueObject right) {
        return Equals(left, right);
    }

    public static bool operator !=(ValueObject left, ValueObject right) {
        return !Equals(left, right);
    }

    #endregion Operators

    #region Abstract Methods

    public abstract IEnumerable<object> GetEqualityComponents();

    #endregion Abstract Methods
}
