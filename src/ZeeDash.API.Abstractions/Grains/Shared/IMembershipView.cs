namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;

/// <summary>
/// Methods for managing access control of the grain
/// </summary>
public interface IMembershipView {

    /// <summary>
    /// Get the list of all members of the grain
    /// </summary>
    /// <param name="level">The level to list</param>
    /// <returns>The list of all <see cref="Member"/> of the grain, ordered by highest role first</returns>
    Task<List<Member>> GetMembersAsync(AccessLevel? level = null);
}
