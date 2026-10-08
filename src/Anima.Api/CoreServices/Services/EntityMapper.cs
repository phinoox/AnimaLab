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
    /// <typeparam name="TSource">The type of the source DTO.</typeparam>
    /// <typeparam name="TTarget">The type of the destination Entity.</typeparam>
    /// <param name="source">The source object containing data.</param>
    /// <param name="target">The target object to be updated.</param>
    /// <param name="teamMemberRole">The role of the user performing the operation, used for permission checks via [MinRole].</param>
    void Map<TSource, TTarget>(TSource source, TTarget target, ProjectMemberRoleEnum teamMemberRole = ProjectMemberRoleEnum.Viewer) where TTarget : class;
}

/// <summary>
/// An implementation of <see cref="IEntityMapper"/> that uses reflection to automate the mapping process 
/// while enforcing business rules defined by custom attributes on properties and classes.
/// </summary>
public class EntityMapper : IEntityMapper
{
    /// <summary>
    /// Maps non-null properties from the source (DTO) to the target (Entity).
    /// Respects [MinRole], [ReadOnly], [Sanitize], and [Validate] attributes.
    /// </summary>
    /// <typeparam name="TSource">The type of the source DTO.</typeparam>
    /// <typeparam name="TTarget">The type of the destination Entity.</typeparam>
    /// <param name="source">The source object containing data.</param>
    /// <param name="target">The target object to be updated.</param>
    /// <param name="teamMemberRole">The role of the user performing the operation, used for permission checks via [MinRole].</param>
    /// <exception cref="InvalidOperationException">Thrown when validation fails or mapping an error occurs.</exception>
    public void Map<TSource, TTarget>(TSource source, TTarget target, ProjectMemberRoleEnum teamMemberRole = ProjectMemberRoleEnum.Viewer) where TTarget : class
    {
        if (source == null || target == null) return;

        var sourceType = typeof(TSource);
        var targetType = typeof(TTarget);

        var minRoleAttribute = sourceType.GetCustomAttribute<MinRoleAttribute>();
        if (minRoleAttribute != null)
        {
            if (minRoleAttribute.MinimumRole < teamMemberRole && minRoleAttribute.BypassRole != teamMemberRole)
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

            if (targetProp != null && targetProp.CanWrite && IsSupportedType(targetProp) && IsInitProperty(targetProp))
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

    /// <summary>
    /// Checks if a property value is null or whitespace for strings.
    /// </summary>
    private bool IsPropertyNull(PropertyInfo prop, object obj)
    {
        var value = prop.GetValue(obj);
        return value == null || (value is string s && string.IsNullOrWhiteSpace(s));
    }

    /// <summary>
    /// Performs basic HTML stripping/sanitization on strings.
    /// </summary>
    private string SanitizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        // Basic HTML stripping/sanitization logic
        return Regex.Replace(input, "<.*?>", string.Empty).Trim(); // ToDo: replace with HTMLSanitizer
    }

    /// <summary>
    /// Validates the property value based on custom rules.
    /// </summary>
    private bool IsValid(object? value, PropertyInfo prop)
    {
        // This is a placeholder for more complex validation logic (e.g., DataAnnotations or custom rules)
        if (value == null) return true;
        return true;
    }

    /// <summary>
    /// Determines if the property type is supported for mapping.
    /// </summary>
    private bool IsSupportedType(PropertyInfo prop)
    {
        var targetType = prop.PropertyType;
        bool isSupportedType = targetType.IsPrimitive ||
                               targetType.IsEnum ||
                               targetType == typeof(string) ||
                               targetType.IsValueType;
        return isSupportedType;
    }

    /// <summary>
    /// Checks if the property uses 'init' only semantics.
    /// </summary>
    private bool IsInitProperty(PropertyInfo prop)
    {
        var setMethod = prop.SetMethod;
        if (setMethod == null) return true; // Getter only

        // Check if the setter's declaring type is IsExternalInit 
        // or if it belongs to a type that uses init-only properties logic.
        return setMethod.GetCustomAttributes(true).Any(a => a.GetType().Name == "IsExternalInit");
    }
}