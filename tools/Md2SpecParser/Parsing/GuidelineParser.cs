using System.Text;
using System.Text.Json;

namespace Flare.Tools.Md2SpecParser.Parsing;

/// <summary>
/// One page-data document rendered as markdown. Everything a theme audit needs is kept - prose,
/// lists, tables, the numbered image captions that name the parts, and above all the redline
/// measurements - and everything that only makes sense in a browser is dropped.
/// </summary>
public static class GuidelineParser
{
    /// <summary>Renders a downloaded page-data document.</summary>
    /// <param name="json">The raw page-data JSON.</param>
    /// <param name="route">The site route it came from, recorded in the output for provenance.</param>
    public static string ToMarkdown(string json, string route)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var sb = new StringBuilder();

        var title = Str(root, "internal_title") ?? route;
        sb.Append("## ").Append(title).Append('\n');
        sb.Append("\nSource: <https://m2.material.io").Append(route).Append(">\n");

        if (Obj(root, "hero") is { } hero)
        {
            var subtitle = Str(hero, "subtitle");
            if (!string.IsNullOrWhiteSpace(subtitle))
                sb.Append('\n').Append(Html.ToMarkdown(subtitle)).Append('\n');
        }

        if (Obj(root, "body") is { } body && body.TryGetProperty("section", out var sections))
            foreach (var section in sections.EnumerateArray())
                AppendSection(sb, section, depth: 3);

        // The specs section is the point of the exercise: it holds the redline drawings.
        if (Obj(root, "specs_section") is { } specs)
        {
            sb.Append("\n### Specs\n");
            AppendSpecs(sb, specs);
        }

        return Html.Collapse(sb.ToString()) + "\n";
    }

    private static void AppendSection(StringBuilder sb, JsonElement section, int depth)
    {
        var title = Str(section, "title");
        if (!string.IsNullOrWhiteSpace(title))
            sb.Append('\n').Append(new string('#', depth)).Append(' ').Append(title).Append('\n');

        var content = Html.ToMarkdown(Str(section, "content"));
        if (content.Length > 0) sb.Append('\n').Append(content).Append('\n');

        AppendImages(sb, section);
        // A few sections carry the same table twice - once as markup inside the prose, once in the
        // pipe-delimited field - so the field is only read when the prose had none.
        if (!content.Contains("\n|", StringComparison.Ordinal)) AppendTable(sb, section);

        if (section.TryGetProperty("subsection", out var subs) && subs.ValueKind == JsonValueKind.Array)
            foreach (var sub in subs.EnumerateArray())
                AppendSection(sb, sub, Math.Min(depth + 1, 6));
    }

    // Captions are where Material 2 names the parts ("1. Text label", "B. Container") and where the
    // occasional measurement is written out in prose, so they are content, not decoration.
    private static void AppendImages(StringBuilder sb, JsonElement section)
    {
        if (!section.TryGetProperty("image", out var images) || images.ValueKind != JsonValueKind.Array) return;

        var index = 0;
        foreach (var image in images.EnumerateArray())
        {
            var caption = Html.ToMarkdown(Str(image, "caption"));
            var alt = Str(image, "a11y_description");
            var verdict = Str(image, "case"); // "do" / "dont" on the guidance pairs
            if (caption.Length == 0 && string.IsNullOrWhiteSpace(alt)) { index++; continue; }

            sb.Append("\n> **image ").Append(index++).Append("**");
            if (!string.IsNullOrWhiteSpace(verdict)) sb.Append(" (").Append(verdict).Append(')');
            sb.Append('\n');
            if (!string.IsNullOrWhiteSpace(alt)) sb.Append("> ").Append(alt.Trim()).Append('\n');
            foreach (var line in caption.Split('\n'))
                sb.Append("> ").Append(line).Append('\n');
        }
    }

    // The "table" field is not HTML like the rest: it is an array of pipe-delimited blocks, already
    // nearly markdown - "Elevation level|White overlay <br>transparency\n-|-|\n00dp|0%\n...". This is
    // where the value tables live (the dark-theme elevation overlays, for one), so it gets its own
    // path rather than being run through the HTML converter, which would flatten it to loose lines.
    private static void AppendTable(StringBuilder sb, JsonElement section)
    {
        if (!section.TryGetProperty("table", out var table)) return;

        var blocks = table.ValueKind switch
        {
            JsonValueKind.String => [table.GetString() ?? string.Empty],
            JsonValueKind.Array => table.EnumerateArray()
                .Where(e => e.ValueKind == JsonValueKind.String)
                .Select(e => e.GetString() ?? string.Empty)
                .ToArray(),
            _ => Array.Empty<string>(),
        };

        foreach (var block in blocks)
        {
            if (string.IsNullOrWhiteSpace(block)) continue;
            sb.Append('\n');
            foreach (var line in block.Replace("\r\n", "\n").Split('\n'))
            {
                var row = Html.ToMarkdown(line.Trim()).Replace('\n', ' ').Trim();
                if (row.Length == 0) continue;
                // A separator row arrives as "-|-|"; markdown wants "---" per column.
                row = row.Trim('|');
                var cells = row.Split('|').Select(c => c.Trim()).ToArray();
                sb.Append("| ").Append(string.Join(" | ", cells.Select(c => c == "-" ? "---" : c))).Append(" |\n");
            }
            sb.Append('\n');
        }
    }

    private static void AppendSpecs(StringBuilder sb, JsonElement specs)
    {
        var content = Obj(specs, "content");
        if (content is null) return;

        var prose = Html.ToMarkdown(Str(content.Value, "content"));
        if (prose.Length > 0) sb.Append('\n').Append(prose).Append('\n');

        if (!content.Value.TryGetProperty("redline_html", out var redlines) || redlines.ValueKind != JsonValueKind.Array)
            return;

        foreach (var redline in redlines.EnumerateArray())
        {
            var title = Str(redline, "title") ?? "Redline";
            var html = Str(redline, "redline_html_image");
            var measurements = Redline.Parse(html);
            var annotations = Redline.Annotations(html);
            sb.Append("\n#### ").Append(title).Append('\n');

            if (measurements.Count > 0)
            {
                sb.Append("\n| Measurement | Kind | Axis |\n|---|---|---|\n");
                foreach (var m in measurements)
                    sb.Append("| ").Append(m.Value).Append(" | ").Append(m.Kind).Append(" | ").Append(m.Axis).Append(" |\n");
            }

            if (annotations.Count > 0)
            {
                sb.Append("\nColor, shape and elevation notes on the same drawing:\n\n");
                foreach (var a in annotations) sb.Append("- ").Append(a).Append('\n');
            }

            if (measurements.Count == 0 && annotations.Count == 0)
                sb.Append("\nNo measurements in this drawing.\n");
        }
    }

    private static string? Str(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static JsonElement? Obj(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Object
            ? value
            : null;
}
