namespace Anima.Api.CoreServices.Interfaces;

/// <summary>
/// Defines a contract for providing reflection-based mapping between DTOs and Entities, 
/// respecting custom attributes like [ReadOnly], [Sanitize], and [Validate].
/// </summary>
public interface IEntityMapper
{
    /// <summary>
    /// Maps non-null properties from the source (DTO) to the target (Entity).
    /// Respects [MinRole], [ReadOnly], [Sanitize], and [Validate] attributes.
    /// </summary>
    /// <typeparam name="TSource">The type of the source DTO.</typeparam>
    /// <typeparam name="TTarget">The type of the destination Entity.</typeparam>
    /// <param name="source">The source object containing data.</param>
    /// <param name="target">The target object to be updated.</param>
    /// <param name="teamMemberRole">The role of the user performing the operation, used for permission checks via [MinRole].</param>
    void Map<TSource, TTarget>(TSource source, TTarget target, Anima.Core.Models.Authentication.ProjectMemberRoleEnum teamMemberRole = Anima.Core.Models.Authentication.ProjectMemberRoleEnum.Viewer) where TTarget : class;
}