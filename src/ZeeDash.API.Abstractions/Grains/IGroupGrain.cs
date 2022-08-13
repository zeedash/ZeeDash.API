namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;

public interface IGroupGrain : IGrainWithStringKey {

    /// <summary>
    /// Create a ne group with its name
    /// </summary>
    /// <param name="name">Name of the group to create</param>
    /// <returns>The newly created group</returns>
    Task<Group> CreateAsync(string name);

    /// <summary>
    /// Get the group name and users list (ids) only
    /// </summary>
    /// <returns>The group (raw)</returns>
    Task<Group> GetAsync();

    /// <summary>
    /// Get the list of all users belonging to the group
    /// </summary>
    /// <returns>The users list</returns>
    Task<List<User>> GetUsersAsync();

    /// <summary>
    /// Add a user to the group
    /// </summary>
    /// <param name="userId">The identifier of the user to add</param>
    /// <returns>The group (raw)</returns>
    Task<Group> AddUserAsync(UserId userId);

    /// <summary>
    /// Remove a user from the group
    /// </summary>
    /// <param name="userId">The identifier of the user to remove</param>
    /// <returns>The group (raw)</returns>
    Task<Group> RemoveUserAsync(UserId userId);
}
