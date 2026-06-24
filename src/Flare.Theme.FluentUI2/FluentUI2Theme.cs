using Flare.Core.Abstractions;
using Flare.Core.Services;
using Flare.Core.Tokens;

namespace Flare.Theme.FluentUI2;

/// <summary>Fluent UI 2 theme (design tokens). Light/dark is a mode; colors come from a palette.</summary>
public sealed class Fluent2Theme : ITheme
{
    public string Id => "fluent2";
    public string DisplayName => "Fluent UI 2";
    public DesignTokens Design => FluentUI2Tokens.Design;
    public string DefaultPaletteId => Fluent2Palettes.Blue.Id;
    public IReadOnlyList<Palette> Palettes => Fluent2Palettes.All;
    public IPaletteGenerator? PaletteGenerator => Fluent2RampGenerator.Instance;

    public IReadOnlyList<string> StyleAssets =>
    [
        "_content/Flare.Theme.FluentUI2/css/fluent2-base.css",
        "_content/Flare.Theme.FluentUI2/css/components/button.css",
    ];

    // Fluent flips a few focus/switch colors in dark mode (mode-specific Extended keys).
    public IReadOnlyDictionary<string, string>? ExtendedDarkOverride => FluentUI2Tokens.DarkExtended;
}

/// <summary>Built-in Fluent UI 2 palettes (Office brand colors).</summary>
public static class Fluent2Palettes
{
    /// <summary>Source label for grouping Fluent palettes in pickers.</summary>
    public const string SourceName = "Fluent UI 2";

    /// <summary>The default Fluent (communication blue) palette -- light + dark.</summary>
    public static readonly Palette Blue = new()
    {
        Id = "fluent-blue",
        Name = "Blue",
        Source = SourceName,
        Light = FluentUI2Tokens.LightColors,
        Dark = FluentUI2Tokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, FluentUI2Tokens.LightColors, FluentUI2Tokens.DarkColors, seed, SourceName);

    /// <summary>Word blue.</summary>
    public static readonly Palette Word = Brand("fluent-word", "Word", "#2B579A");
    /// <summary>Excel green.</summary>
    public static readonly Palette Excel = Brand("fluent-excel", "Excel", "#217346");
    /// <summary>PowerPoint orange-red.</summary>
    public static readonly Palette PowerPoint = Brand("fluent-powerpoint", "PowerPoint", "#C43E1C");
    /// <summary>Teams purple.</summary>
    public static readonly Palette Teams = Brand("fluent-teams", "Teams", "#5B5FC7");
    /// <summary>Outlook blue.</summary>
    public static readonly Palette Outlook = Brand("fluent-outlook", "Outlook", "#0078D4");
    /// <summary>OneNote purple.</summary>
    public static readonly Palette OneNote = Brand("fluent-onenote", "OneNote", "#7719AA");

    /// <summary>All built-in Fluent palettes.</summary>
    public static IReadOnlyList<Palette> All => [Blue, Word, Excel, PowerPoint, Teams, Outlook, OneNote];
}
