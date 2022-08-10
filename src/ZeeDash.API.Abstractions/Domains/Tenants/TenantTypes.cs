namespace ZeeDash.API.Abstractions.Domains.Tenants;

/// <summary>
/// Types of tenant
/// </summary>
public enum TenantTypes {

    /// <summary>
    /// Created when a user is registered or loged in for the first time
    /// </summary>
    Personal = 0,

    /// <summary>
    /// Belong to a company
    /// </summary>
    Corporate = 1
}
