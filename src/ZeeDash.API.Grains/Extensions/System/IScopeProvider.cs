namespace System;

/// <summary>
/// Fournisseur de scope générique
/// </summary>
public interface IScopeProvider {

    /// <summary>
    /// Créer un nouveau scope
    /// </summary>
    /// <returns>L'élément à disposer à la fin de l'exploitation du scope</returns>
    IDisposable CreateScope();
}