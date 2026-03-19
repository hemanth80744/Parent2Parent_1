using Microsoft.Data.SqlClient;

namespace Parent2Parent.Data;

public interface IDbHelper
{
    Task<int> ExecuteNonQueryAsync(string storedProcedure, IEnumerable<SqlParameter>? parameters = null, CancellationToken ct = default);

    Task<T?> ExecuteScalarAsync<T>(string storedProcedure, IEnumerable<SqlParameter>? parameters = null, CancellationToken ct = default);

    Task<IReadOnlyList<T>> QueryAsync<T>(
        string storedProcedure,
        Func<SqlDataReader, T> map,
        IEnumerable<SqlParameter>? parameters = null,
        CancellationToken ct = default);

    Task<T?> QuerySingleOrDefaultAsync<T>(
        string storedProcedure,
        Func<SqlDataReader, T> map,
        IEnumerable<SqlParameter>? parameters = null,
        CancellationToken ct = default);
}
