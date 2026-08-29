using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Flare.Core.Tests;

/// <summary>
/// A component that renders a caller-supplied string must also accept a <see cref="RenderFragment"/> for
/// it, so the natural spelling - writing the content between the tags - means something.
/// </summary>
/// <remarks>
/// Without this, the library gets no help from the compiler at all. Every Flare component inherits an
/// <c>AdditionalAttributes</c> catch-all, so Razor emits child content on a slot-less component as an
/// untyped attribute rather than failing to build; it lands in the unmatched dictionary and splatting
/// discards it. <c>&lt;FlareChip&gt;42&lt;/FlareChip&gt;</c> compiled clean and rendered an empty chip.
/// See docs/issues/implicit-child-content.md.
///
/// The slot's NAME follows what the library already does: <c>ChildContent</c> when the caller's text is
/// the whole of what the component renders (a chip, a divider, a checkbox label), and <c>XxxContent</c>
/// when it is one named part among several - <c>FlareAppBar.TitleContent</c> was doing this before the
/// rule was written down.
/// </remarks>
public sealed class CallerTextSlotTests
{
    // Parameter names that denote text the component renders for the caller to read.
    private static readonly string[] DisplayTextNames =
        ["Label", "Text", "Title", "Caption", "Message", "Description", "Subtitle", "Heading"];

    // Components whose string parameter is NOT rendered markup, with the reason. Each entry is a
    // decision, not a backlog item: adding a fragment here would be wrong, not merely unfinished.
    private static readonly Dictionary<string, string> NotAMarkupSlot = new()
    {
        ["FlareRating"] = "Label is the accessible name for the group; aria-label takes a string, not a fragment.",
        ["FlareHighlighter"] = "Text is the haystack the component searches, not a label it renders verbatim.",
        ["FlareAvatar"] = "Text is the source the initials are derived from, not rendered as given.",
        ["FlareFloatingActionMenuItem"] = "Label is both the visible span and the button's aria-label; splitting them invites an accessible name that drifts from the visible one.",
        ["FlareEmptyState"] = "Title and Description are two strings; a single ChildContent could not say which, and neither has asked for markup yet.",
        ["FlareDateRangePicker"] = "StartLabel and EndLabel are two strings, same ambiguity.",
        ["FlareFieldChrome"] = "The shared field frame; its label is placed by the field family, which owns the slot decision.",
        ["FlareColorCustomizer"] = "Label names a generated swatch row, not caller content.",
        ["FlareShortcutEntry"] = "Description is data from the shortcut registry, not markup a caller writes.",
        ["FlareOnThisPage"] = "Title heads a generated list; the entries come from the document, not the caller.",
        ["FlareMeterSegment"] = "Label is a legend string the meter also exposes to assistive tech.",
        ["FlareColorPicker"] = "Label is forwarded to the shared field frame verbatim.",
        ["DataGridColumnPicker"] = "Label belongs to the generated per-column rows.",
        ["DataGridFilterBuilder"] = "Label belongs to the generated per-rule rows.",
        ["FlareChart"] = "Title and Description are two strings; a single ChildContent could not say which.",
    };

    // Not a decision - a gap. A grid header can only be a string today, so an icon, a unit or a help
    // affordance in a column header is unreachable, and there is no header slot to add the markup to.
    // Fixing it means threading a fragment through the header render path (bands, sort affordance,
    // resize handle), which is more than a parameter. Tracked in docs/issues/implicit-child-content.md.
    private static readonly HashSet<string> PendingHeaderSlot =
        new(StringComparer.Ordinal) { "FlareColumn", "FlareColumnBase", "FlareColumnBand", "FlareColumnRow" };

    [Fact]
    public void EveryComponent_ThatRendersCallerText_AlsoAcceptsMarkupForIt()
    {
        var components = typeof(Flare.Components.FlareChip).Assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsPublic: true } && typeof(ComponentBase).IsAssignableFrom(t))
            .OrderBy(t => t.Name, StringComparer.Ordinal);

        var offenders = new List<string>();

        foreach (var type in components)
        {
            var parameters = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
                .ToArray();

            var textParams = parameters
                .Where(p => p.PropertyType == typeof(string) && DisplayTextNames.Contains(p.Name))
                .Select(p => p.Name)
                .ToArray();

            if (textParams.Length == 0) continue;

            var name = Bare(type);
            if (NotAMarkupSlot.ContainsKey(name) || PendingHeaderSlot.Contains(name)) continue;

            var fragments = parameters
                .Where(p => p.PropertyType == typeof(RenderFragment))
                .Select(p => p.Name)
                .ToArray();

            // Either the catch-all slot, or a named one for the specific text parameter.
            var satisfied = fragments.Contains("ChildContent")
                || textParams.Any(t => fragments.Contains(t + "Content"));

            if (!satisfied)
                offenders.Add($"{name}  renders {string.Join(" / ", textParams)} but takes no fragment for it");
        }

        Assert.True(offenders.Count == 0,
            "A component that renders a caller's string must also take a RenderFragment for it - " +
            "'ChildContent' when the text is the whole of what it renders, 'XxxContent' when it is one " +
            "named part. Content written between the tags of a component with neither is silently " +
            "dropped, and the build stays green. If the string genuinely is not markup (an aria-label, " +
            "an algorithm's input), add it to NotAMarkupSlot with the reason:" +
            Environment.NewLine + string.Join(Environment.NewLine, offenders));
    }

    [Fact]
    public void TheAllowList_HasNoStaleEntries()
    {
        var live = typeof(Flare.Components.FlareChip).Assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsPublic: true } && typeof(ComponentBase).IsAssignableFrom(t))
            .Select(Bare)
            .ToHashSet(StringComparer.Ordinal);

        var stale = NotAMarkupSlot.Keys.Concat(PendingHeaderSlot).Where(k => !live.Contains(k)).ToArray();

        Assert.True(stale.Length == 0,
            "These exemptions name components that no longer exist, so the reason behind each is no " +
            "longer being checked against anything: " + string.Join(", ", stale));
    }

    // Generic components arrive as "FlareRadio`1".
    private static string Bare(Type t) =>
        t.Name.Contains('`', StringComparison.Ordinal)
            ? t.Name[..t.Name.IndexOf('`', StringComparison.Ordinal)]
            : t.Name;
}
