using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Services.Helpers.Auth.Interfaces;
using Backend.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services.Helpers.Auth;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly ResponseSettings _responseMessages;

    public JwtService(IConfiguration configuration, IOptions<ResponseSettings> responseMessages)
    {
        _config = configuration;
        _responseMessages = responseMessages.Value;
    }

    /// <summary>
    ///     Generates a Jwt token with the username and additional claims if needed.
    /// </summary>
    /// <param name="username">The username of the user for which the Jwt token will be generated ("sub" value in Jwt).</param>
    /// <param name="claims">Additional claim that can be added to the Jwt token.</param>
    /// <returns>
    ///     A Jwt token for the user.
    /// </returns>
    public ServiceResponse<string> GenerateToken(string username, List<Claim>? claims = null)
    {
        var authClaims = new List<Claim>
        {
            new(_config["JWT:subject"] ?? "sub", username)
        };
        if (claims != null) authClaims.AddRange(claims);
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(CreateToken(authClaims));
        return new ServiceResponse<string>(jwtToken, true, StatusCodes.Status200OK);
    }

    /// <summary>
    ///     Validates a Jwt token and returns the subject (the username of the user it was issued to).
    /// </summary>
    /// <param name="token">The Jwt token that will be validated.</param>
    /// <returns>
    ///     The username or null.
    /// </returns>
    public ServiceResponse<string> ValidateToken(string token)
    {
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(
                token.Replace("Bearer ", ""),
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:secret"] ?? "sub")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                },
                out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            return new ServiceResponse<string>(jwtToken.Claims.First(x => x.Type == _config["JWT:subject"]).Value, true,
                StatusCodes.Status200OK);
        }
        catch
        {
            return new ServiceResponse<string>("", false, StatusCodes.Status500InternalServerError,
                _responseMessages.NotLoggedIn);
        }
    }

    /// <summary>
    ///     Generates and returns a Jwt token.
    /// </summary>
    /// <param name="authClaims">The claims that are included in the Jwt token.</param>
    /// <returns>
    ///     The generated token.
    /// </returns>
    public JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        var signedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:secret"] ?? "sub"));
        var generatedToken = new JwtSecurityToken(
            _config["JWT:issuer"],
            expires: DateTime.Now.AddHours(24), // remove hardcoded expire date
            claims: authClaims,
            signingCredentials: new SigningCredentials(signedKey, SecurityAlgorithms.HmacSha256));
        return generatedToken;
    }
}