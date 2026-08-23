using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Theming;   // FlattenDesign (CssVarMap) extension
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
        // Fast path: no modifiers and no user Class -- the most common per-render case for many
        // components -- returns the root class directly, avoiding a List allocation and string.Join.
        bool hasClass = !string.IsNullOrWhiteSpace(Class);
        if (!hasClass && (additionalClasses is null || additionalClasses.Length == 0))
            return ComponentCssClass;

        var parts = new List<string>((additionalClasses?.Length ?? 0) + 2) { ComponentCssClass };
        if (additionalClasses is not null)
            foreach (var c in additionalClasses)
                if (!string.IsNullOrWhiteSpace(c))
                    parts.Add(c);
        if (hasClass)
            parts.Add(Class!);
        return parts.Count == 1 ? ComponentCssClass : string.Join(' ', parts);
    }

    // ---- Theme-token reads -------------------------------------------------------------------------
    // Most tokens are consumed by CSS and never enter C#. A few components draw geometry themselves -
    // SVG attributes take numbers, not var() references - and those have to ask the theme for the value.
    // The read goes through the FLATTENED map, the same source the emitted stylesheet is built from, so
    // what a component computes and what the stylesheet paints cannot disagree. Typed component tokens do
    // not appear in Design.Extended, so reading that alone silently returns nothing.
    //
    // Cached per theme instance: a theme's Design is usually an expression-bodied property that rebuilds
    // the record on every access, so the cache is keyed on the THEME - keying it on Design would never hit.

    private object? _tokenCacheKey;
    private Dictionary<string, string>? _tokenCache;

    private Dictionary<string, string>? FlattenedTokens()
    {
        var theme = ThemeService?.CurrentTheme;
        if (theme is null) return null;
        if (!ReferenceEquals(theme, _tokenCacheKey))
        {
            _tokenCache = theme.Design?.FlattenDesign();
            _tokenCacheKey = theme;
        }
        return _tokenCache;
    }

    /// <summary>The raw theme value behind a CSS custom-property name, or null when no theme supplies it.
    /// Custom tokens set on the theme service win over the theme's own value.</summary>
    /// <param name="name">The <c>--flare-*</c> custom-property name.</param>
    protected string? ReadTokenRaw(string name)
    {
        if (ThemeService is null) return null;
        if (ThemeService.GetCustomTokens() is { } custom && custom.TryGetValue(name, out var c)) return c;
        if (FlattenedTokens() is { } all && all.TryGetValue(name, out var v)) return v;
        return null;
    }

    /// <summary>Reads a numeric (optionally <c>px</c>-suffixed) theme token, falling back when it is unset
    /// or unparseable - which is the unthemed case, where the component is meant to render unstyled.</summary>
    /// <param name="name">The <c>--flare-*</c> custom-property name.</param>
    /// <param name="fallback">Value to use when the token is absent or not a number.</param>
    protected double ReadTokenNum(string name, double fallback)
    {
        var raw = ReadTokenRaw(name);
        if (string.IsNullOrWhiteSpace(raw)) return fallback;
        raw = raw.Trim();
        if (raw.EndsWith("px", StringComparison.OrdinalIgnoreCase)) raw = raw[..^2];
        return double.TryParse(raw, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : fallback;
    }

    /// <summary>Reads a string theme token, falling back when it is unset.</summary>
    /// <param name="name">The <c>--flare-*</c> custom-property name.</param>
    /// <param name="fallback">Value to use when the token is absent.</param>
    protected string ReadTokenStr(string name, string fallback)
    {
        var raw = ReadTokenRaw(name);
        return string.IsNullOrWhiteSpace(raw) ? fallback : raw.Trim();
    }

    /// <summary>Disposes the component; override to release JS interop or subscriptions.</summary>
    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
