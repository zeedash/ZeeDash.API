namespace ZeeDash.API.Abstractions.Grains.Common;

using System.Threading.Tasks;
using ZeeDash.API.Abstractions.Domains.IAM;

/// <summary>
/// Methods for managing access control of the grain
/// </summary>
public interface IMembershipView {

    /// <summary>
    /// Get the list of all members of the grain
    /// </summary>
    /// <param name="level">The level to list</param>
    /// <returns>The list of all <see cref="Member"/> of the grain, ordered by highest role first</returns>
    Task<List<Membership>> GetMembersAsync(AccessLevel? level = null);

    /// <summary>
    /// Start a refresh of the view
    /// </summary>
    Task RefreshAsync();
}
