namespace Flare.Core.Tokens;

/// <summary>
/// The color half of a theme: ~45 semantic color roles for a single mode (light or dark).
/// A <see cref="Palette"/> bundles one <see cref="ColorScheme"/> for light and one for dark.
/// Non-color tokens (typography, shape, motion, elevation geometry, components) live in
/// <see cref="DesignTokens"/>; the two are composed at render time.
/// </summary>
public sealed record ColorScheme
{
    /// <summary>Primary color role.</summary>
    public required string Primary { get; init; }
    /// <summary>On primary color role.</summary>
    public required string OnPrimary { get; init; }
    /// <summary>Primary container color role.</summary>
    public required string PrimaryContainer { get; init; }
    /// <summary>On primary container color role.</summary>
    public required string OnPrimaryContainer { get; init; }
    /// <summary>Secondary color role.</summary>
    public required string Secondary { get; init; }
    /// <summary>On secondary color role.</summary>
    public required string OnSecondary { get; init; }
    /// <summary>Secondary container color role.</summary>
    public required string SecondaryContainer { get; init; }
    /// <summary>On secondary container color role.</summary>
    public required string OnSecondaryContainer { get; init; }
    /// <summary>Tertiary color role.</summary>
    public required string Tertiary { get; init; }
    /// <summary>On tertiary color role.</summary>
    public required string OnTertiary { get; init; }
    /// <summary>Tertiary container color role.</summary>
    public required string TertiaryContainer { get; init; }
    /// <summary>On tertiary container color role.</summary>
    public required string OnTertiaryContainer { get; init; }
    /// <summary>Error color role.</summary>
    public required string Error { get; init; }
    /// <summary>On error color role.</summary>
    public required string OnError { get; init; }
    /// <summary>Error container color role.</summary>
    public required string ErrorContainer { get; init; }
    /// <summary>On error container color role.</summary>
    public required string OnErrorContainer { get; init; }
    /// <summary>Success color role.</summary>
    public required string Success { get; init; }
    /// <summary>On success color role.</summary>
    public required string OnSuccess { get; init; }
    /// <summary>Success container color role.</summary>
    public required string SuccessContainer { get; init; }
    /// <summary>On success container color role.</summary>
    public required string OnSuccessContainer { get; init; }
    /// <summary>Warning color role.</summary>
    public required string Warning { get; init; }
    /// <summary>On warning color role.</summary>
    public required string OnWarning { get; init; }
    /// <summary>Warning container color role.</summary>
    public required string WarningContainer { get; init; }
    /// <summary>On warning container color role.</summary>
    public required string OnWarningContainer { get; init; }
    /// <summary>Info color role.</summary>
    public required string Info { get; init; }
    /// <summary>On info color role.</summary>
    public required string OnInfo { get; init; }
    /// <summary>Info container color role.</summary>
    public required string InfoContainer { get; init; }
    /// <summary>On info container color role.</summary>
    public required string OnInfoContainer { get; init; }
    /// <summary>Surface color role.</summary>
    public required string Surface { get; init; }
    /// <summary>On surface color role.</summary>
    public required string OnSurface { get; init; }
    /// <summary>Surface variant color role.</summary>
    public required string SurfaceVariant { get; init; }
    /// <summary>On surface variant color role.</summary>
    public required string OnSurfaceVariant { get; init; }
    /// <summary>Surface container color role.</summary>
    public required string SurfaceContainer { get; init; }
    /// <summary>Surface container low color role.</summary>
    public required string SurfaceContainerLow { get; init; }
    /// <summary>Surface container high color role.</summary>
    public required string SurfaceContainerHigh { get; init; }
    /// <summary>Surface container highest color role.</summary>
    public required string SurfaceContainerHighest { get; init; }
    /// <summary>Background color role.</summary>
    public required string Background { get; init; }
    /// <summary>On background color role.</summary>
    public required string OnBackground { get; init; }
    /// <summary>Outline color role.</summary>
    public required string Outline { get; init; }
    /// <summary>Outline variant color role.</summary>
    public required string OutlineVariant { get; init; }
    /// <summary>Inverse surface color role.</summary>
    public required string InverseSurface { get; init; }
    /// <summary>Inverse on surface color role.</summary>
    public required string InverseOnSurface { get; init; }
    /// <summary>Inverse primary color role.</summary>
    public required string InversePrimary { get; init; }
    /// <summary>Scrim color role.</summary>
    public required string Scrim { get; init; }
    /// <summary>Shadow color role.</summary>
    public required string Shadow { get; init; }

    /// <summary>Dark core of a block shadow (Elevation geometry references it via var()).</summary>
    public string ShadowUmbra { get; init; } = "rgba(0,0,0,0.3)";
    /// <summary>Soft outer falloff of a block shadow (Elevation geometry references it via var()).</summary>
    public string ShadowPenumbra { get; init; } = "rgba(0,0,0,0.15)";
}
