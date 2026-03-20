using System.Data;
using Microsoft.Data.SqlClient;

namespace Parent2Parent.Data;

public sealed class DbHelper : IDbHelper
{
    private readonly string _connectionString;
    private readonly ILogger<DbHelper> _logger;

    public DbHelper(IConfiguration configuration, ILogger<DbHelper> logger)
    {
        _connectionString = configuration.GetConnectionString("Parent2ParentDb")
            ?? throw new InvalidOperationException("Connection string 'Parent2ParentDb' is not configured.");
        _logger = logger;
    }

    public async Task<int> ExecuteNonQueryAsync(string storedProcedure, IEnumerable<SqlParameter>? parameters = null, CancellationToken ct = default)
    {
        _logger.LogInformation("Executing non-query stored procedure: {StoredProcedure}", storedProcedure);
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, storedProcedure, parameters);

        await conn.OpenAsync(ct);
        return await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string storedProcedure, IEnumerable<SqlParameter>? parameters = null, CancellationToken ct = default)
    {
        _logger.LogInformation("Executing scalar stored procedure: {StoredProcedure}", storedProcedure);
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, storedProcedure, parameters);

        await conn.OpenAsync(ct);
        object? result = await cmd.ExecuteScalarAsync(ct);

        if (result is null || result is DBNull) return default;
        return (T)Convert.ChangeType(result, typeof(T));
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(
        string storedProcedure,
        Func<SqlDataReader, T> map,
        IEnumerable<SqlParameter>? parameters = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing query stored procedure: {StoredProcedure}", storedProcedure);
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, storedProcedure, parameters);

        await conn.OpenAsync(ct);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, ct);

        var results = new List<T>();
        while (await reader.ReadAsync(ct))
        {
            results.Add(map(reader));
        }

        return results;
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(
        string storedProcedure,
        Func<SqlDataReader, T> map,
        IEnumerable<SqlParameter>? parameters = null,
        CancellationToken ct = default)
    {
        var list = await QueryAsync(storedProcedure, map, parameters, ct);
        return list.Count > 0 ? list[0] : default;
    }

    private SqlCommand CreateCommand(SqlConnection conn, string storedProcedure, IEnumerable<SqlParameter>? parameters)
    {
        if (string.IsNullOrWhiteSpace(storedProcedure))
            throw new ArgumentException("Stored procedure name is required.", nameof(storedProcedure));

        var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = storedProcedure;
        cmd.CommandTimeout = 30; // 30 seconds timeout

        if (parameters is not null)
        {
            foreach (var p in parameters)
            {
                // Explicitly clone to avoid accidental re-use across commands.
                cmd.Parameters.Add((SqlParameter)((ICloneable)p).Clone());
            }
        }

        return cmd;
    }
}
