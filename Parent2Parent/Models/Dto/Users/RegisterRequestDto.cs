using System.ComponentModel.DataAnnotations;

namespace Parent2Parent.Models.Dto.Users;

public sealed class RegisterRequestDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 4)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string School { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Class { get; set; } = string.Empty;
}

