namespace Parent2Parent.Services;

public sealed record ServiceResult<T>(bool Success, string Message, T? Data = default)
{
    public static ServiceResult<T> Ok(T? data, string message = "OK") => new(true, message, data);
    public static ServiceResult<T> Fail(string message) => new(false, message, default);
}

