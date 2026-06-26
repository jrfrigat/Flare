using Flare.Core.Abstractions;
using Flare.Core.Tokens;

namespace Flare.Core.Components;

/// <summary>
/// Base class for components that need direct theme access. Extends <see cref="FlareComponentBase"/>
/// with convenience properties for reading theme state. Use this when your component needs to
/// react to theme changes (e.g., reading IsDark, CurrentTheme properties).
/// </summary>
public abstract class FlareThemedComponentBase : FlareComponentBase
{
    /// <summary>
    /// Non-null <see cref="IThemeService"/> for components that require theme operations.
    /// Throws if no theme provider is ancestor (use base ThemeService for nullable access).
    /// </summary>
    protected IThemeService RequiredThemeService => ThemeService
        ?? throw new InvalidOperationException(
            "No FlareThemeProvider ancestor found. Wrap your app with <FlareThemeProvider>.");

    /// <summary>
    /// Non-null <see cref="ThemeSnapshot"/> for components that require theme state.
    /// Throws if no theme provider is ancestor.
    /// </summary>
    protected ThemeSnapshot RequiredTheme => Theme
        ?? throw new InvalidOperationException(
            "No FlareThemeProvider ancestor found. Wrap your app with <FlareThemeProvider>.");

    /// <summary>Whether the current mode is dark. Convenience for <c>Theme?.IsDark == true</c>.</summary>
    protected bool IsDark => Theme?.IsDark == true;

    /// <summary>Whether high-contrast mode is active. Convenience for <c>Theme?.IsHighContrast == true</c>.</summary>
    protected bool IsHighContrast => Theme?.IsHighContrast == true;

    /// <summary>Whether the current mode is RTL. Convenience for <c>Theme?.IsRtl == true</c>.</summary>
    protected bool IsRtl => Theme?.IsRtl == true;

    /// <summary>The current theme ID. Convenience for <c>Theme?.Theme?.Id</c>.</summary>
    protected string? ThemeId => Theme?.Theme?.Id;

    /// <summary>The current palette ID. Convenience for <c>Theme?.Palette?.Id</c>.</summary>
    protected string? PaletteId => Theme?.Palette?.Id;
}
