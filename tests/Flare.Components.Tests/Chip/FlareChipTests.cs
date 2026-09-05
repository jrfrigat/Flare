namespace Flare.Components.Tests;

public class FlareChipTests : FlareTestContext
{

    [Fact]
    public void Disabled_MarksTheChipAndLeavesTheTabOrder()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Archived")
            .Add(x => x.Disabled, true));

        var chip = cut.Find($".{Css.Classes.Chip.Root}");
        Assert.Contains(Css.Classes.Chip.Disabled, chip.ClassName);
        Assert.Equal("true", chip.GetAttribute("aria-disabled"));
        Assert.Equal("-1", chip.GetAttribute("tabindex"));
    }

    [Fact]
    public void Enabled_ChipCarriesNoDisabledMarkers()
    {
        var cut = Render<FlareChip>(p => p.Add(x => x.Label, "Active"));

        var chip = cut.Find($".{Css.Classes.Chip.Root}");
        Assert.DoesNotContain(Css.Classes.Chip.Disabled, chip.ClassName);
        Assert.Null(chip.GetAttribute("aria-disabled"));
        Assert.Equal("0", chip.GetAttribute("tabindex"));
    }

    [Fact]
    public void Disabled_SwallowsClickAndKeyboardActivation()
    {
        // pointer-events:none keeps a real mouse away, but a programmatic or
        // assistive-technology activation still reaches the handler.
        int clicks = 0, selections = 0;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Archived")
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, () => clicks++)
            .Add(x => x.SelectedChanged, (bool _) => selections++));

        cut.Find($".{Css.Classes.Chip.Root}").Click();
        cut.Find($".{Css.Classes.Chip.Root}").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });

        Assert.Equal(0, clicks);
        Assert.Equal(0, selections);
    }

    [Fact]
    public void Disabled_SwallowsClose()
    {
        int closes = 0;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Archived")
            .Add(x => x.Closeable, true)
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClose, () => closes++));

        var close = cut.Find($".{Css.Classes.Chip.Close}");
        Assert.True(close.HasAttribute("disabled"));

        close.Click();

        Assert.Equal(0, closes);
    }

    [Fact]
    public void Disabled_ChipInAGroupDoesNotChangeTheSelection()
    {
        IReadOnlyCollection<string>? selected = null;
        var cut = Render<FlareChipGroup>(p => p
            .Add(x => x.SelectedValuesChanged, (IReadOnlyCollection<string> v) => selected = v)
            .AddChildContent<FlareChip>(c => c
                .Add(x => x.Label, "Archived")
                .Add(x => x.Value, "archived")
                .Add(x => x.Disabled, true)));

        cut.Find($".{Css.Classes.Chip.Root}").Click();

        Assert.Null(selected);
    }
}

// ------------------------------------------------------------------------------
// FlareBadge  (5 tests from Wave3)
// ------------------------------------------------------------------------------
