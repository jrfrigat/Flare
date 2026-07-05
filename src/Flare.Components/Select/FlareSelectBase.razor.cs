using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;

// Declarative <option> parsing reads the child-content render tree (as the single-file FlareSelect did
// before this parsing was extracted here). RenderTree types are framework-internal (BL0006); the use is
// deliberate and self-contained to this file.
#pragma warning disable BL0006

namespace Flare.Components;

/// <summary>
/// Declarative <c>&lt;option value=".."&gt;Label&lt;/option&gt;</c> parsing for the select family. Kept in
/// a partial so the component markup + interaction stays readable; the parsed values/labels feed
/// <see cref="FlareSelectBase{TValue}.LabelOf"/> and the item list.
/// </summary>
public partial class FlareSelectBase<TValue>
{
    // Declarative <option> values/labels parsed from ChildContent (in declaration order).
    private bool _declarative => ChildContent is not null;
    private List<TValue> _declaredValues = new();
    // TValue is unconstrained; the declared-option keys are always the non-null converted values
    // (AddOption skips nulls), so the notnull-key warning is not reachable here.
#pragma warning disable CS8714
    private readonly Dictionary<TValue, string> _declaredLabels = new();
#pragma warning restore CS8714

    // Collects declarative <option value="..">Label</option> entries from ChildContent. Static option
    // markup is compiled by Razor into a single Markup frame (a raw HTML string), while options carrying
    // a binding/dynamic attribute become Element frames - so both shapes are handled. The option's value
    // attribute is converted to TValue (TypeConverter); its inner text is the label.
    private void ParseDeclaredOptions()
    {
        if (ChildContent is null)
        {
            if (_declaredValues.Count > 0) { _declaredValues = new(); _declaredLabels.Clear(); }
            return;
        }

        var values = new List<TValue>();
        _declaredLabels.Clear();

        void AddOption(string? rawValue, string labelText)
        {
            var converted = ConvertRaw(rawValue ?? labelText);
            if (converted is null) return;
            values.Add(converted);
            _declaredLabels[converted] = labelText;
        }

        var builder = new RenderTreeBuilder();
        try
        {
            ChildContent(builder);
            var range = builder.GetFrames();
            var frames = range.Array;
            for (var i = 0; i < range.Count; i++)
            {
                ref readonly var frame = ref frames[i];

                // Static option markup -> one Markup frame holding the raw HTML string.
                if (frame.FrameType == RenderTreeFrameType.Markup)
                {
                    ParseOptionsFromHtml(frame.MarkupContent, AddOption);
                    continue;
                }

                // Dynamic option -> an <option> Element frame with its attributes/text as child frames.
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
                i = end - 1; // skip the option's subtree

                AddOption(optionValue, label.ToString().Trim());
            }
        }
        finally
        {
            ((IDisposable)builder).Dispose();
        }

        _declaredValues = values;
    }

    // Extracts <option value="..">label</option> entries from a raw HTML string (static markup case).
    private static readonly Regex _optionRegex = new(
        "<option\\b([^>]*)>(.*?)</option>",
        RegexOptions.IgnoreCase | RegexOptions.Singleline);
    private static readonly Regex _valueAttrRegex = new(
        "value\\s*=\\s*(?:\"([^\"]*)\"|'([^']*)')",
        RegexOptions.IgnoreCase);
    private static readonly Regex _tagRegex = new("<[^>]+>");

    private static void ParseOptionsFromHtml(string? html, Action<string?, string> add)
    {
        if (string.IsNullOrEmpty(html) || html.IndexOf("<option", StringComparison.OrdinalIgnoreCase) < 0)
            return;
        foreach (Match m in _optionRegex.Matches(html))
        {
            var attrs = m.Groups[1].Value;
            var vm = _valueAttrRegex.Match(attrs);
            string? value = vm.Success ? (vm.Groups[1].Success ? vm.Groups[1].Value : vm.Groups[2].Value) : null;
            var label = System.Net.WebUtility.HtmlDecode(_tagRegex.Replace(m.Groups[2].Value, string.Empty)).Trim();
            add(value, label);
        }
    }

    // Converts an <option> string value to TValue (identity for string; TypeConverter otherwise).
    private static TValue? ConvertRaw(string raw)
    {
        if (typeof(TValue) == typeof(string)) return (TValue)(object)raw;
        var target = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
        try
        {
            var converter = TypeDescriptor.GetConverter(target);
            if (converter.CanConvertFrom(typeof(string)))
                return (TValue?)converter.ConvertFromInvariantString(raw);
        }
        catch { }
        return default;
    }
}

#pragma warning restore BL0006
