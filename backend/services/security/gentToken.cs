using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using moviesApi.Models;

namespace moviesApi.Services;

public static class GenJwtToken
{
    public static string CreateToken(User user)
    {
        IdentityOptions _options = new IdentityOptions();

        List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier as String, user.Id.ToString()),
                new Claim(ClaimTypes.Role as String, ((Roles)user.Role).ToString())
            };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("ThisIsTheStrongestSecret"));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            issuer: "https://localhost.local",
            audience: "https://localhost.local",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}