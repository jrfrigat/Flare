namespace Flare.Components;

/// <summary>
/// Font weight override for <c>FlareText</c>. <see cref="Default"/> keeps the weight defined by
/// the active type scale; the other values map to the standard CSS numeric weights via shared
/// <c>flare-text--weight-*</c> classes (no inline style needed).
/// </summary>
public enum FontWeight
{
    /// <summary>Use the type scale's own weight (no override).</summary>
    Default,
    /// <summary>Thin - 100.</summary>
    Thin,
    /// <summary>Extra Light - 200.</summary>
    ExtraLight,
    /// <summary>Light - 300.</summary>
    Light,
    /// <summary>Regular - 400.</summary>
    Regular,
    /// <summary>Medium - 500.</summary>
    Medium,
    /// <summary>Semi Bold - 600.</summary>
    SemiBold,
    /// <summary>Bold - 700.</summary>
    Bold,
    /// <summary>Extra Bold - 800.</summary>
    ExtraBold,
    /// <summary>Black - 900.</summary>
    Black,
}
