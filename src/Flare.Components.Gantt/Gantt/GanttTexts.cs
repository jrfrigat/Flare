using System.Globalization;

namespace Flare.Components.Gantt;

/// <summary>User-facing texts of the Gantt components. Replace any of them through the components' <c>Texts</c> parameter.</summary>
/// <param name="SelfLink">Refusal for a link from an item to itself.</param>
/// <param name="Cycle">Refusal for a link that closes a cycle; <c>{0}</c> is the cycle, keys joined by arrows.</param>
/// <param name="Duplicate">Refusal for a link that already exists.</param>
/// <param name="Expand">Label of the expand toggle.</param>
/// <param name="Collapse">Label of the collapse toggle.</param>
/// <param name="LinkStarted">Announcement after a link was started from an item; <c>{0}</c> is the item.</param>
/// <param name="Moved">Announcement after an item moved; <c>{0}</c> is the item, <c>{1}</c> its new span.</param>
public sealed record GanttTexts(string SelfLink, string Cycle, string Duplicate, string Expand, string Collapse, string LinkStarted, string Moved)
{
    /// <summary>English texts.</summary>
    public static GanttTexts English { get; } = new(
        "An item cannot depend on itself.",
        "This link would create a cycle: {0}.",
        "This link already exists.",
        "Expand", "Collapse",
        "Linking from {0}. Choose the successor and press Enter.",
        "{0} moved to {1}.") { ResizeSide = "Resize the column: drag, arrow keys, Home to reset" };

    /// <summary>Russian texts.</summary>
    public static GanttTexts Russian { get; } = new(
        "\u042D\u043B\u0435\u043C\u0435\u043D\u0442 \u043D\u0435 \u043C\u043E\u0436\u0435\u0442 \u0437\u0430\u0432\u0438\u0441\u0435\u0442\u044C \u043E\u0442 \u0441\u0430\u043C\u043E\u0433\u043E \u0441\u0435\u0431\u044F.",
        "\u0421\u0432\u044F\u0437\u044C \u0441\u043E\u0437\u0434\u0430\u0435\u0442 \u0446\u0438\u043A\u043B: {0}.",
        "\u0422\u0430\u043A\u0430\u044F \u0441\u0432\u044F\u0437\u044C \u0443\u0436\u0435 \u0435\u0441\u0442\u044C.",
        "\u0420\u0430\u0437\u0432\u0435\u0440\u043D\u0443\u0442\u044C", "\u0421\u0432\u0435\u0440\u043D\u0443\u0442\u044C",
        "\u0421\u0432\u044F\u0437\u044C \u043E\u0442 {0}. \u0412\u044B\u0431\u0435\u0440\u0438\u0442\u0435 \u043F\u043E\u0441\u043B\u0435\u0434\u043E\u0432\u0430\u0442\u0435\u043B\u044F \u0438 \u043D\u0430\u0436\u043C\u0438\u0442\u0435 Enter.",
        "{0} \u043F\u0435\u0440\u0435\u043D\u0435\u0441\u0435\u043D: {1}.") { ResizeSide = "\u0418\u0437\u043C\u0435\u043D\u0438\u0442\u0435 \u0448\u0438\u0440\u0438\u043D\u0443 \u043A\u043E\u043B\u043E\u043D\u043A\u0438: \u0442\u044F\u043D\u0438\u0442\u0435 \u043C\u044B\u0448\u044C\u044E, \u0441\u0442\u0440\u0435\u043B\u043A\u0438, Home \u2014 \u0441\u0431\u0440\u043E\u0441" };

    /// <summary>Label of the handle that resizes the row-header column.</summary>
    public string ResizeSide { get; init; } = "Drag to resize the column";




    /// <summary>The built-in texts for <paramref name="culture"/>: Russian for <c>ru</c>, English otherwise.</summary>
    public static GanttTexts For(CultureInfo culture) => culture.TwoLetterISOLanguageName == "ru" ? Russian : English;

    /// <summary>The message for a refused link, or null when it is valid; <paramref name="name"/> turns item keys into words.</summary>
    public string? Describe(GanttValidation validation, Func<string, string>? name = null) => validation.Code switch
    {
        GanttValidationCode.SelfLink => SelfLink,
        GanttValidationCode.Cycle => string.Format(CultureInfo.CurrentCulture, Cycle, string.Join(" \u2192 ", validation.Path.Select(name ?? (key => key)))),
        GanttValidationCode.Duplicate => Duplicate,
        _ => null
    };
}
