using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareChipGroupTests : FlareTestContext
{
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
            b.AddAttribute(4, "ChildContent", (RenderFragment)(inner =>
            {
                var seq = 10;
                foreach (var v in chipValues)
                {
                    inner.OpenComponent<FlareChip>(seq++);
                    inner.AddAttribute(seq++, "Value", v);
                    inner.AddAttribute(seq++, "Label", v);
                    inner.CloseComponent();
                }
            }));
            b.CloseComponent();
        };

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
}
