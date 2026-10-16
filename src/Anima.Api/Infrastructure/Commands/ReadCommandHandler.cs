namespace Anima.Api.Infrastructure.Commands;

using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Base.Commands;
using Anima.Core.Dtos.Shared;
using Anima.Data.Contexts;

/// <summary>
/// Optimized handler for read-only operations. Skips transaction management and activity logging.
/// </summary>
public abstract class ReadCommandHandler<TCommand, TResponse> : BaseCommandHandler<TCommand, TResponse>
    where TCommand : IApiCommand
    where TResponse : class
{
    protected ReadCommandHandler(ILogger logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core) { }

    public override async Task<ApiResponseDto<TResponse>> HandleAsync(TCommand command, RequestContext context)
    {
        // 1. Check Identity Requirement
        var identityError = await ValidateIdentityAsync();
        if (identityError != null) return identityError;

        // 2. Check Scope/Permission requirement
        var scopeError = await ValidateScopeAsync(context);
        if (scopeError != null) return scopeError;

        // 3. Execute the Read operation without a transaction
        return await ExecuteAsync(command, context);
    }

    protected abstract Task<ApiResponseDto<TResponse>> ExecuteAsync(TCommand command, RequestContext context);
}