using AutoMapper;
using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ExampleController : ControllerBase
{
    private readonly IExampleService _exampleService;
    private readonly IMapper _mapper;

    public ExampleController(IExampleService exampleService, IMapper mapper)
    {
        _exampleService = exampleService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Get all examples.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<ExampleDto>> GetAll()
    {
        List<ExampleDto> examples = new();
        foreach (var example in await _exampleService.GetAllExamples()) examples.Add(_mapper.Map<ExampleDto>(example));
        return examples;
    }

    /// <summary>
    ///     Create an example.
    /// </summary>
    [HttpPost(Name = "CreateAnExample")]
    public async Task<ExampleDto> CreateRandom()
    {
        return _mapper.Map<ExampleDto>(await _exampleService.CreateExample());
    }
}