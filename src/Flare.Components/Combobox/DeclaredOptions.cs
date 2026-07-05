using System.ComponentModel;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;

// Declarative <option> parsing reads the child-content render tree. RenderTree types are framework-internal
// (BL0006); the use is deliberate and self-contained to this file. This helper is NOT part of the guarded
// pure engine (it consumes a RenderFragment); it is a shell-side ingest that feeds the engine a plain list.
// TValue is unconstrained (the option value type can be anything); the label-map keys are always the
// non-null converted values (Add skips nulls), so the notnull-key warning (CS8714) is not reachable.
#pragma warning disable BL0006
#pragma warning disable CS8714

namespace Flare.Components.Combobox;

/// <summary>The values and labels parsed from declarative <c>&lt;option&gt;</c> child content.</summary>
/// <typeparam name="TValue">The option value type.</typeparam>
public sealed class DeclaredOptionSet<TValue>
{
    internal DeclaredOptionSet(List<TValue> values, Dictionary<TValue, string> labels)
    {
        Values = values;
        Labels = labels;
    }

    /// <summary>The parsed option values, in declaration order.</summary>
    public IReadOnlyList<TValue> Values { get; }

    /// <summary>Maps a parsed value to its declared label text.</summary>
    public IReadOnlyDictionary<TValue, string> Labels { get; }

    /// <summary>Whether any declarative option was parsed.</summary>
    public bool Any => Values.Count > 0;
}

/// <summary>
/// Parses declarative <c>&lt;option value="..."&gt;Label&lt;/option&gt;</c> child markup into a value list
/// for the select family, replacing the previous regex scan. Dynamic options compile to element frames
/// (walked directly); static markup compiles to one raw-HTML markup frame (scanned by a forward-only
/// tokenizer that correctly skips <c>&gt;</c> inside quoted attribute values). A value that fails
/// <see cref="TypeConverter"/> conversion is skipped and reported to an optional warning sink - it is never
/// coerced to <c>default</c>, so a bad <c>&lt;option value="abc"&gt;</c> on an <c>int</c> select can no
/// longer masquerade as <c>0</c>. For typed correctness use the data-driven <c>Items</c> path; declarative
/// options remain a convenience for string/enum values.
/// </summary>
public static class DeclaredOptions
{
    /// <summary>Parses <paramref name="content"/> into option values and labels.</summary>
    /// <typeparam name="TValue">The option value type.</typeparam>
    /// <param name="content">The child content holding <c>&lt;option&gt;</c> markup, or null.</param>
    /// <param name="comparer">Equality used to key the label map.</param>
    /// <param name="onWarning">Optional sink for a skipped, non-convertible option value.</param>
    public static DeclaredOptionSet<TValue> Parse<TValue>(
        RenderFragment? content,
        IEqualityComparer<TValue>? comparer = null,
        Action<string>? onWarning = null)
    {
        var values = new List<TValue>();
        var labels = new Dictionary<TValue, string>(comparer ?? EqualityComparer<TValue>.Default);
        if (content is null) return new DeclaredOptionSet<TValue>(values, labels);

        void Add(string? rawValue, string labelText)
        {
            var raw = rawValue ?? labelText;
            if (!TryConvert<TValue>(raw, out var converted) || converted is null)
            {
                onWarning?.Invoke($"<option value=\"{raw}\"> is not a valid {typeof(TValue).Name}; option skipped.");
                return;
            }
            values.Add(converted);
            labels[converted] = labelText;
        }

        var builder = new RenderTreeBuilder();
        try
        {
            content(builder);
            var range = builder.GetFrames();
            var frames = range.Array;
            for (var i = 0; i < range.Count; i++)
            {
                ref readonly var frame = ref frames[i];

                if (frame.FrameType == RenderTreeFrameType.Markup)
                {
                    ScanMarkup(frame.MarkupContent, Add);
                    continue;
                }

                if (frame.FrameType != RenderTreeFrameType.Element ||
                    !string.Equals(frame.ElementName, "option", StringComparison.OrdinalIgnoreCase))
                    continue;

                string? optionValue = null;
                var label = new StringBuilder();
                var end = i + frame.ElementSubtreeLength;
                for (var j = i + 1; j < end; j++)
                {
                    ref readonly var child = ref frames[j];
                    if (child.FrameType == RenderTreeFrameType.Attribute)
                    {
                        if (string.Equals(child.AttributeName, "value", StringComparison.OrdinalIgnoreCase))
                            optionValue = child.AttributeValue?.ToString();
                    }
                    else if (child.FrameType is RenderTreeFrameType.Text or RenderTreeFrameType.Markup)
                    {
                        label.Append(child.TextContent);
                    }
                }
                i = end - 1;
                Add(optionValue, label.ToString().Trim());
            }
        }
        finally
        {
            ((IDisposable)builder).Dispose();
        }

        return new DeclaredOptionSet<TValue>(values, labels);
    }

