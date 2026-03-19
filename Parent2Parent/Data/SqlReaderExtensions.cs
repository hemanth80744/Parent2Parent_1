using Microsoft.Data.SqlClient;

namespace Parent2Parent.Data;

internal static class SqlReaderExtensions
{
    public static bool HasColumn(this SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static int GetInt32OrDefault(this SqlDataReader reader, string columnName, int defaultValue = 0)
    {
        if (!reader.HasColumn(columnName)) return defaultValue;
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
    }

    public static string GetStringOrEmpty(this SqlDataReader reader, string columnName)
    {
        if (!reader.HasColumn(columnName)) return string.Empty;
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
    }

    public static string? GetStringOrNull(this SqlDataReader reader, string columnName)
    {
        if (!reader.HasColumn(columnName)) return null;
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    public static DateTime? GetDateTimeOrNull(this SqlDataReader reader, string columnName)
    {
        if (!reader.HasColumn(columnName)) return null;
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }
}

