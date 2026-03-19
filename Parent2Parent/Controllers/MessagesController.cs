using Microsoft.AspNetCore.Mvc;
using Parent2Parent.Models.Dto;
using Parent2Parent.Models.Dto.Messages;
using Parent2Parent.Services;

namespace Parent2Parent.Controllers;

[ApiController]
[Route("api/messages")]
public sealed class MessagesController : ControllerBase
{
    private readonly IMessagesService _messages;

    public MessagesController(IMessagesService messages)
    {
        _messages = messages;
    }

    [HttpPost("send")]
    public async Task<ActionResult<ApiResponse<object>>> Send([FromBody] SendMessageDto dto, CancellationToken ct)
    {
        var result = await _messages.SendMessageAsync(dto, ct);
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, result.Message));
        return Ok(new ApiResponse<object>(true, result.Message));
    }

    [HttpGet("chat")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MessageDto>>>> Chat([FromQuery] int user1, [FromQuery] int user2, CancellationToken ct)
    {
        if (user1 <= 0 || user2 <= 0)
            return BadRequest(new ApiResponse<IReadOnlyList<MessageDto>>(false, "user1 and user2 are required."));

        var result = await _messages.GetChatMessagesAsync(user1, user2, ct);
        if (!result.Success)
            return StatusCode(500, new ApiResponse<IReadOnlyList<MessageDto>>(false, result.Message));

        return Ok(new ApiResponse<IReadOnlyList<MessageDto>>(true, result.Message, result.Data));
    }
}

