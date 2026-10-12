namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;

/// <summary>
/// A marker interface for responses that indicate whether a command operation was successful or not.
/// </summary>
public interface ISuccessfulCommandResponse
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    bool IsSuccessful { get; }
}
