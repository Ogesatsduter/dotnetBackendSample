using Backend.Persistence;
using Backend.Services;
using Backend.Services.Helpers;
using Backend.Services.Helpers.Auth;
using Backend.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=postgres-database;Database=postgres;Port=5432;User Id=admin;Password=topsecret;"));


builder.Services
    .AddScoped<UserService>()
    .AddSingleton<SecurityService>()
    .AddSingleton<JwtService>()
    .AddAutoMapper(typeof(AutomapperProfile))
    .Configure<ResponseSettings>(builder.Configuration.GetSection(nameof(ResponseSettings)));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// automatically creates the database and the tables
using (var serviceScope =
       ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        context.Database.Migrate();
    }
    catch (Npgsql.PostgresException)
    {
        // Ignore, Database Already exists
    }
}


app.UseAuthorization();

app.MapControllers();

app.Run();