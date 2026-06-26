using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.FluentUI2;

/// <summary>Fluent UI 2 theme (design tokens). Light/dark is a mode; colors come from a palette.</summary>
public sealed class Fluent2Theme : ITheme
{
    /// <summary>The stable theme id - use this constant to switch themes without a magic string.</summary>

    public const string ThemeId = "fluent2";

    /// <inheritdoc />

    public string Id => ThemeId;
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
    /// <summary>Palette id for <c>Blue</c> (<c>fluent-blue</c>); switch palettes without a magic string.</summary>
    public const string BlueId = "fluent-blue";
    /// <summary>Palette id for <c>Word</c> (<c>fluent-word</c>); switch palettes without a magic string.</summary>
    public const string WordId = "fluent-word";
    /// <summary>Palette id for <c>Excel</c> (<c>fluent-excel</c>); switch palettes without a magic string.</summary>
    public const string ExcelId = "fluent-excel";
    /// <summary>Palette id for <c>PowerPoint</c> (<c>fluent-powerpoint</c>); switch palettes without a magic string.</summary>
    public const string PowerPointId = "fluent-powerpoint";
    /// <summary>Palette id for <c>Teams</c> (<c>fluent-teams</c>); switch palettes without a magic string.</summary>
    public const string TeamsId = "fluent-teams";
    /// <summary>Palette id for <c>Outlook</c> (<c>fluent-outlook</c>); switch palettes without a magic string.</summary>
    public const string OutlookId = "fluent-outlook";
    /// <summary>Palette id for <c>OneNote</c> (<c>fluent-onenote</c>); switch palettes without a magic string.</summary>
    public const string OneNoteId = "fluent-onenote";

    /// <summary>Source label for grouping Fluent palettes in pickers.</summary>
    public const string SourceName = "Fluent UI 2";

    /// <summary>The default Fluent (communication blue) palette -- light + dark.</summary>
    public static readonly Palette Blue = new()
    {
        Id = BlueId,
        Name = "Blue",
        Source = SourceName,
        Light = FluentUI2Tokens.LightColors,
        Dark = FluentUI2Tokens.DarkColors,
    };

    private static Palette Brand(string id, string name, string seed) =>
        PaletteFactory.Brand(id, name, FluentUI2Tokens.LightColors, FluentUI2Tokens.DarkColors, seed, SourceName);

    /// <summary>Word blue.</summary>
    public static readonly Palette Word = Brand(WordId, "Word", "#2B579A");
    /// <summary>Excel green.</summary>
    public static readonly Palette Excel = Brand(ExcelId, "Excel", "#217346");
    /// <summary>PowerPoint orange-red.</summary>
    public static readonly Palette PowerPoint = Brand(PowerPointId, "PowerPoint", "#C43E1C");
    /// <summary>Teams purple.</summary>
    public static readonly Palette Teams = Brand(TeamsId, "Teams", "#5B5FC7");
    /// <summary>Outlook blue.</summary>
    public static readonly Palette Outlook = Brand(OutlookId, "Outlook", "#0078D4");
    /// <summary>OneNote purple.</summary>
    public static readonly Palette OneNote = Brand(OneNoteId, "OneNote", "#7719AA");

    /// <summary>All built-in Fluent palettes.</summary>
    public static IReadOnlyList<Palette> All => [Blue, Word, Excel, PowerPoint, Teams, Outlook, OneNote];
}
