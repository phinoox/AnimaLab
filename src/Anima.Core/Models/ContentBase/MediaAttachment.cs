namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Represents a media file attached to a content item (e.g., images, PDFs, etc.).
/// Supports tracking of uploaded files and their storage locations.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class MediaAttachment : MetaEntity<ContentMetaInfo>
{
   
    /// <summary>
    /// Gets or sets the original name of the uploaded file.
    /// </summary>
    [Required, Display(Name = "File Name")]
    public string FileName { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the MIME type of the file.
    /// </summary>
    [Display(Name = "Content Type")]
    public string ContentType { get; set; } = "application/octet-stream";
    /// <summary>
    /// Gets or sets the storage path for the uploaded file in the system.
    /// </summary>
    [Column("storage_path"), MaxLength(2048)]
    public string StoragePath { get; set; } = "";
    /// <summary>
    /// Gets or sets the size of the file in bytes.
    /// </summary>
    [Display(Name = "File Size")]
    public long FileSize { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the user who uploaded this media.
    /// </summary>
    public Guid UploadedByUserId { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the file was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets a collection of external references associated with this media attachment.
    /// </summary>
    public ICollection<ExternalReference> ExternalReferences { get; set; } = null!; 
}