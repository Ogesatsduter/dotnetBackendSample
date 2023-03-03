using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Persistence;
using Backend.Persistence.Models;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        
        private readonly IExampleService _exampleService;
        public ExampleController(IExampleService exampleService)
        {
            _exampleService = exampleService;
        }
        
        /// <summary>
        /// Get all examples.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<Example>> GetAll()
        {
            return await _exampleService.GetAllExamples(); 
        }
        
        /// <summary>
        /// Create an example.
        /// </summary>
        [HttpPost(Name = "CreateAnExample")]
        public async Task<Example> CreateRandom()
        {
            return await _exampleService.CreateExample();
        }
        
    }
}