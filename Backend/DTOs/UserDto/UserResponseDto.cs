using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.UserDto;

public class UserResponseDto
{
    public long Id { get; set; }

    [Required] [MaxLength(128)] public string Username { get; set; } = string.Empty;

    [Required] public DateTime CreatedAt { get; set; }

    public string? Jwt { get; set; }
}