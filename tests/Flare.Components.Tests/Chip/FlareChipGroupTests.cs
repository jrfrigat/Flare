using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareChipGroupTests : FlareTestContext
{
    private static RenderFragment ChipChildren(params string[] chipValues) => inner =>
    {
        var seq = 10;
        foreach (var v in chipValues)
        {
            inner.OpenComponent<FlareChip>(seq++);
            inner.AddAttribute(seq++, nameof(FlareChip.Value), v);
            inner.AddAttribute(seq++, nameof(FlareChip.Label), v);
            inner.CloseComponent();
        }
    };

    private static RenderFragment ChipGroupWith(string[] chipValues, bool multiSelect = false,
        IReadOnlyCollection<string>? selectedValues = null,
        EventCallback<IReadOnlyCollection<string>> selectedValuesChanged = default) =>
        b =>
        {
            b.OpenComponent<FlareChipGroup>(0);
            b.AddAttribute(1, "MultiSelect", multiSelect);
            if (selectedValues is not null)
                b.AddAttribute(2, "SelectedValues", selectedValues);
            if (selectedValuesChanged.HasDelegate)
                b.AddAttribute(3, "SelectedValuesChanged", selectedValuesChanged);
            b.AddAttribute(4, "ChildContent", ChipChildren(chipValues));
            b.CloseComponent();
        };

    // Drives the real cycle a page goes through: a chip click raises SelectedValuesChanged, this host
    // stores the new value and re-renders the same group with it. The chips must update in place - a
    // fixed cascade left them holding the snapshot they first received, so the selected class froze.
    private sealed class ChipGroupHost : ComponentBase
    {
        [Parameter] public bool Multi { get; set; }

        public IReadOnlyCollection<string> Selected { get; private set; } = ["A"];

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            b.OpenComponent<FlareChipGroup>(0);
            b.AddAttribute(1, nameof(FlareChipGroup.MultiSelect), Multi);
            b.AddAttribute(2, nameof(FlareChipGroup.SelectedValues), Selected);
            b.AddAttribute(3, nameof(FlareChipGroup.SelectedValuesChanged),
                EventCallback.Factory.Create<IReadOnlyCollection<string>>(this, v => Selected = v));
            b.AddAttribute(4, nameof(FlareChipGroup.ChildContent), ChipChildren("A", "B", "C"));
            b.CloseComponent();
        }
    }

    [Fact]
    public void RendersChildren()
    {
        var cut = Render(ChipGroupWith(["Red", "Green", "Blue"]));

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Chip.Root}").Count);
    }

    [Fact]
    public void SingleSelectMode_OnlyOneChipSelected()
    {
        var cut = Render(ChipGroupWith(["A", "B"], multiSelect: false,
            selectedValues: ["A"]));

        var chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.Contains(Css.Classes.Chip.Selected, chips[0].ClassName);
        Assert.DoesNotContain(Css.Classes.Chip.Selected, chips[1].ClassName);
    }

    [Fact]
    public void MultiSelectMode_AllowsMultipleSelected()
    {
        var cut = Render(ChipGroupWith(["X", "Y", "Z"], multiSelect: true,
            selectedValues: ["X", "Z"]));

        var chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.Contains(Css.Classes.Chip.Selected, chips[0].ClassName);
        Assert.DoesNotContain(Css.Classes.Chip.Selected, chips[1].ClassName);
        Assert.Contains(Css.Classes.Chip.Selected, chips[2].ClassName);
    }

    [Fact]
    public void RendersWithSelectedValues()
    {
        var cut = Render(ChipGroupWith(["One", "Two"], selectedValues: ["Two"]));

        var chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.DoesNotContain(Css.Classes.Chip.Selected, chips[0].ClassName);
        Assert.Contains(Css.Classes.Chip.Selected, chips[1].ClassName);
    }

    [Fact]
    public void SelectedValuesChanged_FiresOnChipToggle()
    {
        IReadOnlyCollection<string>? captured = null;
        var callback = EventCallback.Factory.Create<IReadOnlyCollection<string>>(
            this, v => captured = v);

        var cut = Render(ChipGroupWith(["Alpha", "Beta"],
            selectedValuesChanged: callback));

        cut.FindAll($".{Css.Classes.Chip.Root}")[0].Click();

        Assert.NotNull(captured);
        Assert.Contains("Alpha", captured!);
    }

    [Fact]
    public void ChipWithValue_RendersCorrectLabel()
    {
        var cut = Render(ChipGroupWith(["MyValue"]));

        Assert.Contains("MyValue", cut.Find($".{Css.Classes.Chip.Label}").TextContent);
    }

    [Fact]
    public void RendersSelectedChipWithClass()
    {
        var cut = Render(ChipGroupWith(["P", "Q"], selectedValues: ["Q"]));

        var chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.Contains(Css.Classes.Chip.Selected, chips[1].ClassName);
    }

    [Fact]
    public void ClearSelection_TogglingSelectedChipInSingleMode()
    {
        IReadOnlyCollection<string>? captured = null;
        var callback = EventCallback.Factory.Create<IReadOnlyCollection<string>>(
            this, v => captured = v);

        var cut = Render(ChipGroupWith(["Cat", "Dog"],
            multiSelect: false,
            selectedValues: ["Cat"],
            selectedValuesChanged: callback));

        cut.FindAll($".{Css.Classes.Chip.Root}")[0].Click();

        Assert.NotNull(captured);
        Assert.Empty(captured!);
    }

    [Fact]
    public void ClickingAChip_MovesTheSelectedClassInSingleMode()
    {
        var cut = Render<ChipGroupHost>();

        cut.FindAll($".{Css.Classes.Chip.Root}")[1].Click();

        var chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.DoesNotContain(Css.Classes.Chip.Selected, chips[0].ClassName);
        Assert.Contains(Css.Classes.Chip.Selected, chips[1].ClassName);
    }

    [Fact]
    public void ClickingTheSelectedChipAgain_ClearsTheSelectedClass()
    {
        var cut = Render<ChipGroupHost>();

        cut.FindAll($".{Css.Classes.Chip.Root}")[0].Click();

        Assert.DoesNotContain(Css.Classes.Chip.Selected,
            cut.FindAll($".{Css.Classes.Chip.Root}")[0].ClassName);
    }

    [Fact]
    public void MultiSelect_TogglesEachChipIndependently()
    {
        var cut = Render<ChipGroupHost>(p => p.Add(x => x.Multi, true));

        cut.FindAll($".{Css.Classes.Chip.Root}")[1].Click();
        cut.FindAll($".{Css.Classes.Chip.Root}")[2].Click();

        var chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.All(chips, c => Assert.Contains(Css.Classes.Chip.Selected, c.ClassName));

        cut.FindAll($".{Css.Classes.Chip.Root}")[1].Click();

        chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.Contains(Css.Classes.Chip.Selected, chips[0].ClassName);
        Assert.DoesNotContain(Css.Classes.Chip.Selected, chips[1].ClassName);
        Assert.Contains(Css.Classes.Chip.Selected, chips[2].ClassName);
    }

    [Fact]
    public void ExternalSelectedValues_UpdateExistingChips()
    {
        var cut = Render<FlareChipGroup>(p => p
            .Add(x => x.SelectedValues, new[] { "A" })
            .Add(x => x.ChildContent, ChipChildren("A", "B")));

        cut.Render(p => p
            .Add(x => x.SelectedValues, new[] { "B" })
            .Add(x => x.ChildContent, ChipChildren("A", "B")));

        var chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.DoesNotContain(Css.Classes.Chip.Selected, chips[0].ClassName);
        Assert.Contains(Css.Classes.Chip.Selected, chips[1].ClassName);
    }

    [Fact]
    public void ExternalMultiSelectChange_UpdatesExistingChips()
    {
        var cut = Render<FlareChipGroup>(p => p
            .Add(x => x.MultiSelect, false)
            .Add(x => x.SelectedValues, new[] { "A" })
            .Add(x => x.ChildContent, ChipChildren("A", "B")));

        cut.Render(p => p
            .Add(x => x.MultiSelect, true)
            .Add(x => x.SelectedValues, new[] { "A", "B" })
            .Add(x => x.ChildContent, ChipChildren("A", "B")));

        var chips = cut.FindAll($".{Css.Classes.Chip.Root}");
        Assert.Contains(Css.Classes.Chip.Selected, chips[0].ClassName);
        Assert.Contains(Css.Classes.Chip.Selected, chips[1].ClassName);
    }
}
