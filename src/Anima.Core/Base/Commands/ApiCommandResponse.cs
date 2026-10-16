namespace Anima.Core.Base.Commands;

/// <summary>
/// Represents the result of a command execution.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class CommandResponse<T>
{
    public bool Successful { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }

    private CommandResponse(bool successful, T? data, string? message)
    {
        Successful = successful;
        Data = data;
        Message = message;
    }

    public static CommandResponse<T> Success(T data) => new(true, data, null);
    public static CommandResponse<T> Failure(string message) => new(false, default, message);
}
