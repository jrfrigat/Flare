using System.Globalization;
using System.Resources;

namespace Flare.Components.Gantt;

/// <summary>
/// User-facing texts of the Gantt components. Every text comes from the package's resources in the chart's culture;
/// set only the ones to replace (<c>new GanttTexts { Moved = "..." }</c>) and pass them as <c>FlareGantt.Texts</c>.
/// </summary>
public sealed record GanttTexts
{
    private static readonly ResourceManager Resources =
        new("Flare.Components.Resources.GanttStrings", typeof(GanttTexts).Assembly);

    /// <summary>Refusal for a link from an item to itself.</summary>
    public string? SelfLink { get; init; }

    /// <summary>Refusal for a link that closes a cycle; <c>{0}</c> is the cycle, item names joined by arrows.</summary>
    public string? Cycle { get; init; }

    /// <summary>Refusal for a link that already exists.</summary>
    public string? Duplicate { get; init; }

    /// <summary>Label of the toggle that shows a tree row's children.</summary>
    public string? Expand { get; init; }

    /// <summary>Label of the toggle that hides a tree row's children.</summary>
    public string? Collapse { get; init; }

    /// <summary>Announcement after a link was started from an item with the keyboard; <c>{0}</c> is the item.</summary>
    public string? LinkStarted { get; init; }

    /// <summary>Announcement after an item moved; <c>{0}</c> is the item, <c>{1}</c> its new dates.</summary>
    public string? Moved { get; init; }

    /// <summary>Label of the separator that resizes the row-header column.</summary>
    public string? ResizeSide { get; init; }

    /// <summary>Every text from the package's resources in <paramref name="culture"/>.</summary>
    public static GanttTexts For(CultureInfo culture) => new()
    {
        SelfLink = Text("Gantt_SelfLink", culture),
        Cycle = Text("Gantt_Cycle", culture),
        Duplicate = Text("Gantt_Duplicate", culture),
        Expand = Text("Gantt_Expand", culture),
        Collapse = Text("Gantt_Collapse", culture),
        LinkStarted = Text("Gantt_LinkStarted", culture),
        Moved = Text("Gantt_Moved", culture),
        ResizeSide = Text("Gantt_ResizeSide", culture),
    };

    /// <summary>These texts with every one that is not set taken from the resources in <paramref name="culture"/>.</summary>
    public GanttTexts Over(CultureInfo culture)
    {
        var builtIn = For(culture);
        return new GanttTexts
        {
            SelfLink = SelfLink ?? builtIn.SelfLink,
            Cycle = Cycle ?? builtIn.Cycle,
            Duplicate = Duplicate ?? builtIn.Duplicate,
            Expand = Expand ?? builtIn.Expand,
            Collapse = Collapse ?? builtIn.Collapse,
            LinkStarted = LinkStarted ?? builtIn.LinkStarted,
            Moved = Moved ?? builtIn.Moved,
            ResizeSide = ResizeSide ?? builtIn.ResizeSide,
        };
    }

    /// <summary>
    /// The message for a refused link, or null when it is valid. <paramref name="name"/> turns item keys into words;
    /// texts that are not set come from the resources in <paramref name="culture"/> (the current UI culture when null).
    /// </summary>
    public string? Describe(GanttValidation validation, Func<string, string>? name = null, CultureInfo? culture = null)
    {
        var texts = Over(culture ?? CultureInfo.CurrentUICulture);
        return validation.Code switch
        {
            GanttValidationCode.SelfLink => texts.SelfLink,
            GanttValidationCode.Cycle => string.Format(culture ?? CultureInfo.CurrentCulture, texts.Cycle!,
                string.Join(" \u2192 ", validation.Path.Select(name ?? (key => key)))),
            GanttValidationCode.Duplicate => texts.Duplicate,
            _ => null
        };
    }

    private static string Text(string key, CultureInfo culture) => Resources.GetString(key, culture) ?? key;
}
