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
    /// var myTheme = new SomeBaseTheme().Derive(
    ///     id: "my-theme",
    ///     design: d => d with { Shape = d.Shape with { Medium = "10px" } });
    /// services.AddFlareTheme(myTheme);
    /// </code>
    /// </summary>
    /// <param name="baseTheme">The theme to derive from; all non-overridden members are taken from it.</param>
    /// <param name="id">The derived theme's stable id (must differ from the base id).</param>
    /// <param name="displayName">Display name; defaults to the base theme's.</param>
    /// <param name="design">Transforms the base <see cref="DesignTokens"/> (e.g. <c>d =&gt; d with { ... }</c>);
    /// when null the base design is used unchanged.</param>
    /// <param name="palettes">Overrides the palettes; defaults to the base theme's.</param>
    /// <param name="defaultPaletteId">Overrides the default palette id; defaults to the base theme's.</param>
    /// <param name="styleAssets">Stylesheets this theme adds. They are loaded after the base theme's, so a
    /// rule scoped to this theme's id wins over the base's rule for the same property; an asset the base
    /// already lists is not repeated.</param>
    /// <param name="paletteGenerator">Overrides the palette generator; defaults to the base theme's.</param>
    /// <param name="extendedDarkOverride">Overrides the dark-mode extras; defaults to the base theme's.</param>
    /// <param name="scriptAssets">JavaScript modules this theme adds, loaded after the base theme's; an
    /// asset the base already lists is not repeated.</param>
    /// <param name="inheritStyleAssets">Whether the base theme's stylesheets are loaded at all. Pass
    /// <c>false</c> to replace them with <paramref name="styleAssets"/> - for example to self-host a font
    /// the base loads from a CDN. The root still carries the base theme's class either way.</param>
    /// <remarks>The derived theme's <see cref="ITheme.Base"/> is <paramref name="baseTheme"/>, so its root
    /// carries the class of every theme in the chain and the stylesheets of each keep applying to it.</remarks>
    public static ITheme Derive(
        this ITheme baseTheme,
        string id,
        string? displayName = null,
        Func<DesignTokens, DesignTokens>? design = null,
        IReadOnlyList<Palette>? palettes = null,
        string? defaultPaletteId = null,
        IReadOnlyList<string>? styleAssets = null,
        IPaletteGenerator? paletteGenerator = null,
        IReadOnlyDictionary<string, string>? extendedDarkOverride = null,
        IReadOnlyList<string>? scriptAssets = null,
        bool inheritStyleAssets = true)
    {
        ArgumentNullException.ThrowIfNull(baseTheme);
        ArgumentException.ThrowIfNullOrEmpty(id);
        return new DerivedTheme(baseTheme, id, displayName, design, palettes,
            defaultPaletteId, styleAssets, scriptAssets, inheritStyleAssets, paletteGenerator, extendedDarkOverride);
    }
}

/// <summary>An <see cref="ITheme"/> that decorates a base theme, overriding only the specified members.</summary>
internal sealed class DerivedTheme : ITheme
{
    private readonly DesignTokens _design;
    private readonly IReadOnlyList<Palette> _palettes;
    private readonly string _defaultPaletteId;
    private readonly IReadOnlyList<string> _styleAssets;
    private readonly IReadOnlyList<string> _scriptAssets;
    private readonly IPaletteGenerator? _paletteGenerator;
    private readonly IReadOnlyDictionary<string, string>? _extendedDarkOverride;

    public DerivedTheme(
        ITheme baseTheme, string id, string? displayName,
        Func<DesignTokens, DesignTokens>? design, IReadOnlyList<Palette>? palettes,
        string? defaultPaletteId, IReadOnlyList<string>? styleAssets, IReadOnlyList<string>? scriptAssets,
        bool inheritStyleAssets,
        IPaletteGenerator? paletteGenerator, IReadOnlyDictionary<string, string>? extendedDarkOverride)
    {
        Id = id;
        Base = baseTheme;
        DisplayName = displayName ?? baseTheme.DisplayName;
        // Compute the derived design once: the base design is stable, so there is no need to re-run
        // the transform on every Design access.
        _design = design is null ? baseTheme.Design : design(baseTheme.Design);
        _palettes = palettes ?? baseTheme.Palettes;
        _defaultPaletteId = defaultPaletteId ?? baseTheme.DefaultPaletteId;
        // Ancestors first: the rules of a later stylesheet win a tie, and the root carries every
        // generation's class on the same element, so load order is what lets this theme override its base.
        _styleAssets = ThemeLineage.Compose(inheritStyleAssets ? baseTheme.StyleAssets : [], styleAssets ?? []);
        _scriptAssets = ThemeLineage.Compose(baseTheme.ScriptAssets, scriptAssets ?? []);
        _paletteGenerator = paletteGenerator ?? baseTheme.PaletteGenerator;
        _extendedDarkOverride = extendedDarkOverride ?? baseTheme.ExtendedDarkOverride;
    }

    public string Id { get; }
    public ITheme Base { get; }
    public string DisplayName { get; }
    public DesignTokens Design => _design;
    public string DefaultPaletteId => _defaultPaletteId;
    public IReadOnlyList<Palette> Palettes => _palettes;
    public IReadOnlyList<string> StyleAssets => _styleAssets;
    public IReadOnlyList<string> ScriptAssets => _scriptAssets;
    public IPaletteGenerator? PaletteGenerator => _paletteGenerator;
    public IReadOnlyDictionary<string, string>? ExtendedDarkOverride => _extendedDarkOverride;
}
