using Backend.Persistence.Models;

namespace Backend.Services.Interfaces;

public interface IExampleService
{
    public Task<IEnumerable<Example>> GetAllExamples();
    public Task<Example> CreateExample();
}