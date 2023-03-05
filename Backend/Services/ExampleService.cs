using Backend.Persistence;
using Backend.Persistence.Entities;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ExampleService : IExampleService
{
    private readonly AppDbContext _db;

    public ExampleService(AppDbContext dbContext)
    {
        _db = dbContext;
    }

    public async Task<IEnumerable<Example>> GetAllExamples()
    {
        return await _db.Example.ToListAsync();
    }

    public async Task<Example> CreateExample()
    {
        var exampleToBeCreated = new Example
        {
            Name = "Cap",
            Number = 187,
            Date = DateTime.UtcNow,
            Summary = "A short summary which, by the way, is actually really very short",
            TemperatureC = 2023
        };
        _db.Example.Add(exampleToBeCreated);
        await _db.SaveChangesAsync();
        return exampleToBeCreated;
    }
}