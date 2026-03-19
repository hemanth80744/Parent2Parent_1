using Parent2Parent.Models.Dto.Messages;

namespace Parent2Parent.Services;

public interface IMessagesService
{
    Task<ServiceResult<object>> SendMessageAsync(SendMessageDto dto, CancellationToken ct);
    Task<ServiceResult<IReadOnlyList<MessageDto>>> GetChatMessagesAsync(int user1, int user2, CancellationToken ct);
}

