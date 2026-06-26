using Flare.Abstractions;
using Flare.Abstractions.Tokens;

namespace Flare.Theming;

/// <summary>
/// Helpers for deriving a tweaked <see cref="ITheme"/> from an existing one by composition - the
/// preferred alternative to subclassing a (sealed) theme. <see cref="Derive"/> wraps a base theme and
/// overrides only the parts you specify, forwarding everything else, so you get the ergonomics of
/// "take this theme and change a few parameters" without inheritance or a fragile base class.
/// </summary>
public static class ThemeDerivation
{
    /// <summary>
    /// Creates a new <see cref="ITheme"/> that forwards to <paramref name="baseTheme"/> for everything
    /// except the members you override. Only <paramref name="id"/> is required (a derived theme needs a
    /// distinct id so it can be registered alongside its base).
    /// <code>
    /// var myFluent = new Fluent2Theme().Derive(
    ///     id: "my-fluent",
    ///     design: d => d with { Shape = d.Shape with { Medium = "10px" } });
    /// services.AddFlareTheme(myFluent);
    /// </code>
    /// </summary>
    /// <param name="baseTheme">The theme to derive from; all non-overridden members are taken from it.</param>
    /// <param name="id">The derived theme's stable id (must differ from the base id).</param>
    /// <param name="displayName">Display name; defaults to the base theme's.</param>
    /// <param name="design">Transforms the base <see cref="DesignTokens"/> (e.g. <c>d =&gt; d with { ... }</c>);
    /// when null the base design is used unchanged.</param>
    /// <param name="palettes">Overrides the palettes; defaults to the base theme's.</param>
    /// <param name="defaultPaletteId">Overrides the default palette id; defaults to the base theme's.</param>
    /// <param name="styleAssets">Overrides the static style assets; defaults to the base theme's.</param>
    /// <param name="paletteGenerator">Overrides the palette generator; defaults to the base theme's.</param>
    /// <param name="extendedDarkOverride">Overrides the dark-mode extras; defaults to the base theme's.</param>
    public static ITheme Derive(
        this ITheme baseTheme,
        string id,
        string? displayName = null,
        Func<DesignTokens, DesignTokens>? design = null,
        IReadOnlyList<Palette>? palettes = null,
        string? defaultPaletteId = null,
        IReadOnlyList<string>? styleAssets = null,
        IPaletteGenerator? paletteGenerator = null,
        IReadOnlyDictionary<string, string>? extendedDarkOverride = null)
    {
        ArgumentNullException.ThrowIfNull(baseTheme);
        ArgumentException.ThrowIfNullOrEmpty(id);
        return new DerivedTheme(baseTheme, id, displayName, design, palettes,
            defaultPaletteId, styleAssets, paletteGenerator, extendedDarkOverride);
    }
}

/// <summary>An <see cref="ITheme"/> that decorates a base theme, overriding only the specified members.</summary>
internal sealed class DerivedTheme : ITheme
{
    private readonly DesignTokens _design;
    private readonly IReadOnlyList<Palette> _palettes;
    private readonly string _defaultPaletteId;
    private readonly IReadOnlyList<string> _styleAssets;
    private readonly IPaletteGenerator? _paletteGenerator;
    private readonly IReadOnlyDictionary<string, string>? _extendedDarkOverride;

    public DerivedTheme(
        ITheme baseTheme, string id, string? displayName,
        Func<DesignTokens, DesignTokens>? design, IReadOnlyList<Palette>? palettes,
        string? defaultPaletteId, IReadOnlyList<string>? styleAssets,
        IPaletteGenerator? paletteGenerator, IReadOnlyDictionary<string, string>? extendedDarkOverride)
    {
        Id = id;
        DisplayName = displayName ?? baseTheme.DisplayName;
        // Compute the derived design once: the base design is stable, so there is no need to re-run
        // the transform on every Design access.
        _design = design is null ? baseTheme.Design : design(baseTheme.Design);
        _palettes = palettes ?? baseTheme.Palettes;
        _defaultPaletteId = defaultPaletteId ?? baseTheme.DefaultPaletteId;
        _styleAssets = styleAssets ?? baseTheme.StyleAssets;
        _paletteGenerator = paletteGenerator ?? baseTheme.PaletteGenerator;
        _extendedDarkOverride = extendedDarkOverride ?? baseTheme.ExtendedDarkOverride;
    }

    public string Id { get; }
    public string DisplayName { get; }
    public DesignTokens Design => _design;
    public string DefaultPaletteId => _defaultPaletteId;
    public IReadOnlyList<Palette> Palettes => _palettes;
    public IReadOnlyList<string> StyleAssets => _styleAssets;
    public IPaletteGenerator? PaletteGenerator => _paletteGenerator;
    public IReadOnlyDictionary<string, string>? ExtendedDarkOverride => _extendedDarkOverride;
}
