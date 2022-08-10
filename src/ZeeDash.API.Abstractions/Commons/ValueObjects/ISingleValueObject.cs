namespace ZeeDash.API.Abstractions.Commons.ValueObjects;

using System.Diagnostics.CodeAnalysis;

public interface ISingleValueObject<T>
    : IValueObject {
    public T Value { get; }
}

#pragma warning disable CA1708 // Les identificateurs ne doivent pas différer uniquement par leur casse

public abstract class SingleValueObject<T>
#pragma warning restore CA1708 // Les identificateurs ne doivent pas différer uniquement par leur casse
    : ValueObject
#pragma warning disable CS8767 // La nullabilité des types référence dans le type du paramètre ne correspond pas au membre implémenté implicitement (probablement en raison des attributs de nullabilité).
    , ISingleValueObject<T>
#pragma warning restore CS8767 // La nullabilité des types référence dans le type du paramètre ne correspond pas au membre implémenté implicitement (probablement en raison des attributs de nullabilité).
    , IEqualityComparer<T> {

    #region Private Fields

#pragma warning disable CA1051 // Ne pas déclarer de champs d'instances visibles
#pragma warning disable IDE1006 // Styles d'affectation de noms
    protected T value;
#pragma warning restore IDE1006 // Styles d'affectation de noms
#pragma warning restore CA1051 // Ne pas déclarer de champs d'instances visibles

    #endregion Private Fields

    #region Ctor.Dtor

    //protected SingleValueObject() {
    //    _value = default;
    //}
    protected SingleValueObject(T value) {
        this.value = value;
    }

    #endregion Ctor.Dtor

    #region Properties

    public T Value => this.value;

    #endregion Properties

    #region IValueObject

    public override IEnumerable<object> GetEqualityComponents() {
        yield return this.value!;
    }

    #endregion IValueObject

    #region IEqualityComparer

    bool IEqualityComparer<T>.Equals(T? x, T? y)
        => x?.Equals(y) ?? false;

    int IEqualityComparer<T>.GetHashCode([DisallowNull] T obj)
        => obj.GetHashCode();

    #endregion IEqualityComparer
}