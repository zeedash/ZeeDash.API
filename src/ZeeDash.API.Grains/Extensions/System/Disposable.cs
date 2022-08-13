namespace System;

public class Disposable<T>
    : IDisposable
    , IDisposing {

    #region Private Members

    /// <summary>
    /// Indique si l'objet courant a été détruit
    /// </summary>
    private bool disposed;

    #endregion Private Members

    #region IDisposable Members

    /// <summary>
    /// Libère les ressources non managées utilisées par SageTransaction
    /// </summary>
    public void Dispose() {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Libère les ressources non managées utilisées par SageTransaction en prenant en compte l'état de libération
    /// </summary>
    /// <param name="disposing">Défini si les objets managés doivent être détruit</param>
    protected virtual void Dispose(bool disposing) {
        // Protect from being called multiple times.
        if (this.disposed) {
            return;
        }

        if (disposing) {
            this.OnDisposing();
        }

        this.disposed = true;
    }

    #endregion IDisposable Members

    #region IDisposing Members

    /// <summary>
    /// Evènement prévenant l'action Dispose sur la classe en cours
    /// </summary>
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

    public event EventHandler Disposing;

#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

    /// <summary>
    /// Soulève l'évènement Disposing de la classe
    /// </summary>
    protected virtual void OnDisposing() {
        if (this.Disposing != null) {
            this.Disposing.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion IDisposing Members

    protected void EnsureIsNotDisposed() {
        if (this.disposed) {
            throw new ObjectDisposedException(typeof(T).Name);
        }
    }
}
