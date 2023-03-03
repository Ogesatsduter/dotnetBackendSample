namespace Backend.DTOs;

public class ExampleDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = String.Empty;
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}