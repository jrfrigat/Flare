namespace Flare.Components;

/// <summary>Decorates a model property to control how FlareFormBuilder renders it as a form field.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class FlareFormFieldAttribute : Attribute
{
    /// <summary>Display label for the field. Defaults to the property name split on camel-case boundaries.</summary>
    public string? Label { get; set; }
    /// <summary>Placeholder text shown inside the input when it is empty.</summary>
    public string? Placeholder { get; set; }
    /// <summary>Render order relative to other fields. Lower values appear first; ties are broken alphabetically by label.</summary>
    public int Order { get; set; }
    /// <summary>When <see langword="true"/>, the property is excluded from the generated form entirely.</summary>
    public bool Ignore { get; set; }
}
