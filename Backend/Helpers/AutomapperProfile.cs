using AutoMapper;
using Backend.DTOs;
using Backend.Persistence.Models;

namespace Backend.Helpers;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Example, ExampleDto>().ReverseMap();
    }
}
