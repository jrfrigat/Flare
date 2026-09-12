using System.Net;
using System.Text.RegularExpressions;

namespace Flare.Tools.Md2SpecParser.Parsing;

/// <summary>
/// The measurements off a redline drawing. Material 2 has no token database - the numbers that a
/// theme has to match are drawn onto the spec images as an overlay, and that overlay ships as markup:
/// each measurement is an <c>li</c> whose class says what kind it is and whose
/// <c>span.measurement__value</c> carries the value. Reading them is what makes an audit checkable
/// against the source instead of against memory.
/// </summary>
public static partial class Redline
{
    /// <summary>One annotated measurement.</summary>
    /// <param name="Kind">What it measures: dimensions, padding, typography, radius, elevation.</param>
    /// <param name="Axis">Horizontal or vertical, as the drawing places it.</param>
    /// <param name="Value">The label as drawn, e.g. <c>36</c> or <c>min-width: 64dp</c>.</param>
    public sealed record Measurement(string Kind, string Axis, string Value);

    /// <summary>
    /// The colour, shape and elevation notes drawn on the same overlay, on their own layers. These
    /// are where a redline states a value that is not a distance - the switch's track is primary at
    /// 54% and its thumb sits at 0dp, and neither number appears anywhere else on the page.
    /// </summary>
    public static IReadOnlyList<string> Annotations(string? redlineHtml)
    {
        if (string.IsNullOrWhiteSpace(redlineHtml)) return [];

        var found = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match item in AnnotationItem().Matches(redlineHtml))
        {
            var text = WebUtility.HtmlDecode(Regex.Replace(item.Groups[1].Value, "<[^>]+>", " "));
            // "open_in_new" is a material icon ligature standing in for an outbound link.
            text = Regex.Replace(text.Replace("open_in_new", " "), @"\s+", " ").Trim();
            if (text.Length == 0 || text == " ") continue;
            if (seen.Add(text)) found.Add(text);
        }
        return found;
    }

    /// <summary>Extracts every measurement from one redline overlay, in document order.</summary>
    public static IReadOnlyList<Measurement> Parse(string? redlineHtml)
    {
        if (string.IsNullOrWhiteSpace(redlineHtml)) return [];

        var found = new List<Measurement>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match item in MeasurementItem().Matches(redlineHtml))
        {
            var classes = item.Groups["class"].Value;
            // The keyline items draw the guide lines themselves and carry no value.
            if (!classes.Contains("measurement--", StringComparison.Ordinal)) continue;

            var value = ValueSpan().Match(item.Groups["body"].Value);
            if (!value.Success) continue;

            var text = WebUtility.HtmlDecode(Regex.Replace(value.Groups[1].Value, "<[^>]+>", string.Empty)).Trim();
            if (text.Length == 0) continue;

            var kind = Modifier(classes, ["dimensions", "padding", "typography", "radius", "elevation", "spacing"]) ?? "measurement";
            var axis = Modifier(classes, ["horizontal", "vertical"]) ?? "-";

            // The same measurement is drawn twice - once for the eye, once for a screen reader
            // ("Measurement 36") - and mirrored drawings repeat a value on both sides.
            if (!seen.Add($"{kind}|{axis}|{text}")) continue;
            found.Add(new Measurement(kind, axis, text));
        }

        return found;
    }

    private static string? Modifier(string classes, string[] candidates) =>
        candidates.FirstOrDefault(c => classes.Contains("measurement--" + c, StringComparison.Ordinal));

    [GeneratedRegex(@"<li class=""(?<class>[^""]*)""[^>]*>(?<body>.*?)</li>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex MeasurementItem();

    [GeneratedRegex(@"<li class=""annotation[^""]*""[^>]*>([\s\S]*?)</li>", RegexOptions.IgnoreCase)]
    private static partial Regex AnnotationItem();

    // The visible label; the screen-reader twin repeats it prefixed with "Measurement".
    [GeneratedRegex(@"<span class=""measurement__value(?![^""]*screenreader)[^""]*""[^>]*>(.*?)</span>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ValueSpan();
}
