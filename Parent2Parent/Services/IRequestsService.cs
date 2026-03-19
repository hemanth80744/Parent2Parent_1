using Parent2Parent.Models.Dto.Requests;

namespace Parent2Parent.Services;

public interface IRequestsService
{
    Task<ServiceResult<object>> SendRequestAsync(SendRequestDto dto, CancellationToken ct);
    Task<ServiceResult<IReadOnlyList<ConnectionRequestDto>>> ViewRequestsAsync(int userId, CancellationToken ct);
    Task<ServiceResult<IReadOnlyList<ConnectionRequestDto>>> ViewSentRequestsAsync(int userId, CancellationToken ct);
    Task<ServiceResult<object>> AcceptRequestAsync(int requestId, CancellationToken ct);
    Task<ServiceResult<object>> RejectRequestAsync(int requestId, CancellationToken ct);
}

