namespace ZeeDash.API.Abstractions.Domains.Tenants;

using Orleans;
using ZeeDash.API.Abstractions.Domains.IAM;

/// <summary>
/// Managing membership to a manageable grain
/// </summary>
public interface IMembershipGrain : IGrainWithStringKey {

    /// <summary>
    /// Get the list of all members of the grain
    /// </summary>
    /// <param name="level">The level to list</param>
    /// <param name="kind">The kind of access to list</param>
    /// <returns>The list of all <see cref="Member"/> of the grain, ordered by highest role first</returns>
    Task<List<Membership>> GetMembersAsync(AccessLevel? level = null, AccessLevelKind? kind = null);

    /// <summary>
    /// Start a refresh of the view
    /// </summary>
    Task RefreshAsync();
}
