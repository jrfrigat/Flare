using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class C_FlareTagFieldTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareTagField<string>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.TagInput.Root}"));
    }

    [Fact]
    public void RendersInputField()
    {
        var cut = Render<FlareTagField<string>>();

        Assert.NotEmpty(cut.FindAll($"input.{Css.Classes.TagInput.Input}"));
    }

    [Fact]
    public void PlaceholderRendered()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Placeholder, "Add a tag..."));

        Assert.Equal("Add a tag...", cut.Find($"input.{Css.Classes.TagInput.Input}").GetAttribute("placeholder"));
    }

    [Fact]
    public void DisabledState_HidesInput()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Empty(cut.FindAll($"input.{Css.Classes.TagInput.Input}"));
    }

    [Fact]
    public void RendersExistingTags()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Values, (IReadOnlyList<string>)["Alpha", "Beta"]));

        var chips = cut.FindAll($".{Css.Classes.Multiselect.Chip}");   // shared FlareChipStrip renders the unified chip class
        Assert.Equal(2, chips.Count);
    }
}

// ------------------------------------------------------------------------------
// FlareSlider  (5 tests from Wave3)
// ------------------------------------------------------------------------------
