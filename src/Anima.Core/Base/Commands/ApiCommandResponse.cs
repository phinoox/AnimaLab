namespace Anima.Core.Base.Commands;

/// <summary>
/// Represents the result of a command execution.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class ApiCommandResponse<T>
{
    public bool Successful { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }

    private ApiCommandResponse(bool successful, T? data, string? message)
    {
        Successful = successful;
        Data = data;
        Message = message;
    }

    public static ApiCommandResponse<T> Success(T data) => new(true, data, null);
    public static ApiCommandResponse<T> Failure(string message) => new(false, default, message);
}
