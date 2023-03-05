using AutoMapper;
using Backend.DTOs.UserDto;
using Backend.Services.Helpers.Interfaces;
using Backend.Services.Interfaces;
using Backend.Settings;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[EnableCors]
public class UserController : ControllerBase
{
    private readonly IJwtService _jwtService;

    private readonly IMapper _mapper;

    private readonly ResponseSettings _responseMessages;

    private readonly ISecurityService _security;

    private readonly IUserService _userService;

    public UserController(IUserService userService, IMapper mapper, IJwtService jwtService, ISecurityService security,
        IOptions<ResponseSettings> responseMessages)
    {
        _userService = userService;
        _mapper = mapper;
        _jwtService = jwtService;
        _security = security;
        _responseMessages = responseMessages.Value;
    }

    /// <summary>
    ///     Gets detail about the own user profile.
    /// </summary>
    /// <returns name="UserResponseDto">The user-object and the corresponding resource url with the status code.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetOwnUser()
    {
        var username = _jwtService.ValidateToken(Request.Headers["Authorization"]);
        if (username.PayloadIsNull() || !(await _userService.UsernameExists(username.Payload)).Payload)
            return Unauthorized(_responseMessages.NotLoggedIn);

        var serviceResponse = await _userService.GetUserByUsername(username.Payload);
        if (serviceResponse.Success) return serviceResponse.ToUserResponseDto().ToObjectResult();

        return serviceResponse.ToObjectResult();
    }

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="userRequestDto">The user data for the user to be created.</param>
    /// <returns>the corresponding resource url with the status code.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponseDto))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserRequestDto userRequestDto)
    {
        var serviceResponse = await _userService.CreateUser(userRequestDto);
        return serviceResponse.Success
            ? serviceResponse.ToUserResponseDto().ToObjectResult()
            : serviceResponse.ToObjectResult();
    }

    /// <summary>
    ///     Logs a user in.
    /// </summary>
    /// <param name="userRequestDto">The user to be logged in.</param>
    /// <returns>Jwt Token of the user and the corresponding resource url with 200 status code.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> Login([FromBody] UserRequestDto userRequestDto)
    {
        var usernameExists = await _userService.UsernameExists(userRequestDto.Username);
        if (!usernameExists.Payload) return Unauthorized(_responseMessages.UserDoesNotExist);

        var serviceResponse = await _userService.Login(userRequestDto);
        return serviceResponse.ToObjectResult();
    }

    /// <summary>
    ///     Terminates a user account.
    /// </summary>
    /// <param name="userRequestDto">The user to be terminated.</param>
    /// <returns>the corresponding resource url with 200 status code.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> TerminateUserAccount([FromBody] UserRequestDto userRequestDto)
    {
        string jwtToken = Request.Headers["Authorization"];
        var username = _jwtService.ValidateToken(jwtToken);
        var usernameExists = await _userService.UsernameExists(username.Payload);

        if (!usernameExists.Payload) return usernameExists.ToObjectResult();
        if (!username.Success || username.PayloadIsNull()) return Unauthorized(_responseMessages.NotLoggedIn);
        if (userRequestDto.Username != username.Payload)
            return Forbid(_responseMessages.CannotTerminateAccountOfOtherUser);
        var userResponse = await _userService.GetUserByUsername(username.Payload);


        if (!_security.CheckPassword(userRequestDto.Password, userResponse.Payload.Salt, userResponse.Payload.Password))
            return Unauthorized(_responseMessages.NotLoggedIn);

        var serviceResponse = await _userService.TerminateAccount(userRequestDto, jwtToken);
        return serviceResponse.ToUserResponseDto().ToObjectResult();
    }
}