namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Specifies the type of external reference being linked to a content item.
/// </summary>
public enum ExternalReferenceTypeEnum
{
    /// <summary>A link to a Google Doc.</summary>
    GoogleDoc,

    /// <summary>A link to a Pinterest board or pin.</summary>
    Pinterest,

    /// <summary>A general document reference (e.g., PDF, Word).</summary>
    Document,

    /// <summary>An image reference.</summary>
    Image,

    /// <summary>A video reference.</summary>
    VIdeo
}