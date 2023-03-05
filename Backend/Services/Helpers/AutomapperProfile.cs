using AutoMapper;
using Backend.DTOs;
using Backend.DTOs.UserDto;
using Backend.Persistence.Entities;

namespace Backend.Services.Helpers;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Example, ExampleDto>().ReverseMap();
        CreateMap<User, UserResponseDto>().ReverseMap();
        CreateMap<User, UserRequestDto>().ReverseMap();
    }
}