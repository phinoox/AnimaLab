namespace Anima.Core.Utils;

/// <summary>
/// Indicates that a property should be handled by the fixture generation system.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FixtureAttribute : Attribute
{
    /// <summary>
    /// Gets the hint specifying how the fixture should be generated.
    /// </summary>
    public FixtureHintEnum Hint { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FixtureAttribute"/> class with a specified hint.
    /// </summary>
    /// <param name="hint">The fixture generation hint.</param>
    public FixtureAttribute(FixtureHintEnum hint)
    {
        Hint = hint;
    }
}

/// <summary>
/// Specifies hints for the fixture generation process.
/// </summary>
public enum FixtureHintEnum
{
    /// <summary>No specific hint provided.</summary>
    None,
    
    /// <summary>The property should be omitted during fixture creation.</summary>
    Omit
}
