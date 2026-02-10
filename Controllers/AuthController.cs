using EnterpriseRequestManagement.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnterpriseRequestManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        //  ROLE assignment 
        // requester@test.com -> Requester
        // approver@test.com  -> Approver
        // admin@test.com     -> Admin
        var role =
     dto.Email.Contains("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" :
     dto.Email.Contains("approver", StringComparison.OrdinalIgnoreCase) ? "Approver" :
     "Requester";

        var userId =
            dto.Email.Contains("admin", StringComparison.OrdinalIgnoreCase) ? 3 :
            dto.Email.Contains("approver", StringComparison.OrdinalIgnoreCase) ? 2 :
            1;

        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
    new Claim(ClaimTypes.Name, dto.Email),
    new Claim(ClaimTypes.Email, dto.Email),
    new Claim(ClaimTypes.Role, role)
};

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(
            Convert.ToDouble(_config["Jwt:DurationInMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            role
        });
    }
}
