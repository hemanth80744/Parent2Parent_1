using Microsoft.AspNetCore.Mvc;
using Parent2Parent.Models.Dto;
using Parent2Parent.Models.Dto.Users;
using Parent2Parent.Services;

namespace Parent2Parent.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUsersService _users;

    public UsersController(IUsersService users)
    {
        _users = users;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterRequestDto dto, CancellationToken ct)
    {
        var result = await _users.RegisterAsync(dto, ct);
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, result.Message));
        return Ok(new ApiResponse<object>(true, result.Message));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
    {
        var result = await _users.LoginAsync(dto, ct);
        if (!result.Success) return Unauthorized(new ApiResponse<AuthResponseDto>(false, result.Message));
        return Ok(new ApiResponse<AuthResponseDto>(true, result.Message, result.Data));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SchoolSearchResultDto>>>> SearchSchool([FromQuery] string schoolName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(schoolName))
            return BadRequest(new ApiResponse<IReadOnlyList<SchoolSearchResultDto>>(false, "schoolName is required."));

        var result = await _users.SearchSchoolAsync(schoolName, ct);
        return Ok(new ApiResponse<IReadOnlyList<SchoolSearchResultDto>>(true, result.Message, result.Data));
    }
}

