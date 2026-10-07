namespace Anima.Core.Utils;


/// <summary>
/// Defines a contract for entities that support soft deletion, allowing them to be marked as deleted without being removed from the database.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity has been soft-deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was soft-deleted.
    /// </summary>
    DateTime? DeletedAt { get; set; }
}