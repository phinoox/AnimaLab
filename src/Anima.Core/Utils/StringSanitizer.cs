using System.Text.RegularExpressions;

namespace Anima.Core.Utils;

/// <summary>
/// Provides utility methods for sanitizing and normalizing strings.
/// </summary>
public static class StringSanitizer
{
    /// <summary>
    /// Converts a human-readable string into a web-friendly slug (e.g., "Dark Tone" -> "dark_tone").
    /// </summary>
    /// <param name="input">The input string to normalize.</param>
    /// <returns>A normalized, lowercase, underscore-separated string, or an empty string if the input is null or whitespace.</returns>
    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // 1. Convert to lowercase
        string result = input.ToLowerInvariant();

        // 2. Replace spaces and special characters with underscores
        result = Regex.Replace(result, @"[^a-z0-9]+", "_");

        // 3. Trim leading/trailing underscores
        return result.Trim('_');
    }
}