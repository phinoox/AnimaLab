namespace Anima.Api.Infrastructure.Commands;

using Anima.Core.Base.Commands;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A simple implementation of the ICommandBus that resolves handlers from the DI container.
/// </summary>
public class CommandBus : ICommandBus
{
    private readonly IServiceProvider _serviceProvider;

    public CommandBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ApiResponseDto<TResponse>> SendAsync<TResponse>(ICommand command,RequestContext context) where TResponse : class
    {
        // We need to find the handler for this specific command type.
        // Since TCommand is not known at compile time here, we use reflection-based resolution via DI.
        var commandType = command.GetType();
        
        // Construct the interface type: ICommandHandler<TCommand, TResponse>
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));

        // Resolve the handler from the service provider
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for command: {commandType.Name}");
        }

        // Invoke HandleAsync via reflection to maintain type safety within the generic method signature
        var method = handlerType.GetMethod(nameof(ICommandHandler<,>.HandleAsync));
        if (method == null)
        {
            throw new InvalidOperationException($"Handler for {commandType.Name} does not implement HandleAsync.");
        }

        return await (Task<ApiResponseDto<TResponse>>)method.Invoke(handler, new object[] { command,context })!;
    }
}
