using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Services.Helpers.Interfaces;

public interface IJwtService
{
    ServiceResponse<string> GenerateToken(string username, List<Claim>? claims = null);

    ServiceResponse<string> ValidateToken(string token);

    JwtSecurityToken CreateToken(List<Claim> authClaims);
}