namespace Flare.Abstractions.Tokens;

/// <summary>
/// Light/dark selection for a theme. A theme carries both schemes (via its palette);
/// the mode chooses which one is active. <see cref="Auto"/> follows the OS preference.
/// <see cref="HighContrast"/> uses a high-contrast color scheme for accessibility.
/// </summary>
public enum ThemeMode
{
    /// <summary>Always use the light color scheme.</summary>
    Light,
    /// <summary>Always use the dark color scheme.</summary>
    Dark,
    /// <summary>Follow the OS <c>prefers-color-scheme</c> setting.</summary>
    Auto,
    /// <summary>Use high-contrast color scheme for improved accessibility (WCAG AAA).</summary>
    HighContrast,
}
