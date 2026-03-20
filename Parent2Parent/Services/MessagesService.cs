using System.Data;
using Microsoft.Data.SqlClient;
using Parent2Parent.Data;
using Parent2Parent.Models.Dto.Messages;

namespace Parent2Parent.Services;

public sealed class MessagesService : IMessagesService
{
    private readonly IDbHelper _db;

    public MessagesService(IDbHelper db)
    {
        _db = db;
    }

    public async Task<ServiceResult<object>> SendMessageAsync(SendMessageDto dto, CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@SenderId", SqlDbType.Int) { Value = dto.SenderId },
            new SqlParameter("@ReceiverId", SqlDbType.Int) { Value = dto.ReceiverId },
            new SqlParameter("@Message", SqlDbType.VarChar, -1) { Value = dto.Message },
        };

        // sp_send_message uses PRINT for error handling, which doesn't affect rows affected.
        // We might need to handle this differently if we want to capture the PRINT message.
        await _db.ExecuteNonQueryAsync("sp_send_message", parameters, ct);
        return ServiceResult<object>.Ok(null, "Message sent.");
    }

    public async Task<ServiceResult<IReadOnlyList<MessageDto>>> GetChatMessagesAsync(int user1, int user2, CancellationToken ct)
    {
        try
        {
            var parameters = new[]
            {
                new SqlParameter("@User1", SqlDbType.Int) { Value = user1 },
                new SqlParameter("@User2", SqlDbType.Int) { Value = user2 },
            };

            var messages = await _db.QueryAsync("sp_get_messages", MapMessage, parameters, ct);
            return ServiceResult<IReadOnlyList<MessageDto>>.Ok(messages, "OK");
        }
        catch (Exception ex)
        {
            // Log the actual error to help debugging
            Console.WriteLine($"Error in GetChatMessagesAsync: {ex.Message}");
            return ServiceResult<IReadOnlyList<MessageDto>>.Fail($"Database error: {ex.Message}");
        }
    }

    private static MessageDto MapMessage(SqlDataReader r)
    {
        try
        {
            var dto = new MessageDto();
            
            // Loop through columns for maximum resilience against type/name mismatches
            for (int i = 0; i < r.FieldCount; i++)
            {
                string col = r.GetName(i).ToLower();
                object val = r.GetValue(i);
                if (val == DBNull.Value) continue;

                try
                {
                    if (col == "senderid") dto.SenderId = Convert.ToInt32(val);
                    else if (col == "receiverid") dto.ReceiverId = Convert.ToInt32(val);
                    else if (col == "message") dto.Message = val.ToString() ?? "";
                    else if (col == "sentat") dto.SentAt = Convert.ToDateTime(val);
                }
                catch (Exception mappingEx)
                {
                    Console.WriteLine($"Mapping error for column {col}: {mappingEx.Message}");
                }
            }

            return dto;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical error in MapMessage: {ex.Message}");
            return new MessageDto();
        }
    }
}
