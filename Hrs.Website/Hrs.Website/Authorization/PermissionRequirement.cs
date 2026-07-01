using Microsoft.AspNetCore.Authorization;

namespace Hrs.Website.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
