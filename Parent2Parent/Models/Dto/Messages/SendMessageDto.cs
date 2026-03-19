using System.ComponentModel.DataAnnotations;

namespace Parent2Parent.Models.Dto.Messages;

public sealed class SendMessageDto
{
    [Required]
    public int SenderId { get; set; }

    [Required]
    public int ReceiverId { get; set; }

    [Required, StringLength(4000)]
    public string Message { get; set; } = string.Empty;
}

