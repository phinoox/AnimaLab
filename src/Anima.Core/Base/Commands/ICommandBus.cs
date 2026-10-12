using Anima.Core.Dtos.Shared;

namespace Anima.Core.Base.Commands;

/// <summary>
/// Defines the contract for a mediator that routes commands to their appropriate handlers.
/// </summary>
public interface ICommandBus
{
    /// <summary>
    /// Dispatches a command to its registered handler.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="command">The command instance to execute.</param>
    /// <param name="context"></param>
    /// <returns>A task representing the asynchronous operation, returning the handler's response.</returns>
    Task<ApiResponseDto<TResponse>> SendAsync<TResponse>(ICommand command,RequestContext context) where TResponse : class;
}
