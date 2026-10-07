using System.Reflection;


namespace Anima.Core.Utils;

/// <summary>
/// Specifies an index for a module within the system, used to generate deterministic GUIDs.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public class ModuleIndexAttribute(int index) : Attribute
{
    /// <summary>
    /// Gets the index associated with the module.
    /// </summary>
    public int Index { get; } = index;
}

/// <summary>
/// Provides extension methods for enums to facilitate conversion to deterministic GUIDs.
/// </summary>
public static class EnumGuidExtensions
{
    /// <summary>
    /// Converts an enum member into a deterministic GUID using the <see cref="ModuleIndexAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="value">The enum value to convert.</param>
    /// <returns>A unique, deterministic <see cref="Guid"/> based on the module index and enum serial.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the enum type is missing the <see cref="ModuleIndexAttribute"/>.</exception>
    public static Guid ToGuid<T>(this T value) where T : Enum
    {
        var type = typeof(T);
        var attr = type.GetCustomAttribute<ModuleIndexAttribute>();

        if (attr == null)
            throw new InvalidOperationException($"Type {type.Name} is missing the [ModuleIndex] attribute.");

        // The integer value of the enum acts as our unique serial within the module
        int serial = Convert.ToInt32(value);

        return ReservedGuid.Create(attr.Index, serial);
    }
}