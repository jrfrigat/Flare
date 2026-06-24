using Flare.Core.Abstractions;
using Flare.Core.Services;
using Flare.Core.Tokens;

namespace Flare.Theme.VisualStudio;

/// <summary>Visual Studio 2026 theme (modern VS shell). Light/dark is a mode; colors come from a palette.</summary>
public sealed class VisualStudioTheme : ITheme
{
    public string Id => "visualstudio";
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
    /// <summary>Source label for grouping Visual Studio palettes in pickers.</summary>
    public const string SourceName = "Visual Studio";

    /// <summary>The default Visual Studio blue palette -- light + dark.</summary>
    public static readonly Palette Blue = new()
    {
        Id = "vs-blue",
        Name = "VS Blue",
        Source = SourceName,
        Light = VisualStudioTokens.LightColors,
        Dark = VisualStudioTokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, VisualStudioTokens.LightColors, VisualStudioTokens.DarkColors, seed, SourceName);

    /// <summary>The classic Visual Studio brand purple.</summary>
    public static readonly Palette Purple = Brand("vs-purple", "VS Purple", "#68217A");
    /// <summary>The classic VS 2017 teal accent.</summary>
    public static readonly Palette Teal = Brand("vs-teal", "VS Teal", "#007E7E");
    /// <summary>.NET / build green.</summary>
    public static readonly Palette Green = Brand("vs-green", "VS Green", "#16825D");
    /// <summary>Warm amber accent.</summary>
    public static readonly Palette Amber = Brand("vs-amber", "VS Amber", "#CA5010");

    /// <summary>All built-in Visual Studio palettes.</summary>
    public static IReadOnlyList<Palette> All => [Blue, Purple, Teal, Green, Amber];
}
