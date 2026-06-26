using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Base class for all Flare components. Provides theme access via cascading parameters
/// and automatic re-renders when the theme changes (via CascadingValue pattern, not subscriptions).
/// </summary>
public abstract class FlareComponentBase : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// The active theme service. Cascaded from FlareThemeProvider. Use for theme operations
    /// (switching themes, palettes, modes). For reading current theme state, prefer <see cref="Theme"/>.
    /// </summary>
    [CascadingParameter]
    protected IThemeService? ThemeService { get; set; }

    /// <summary>
    /// Immutable snapshot of the current theme. Automatically triggers re-render when changed.
    /// Use this for reading theme properties (IsDark, CurrentTheme, etc.).
    /// </summary>
    [CascadingParameter]
    protected ThemeSnapshot? Theme { get; set; }

    /// <summary>Additional attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Additional CSS class(es) appended to the component's root element.</summary>
    [Parameter] public string? Class { get; set; }
    /// <summary>Inline <c>style</c> string appended to the component's root element.</summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>The component's root CSS class; each component overrides this.</summary>
    protected abstract string ComponentCssClass { get; }

    /// <summary>Combines the root class, the given modifier classes and the user-supplied <see cref="Class"/>.</summary>
    protected string BuildCssClass(params string?[] additionalClasses)
    {
        var parts = new List<string>(4) { ComponentCssClass };
        foreach (var c in additionalClasses)
            if (!string.IsNullOrWhiteSpace(c))
                parts.Add(c);
        if (!string.IsNullOrWhiteSpace(Class))
            parts.Add(Class);
        return string.Join(' ', parts);
    }

    /// <summary>Disposes the component; override to release JS interop or subscriptions.</summary>
    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
