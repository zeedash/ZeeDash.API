namespace System;

/// <summary>
/// Fournit un mécanisme indiquant que la libération des ressources non managées est en cours.
/// </summary>
public interface IDisposing {

    /// <summary>
    /// Evènement levé lorsque la libération est en cours
    /// </summary>
    event EventHandler Disposing;
}