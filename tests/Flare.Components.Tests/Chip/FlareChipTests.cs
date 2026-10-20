using Microsoft.AspNetCore.Components;
namespace Flare.Components.Tests;

public class FlareChipTests : FlareTestContext
{

    [Fact]
    public void Disabled_MarksTheChipAndLeavesTheTabOrder()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Archived")
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => { }))
            .Add(x => x.Disabled, true));

        var chip = cut.Find($".{Css.Classes.Chip.Root}");
        Assert.Contains(Css.Classes.Chip.Disabled, chip.ClassName);
        Assert.Null(chip.GetAttribute("role"));
        Assert.Null(chip.GetAttribute("tabindex"));
        Assert.True(cut.Find($"button.{Css.Classes.Chip.Action}").HasAttribute("disabled"));
    }

    [Fact]
    public void Enabled_ChipCarriesNoDisabledMarkers()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Active")
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => { })));

        var chip = cut.Find($".{Css.Classes.Chip.Root}");
        Assert.DoesNotContain(Css.Classes.Chip.Disabled, chip.ClassName);
        Assert.Null(chip.GetAttribute("tabindex"));
        Assert.False(cut.Find($"button.{Css.Classes.Chip.Action}").HasAttribute("disabled"));
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

        var action = cut.Find($"button.{Css.Classes.Chip.Action}");
        Assert.True(action.HasAttribute("disabled"));

        action.Click();

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

        cut.Find($"button.{Css.Classes.Chip.Action}").Click();

        Assert.Null(selected);
    }
}

// A chip is a control when something is wired to it and a tag when nothing is. Before this, every chip
// was a control: a screenful of identifiers and version labels each announced itself as a button and
// each took a tab stop, and the only way out was to stop using the component.
public class FlareChipInteractionTests : FlareTestContext
{
    [Fact]
    public void NoWiring_IsATagNotAButton()
    {
        var chip = Render<FlareChip>(p => p.Add(x => x.Label, "STORY-01"))
            .Find($".{Css.Classes.Chip.Root}");

        Assert.Null(chip.GetAttribute("role"));
        Assert.Null(chip.GetAttribute("tabindex"));
        Assert.Contains(Css.Classes.Chip.Static, chip.ClassName);
    }

    // The state layers answer a pointer and a focus ring a tag does not have.
    [Fact]
    public void NoWiring_DropsTheStateLayers()
    {
        var chip = Render<FlareChip>(p => p.Add(x => x.Label, "STORY-01"))
            .Find($".{Css.Classes.Chip.Root}");

        Assert.DoesNotContain(Css.Classes.State.LayerHover, chip.ClassName);
        Assert.DoesNotContain(Css.Classes.State.LayerFocus, chip.ClassName);
        Assert.DoesNotContain(Css.Classes.State.LayerPressed, chip.ClassName);
    }

    [Fact]
    public void OnClickBound_IsAButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Filter")
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => { })));

        var chip = cut.Find($".{Css.Classes.Chip.Root}");
        Assert.Null(chip.GetAttribute("role"));
        Assert.Equal("BUTTON", cut.Find($".{Css.Classes.Chip.Action}").TagName);
        Assert.DoesNotContain(Css.Classes.Chip.Static, chip.ClassName);
    }

    [Fact]
    public void SelectedChangedBound_IsAButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Filter")
            .Add(x => x.SelectedChanged, EventCallback.Factory.Create<bool>(this, _ => { })));

        Assert.Null(cut.Find($".{Css.Classes.Chip.Root}").GetAttribute("role"));
        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Chip.Action}"));
    }

    // Selected without a way to change it is a state being shown, not a control.
    [Fact]
    public void SelectedWithoutAHandler_IsStillATag()
    {
        var chip = Render<FlareChip>(p => p
                .Add(x => x.Label, "Active")
                .Add(x => x.Selected, true))
            .Find($".{Css.Classes.Chip.Root}");

        Assert.Null(chip.GetAttribute("role"));
        Assert.Contains(Css.Classes.Chip.Selected, chip.ClassName);
    }

    // The close button is its own control and carries its own focus, so it does not make the body one.
    [Fact]
    public void Closeable_LeavesTheBodyATag()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "tag")
            .Add(x => x.Closeable, true)
            .Add(x => x.OnClose, EventCallback.Factory.Create(this, () => { })));

        Assert.Null(cut.Find($".{Css.Classes.Chip.Root}").GetAttribute("role"));
        Assert.NotNull(cut.Find($".{Css.Classes.Chip.Close}"));
    }

    [Fact]
    public void ExplicitButton_OverridesTheAbsenceOfWiring()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "splatted")
            .Add(x => x.Interaction, ChipInteraction.Button));

        Assert.Null(cut.Find($".{Css.Classes.Chip.Root}").GetAttribute("role"));
        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Chip.Action}"));
    }

    [Fact]
    public void ExplicitStatic_OverridesABoundHandler()
    {
        var clicks = 0;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "read only")
            .Add(x => x.Interaction, ChipInteraction.Static)
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicks++)));

        var chip = cut.Find($".{Css.Classes.Chip.Root}");
        Assert.Null(chip.GetAttribute("role"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Chip.Action}"));
        Assert.Equal(0, clicks);
    }

    // A chip is an inline run - a tag beside prose, a label in a table cell - so a div was invalid
    // wherever it sat inside a paragraph.
    [Fact]
    public void RootIsAnInlineElement()
    {
        var chip = Render<FlareChip>(p => p.Add(x => x.Label, "tag"))
            .Find($".{Css.Classes.Chip.Root}");

        Assert.Equal("SPAN", chip.TagName);
    }
}
