using Backend.Persistence.Entities;

namespace Backend.Services.Helpers.Auth.Interfaces;

public interface ISecurityService
{
    ServiceResponse<string> Login(string password, User user);

    string GenerateSalt();

    string SaltPassword(string salt, string password);

    string EncryptPassword(string saltedPassword);

    bool CheckPassword(string givenPassword, string salt, string hashedSaltedPassword);

    string ComputeSha256Hash(string rawData);
}