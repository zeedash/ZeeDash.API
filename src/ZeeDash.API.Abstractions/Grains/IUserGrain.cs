namespace ZeeDash.API.Abstractions.Grains;

using System.Threading.Tasks;
using Orleans;
using ZeeDash.API.Abstractions.Domains.Identity;

public interface IUserGrain : IGrainWithStringKey {

    Task<User> CreateAsync(string email, string fullName);

    Task<User> GetAsync();
}
