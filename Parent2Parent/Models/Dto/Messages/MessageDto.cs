namespace Parent2Parent.Models.Dto.Messages;

public sealed class MessageDto
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime? SentAt { get; set; }
}