    // Forward-only scan for <option ...>label</option> in a raw HTML string. Attribute values are read
    // quote-aware so a '>' inside value="a>b" does not end the tag early (the regex could not do this).
    private static void ScanMarkup(string? html, Action<string?, string> add)
    {
        if (string.IsNullOrEmpty(html)) return;
        int i = 0, n = html.Length;
        while (i < n)
        {
            var open = html.IndexOf("<option", i, StringComparison.OrdinalIgnoreCase);
            if (open < 0) break;
            var p = open + 7;
            if (p < n && !(char.IsWhiteSpace(html[p]) || html[p] == '>' || html[p] == '/'))
            {
                i = p; // e.g. <optiongroup - not an <option> tag
                continue;
            }

            string? value = null;
            var selfClose = false;
            while (p < n && html[p] != '>')
            {
                while (p < n && char.IsWhiteSpace(html[p])) p++;
                if (p >= n || html[p] == '>') break;
                if (html[p] == '/') { selfClose = true; p++; continue; }

                var nameStart = p;
                while (p < n && html[p] != '=' && html[p] != '>' && html[p] != '/' && !char.IsWhiteSpace(html[p])) p++;
                var name = html.Substring(nameStart, p - nameStart);

                string? attrVal = null;
                while (p < n && char.IsWhiteSpace(html[p])) p++;
                if (p < n && html[p] == '=')
                {
                    p++;
                    while (p < n && char.IsWhiteSpace(html[p])) p++;
                    if (p < n && (html[p] == '"' || html[p] == '\''))
                    {
                        var q = html[p++];
                        var vs = p;
                        while (p < n && html[p] != q) p++;
                        attrVal = html.Substring(vs, p - vs);
                        if (p < n) p++;
                    }
                    else
                    {
                        var vs = p;
                        while (p < n && !char.IsWhiteSpace(html[p]) && html[p] != '>') p++;
                        attrVal = html.Substring(vs, p - vs);
                    }
                }
                if (string.Equals(name, "value", StringComparison.OrdinalIgnoreCase)) value = attrVal;
            }
            if (p < n && html[p] == '>') p++;

            if (selfClose)
            {
                add(value, string.Empty);
                i = p;
                continue;
            }

            var close = html.IndexOf("</option>", p, StringComparison.OrdinalIgnoreCase);
            var inner = close < 0 ? html[p..] : html.Substring(p, close - p);
            add(value, StripTags(inner));
            i = close < 0 ? n : close + 9;
        }
    }

    // Removes any nested tags, decodes entities and trims - the option's visible label text.
    private static string StripTags(string inner)
    {
        if (inner.IndexOf('<') < 0)
            return System.Net.WebUtility.HtmlDecode(inner).Trim();

        var sb = new StringBuilder(inner.Length);
        var inTag = false;
        foreach (var c in inner)
        {
            if (c == '<') inTag = true;
            else if (c == '>') inTag = false;
            else if (!inTag) sb.Append(c);
        }
        return System.Net.WebUtility.HtmlDecode(sb.ToString()).Trim();
    }

    // Converts an <option> string value to TValue (identity for string; TypeConverter otherwise).
    // Returns false on failure so the caller can skip (never coerce to default).
    private static bool TryConvert<TValue>(string raw, out TValue? value)
    {
        if (typeof(TValue) == typeof(string))
        {
            value = (TValue)(object)raw;
            return true;
        }
        var target = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
        try
        {
            var converter = TypeDescriptor.GetConverter(target);
            if (converter.CanConvertFrom(typeof(string)))
            {
                value = (TValue?)converter.ConvertFromInvariantString(raw);
                return value is not null;
            }
        }
        catch (Exception ex) when (ex is FormatException or InvalidOperationException or NotSupportedException or ArgumentException)
        {
            // fall through to the skip path
        }
        value = default;
        return false;
    }
}

#pragma warning restore CS8714
#pragma warning restore BL0006
