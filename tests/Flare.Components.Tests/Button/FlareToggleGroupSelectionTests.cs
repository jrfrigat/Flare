using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

public class FlareToggleGroupSelectionTests : FlareTestContext
{
    private static string ButtonSelector => $"button.{Css.Classes.Button.Root}";

    private IRenderedComponent<FlareToggleGroup<T>> RenderGroup<T>(
        IReadOnlyList<(T? Value, string Label)> items, Action<T?> changed, bool mandatory = false)
        => Render<FlareToggleGroup<T>>(p => p
            .Add(x => x.Mandatory, mandatory)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<T?>(this, changed))
            .Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var seq = i * 10;
                    var label = items[i].Label;
                    builder.OpenComponent<FlareToggleButton>(seq);
                    builder.AddAttribute(seq + 1, nameof(FlareToggleButton.Value), (object?)items[i].Value);
                    builder.AddAttribute(seq + 2, nameof(FlareToggleButton.ChildContent),
                        (RenderFragment)(content => content.AddContent(0, label)));
                    builder.CloseComponent();
                }
            })));

    private static string? Pressed(IRenderedComponent<FlareToggleGroup<int?>> cut, int index) =>
        cut.FindAll(ButtonSelector)[index].GetAttribute("aria-pressed");

    [Fact]
    public void AnIntGroup_StartsWithNothingSelected()
    {
        var cut = RenderGroup<int?>([(0, "Zero"), (1, "One")], _ => { });

        Assert.All(cut.FindAll(ButtonSelector), b => Assert.Equal("false", b.GetAttribute("aria-pressed")));
    }

    [Fact]
    public void Zero_IsAnOrdinaryChoice_AndTheSelectionCanBeCleared()
    {
        var changed = new List<int?>();
        var cut = RenderGroup<int?>([(0, "Zero"), (1, "One")], changed.Add);

        cut.FindAll(ButtonSelector)[0].Click();

        Assert.Equal("true", Pressed(cut, 0));
        Assert.Equal("false", Pressed(cut, 1));
        Assert.Equal([0], changed);

        cut.FindAll(ButtonSelector)[0].Click();

        Assert.Equal("false", Pressed(cut, 0));
        Assert.Equal([0, null], changed);
    }

    [Fact]
    public void SelectingAnotherValue_MovesTheSelection()
    {
        var changed = new List<int?>();
        var cut = RenderGroup<int?>([(0, "Zero"), (1, "One")], changed.Add);

        cut.FindAll(ButtonSelector)[0].Click();
        cut.FindAll(ButtonSelector)[1].Click();

        Assert.Equal("false", Pressed(cut, 0));
        Assert.Equal("true", Pressed(cut, 1));
        Assert.Equal([0, 1], changed);
    }

    [Fact]
    public void Mandatory_KeepsTheSelectedValue()
    {
        var changed = new List<int?>();
        var cut = RenderGroup<int?>([(0, "Zero"), (1, "One")], changed.Add, mandatory: true);

        cut.FindAll(ButtonSelector)[0].Click();
        cut.FindAll(ButtonSelector)[0].Click();

        Assert.Equal("true", Pressed(cut, 0));
        Assert.Equal([0], changed);
    }

    [Fact]
    public void ANullableEnum_WorksLikeANumber()
    {
        var changed = new List<DayOfWeek?>();
        var cut = RenderGroup<DayOfWeek?>(
            [(DayOfWeek.Monday, "Mon"), (DayOfWeek.Tuesday, "Tue")], changed.Add);

        cut.FindAll(ButtonSelector)[0].Click();
        Assert.Equal("true", cut.FindAll(ButtonSelector)[0].GetAttribute("aria-pressed"));

        cut.FindAll(ButtonSelector)[0].Click();
        Assert.Equal("false", cut.FindAll(ButtonSelector)[0].GetAttribute("aria-pressed"));
        Assert.Equal([DayOfWeek.Monday, null], changed);
    }

    [Fact]
    public void AStringGroup_ClearsToNull()
    {
        var changed = new List<string?>();
        var cut = RenderGroup<string>([("A", "A"), ("B", "B")], changed.Add);

        cut.FindAll(ButtonSelector)[0].Click();
        cut.FindAll(ButtonSelector)[0].Click();

        Assert.Equal("false", cut.FindAll(ButtonSelector)[0].GetAttribute("aria-pressed"));
        Assert.Equal(["A", null], changed);
    }

    [Fact]
    public void ANonNullableValueType_IsRefusedWithGuidance()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Render<FlareToggleGroup<int>>());
        Assert.Contains("int?", ex.Message, StringComparison.Ordinal);

        Assert.Throws<InvalidOperationException>(() =>
            Render<FlareToggleGroup<int>>(p => p.Add(x => x.Mandatory, true)));
    }

    [Fact]
    public void MultiSelect_WithAnInt_HandlesZeroAndClearsTheList()
    {
        var changes = new List<IReadOnlyList<int>>();
        var cut = Render<FlareToggleGroup<int>>(p => p
            .Add(x => x.MultiSelect, true)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, changes.Add))
            .Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                AddItem(builder, 0, 0, "Zero");
                AddItem(builder, 10, 1, "One");
            })));

        cut.FindAll(ButtonSelector)[0].Click();
        Assert.Equal("true", cut.FindAll(ButtonSelector)[0].GetAttribute("aria-pressed"));
        Assert.Equal([0], changes[^1]);

        cut.FindAll(ButtonSelector)[1].Click();
        Assert.Equal("true", cut.FindAll(ButtonSelector)[0].GetAttribute("aria-pressed"));
        Assert.Equal("true", cut.FindAll(ButtonSelector)[1].GetAttribute("aria-pressed"));
        Assert.Equal([0, 1], changes[^1]);

        cut.FindAll(ButtonSelector)[0].Click();
        Assert.Equal("false", cut.FindAll(ButtonSelector)[0].GetAttribute("aria-pressed"));
        Assert.Equal([1], changes[^1]);
    }

    private static void AddItem(RenderTreeBuilder builder, int seq, int value, string label)
    {
        builder.OpenComponent<FlareToggleButton>(seq);
        builder.AddAttribute(seq + 1, nameof(FlareToggleButton.Value), (object?)value);
        builder.AddAttribute(seq + 2, nameof(FlareToggleButton.ChildContent),
            (RenderFragment)(content => content.AddContent(0, label)));
        builder.CloseComponent();
    }
}
