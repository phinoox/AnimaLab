namespace Anima.Core.Dtos.Shared;

/// <summary>
/// Represents a single search result hit containing enough context for client-side navigation.
/// </summary>
public class SearchHitDto 
{
    /// <summary>
    /// The unique identifier of the actual resource found (e.g., Project ID, Character ID).
    /// </summary>
    public Guid ResourceId { get; set; } 

    /// <summary>
    /// The display name used to show the result in a list (e.g., Title or Name).
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug of the resource for direct navigation via web routes.
    /// </summary>
    public string Slug { get; set; } = "";

    /// <summary>
    /// The type of resource found (e.g., "Project", "Character", "Scene").
    /// </summary>
    public string ResourceType { get; set; } = "";

    /// <summary>
    /// The parent context ID. Null for root anchors (Projects), or the ProjectId for content entities.
    /// </summary>
    public Guid? ScopeId { get; set; } 

    /// <summary>
    /// The API endpoint used to fetch the full details of this resource.
    /// </summary>
    public string ResourceLink { get; set; } = "";
}