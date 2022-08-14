namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;
using ZeeDash.API.Abstractions.Grains.Common;

public interface IGroupGrain
    : IGrainWithStringKey
    , IGrainMembership<UserId> {

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
}
