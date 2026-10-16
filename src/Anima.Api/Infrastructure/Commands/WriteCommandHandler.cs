namespace Anima.Api.Infrastructure.Commands;

using Microsoft.EntityFrameworkCore.Storage;
using Anima.Core.Dtos.Shared;
using Anima.Core.Base.Commands;
using Anima.Data.Contexts;
using Anima.Api.CoreServices.Interfaces;

/// <summary>
/// Manages the transaction lifecycle and activity logging for data-modifying operations.
/// </summary>
public abstract class WriteCommandHandler<TCommand, TResponse> : BaseCommandHandler<TCommand, TResponse>
    where TCommand : IApiCommand
    where TResponse : class
{
    protected WriteCommandHandler(ILogger logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core) { }

    public override async Task<ApiResponseDto<TResponse>> HandleAsync(TCommand command, RequestContext context)
    {
        var identityError = await ValidateIdentityAsync();
        if (identityError != null) return identityError;

        var scopeError = await ValidateScopeAsync(context);
        if (scopeError != null) return scopeError;

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var response = await ExecuteAsync(command, context);

            if (!response.Successful)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning("Command {CommandName} failed logic. Transaction rolled back.", typeof(TCommand).Name);
            }
            else
            {
                await transaction.CommitAsync();
                await LogActivityAsync(command, context);
            }

            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error executing WriteCommand {CommandName}. Transaction rolled back.", typeof(TCommand).Name);
            throw;
        }
    }

    protected abstract Task<ApiResponseDto<TResponse>> ExecuteAsync(TCommand command, RequestContext context);

    private async Task LogActivityAsync(TCommand command, RequestContext context)
    {
        var activityLog = new ActivityLog
        {
            Action = typeof(TCommand).Name,
            CreatedAt = DateTime.UtcNow,
            EntityId = context.EntityId,
            ProjectId = context.ProjectId,
            ParentId = context.ParentId
        };

        await OnLogActivityOnSuccess(activityLog);
        await _core.AuditService.LogDbAsync(activityLog);
    }

    protected virtual Task OnLogActivityOnSuccess(ActivityLog activityLog) => Task.CompletedTask;
}