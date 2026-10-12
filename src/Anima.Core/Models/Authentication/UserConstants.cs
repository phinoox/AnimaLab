namespace Anima.Core.Models.Authentication;

public static class UserConstants
{
    /// <summary>
    /// The unique identifier for the system itself. 
    /// Used for automated processes, background jobs, or system-generated content.
    /// </summary>
    public static readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid DefaultAdminId = Guid.Parse("00000000-0000-0000-0000-000000000002");

}