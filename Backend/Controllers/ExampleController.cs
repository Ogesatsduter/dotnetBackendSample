using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Persistence;
using Backend.Persistence.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ExampleController(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        
        /// <summary>
        /// Get all examples.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<Example>> GetAll()
        {
            return await _db.Example.ToListAsync();
        }
        
        /// <summary>
        /// Create an example.
        /// </summary>
        [HttpPost(Name = "CreateAnExample")]
        public async Task<Example> CreateRandom()
        {
            Example exampleToBeCreated = new Example()
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
}