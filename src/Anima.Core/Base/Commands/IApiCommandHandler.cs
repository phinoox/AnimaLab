namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Dtos.Shared;

/// <summary>
/// Defines the contract for a handler that processes a specific command and produces a response.
/// </summary>
/// <typeparam name="TCommand">The type of the command to handle.</typeparam>
/// <typeparam name="TResponse">The type of the response produced by the handler.</typeparam>
public interface IApiCommandHandler<in TCommand, TResponse> 
    where TCommand : IApiCommand
    where TResponse : class
{
    /// <summary>
    /// Executes the business logic associated with the command.
    /// </summary>
    /// <param name="command">The command containing the intent and data.</param>
    /// <param name="context"></param>
    /// <returns>A task representing the asynchronous operation, containing the command response.</returns>
    Task<ApiResponseDto<TResponse>> HandleAsync(TCommand command,RequestContext context);
}
