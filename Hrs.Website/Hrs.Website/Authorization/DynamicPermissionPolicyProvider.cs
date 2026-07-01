using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Hrs.Website.Authorization;

public class DynamicPermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = options.Value.GetPolicy(policyName);
        if (policy != null)
        {
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        var policyBuilder = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName));

        return Task.FromResult<AuthorizationPolicy?>(policyBuilder.Build());
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}
