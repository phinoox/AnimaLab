using Anima.Core.Models.Authentication;

namespace Anima.Core.Utils;

public class ReadOnlyAttribute : Attribute{}

public class SanitizeAttribute : Attribute{}

public class ValidateAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
public class MinRoleAttribute : Attribute
{
    public ProjectMemberRoleEnum MinimumRole { get; }
    public ProjectMemberRoleEnum? BypassRole { get; }

    public MinRoleAttribute(ProjectMemberRoleEnum minimumRole, ProjectMemberRoleEnum? bypassRole = null)
    {
        MinimumRole = minimumRole;
        BypassRole = bypassRole;
    }
}