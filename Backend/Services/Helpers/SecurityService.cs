using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Persistence.Entities;
using Backend.Services.Helpers.Interfaces;
using Backend.Settings;
using Microsoft.Extensions.Options;

namespace Backend.Services.Helpers;

public class SecurityService : ISecurityService
{
    private readonly IConfiguration _config;

    private readonly JwtService _jwtService;
    private readonly ResponseSettings _responseMessages;

    public SecurityService(IConfiguration configuration, JwtService jwtService,
        IOptions<ResponseSettings> responseMessages)
    {
        _config = configuration;
        _jwtService = jwtService;
        _responseMessages = responseMessages.Value;
    }

    /// <summary>
    ///     Logs a user in.
    /// </summary>
    /// <param name="password">The password of the user.</param>
    /// <param name="user">The user object.</param>
    /// <returns>
    ///     The generated Jwt token or null.
    /// </returns>
    public ServiceResponse<string> Login(string password, User user)
    {
        if (CheckPassword(password, user.Salt, user.Password))
            return _jwtService.GenerateToken(
                user.Username,
                new List<Claim>
                {
                    new("Id", user.Id.ToString())
                });

        return new ServiceResponse<string>("", false, StatusCodes.Status401Unauthorized,
            _responseMessages.PasswordOrUsernameWrong);
    }

    /// <summary>
    ///     Generates a random salt for a user.
    /// </summary>
    /// <returns>
    ///     The random salt.
    /// </returns>
    public string GenerateSalt()
    {
        var length = _config["Security:maxLength"];
        var possibleChars = _config["Security:possibleChars"];
        var chars = new char[Convert.ToInt32(length)];
        var random = new Random();
        for (var i = 0; i < chars.Length; i++) chars[i] = possibleChars[random.Next(possibleChars.Length)];

        return new string(chars);
    }

    /// <summary>
    ///     Salts a password (concatenates the password and salt).
    /// </summary>
    /// <param name="salt">The salt of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>
    ///     The salted password.
    /// </returns>
    public string SaltPassword(string salt, string password)
    {
        return password + salt;
    }

    /// <summary>
    ///     Encrypts a password.
    /// </summary>
    /// <param name="saltedPassword">The salted password to be encrypted.</param>
    /// <returns>
    ///     The encrypted password.
    /// </returns>
    public string EncryptPassword(string saltedPassword)
    {
        return ComputeSha256Hash(saltedPassword);
    }

    /// <summary>
    ///     Encrypts a password.
    /// </summary>
    /// <param name="givenPassword">The password which a user has given to authenticate as the user.</param>
    /// <param name="salt">The salt of the user.</param>
    /// <param name="hashedSaltedPassword">The salted and hashed password as it is found in the database.</param>
    /// <returns>
    ///     A boolean value of whether the given password is correct.
    /// </returns>
    public bool CheckPassword(string givenPassword, string salt, string hashedSaltedPassword)
    {
        return EncryptPassword(givenPassword + salt) == hashedSaltedPassword;
    }

    // https://www.c-sharpcorner.com/article/compute-sha256-hash-in-c-sharp/#:~:text=Hashing%20(also%20known%20as%20hash,as%20passwords%20and%20digital%20signatures.

    /// <summary>
    ///     Computes the Sha256 Hash of a string.
    /// </summary>
    /// <param name="rawData">The string that is to be hashed.</param>
    /// <returns>
    ///     The Sha 256 Hash of the string.
    /// </returns>
    public string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256
        using (var sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            // Convert byte array to a string
            var builder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));

            return builder.ToString();
        }
    }
}