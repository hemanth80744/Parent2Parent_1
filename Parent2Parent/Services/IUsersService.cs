using Parent2Parent.Models.Dto.Users;

namespace Parent2Parent.Services;

public interface IUsersService
{
    Task<ServiceResult<object>> RegisterAsync(RegisterRequestDto dto, CancellationToken ct);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto dto, CancellationToken ct);
    Task<ServiceResult<IReadOnlyList<SchoolSearchResultDto>>> SearchSchoolAsync(string schoolName, CancellationToken ct);
}

