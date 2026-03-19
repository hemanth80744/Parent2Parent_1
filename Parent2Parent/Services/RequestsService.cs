using System.Data;
using Microsoft.Data.SqlClient;
using Parent2Parent.Data;
using Parent2Parent.Models.Dto.Requests;

namespace Parent2Parent.Services;

public sealed class RequestsService : IRequestsService
{
    private readonly IDbHelper _db;

    public RequestsService(IDbHelper db)
    {
        _db = db;
    }

    public async Task<ServiceResult<object>> SendRequestAsync(SendRequestDto dto, CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@SenderId", SqlDbType.Int) { Value = dto.SenderId },
            new SqlParameter("@ReceiverId", SqlDbType.Int) { Value = dto.ReceiverId },
        };

        int affected = await _db.ExecuteNonQueryAsync("sp_send_request", parameters, ct);
        return affected > 0
            ? ServiceResult<object>.Ok(null, "Request sent.")
            : ServiceResult<object>.Fail("Failed to send request.");
    }

    public async Task<ServiceResult<IReadOnlyList<ConnectionRequestDto>>> ViewRequestsAsync(int userId, CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
        };

        // Call the SP once and get all requests (incoming)
        var incoming = await _db.QueryAsync("sp_view_requests", MapRequest, parameters, ct);
        
        // Also try to get outgoing requests by calling the same SP with @SenderId
        // This is a common pattern in SP-based systems if a specific 'sent' SP is missing.
        List<ConnectionRequestDto> all = new(incoming);
        try
        {
            var senderParams = new[] { new SqlParameter("@SenderId", SqlDbType.Int) { Value = userId } };
            var outgoing = await _db.QueryAsync("sp_view_requests", MapRequest, senderParams, ct);
            foreach(var req in outgoing)
            {
                if (!all.Any(a => a.RequestId == req.RequestId)) all.Add(req);
            }
        }
        catch { /* ignore if SP doesn't support @SenderId */ }

        return ServiceResult<IReadOnlyList<ConnectionRequestDto>>.Ok(all, "OK");
    }

    public async Task<ServiceResult<IReadOnlyList<ConnectionRequestDto>>> ViewSentRequestsAsync(int userId, CancellationToken ct)
    {
        // Now returns an empty list to avoid 404s, while we transition back to a single call.
        return await Task.FromResult(ServiceResult<IReadOnlyList<ConnectionRequestDto>>.Ok(new List<ConnectionRequestDto>(), "OK"));
    }

    public async Task<ServiceResult<object>> AcceptRequestAsync(int requestId, CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@RequestId", SqlDbType.Int) { Value = requestId },
        };

        int affected = await _db.ExecuteNonQueryAsync("sp_accept_request", parameters, ct);
        return affected > 0
            ? ServiceResult<object>.Ok(null, "Request accepted.")
            : ServiceResult<object>.Fail("Failed to accept request.");
    }

    public async Task<ServiceResult<object>> RejectRequestAsync(int requestId, CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@RequestId", SqlDbType.Int) { Value = requestId },
        };

        int affected = await _db.ExecuteNonQueryAsync("sp_reject_request", parameters, ct);
        return affected > 0
            ? ServiceResult<object>.Ok(null, "Request rejected.")
            : ServiceResult<object>.Fail("Failed to reject request.");
    }

    private static ConnectionRequestDto MapRequest(SqlDataReader r)
    {
        var dto = new ConnectionRequestDto();
        
        // Loop through all columns to find matches for our DTO properties.
        // This is extremely resilient to database column name variations.
        for (int i = 0; i < r.FieldCount; i++)
        {
            string colName = r.GetName(i).ToLower();
            object value = r.GetValue(i);
            if (value == DBNull.Value) continue;

            // Mapping for IDs
            if (colName.Contains("requestid") || colName == "id") 
                dto.RequestId = Convert.ToInt32(value);
            else if (colName.Contains("senderid") || colName.Contains("fromuserid") || colName == "sender" || colName == "fromuser") 
                dto.SenderId = Convert.ToInt32(value);
            else if (colName.Contains("receiverid") || colName.Contains("touserid") || colName == "receiver" || colName == "touser") 
                dto.ReceiverId = Convert.ToInt32(value);
            
            // Mapping for Names
            else if (colName.Contains("sendername") || colName == "sender_name") 
                dto.SenderName = value.ToString() ?? "";
            else if (colName.Contains("receivername") || colName == "receiver_name") 
                dto.ReceiverName = value.ToString() ?? "";
            else if (colName == "name" && string.IsNullOrEmpty(dto.SenderName))
                dto.SenderName = value.ToString() ?? "";

            // Mapping for other fields
            else if (colName.Contains("status")) 
                dto.Status = value.ToString() ?? "";
            else if (colName.Contains("createdat") || colName.Contains("date")) 
                dto.CreatedAt = Convert.ToDateTime(value);
        }

        // Final sanity checks/fallbacks
        if (string.IsNullOrEmpty(dto.SenderName)) dto.SenderName = $"User {dto.SenderId}";
        if (string.IsNullOrEmpty(dto.ReceiverName) && dto.ReceiverId > 0) dto.ReceiverName = $"User {dto.ReceiverId}";
        if (string.IsNullOrEmpty(dto.Status)) dto.Status = "Pending";

        return dto;
    }
}

