using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using Hrs.Api.Shared.Dtos;

namespace Hrs.Api.Filters;

public class BasicAuthFilter : IAuthorizationFilter
{
    private readonly JwtSettings _settings;

    public BasicAuthFilter(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var encoded = authHeader.Substring("Basic ".Length).Trim();
        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            var parts = decoded.Split(':', 2);
            if (parts.Length != 2 || parts[0] != _settings.Username || parts[1] != _settings.Password)
            {
                context.Result = new UnauthorizedResult();
            }
            // else authorized – user identity name set to username
            else
            {
                var claims = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, parts[0])
                }, "Basic");
                context.HttpContext.User = new System.Security.Claims.ClaimsPrincipal(claims);
            }
        }
        catch
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
