using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.VisualStudio;

/// <summary>Visual Studio 2026 theme (modern VS shell). Light/dark is a mode; colors come from a palette.</summary>
public sealed class VisualStudioTheme : ITheme
{
    /// <summary>The stable theme id - use this constant to switch themes without a magic string.</summary>

    public const string ThemeId = "visualstudio";

    /// <inheritdoc />

    public string Id => ThemeId;
    public string DisplayName => "Visual Studio 2026";
    public DesignTokens Design => VisualStudioTokens.Design;
    public string DefaultPaletteId => VisualStudioPalettes.Blue.Id;
    public IReadOnlyList<Palette> Palettes => VisualStudioPalettes.All;
    public IPaletteGenerator? PaletteGenerator => VisualStudioRampGenerator.Instance;

    public IReadOnlyList<string> StyleAssets =>
    [
        "_content/Flare.Theme.VisualStudio/css/vs-base.css",
        "_content/Flare.Theme.VisualStudio/css/components/tabs.css",
    ];

    // The VS focus ring color flips to the brighter blue in dark mode.
    public IReadOnlyDictionary<string, string>? ExtendedDarkOverride => VisualStudioTokens.DarkExtended;
}

/// <summary>Built-in Visual Studio palettes (VS accent colors).</summary>
public static class VisualStudioPalettes
{
    /// <summary>Palette id for <c>Blue</c> (<c>vs-blue</c>); switch palettes without a magic string.</summary>
    public const string BlueId = "vs-blue";
    /// <summary>Palette id for <c>Purple</c> (<c>vs-purple</c>); switch palettes without a magic string.</summary>
    public const string PurpleId = "vs-purple";
    /// <summary>Palette id for <c>Teal</c> (<c>vs-teal</c>); switch palettes without a magic string.</summary>
    public const string TealId = "vs-teal";
    /// <summary>Palette id for <c>Green</c> (<c>vs-green</c>); switch palettes without a magic string.</summary>
    public const string GreenId = "vs-green";
    /// <summary>Palette id for <c>Amber</c> (<c>vs-amber</c>); switch palettes without a magic string.</summary>
    public const string AmberId = "vs-amber";

    /// <summary>Source label for grouping Visual Studio palettes in pickers.</summary>
    public const string SourceName = "Visual Studio";

    /// <summary>The default Visual Studio blue palette -- light + dark.</summary>
    public static readonly Palette Blue = new()
    {
        Id = BlueId,
        Name = "VS Blue",
        Source = SourceName,
        Light = VisualStudioTokens.LightColors,
        Dark = VisualStudioTokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, VisualStudioTokens.LightColors, VisualStudioTokens.DarkColors, seed, SourceName);

    /// <summary>The classic Visual Studio brand purple.</summary>
    public static readonly Palette Purple = Brand(PurpleId, "VS Purple", "#68217A");
    /// <summary>The classic VS 2017 teal accent.</summary>
    public static readonly Palette Teal = Brand(TealId, "VS Teal", "#007E7E");
    /// <summary>.NET / build green.</summary>
    public static readonly Palette Green = Brand(GreenId, "VS Green", "#16825D");
    /// <summary>Warm amber accent.</summary>
    public static readonly Palette Amber = Brand(AmberId, "VS Amber", "#CA5010");

    /// <summary>All built-in Visual Studio palettes.</summary>
    public static IReadOnlyList<Palette> All => [Blue, Purple, Teal, Green, Amber];
}
