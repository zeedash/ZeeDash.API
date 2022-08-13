namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;

public interface IUserGrain : IGrainWithStringKey {

    /// <summary>
    /// Create a grain dedicated to the user
    /// </summary>
    /// <param name="fullName">the user's fullname</param>
    /// <param name="email">The user's email</param>
    /// <returns>The user newly created</returns>
    Task<User> CreateAsync(string fullName, string email);

    /// <summary>
    /// Get the user data
    /// </summary>
    /// <returns>The User</returns>
    Task<User> GetAsync();

    /// <summary>
    /// Update the user's email
    /// </summary>
    /// <param name="email">The new email address</param>
    /// <returns>The User</returns>
    Task<User> ChangeEmailAsync(string email);

    /// <summary>
    ///Update the user's full name
    /// </summary>
    /// <param name="fullName">The new full name</param>
    /// <returns>The User</returns>
    Task<User> ChangeFullNameAsync(string fullName);
}
