using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hrs.Api.Shared.Dtos;
using Hrs.Api.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Hrs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IOptions<JwtSettings> jwtSettings) : ControllerBase
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    [HttpPost("login")]
    
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        var username = loginDto.Username;
        if (loginDto.Username != _jwtSettings.Username || loginDto.Password != _jwtSettings.Password)
        {
            return Unauthorized(new { Message = "Invalid username or password." });
        }

        // Create JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        
        var permissions = new List<string> { "hotel.view", "hotel.create" };
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
        claims.AddRange(permissions.Select(p => new Claim("Permission", p)));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = System.DateTime.UtcNow.AddHours(1),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var response = new AuthResponseDto
        {
            Token = tokenString,
            Name = username,
            Username = "",
            Role = "User",
            Permissions = permissions
        };
        return Ok(response);
    }
}