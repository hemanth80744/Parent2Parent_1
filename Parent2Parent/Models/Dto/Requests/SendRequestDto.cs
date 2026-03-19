using System.ComponentModel.DataAnnotations;

namespace Parent2Parent.Models.Dto.Requests;

public sealed class SendRequestDto
{
    [Required]
    public int SenderId { get; set; }

    [Required]
    public int ReceiverId { get; set; }
}

