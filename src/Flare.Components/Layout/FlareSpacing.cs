namespace Flare.Components;

/// <summary>
/// Design-token spacing values for gap, padding, and margin parameters.
/// Maps to pre-compiled CSS modifier classes using the component's spacing scale.
/// </summary>
public enum FlareSpacing
{
    /// <summary>No spacing (0).</summary>
    None,
    /// <summary>Extra-extra-small spacing (0.25 rem / 4 px).</summary>
    XXSmall,
    /// <summary>Extra-small spacing (0.5 rem / 8 px).</summary>
    XSmall,
    /// <summary>Small spacing (0.75 rem / 12 px).</summary>
    Small,
    /// <summary>Medium spacing (1 rem / 16 px). Default for most layout components.</summary>
    Medium,
    /// <summary>Large spacing (1.5 rem / 24 px).</summary>
    Large,
    /// <summary>Extra-large spacing (2 rem / 32 px).</summary>
    XLarge,
    /// <summary>Use a raw CSS value supplied via the companion *Value parameter.</summary>
    Custom,
}
