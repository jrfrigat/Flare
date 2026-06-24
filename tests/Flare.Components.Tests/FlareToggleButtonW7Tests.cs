// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareToggleButton with Group  (5 tests)
// -----------------------------------------------------------------------------

public class FlareToggleButtonW7Tests : FlareTestContext
{
    [Fact]
    public void Standalone_RendersButton()
    {
        var cut = Render<FlareToggleButton>();

        Assert.NotEmpty(cut.FindAll("button.flare-toggle-btn"));
    }

    [Fact]
    public void Toggled_True_AddsPressedClass()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Toggled, true));

        Assert.Contains("flare-toggle-btn--pressed", cut.Find("button").ClassName ?? "");
    }

    [Fact]
    public void Value_Param_Exists()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Value, "btn-value"));

        Assert.Equal("btn-value", cut.Instance.Value);
    }

    [Fact]
    public void RendersAriaPressedAttribute()
    {
        var cut = Render<FlareToggleButton>();

        Assert.NotNull(cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Disabled_RendersDisabledAttribute()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }
}
