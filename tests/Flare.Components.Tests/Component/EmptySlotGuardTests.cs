using System.Text;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// The blind spot that let an empty support row ship: a component decides whether to draw a slot
/// container by asking whether the RenderFragment is null, while the fragment's own body is
/// conditional. A non-null fragment that renders nothing then leaves a real element in the DOM with
/// nothing in it - and every assertion of the form "the fragment was passed" agrees that this is fine.
/// Only the rendered result disagrees, which is why this guard reads the DOM.
///
/// It looks at the slot containers specifically, not at every empty element: a slot exists to hold
/// content, so an empty one is a defect by definition, whereas an empty decorative span (a track, a
/// thumb, a rail) is doing its job. A component that legitimately draws an empty slot belongs in
/// <see cref="AllowedEmpty"/> with a reason, so the exception is a decision someone made rather than a
/// silence.
/// </summary>
public sealed class EmptySlotGuardTests : FlareTestContext
{
    // Class-name fragments that mark an element as "a place optional content goes". Matched against the
    // whole class name, so `flare-input__support` matches "__support" and `flare-datagrid__toolbar-end`
    // matches "__toolbar".
    private static readonly string[] SlotMarkers =
    [
        "__support", "__hint", "__helper", "__counter", "__toolbar", "__actions",
        "__header-start", "__header-end", "__prefix", "__suffix", "__adornment", "__footer",
    ];

    // Slots that are allowed to render empty, with the reason. Keyed by the exact class name.
    private static readonly Dictionary<string, string> AllowedEmpty = new(StringComparer.Ordinal);

    private static bool IsSlot(IElement el) =>
        el.ClassList.Any(c => SlotMarkers.Any(m => c.Contains(m, StringComparison.Ordinal)));

    private static bool IsEmpty(IElement el) =>
        el.Children.Length == 0 && string.IsNullOrWhiteSpace(el.TextContent);

    private static void AssertNoEmptySlots(string what, IRenderedComponent<IComponent> cut)
    {
        var offenders = new StringBuilder();
        foreach (var el in cut.FindAll("*"))
        {
            if (!IsSlot(el) || !IsEmpty(el)) continue;
            var slotClass = el.ClassList.First(c => SlotMarkers.Any(m => c.Contains(m, StringComparison.Ordinal)));
            if (AllowedEmpty.ContainsKey(slotClass)) continue;
            offenders.Append(offenders.Length == 0 ? "" : ", ").Append('<').Append(el.LocalName)
                .Append(" class=\"").Append(el.ClassName).Append("\">");
        }

        Assert.True(offenders.Length == 0,
            $"{what} renders a slot container with nothing in it: {offenders}. A slot exists to hold "
            + "content, so an empty one takes its own spacing and its own row out of the layout for "
            + "nothing. Pass the fragment only when it has something to draw (or add the class to "
            + "AllowedEmpty with the reason it is deliberate).");
    }

    [Fact]
    public void Field_NoLabelNoHintNoCounter_DrawsNoSupportRow() =>
        AssertNoEmptySlots("FlareField", Render<FlareField<string>>(p => p.Add(x => x.Label, "L")));

    [Fact]
    public void TextArea_NoHintNoCounter_DrawsNoSupportRow() =>
        AssertNoEmptySlots("FlareTextArea", Render<FlareTextArea>(p => p.Add(x => x.Label, "L")));

    [Fact]
    public void NumericField_Bare_DrawsNoEmptySlot() =>
        AssertNoEmptySlots("FlareNumericField", Render<FlareNumericField<int>>(p => p.Add(x => x.Label, "L")));

    [Fact]
    public void PasswordField_Bare_DrawsNoEmptySlot() =>
        AssertNoEmptySlots("FlarePasswordField", Render<FlarePasswordField>(p => p.Add(x => x.Label, "L")));

    [Fact]
    public void Select_Bare_DrawsNoEmptySlot() =>
        AssertNoEmptySlots("FlareSelect", Render<FlareSelect<string>>(p => p.Add(x => x.Label, "L")));

    [Fact]
    public void MultiSelect_Bare_DrawsNoEmptySlot() =>
        AssertNoEmptySlots("FlareMultiSelect", Render<FlareMultiSelect<string>>(p => p.Add(x => x.Label, "L")));

    [Fact]
    public void Tabs_NoHeaderZones_DrawsNoEmptyZone()
    {
        RenderFragment tabs = b =>
        {
            b.OpenComponent<FlareTab>(0);
            b.AddAttribute(1, "Label", "One");
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddContent(0, "content")));
            b.CloseComponent();
        };
        AssertNoEmptySlots("FlareTabs", Render<FlareTabs>(p => p.Add(x => x.ChildContent, tabs)));
    }

    [Fact]
    public void DataGrid_NoToolbarContent_DrawsNoEmptyToolbar()
    {
        RenderFragment cols = b =>
        {
            b.OpenComponent<FlareColumn<Row>>(0);
            b.AddAttribute(1, "Title", "Name");
            b.AddAttribute(2, "Field", (Func<Row, object?>)(r => r.Name));
            b.CloseComponent();
        };
        AssertNoEmptySlots("FlareDataGrid", Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, new[] { new Row("A") }.AsEnumerable())
            .Add(x => x.Columns, cols)));
    }

    // A guard that cannot fail proves nothing, and this one is a heuristic over class names - so it has
    // to be shown catching the shape it exists for: a fragment that was passed and drew nothing.
    [Fact]
    public void Guard_CatchesASlotWhoseFragmentDrewNothing()
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.HeaderStart, (RenderFragment)(_ => { }))
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
            })));

        var failure = Assert.Throws<Xunit.Sdk.TrueException>(
            () => AssertNoEmptySlots("a tab set with an empty header zone", cut));
        Assert.Contains(Css.Classes.Tabs.HeaderStart, failure.Message, StringComparison.Ordinal);
    }

    private sealed record Row(string Name);
}
