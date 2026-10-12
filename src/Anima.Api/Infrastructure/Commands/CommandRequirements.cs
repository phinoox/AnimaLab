namespace Anima.Api.Infrastructure.Commands;

public enum IdentityRequirementEnum
{
    MustBeUnauthenticated, // For Sign In / Register
    MustBeAuthenticated   // For everything else
}

public enum ScopeRequirementEnum
{
    None,        // Global (Profile)
    Project,     // Project-level (Create Project, Update Project Meta)
    Entity       // Entity-level (Update Blog Post, Delete Blog Post)
}