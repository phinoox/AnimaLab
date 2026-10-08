namespace Anima.Api.CoreServices.Interfaces;

/// <summary>
/// Provides reflection-based mapping between DTOs and Entities, 
/// respecting custom attributes like [ReadOnly], [Sanitize], and [Validate].
/// </summary>
public interface IEntityMapper
{
    /// <summary>
    /// Maps non-null properties from the source (DTO) to the target (Entity).
    /// Respects [ReadOnly], [Sanitize], and [Validate] attributes.
    /// </summary>
    void Map<TSource, TTarget>(TSource source, TTarget target) where TTarget : class;
}
