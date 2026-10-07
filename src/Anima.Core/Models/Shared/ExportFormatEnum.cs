// =============================================================================
// ExportFormatEnum - Enum for content export formats supported by the engine
// =============================================================================

namespace Anima.Core.Models.Shared;

/// <summary>
/// Specifies the format used when exporting content items to external systems or engines.
/// </summary>
public enum ExportFormatEnum
{
    /// <summary>
    /// JSON format for structured data export, suitable for Unity/Unreal integration.
    /// </summary>
    Json,
    
    /// <summary>
    /// CSV format for spreadsheet-based data analysis and migration.
    /// </summary>
    Csv,
    
    /// <summary>
    /// XML format following the Game Design Document (GDD) standard structure.
    /// </summary>
    XmlGdd,
    
    /// <summary>
    /// PDF format for documentation and presentation purposes.
    /// </summary>
    Pdf
}

