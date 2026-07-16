namespace Flare.Components;

/// <summary>Size of <c>FlareSwitch</c> on the shared Xs..Xl scale; medium is the default. The steps are
/// labels, not measurements - each theme maps them onto its own track and thumb tokens, and they differ:
/// one theme's medium switch is not another's.</summary>
public enum SwitchSize
{
    /// <summary>Extra small.</summary>
    Xs,
    /// <summary>Small (compact) switch.</summary>
    Sm,
    /// <summary>Medium - the default.</summary>
    Md,
    /// <summary>Large.</summary>
    Lg,
    /// <summary>Extra large.</summary>
    Xl,
}
