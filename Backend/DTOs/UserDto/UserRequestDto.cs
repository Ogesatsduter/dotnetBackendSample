using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.UserDto;

public class UserRequestDto
{
    public int Id { get; set; }

    [MaxLength(128)] [Required] public string Username { get; set; }

    [MaxLength(256)]
    [MinLength(6)]
    [Required]
    public string Password { get; set; }
}