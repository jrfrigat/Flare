using System.Runtime.CompilerServices;
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
    /// Immutable snapshot of the current theme; the component renders again when the theme changes.
    /// Use this for reading theme properties (IsDark, CurrentTheme, etc.).
    /// </summary>
    protected ThemeSnapshot? Theme => ThemeHolder?.Snapshot;

    // The provider cascades a fixed holder and the theme version beside it: only a new version - a real theme
    // change - re-renders the component, not every render of the provider's parent (see ThemeState).
    [CascadingParameter] private ThemeState? ThemeHolder { get; set; }
    [CascadingParameter(Name = ThemeState.VersionName)] private int ThemeVersion { get; set; }

    /// <summary>Additional attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Additional CSS class(es) appended to the component's root element.</summary>
    [Parameter] public string? Class { get; set; }
    /// <summary>Inline <c>style</c> string appended to the component's root element.</summary>
    [Parameter] public virtual string? Style { get; set; }

    /// <summary>The component's root CSS class; each component overrides this.</summary>
    protected abstract string ComponentCssClass { get; }

    /// <summary>Combines the root class, the given modifier classes and the user-supplied <see cref="Class"/>.</summary>
    protected string BuildCssClass(params string?[] additionalClasses) => BuildCssClassFromModifiers(additionalClasses);

    internal string BuildCssClassFromModifiers(ReadOnlySpan<string?> modifiers)
    {
        var root = ComponentCssClass;
        var userClass = string.IsNullOrWhiteSpace(Class) ? null : Class;
        while (!modifiers.IsEmpty && string.IsNullOrWhiteSpace(modifiers[0]))
            modifiers = modifiers[1..];

        if (modifiers.IsEmpty)
            return userClass is null ? root : string.Concat(root, " ", userClass);

        return JoinCssClasses(root, modifiers, userClass);
    }

    private static string JoinCssClasses(string root, ReadOnlySpan<string?> modifiers, string? userClass)
    {
        var capacity = modifiers.Length + 2;
#if NET9_0_OR_GREATER
        CssClassBuffer buffer = default;
        Span<string?> classes = capacity <= CssClassBuffer.Capacity ? buffer : new string?[capacity];
#else
        var classes = new string?[capacity];
#endif

        var count = 0;
        classes[count++] = root;
        foreach (var modifier in modifiers)
        {
            if (!string.IsNullOrWhiteSpace(modifier))
                classes[count++] = modifier;
        }

        if (userClass is not null)
            classes[count++] = userClass;

#if NET9_0_OR_GREATER
        return string.Join(' ', classes[..count]);
#else
        return string.Join(' ', classes, 0, count);
#endif
    }

#if NET9_0_OR_GREATER
    [InlineArray(Capacity)]
    private struct CssClassBuffer
    {
        public const int Capacity = 12;
        private string? _first;
    }
#endif

    // Most tokens are consumed by CSS and never enter C#. A few components draw geometry themselves -
    // SVG attributes take numbers, not var() references - and those have to ask the theme for the value.
    // The read goes through the FLATTENED map, the same source the emitted stylesheet is built from, so
    // what a component computes and what the stylesheet paints cannot disagree. Typed component tokens do
    // not appear in Design.Extended, so reading that alone silently returns nothing.
    //
    // Share the map by theme identity and let it go when the theme is no longer used.
    private static readonly ConditionalWeakTable<ITheme, Lazy<Dictionary<string, string>?>> TokenMaps = new();

    private Dictionary<string, string>? FlattenedTokens()
    {
        var theme = ThemeService?.CurrentTheme;
        return theme is null ? null : TokenMaps.GetValue(theme, static key =>
            new Lazy<Dictionary<string, string>?>(() => key.Design?.FlattenDesign())).Value;
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
    /// <remarks>
    /// Blazor calls this contract alone when a component implements both <see cref="IDisposable"/> and
    /// <see cref="IAsyncDisposable"/>, so the default forwards to <see cref="IDisposable.Dispose"/>: a component
    /// releases its state in one of the two, never in both, and a component that only implements the shorter
    /// contract is still cleaned up.
    /// </remarks>
    public virtual ValueTask DisposeAsync()
    {
        (this as IDisposable)?.Dispose();
        return ValueTask.CompletedTask;
    }
}
