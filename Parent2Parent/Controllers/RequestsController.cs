using Microsoft.AspNetCore.Mvc;
using Parent2Parent.Models.Dto;
using Parent2Parent.Models.Dto.Requests;
using Parent2Parent.Services;

namespace Parent2Parent.Controllers;

[ApiController]
[Route("api/requests")]
public sealed class RequestsController : ControllerBase
{
    private readonly IRequestsService _requests;

    public RequestsController(IRequestsService requests)
    {
        _requests = requests;
    }

    [HttpPost("send")]
    public async Task<ActionResult<ApiResponse<object>>> Send([FromBody] SendRequestDto dto, CancellationToken ct)
    {
        var result = await _requests.SendRequestAsync(dto, ct);
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, result.Message));
        return Ok(new ApiResponse<object>(true, result.Message));
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ConnectionRequestDto>>>> ViewRequests([FromRoute] int userId, CancellationToken ct)
    {
        var result = await _requests.ViewRequestsAsync(userId, ct);
        return Ok(new ApiResponse<IReadOnlyList<ConnectionRequestDto>>(true, result.Message, result.Data));
    }

    [HttpGet("sent/{userId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ConnectionRequestDto>>>> ViewSentRequests([FromRoute] int userId, CancellationToken ct)
    {
        var result = await _requests.ViewSentRequestsAsync(userId, ct);
        return Ok(new ApiResponse<IReadOnlyList<ConnectionRequestDto>>(true, result.Message, result.Data));
    }

    [HttpPut("accept/{requestId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Accept([FromRoute] int requestId, CancellationToken ct)
    {
        var result = await _requests.AcceptRequestAsync(requestId, ct);
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, result.Message));
        return Ok(new ApiResponse<object>(true, result.Message));
    }

    [HttpPut("reject/{requestId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Reject([FromRoute] int requestId, CancellationToken ct)
    {
        var result = await _requests.RejectRequestAsync(requestId, ct);
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, result.Message));
        return Ok(new ApiResponse<object>(true, result.Message));
    }
}

