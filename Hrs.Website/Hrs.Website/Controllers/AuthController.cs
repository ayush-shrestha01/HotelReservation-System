using System.Security.Claims;
using Hrs.Website.Business.AuthBusiness;
using Hrs.Website.Shared.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrs.Website.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var authBusiness = HttpContext.RequestServices.GetRequiredService<IAuthBusiness>();
        var result = await authBusiness.Login(model);

        if (result != null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, result.Name),
                new(ClaimTypes.Email, result.Email),
                new(ClaimTypes.Role, result.Role ?? string.Empty),
                new("Token", result.Token),
            };

            foreach (var permission in result.Permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return View(model);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
