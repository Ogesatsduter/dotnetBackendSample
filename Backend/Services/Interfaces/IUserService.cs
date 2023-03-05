using Backend.DTOs.UserDto;
using Backend.Persistence.Entities;
using Backend.Services.Helpers;

namespace Backend.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResponse<User>> CreateUser(UserRequestDto newUser);

    Task<ServiceResponse<User>> GetUserByUsername(string username);

    Task<ServiceResponse<string>> Login(UserRequestDto userRequestDto);

    Task<ServiceResponse<User>> TerminateAccount(UserRequestDto userRequestDto, string jwtToken);

    Task<ServiceResponse<bool>> UsernameExists(string username);
}