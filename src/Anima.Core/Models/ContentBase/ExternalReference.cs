namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Represents an external reference (e.g., Google Docs, Pinterest boards) linked to a content item.
/// Used for referencing external resources such as GDD documents, art inspirations, or research material.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class ExternalReference  : MetaEntity<ContentMetaInfo>
{
   
    // --- Domain Properties (The "Body") ---
    /// <summary>
    /// Gets or sets the type of reference (e.g., Document, Image, Video).
    /// </summary>
    [Required]
    public ExternalReferenceTypeEnum ReferenceType { get; set; } = ExternalReferenceTypeEnum.Document;
    /// <summary>
    /// Gets or sets the external URL for the resource.
    /// </summary>
    [Required, MaxLength(2048)]
    public string Url { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the title of the external resource.
    /// </summary>
    [MaxLength(128)]
    public string? Title { get; set; }
    
    /// <summary>
    /// Gets or sets the author or creator of the referenced material.
    /// </summary>
    [MaxLength(128)]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets additional notes or context regarding this reference.
    /// </summary>
    [MaxLength(4096)]
    public string? Notes { get; set; }

    /// <summary>
        /// Gets or sets a value indicating whether the reference is currently active and relevant.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a URL-friendly slug for this reference, if applicable.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; } 
}