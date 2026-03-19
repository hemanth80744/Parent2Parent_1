using System.ComponentModel.DataAnnotations;

namespace Parent2Parent.Models.Dto.Users;

public sealed class LoginRequestDto
{
    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Password { get; set; } = string.Empty;
}

