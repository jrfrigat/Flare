using System.Globalization;
using System.Resources;
using Microsoft.Extensions.Localization;

namespace Flare.Components.Gantt;

/// <summary>Accessible names, validation messages and announcements in the current UI culture.</summary>
public sealed class GanttStrings
{
    private GanttStrings() { }
    private static readonly ResourceManager Resources =
        new("Flare.Components.Gantt.GanttStrings", typeof(GanttStrings).Assembly);

    private static string Get(string key) => Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;

    /// <summary>Refusal for a link from an item to itself.</summary>
    public static string SelfLink => Get("Gantt_SelfLink");
    /// <summary>Refusal containing the path of a dependency cycle in placeholder zero.</summary>
    public static string Cycle => Get("Gantt_Cycle");
    /// <summary>Refusal for an existing dependency.</summary>
    public static string Duplicate => Get("Gantt_Duplicate");
    /// <summary>Accessible name of the toggle showing a row's children.</summary>
    public static string Expand => Get("Gantt_Expand");
    /// <summary>Accessible name of the toggle hiding a row's children.</summary>
    public static string Collapse => Get("Gantt_Collapse");
    /// <summary>Announcement with the source item in placeholder zero.</summary>
    public static string LinkStarted => Get("Gantt_LinkStarted");
    /// <summary>Announcement with source and target names in placeholders zero and one.</summary>
    public static string LinkCreated => Get("Gantt_LinkCreated");
    /// <summary>Announcement with item and new dates in placeholders zero and one.</summary>
    public static string Moved => Get("Gantt_Moved");
    /// <summary>Accessible name of the row-header separator.</summary>
    public static string ResizeSide => Get("Gantt_ResizeSide");

    /// <summary>Describes a refused dependency using the current UI culture; returns null for valid links.</summary>
    /// <param name="validation">Dependency validation result.</param>
    /// <param name="name">Optional mapping from item keys to display names.</param>
    /// <param name="localizer">Application context; omitted to use ambient built-in resources.</param>
    /// <returns>Localized refusal or null.</returns>
    public static string? Describe(GanttValidation validation, Func<string, string>? name = null,
        IStringLocalizer<GanttStrings>? localizer = null) => validation.Code switch
    {
        GanttValidationCode.SelfLink => localizer?["Gantt_SelfLink"].Value ?? SelfLink,
        GanttValidationCode.Cycle => string.Format(CultureInfo.CurrentCulture, localizer?["Gantt_Cycle"].Value ?? Cycle,
            string.Join(" \u2192 ", validation.Path.Select(name ?? (key => key)))),
        GanttValidationCode.Duplicate => localizer?["Gantt_Duplicate"].Value ?? Duplicate,
        _ => null
    };
}
