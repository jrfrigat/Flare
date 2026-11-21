using System.Globalization;
using System.Text;
using Flare.Css;

namespace Flare.Components.IDE;

internal sealed class DockGeometry
{
    internal const int Gutter = 12;
    internal const int Header = 32;

    // The header grows with the text: the base spacing step is rem-based, so a text-only zoom
    // enlarges the header and the pane top together.
    internal static readonly string HeaderHeight =
        $"var(--flare-dock-header-height, var({Css.Tokens.Spacing.S8}, {Header}px))";

    internal sealed record Entry(string Id, string Style, DockGroup? Group = null, DockSplit? Split = null,
        string? Item = null, double Size = 0, double Min = 0, double Max = 0, double Available = 0);

    internal List<Entry> Entries { get; } = [];
    internal string Style { get; private set; } = "";

    internal static DockGeometry Build(DockNode root, Func<string, string> id, double width, double height)
    {
        var layout = new DockGeometry();
        var size = new Dictionary<string, (int Width, int Height)>(StringComparer.Ordinal);
        Measure(root, size);
        var min = size[root.Key];
        var css = new StringBuilder($"min-inline-size:{min.Width}px;min-block-size:{min.Height}px;");
        layout.Add(root, id, size, css, "0px", "0px", "100%", "100%",
            Math.Max(width, min.Width), Math.Max(height, min.Height));
        layout.Style = css.ToString();
        return layout;
    }

    private static (int Width, int Height) Measure(DockNode node, Dictionary<string, (int Width, int Height)> sizes)
    {
        var result = (Width: 96, Height: 96);
        if (node is DockSplit split)
        {
            var first = Measure(split.First, sizes);
            var next = Measure(split.Second, sizes);
            result = split.Axis == DockAxis.Horizontal
                ? (first.Width + Gutter + next.Width, Math.Max(first.Height, next.Height))
                : (Math.Max(first.Width, next.Width), first.Height + Gutter + next.Height);
        }
        sizes.Add(node.Key, result);
        return result;
    }

    private void Add(DockNode node, Func<string, string> id, Dictionary<string, (int Width, int Height)> sizes,
        StringBuilder css, string x, string y, string w, string h, double width, double height)
    {
        var key = id(node.Key);
        x = Variable(css, key, "x", x);
        y = Variable(css, key, "y", y);
        w = Variable(css, key, "w", w);
        h = Variable(css, key, "h", h);
        if (node is DockGroup group)
        {
            Entries.Add(new(key, Position(x, y, w, HeaderHeight), Group: group));
            var body = Position(x, $"calc({y} + {HeaderHeight})", w, $"calc({h} - {HeaderHeight})");
            foreach (var item in group.Items)
                Entries.Add(new(key, body, Group: group, Item: item));
            return;
        }

        var split = (DockSplit)node;
        var horizontal = split.Axis == DockAxis.Horizontal;
        var start = sizes[split.First.Key];
        var end = sizes[split.Second.Key];
        var available = (horizontal ? width : height) - Gutter;
        var min = (double)(horizontal ? start.Width : start.Height);
        var max = Math.Max(min, available - (horizontal ? end.Width : end.Height));
        var firstSize = Math.Clamp(available * split.Fraction, min, max);
        var axis = horizontal ? w : h;
        var farMin = horizontal ? end.Width : end.Height;
        var preferred = $"calc(({axis} - {Gutter}px) * {CssNumber(split.Fraction)})";
        var first = $"var({Preview(key)}, clamp({CssNumber(min)}px, {preferred}, calc({axis} - {Gutter + farMin}px)))";
        first = Variable(css, key, "first", first);
        var next = $"calc({axis} - {first} - {Gutter}px)";
        if (horizontal)
        {
            Add(split.First, id, sizes, css, x, y, first, h, firstSize, height);
            Entries.Add(new(key, Position($"calc({x} + {first})", y, $"{Gutter}px", h), Split: split,
                Size: firstSize, Min: min, Max: max, Available: available));
            Add(split.Second, id, sizes, css, $"calc({x} + {first} + {Gutter}px)", y, next, h, available - firstSize, height);
        }
        else
        {
            Add(split.First, id, sizes, css, x, y, w, first, width, firstSize);
            Entries.Add(new(key, Position(x, $"calc({y} + {first})", w, $"{Gutter}px"), Split: split,
                Size: firstSize, Min: min, Max: max, Available: available));
            Add(split.Second, id, sizes, css, x, $"calc({y} + {first} + {Gutter}px)", w, next, width, available - firstSize);
        }
    }

    internal static string CssNumber(double value) => value.ToString("0.########", CultureInfo.InvariantCulture);

    internal static string Preview(string id) => $"--flare-dock-preview-{id}";

    private static string Variable(StringBuilder css, string id, string axis, string value)
    {
        var name = $"--flare-dock-{id}-{axis}";
        css.Append(name).Append(':').Append(value).Append(';');
        return $"var({name})";
    }

    private static string Position(string x, string y, string width, string height) =>
        $"inset-inline-start:{x};inset-block-start:{y};inline-size:{width};block-size:{height};";
}
