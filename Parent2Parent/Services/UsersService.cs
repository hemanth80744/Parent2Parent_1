using System.Data;
using Microsoft.Data.SqlClient;
using Parent2Parent.Data;
using Parent2Parent.Models.Dto.Users;

namespace Parent2Parent.Services;

public sealed class UsersService : IUsersService
{
    private readonly IDbHelper _db;

    public UsersService(IDbHelper db)
    {
        _db = db;
    }

    public async Task<ServiceResult<object>> RegisterAsync(RegisterRequestDto dto, CancellationToken ct)
    {
        // NOTE: Parameter names must match the stored procedure definition.
        var parameters = new[]
        {
            new SqlParameter("@Name", SqlDbType.VarChar, 100) { Value = dto.Name },
            new SqlParameter("@Username", SqlDbType.VarChar, 50) { Value = dto.Username },
            new SqlParameter("@Password", SqlDbType.VarChar, 100) { Value = dto.Password },
            new SqlParameter("@School", SqlDbType.VarChar, 150) { Value = dto.School },
            new SqlParameter("@Class", SqlDbType.VarChar, 50) { Value = dto.Class },
        };

        int affected = await _db.ExecuteNonQueryAsync("sp_register_user", parameters, ct);
        return affected > 0
            ? ServiceResult<object>.Ok(null, "Registered successfully.")
            : ServiceResult<object>.Fail("Registration failed.");
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto dto, CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@Username", SqlDbType.VarChar, 50) { Value = dto.Username },
            new SqlParameter("@Password", SqlDbType.VarChar, 100) { Value = dto.Password },
        };

        var user = await _db.QuerySingleOrDefaultAsync("sp_login_user", MapAuthUser, parameters, ct);
        return user is null
            ? ServiceResult<AuthResponseDto>.Fail("Invalid username or password.")
            : ServiceResult<AuthResponseDto>.Ok(user, "Login successful.");
    }

    public async Task<ServiceResult<IReadOnlyList<SchoolSearchResultDto>>> SearchSchoolAsync(string schoolName, CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@SchoolName", SqlDbType.VarChar, 150) { Value = schoolName },
        };

        var schools = await _db.QueryAsync("sp_search_school", MapSchool, parameters, ct);
        return ServiceResult<IReadOnlyList<SchoolSearchResultDto>>.Ok(schools, "OK");
    }

    private static AuthResponseDto MapAuthUser(SqlDataReader r)
    {
        int userId = r.GetInt32OrDefault("Id");
        return new AuthResponseDto
        {
            UserId = userId,
            Name = r.GetStringOrEmpty("Name"),
        };
    }

    private static SchoolSearchResultDto MapSchool(SqlDataReader r)
    {
        return new SchoolSearchResultDto
        {
            UserId = r.GetInt32OrDefault("Id"),
            Name = r.GetStringOrEmpty("Name"),
            ChildClass = r.GetStringOrNull("ChildClass"),
        };
    }
}

