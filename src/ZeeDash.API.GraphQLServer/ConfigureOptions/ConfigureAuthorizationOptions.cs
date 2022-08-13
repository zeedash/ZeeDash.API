namespace ZeeDash.API.GraphQLServer.ConfigureOptions;

using ZeeDash.API.GraphQLServer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class ConfigureAuthorizationOptions : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options) =>
        options.AddPolicy(AuthorizationPolicyName.Admin, x => x.RequireAuthenticatedUser());
}
