namespace Flare.Components;

/// <summary>Maximum-width size presets for <see cref="FlareDialog"/>.</summary>
public enum DialogSize
{
    /// <summary>Extra-small dialog (max 20 rem).</summary>
    Xs,
    /// <summary>Small dialog (max 28 rem).</summary>
    Sm,
    /// <summary>Medium dialog (max 35 rem). Default.</summary>
    Md,
    /// <summary>Large dialog (max 50 rem).</summary>
    Lg,
    /// <summary>Extra-large dialog (max 70 rem).</summary>
    Xl,
    /// <summary>Full-screen dialog (nearly the full viewport).</summary>
    FullScreen,
    /// <summary>Full-width dialog (spans the full viewport width with a small margin).</summary>
    FullWidth,
}
