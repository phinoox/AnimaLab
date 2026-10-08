using System.Reflection;
using System.Text.RegularExpressions;
using Anima.Core.Models.Authentication;

namespace Anima.Api.CoreServices.Services;

/// <summary>
/// Provides reflection-based mapping between DTOs and Entities, 
/// respecting custom attributes like [ReadOnly], [Sanitize], and [Validate].
/// </summary>
public interface IEntityMapper
{
    /// <summary>
    /// Maps non-null properties from the source (DTO) to the target (Entity).
    /// Respects [ReadOnly], [Sanitize], and [Validate] attributes.
    /// </summary>
    void Map<TSource, TTarget>(TSource source, TTarget target,ProjectMemberRoleEnum teamMemberRole = ProjectMemberRoleEnum.Viewer) where TTarget : class;
}

public class EntityMapper : IEntityMapper
{
    public void Map<TSource, TTarget>(TSource source, TTarget target,ProjectMemberRoleEnum teamMemberRole = ProjectMemberRoleEnum.Viewer) where TTarget : class
    {
        if (source == null || target == null) return;

        var sourceType = typeof(TSource);
        var targetType = typeof(TTarget);

        var minRoleAttribute =sourceType.GetCustomAttribute<MinRoleAttribute>();
        if(minRoleAttribute != null)
        {
            if(minRoleAttribute.MinimumRole < teamMemberRole && minRoleAttribute.BypassRole != teamMemberRole)
             return;
        }

        // Get all non-null properties from the source DTO
        var sourceProperties = sourceType.GetProperties()
            .Where(p => p.CanRead && !IsPropertyNull(p, source))
            .ToList();

        foreach (var sourceProp in sourceProperties)
        {
            // Find a matching property on the target entity by name
            var targetProp = targetType.GetProperty(sourceProp.Name, BindingFlags.Public | BindingFlags.Instance);

            

            if (targetProp != null && targetProp.CanWrite && IsSupportedType(targetProp) &&IsInitProperty(targetProp))
            {
                var minRolePropertyAttribute = targetProp.GetCustomAttribute<MinRoleAttribute>();
                if (minRolePropertyAttribute != null)
                {
                    if (minRolePropertyAttribute.MinimumRole < teamMemberRole && minRolePropertyAttribute.BypassRole != teamMemberRole)
                        return;
                }
                // 1. Check [ReadOnly] attribute on the TARGET property
                if (targetProp.GetCustomAttribute<ReadOnlyAttribute>() != null)
                    continue;

                var value = sourceProp.GetValue(source);

                // 2. Handle [Sanitize] attribute
                if (value is string strValue && targetProp.GetCustomAttribute<SanitizeAttribute>() != null)
                {
                    value = SanitizeString(strValue);
                }

                // 3. Handle [Validate] attribute
                if (targetProp.GetCustomAttribute<ValidateAttribute>() != null)
                {
                    if (!IsValid(value, targetProp))
                        throw new InvalidOperationException($"Validation failed for property: {targetProp.Name}");
                }

                // Apply the value
                try
                {
                    targetProp.SetValue(target, value);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to map property {targetProp.Name}: {ex.Message}", ex);
                }
            }
        }
    }

    private bool IsPropertyNull(PropertyInfo prop, object obj)
    {
        var value = prop.GetValue(obj);
        return value == null || (value is string s && string.IsNullOrWhiteSpace(s));
    }

    private string SanitizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        // Basic HTML stripping/sanitization logic
        return Regex.Replace(input, "<.*?>", string.Empty).Trim();//ToDo: replace with HTMLSanitizer
    }

    private bool IsValid(object? value, PropertyInfo prop)
    {
        // This is a placeholder for more complex validation logic (e.g., DataAnnotations or custom rules)
        if (value == null) return true;
        return true;
    }

    private bool IsSupportedType(PropertyInfo prop)
    {
        var targetType = prop.PropertyType;
        bool isSupportedType = targetType.IsPrimitive ||
                               targetType.IsEnum ||
                               targetType == typeof(string) ||
                               targetType.IsValueType;
        return isSupportedType;

    }

    private bool IsInitProperty(PropertyInfo prop)
    {
        var setMethod = prop.SetMethod;
        if (setMethod == null) return true; // Getter only

        // Check if the setter's declaring type is IsExternalInit 
        // or if it belongs to a type that uses init-only properties logic.
        // This is slightly complex in pure reflection but we can check for the presence of the attribute/marker.
        return setMethod.GetCustomAttributes(true).Any(a => a.GetType().Name == "IsExternalInit");
    }
}
