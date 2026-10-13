
using System;

namespace Anima.Core.Utils.Attributes;

public enum PreviewType
{
    Markdown,
    Html,
    Json,
    Image
}

[AttributeUsage(AttributeTargets.Property)]
public class PreviewableAttribute : Attribute
{
    public PreviewType Type { get; }

    public PreviewableAttribute(PreviewType type = PreviewType.Markdown)
    {
        Type = type;
    }
}
