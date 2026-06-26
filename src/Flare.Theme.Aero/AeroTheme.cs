using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.Aero;

/// <summary>Aero theme (Windows 7 Aero / Office 2010 / 1C). Light/dark is a mode; colors come from a palette.</summary>
public sealed class AeroTheme : ITheme
{
    /// <summary>The stable theme id - use this constant to switch themes without a magic string.</summary>

    public const string ThemeId = "aero";

    /// <inheritdoc />

    public string Id => ThemeId;
    public string DisplayName => "Aero";
    public DesignTokens Design => AeroTokens.Design;
    public string DefaultPaletteId => AeroPalettes.Blue.Id;
    public IReadOnlyList<Palette> Palettes => AeroPalettes.All;
    public IPaletteGenerator? PaletteGenerator => AeroRampGenerator.Instance;

    public IReadOnlyList<string> StyleAssets =>
    [
        "_content/Flare.Theme.Aero/css/aero-base.css",
        "_content/Flare.Theme.Aero/css/components/button.css",
        "_content/Flare.Theme.Aero/css/components/input.css",
        "_content/Flare.Theme.Aero/css/components/controls.css",
        "_content/Flare.Theme.Aero/css/components/surfaces.css",
        "_content/Flare.Theme.Aero/css/components/chrome.css",
        "_content/Flare.Theme.Aero/css/components/pickers.css",
    ];

    // The Aero focus glow flips to the lighter blue in dark mode.
    public IReadOnlyDictionary<string, string>? ExtendedDarkOverride => AeroTokens.DarkExtended;
}

/// <summary>Built-in Aero palettes (era-accurate accent colors).</summary>
public static class AeroPalettes
{
    /// <summary>Palette id for <c>Blue</c> (<c>aero-blue</c>); switch palettes without a magic string.</summary>
    public const string BlueId = "aero-blue";
    /// <summary>Palette id for <c>Olive</c> (<c>aero-olive</c>); switch palettes without a magic string.</summary>
    public const string OliveId = "aero-olive";
    /// <summary>Palette id for <c>Steel</c> (<c>aero-steel</c>); switch palettes without a magic string.</summary>
    public const string SteelId = "aero-steel";
    /// <summary>Palette id for <c>Amber</c> (<c>aero-amber</c>); switch palettes without a magic string.</summary>
    public const string AmberId = "aero-amber";
    /// <summary>Palette id for <c>Teal</c> (<c>aero-teal</c>); switch palettes without a magic string.</summary>
    public const string TealId = "aero-teal";

    /// <summary>Source label for grouping Aero palettes in pickers.</summary>
    public const string SourceName = "Aero";

    /// <summary>The default Aero blue palette -- light + dark.</summary>
    public static readonly Palette Blue = new()
    {
        Id = BlueId,
        Name = "Aero Blue",
        Source = SourceName,
        Light = AeroTokens.LightColors,
        Dark = AeroTokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, AeroTokens.LightColors, AeroTokens.DarkColors, seed, SourceName);

    /// <summary>Windows classic olive green.</summary>
    public static readonly Palette Olive = Brand(OliveId, "Olive", "#6B7A34");
    /// <summary>Office 2010 silver / steel.</summary>
    public static readonly Palette Steel = Brand(SteelId, "Steel", "#5A6B7B");
    /// <summary>1C-style warm amber accent.</summary>
    public static readonly Palette Amber = Brand(AmberId, "Amber", "#C98A1E");
    /// <summary>Royale teal (the other XP/Win7 accent).</summary>
    public static readonly Palette Teal = Brand(TealId, "Teal", "#1E8A8A");

    /// <summary>All built-in Aero palettes.</summary>
    public static IReadOnlyList<Palette> All => [Blue, Steel, Olive, Teal, Amber];
}
