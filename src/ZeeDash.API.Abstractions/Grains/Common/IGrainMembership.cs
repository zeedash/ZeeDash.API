namespace ZeeDash.API.Abstractions.Grains.Common;

using System.Threading.Tasks;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;

/// <summary>
/// Methods for managing access control of the grain
/// </summary>
public interface IGrainMembership {

    /// <summary>
    /// Get the list of all members of the grain
    /// </summary>
    /// <param name="level">The level to list</param>
    /// <param name="kind">The level to list</param>
    /// <returns>The list of all <see cref="Member"/> of the grain</returns>
    Task<List<Member>> GetMembersAsync(AccessLevel? level = null, AccessLevelKind? kind = null);

    /// <summary>
    /// Set a user as <see cref="AccessLevel.Owner"/> of the grain
    /// </summary>
    /// <param name="userId">The identifier of the user to affect</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Task<Member> SetOwnerAsync(UserId userId);

    /// <summary>
    /// Set a user as <see cref="AccessLevel.Contributor"/> of the grain
    /// </summary>
    /// <param name="userId">The identifier of the user to affect</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Task<Member> SetContributorAsync(UserId userId);

    /// <summary>
    /// Set a user as <see cref="AccessLevel.Reader"/> of the grain
    /// </summary>
    /// <param name="userId">The identifier of the user to affect</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Task<Member> SetReaderAsync(UserId userId);

    /// <summary>
    /// Remove a <see cref="Member"/> to the grain
    /// </summary>
    /// <param name="userId">The identifier of the user to affect</param>
    /// <returns>The <see cref="Member"/> removed, or null if it wasn't managed on this grain</returns>
    Task<Member> RemoveMemberAsync(UserId userId);
}
