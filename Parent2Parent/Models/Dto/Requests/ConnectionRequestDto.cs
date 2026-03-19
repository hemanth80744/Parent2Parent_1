using System.Text.Json.Serialization;

namespace Parent2Parent.Models.Dto.Requests;

public sealed class ConnectionRequestDto
{
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }

    [JsonPropertyName("senderId")]
    public int SenderId { get; set; }

    [JsonPropertyName("senderName")]
    public string SenderName { get; set; } = string.Empty;

    [JsonPropertyName("receiverId")]
    public int ReceiverId { get; set; }

    [JsonPropertyName("receiverName")]
    public string? ReceiverName { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
}

