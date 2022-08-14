namespace ZeeDash.API.Abstractions.Grains.Common;

using System.Threading.Tasks;
using ZeeDash.API.Abstractions.Domains.IAM;
using ZeeDash.API.Abstractions.Domains.Identity;

/// <summary>
/// Methods for managing access control of the grain
/// </summary>
public interface IGrainMembership
    : IGrainMembership<UserId>
    , IGrainMembership<GroupId> { }

/// <summary>
/// Methods for managing access control of the grain
/// </summary>
public interface IGrainMembership<TId> {

    /// <summary>
    /// Set a user as <see cref="AccessLevel.Owner"/> of the grain
    /// </summary>
    /// <param name="memberId">The identifier of the member to affect</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Task<Member> SetOwnerAsync(TId memberId);

    /// <summary>
    /// Set a user as <see cref="AccessLevel.Contributor"/> of the grain
    /// </summary>
    /// <param name="memberId">The identifier of the member to affect</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Task<Member> SetContributorAsync(TId memberId);

    /// <summary>
    /// Set a user as <see cref="AccessLevel.Reader"/> of the grain
    /// </summary>
    /// <param name="memberId">The identifier of the member to affect</param>
    /// <returns>The <see cref="Member"/> as managed by the grain</returns>
    Task<Member> SetReaderAsync(TId memberId);

    /// <summary>
    /// Remove a <see cref="Member"/> to the grain
    /// </summary>
    /// <param name="memberId">The identifier of the user to affect</param>
    /// <returns>The <see cref="Member"/> removed, or null if it wasn't managed on this grain</returns>
    Task<Member> RemoveMemberAsync(TId memberId);
}
