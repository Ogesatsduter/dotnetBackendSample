using Backend.DTOs.UserDto;
using Backend.Persistence;
using Backend.Persistence.Entities;
using Backend.Services.Helpers;
using Backend.Services.Interfaces;
using Backend.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    private readonly JwtService _jwtService;

    private readonly ResponseSettings _responseMessages;

    private readonly SecurityService _security;

    public UserService(AppDbContext db, SecurityService security, JwtService jwtService,
        IOptions<ResponseSettings> responseMessages)
    {
        _db = db;
        _security = security;
        _jwtService = jwtService;
        _responseMessages = responseMessages.Value;
    }

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="userRequest"></param>
    /// <returns>The http status code and the user object or an error message.</returns>
    public async Task<ServiceResponse<User>> CreateUser(UserRequestDto userRequest)
    {
        if ((await UsernameExists(userRequest.Username)).Payload)
            return new ServiceResponse<User>(null, false, 403, _responseMessages.UsernameAlreadyTaken);

        User newUser = new()
        {
            Salt = _security.GenerateSalt()
        };

        newUser.Password = _security.EncryptPassword(_security.SaltPassword(newUser.Salt, userRequest.Password));
        Console.WriteLine(newUser.Id);
        _db.Add(newUser);
        await _db.SaveChangesAsync();
        return new ServiceResponse<User>(newUser, true, StatusCodes.Status201Created, "User successfully created");
    }

    /// <summary>
    ///     Gets a user by its username.
    /// </summary>
    /// <param name="username">The username of the user to be gotten.</param>
    /// <returns>The http status code and the user object or an error message.</returns>
    public async Task<ServiceResponse<User>> GetUserByUsername(string username)
    {
        try
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
            return new ServiceResponse<User>(user, true, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            return new ServiceResponse<User>(null, false, StatusCodes.Status404NotFound,
                _responseMessages.UserNotFound);
        }
    }

    /// <summary>
    ///     Logs a user in.
    /// </summary>
    /// <param name="userRequestDto">The password and username of the user.</param>
    /// <returns>The http status code and the jwt token for the user or an error message.</returns>
    public async Task<ServiceResponse<string>> Login(UserRequestDto userRequestDto)
    {
        var serviceResponse = await GetUserByUsername(userRequestDto.Username);
        if (!serviceResponse.Success)
            return new ServiceResponse<string>("", false, StatusCodes.Status401Unauthorized,
                _responseMessages.PasswordOrUsernameWrong);

        return _security.Login(userRequestDto.Password, serviceResponse.Payload);
    }

    /// <summary>
    ///     Terminates an account.
    /// </summary>
    /// <param name="userRequestDto">The username and password of the user.</param>
    /// <param name="jwtToken">The jwt token of the user.</param>
    /// <returns>The http status code and the message.</returns>
    public async Task<ServiceResponse<User>> TerminateAccount(UserRequestDto userRequestDto, string jwtToken)
    {
        var username = _jwtService.ValidateToken(jwtToken);


        var user = await GetUserByUsername(username.Payload);

        if (userRequestDto.Username != username.Payload)
            return new ServiceResponse<User>(null, false, StatusCodes.Status400BadRequest,
                _responseMessages.CannotTerminateAccountOfOtherUser);

        if (!user.Success || user.PayloadIsNull())
            return new ServiceResponse<User>(null, false, StatusCodes.Status404NotFound,
                _responseMessages.UserNotFound);

        if (!_security.CheckPassword(userRequestDto.Password, user.Payload.Salt, user.Payload.Password))
            return new ServiceResponse<User>(null, false, StatusCodes.Status401Unauthorized,
                _responseMessages.PasswordOrUsernameWrong);

        try
        {
            _db.Remove(user);
            await _db.SaveChangesAsync();
            return new ServiceResponse<User>(user.Payload, true, StatusCodes.Status200OK, "Deleted user");
        }
        catch
        {
            return new ServiceResponse<User>(null, false, StatusCodes.Status409Conflict,
                _responseMessages.CouldNotRemoveUser);
        }
    }


    /// <summary>
    ///     Checks if a username already exists.
    /// </summary>
    /// <param name="username">The username to be searched for.</param>
    /// <returns>The http status code and a boolean value of whether the username already exists.</returns>
    public async Task<ServiceResponse<bool>> UsernameExists(string username)
    {
        return new ServiceResponse<bool>(await _db.Users.AnyAsync(u => u.Username == username), true,
            StatusCodes.Status200OK);
    }
}