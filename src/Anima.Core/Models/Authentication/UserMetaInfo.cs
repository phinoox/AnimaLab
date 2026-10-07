namespace Anima.Core.Models.Authentication;

/// <summary>
/// Represents the metadata attached to a user identity, following the Scale-Invariant pattern.
/// </summary>
public class UserMetaInfo : BaseMetaInfo
{
    /// <summary>
    /// URL to the user's avatar image.
    /// </summary>
    public string? AvatarUrl { get; set; }

    public override  string? TypeHint {get => typeof(User).ToString();set;} 
}
