using Backend.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Persistence; 

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Example> Example { get; set; }
}