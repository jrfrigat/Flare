namespace Flare.Components.Tests;

// A spacer either stretches (no size) or holds a fixed size given as a spacing step or a raw value;
// the fixed form must stop stretching so the size is what shows.
public class FlareSpacerTests : FlareTestContext
{
    [Fact]
    public void DefaultSpacer_StretchesWithNoSize()
    {
        var cut = Render<FlareSpacer>();

        var el = cut.Find($".{Css.Classes.Spacer.Root}");
        Assert.DoesNotContain(Css.Classes.Spacer.Fixed, el.ClassName);
        Assert.Null(el.GetAttribute("style"));
    }

    [Fact]
    public void HeightStep_UsesTheSpacingTokenAndStopsStretching()
    {
        var cut = Render<FlareSpacer>(p => p.Add(x => x.Height, FlareSpacing.Small));

        var el = cut.Find($".{Css.Classes.Spacer.Root}");
        Assert.Contains(Css.Classes.Spacer.Fixed, el.ClassName);
        Assert.Contains($"height:var({Css.Tokens.Spacing.S6})", el.GetAttribute("style"));
    }

    [Fact]
    public void CustomHeight_IsApplied()
    {
        var cut = Render<FlareSpacer>(p => p
            .Add(x => x.Height, FlareSpacing.Custom)
            .Add(x => x.HeightValue, "14rem"));

        Assert.Contains("height:14rem", cut.Find($".{Css.Classes.Spacer.Root}").GetAttribute("style"));
    }

    [Fact]
    public void CustomWidth_IsApplied()
    {
        var cut = Render<FlareSpacer>(p => p
            .Add(x => x.Width, FlareSpacing.Custom)
            .Add(x => x.WidthValue, "8px"));

        Assert.Contains("width:8px", cut.Find($".{Css.Classes.Spacer.Root}").GetAttribute("style"));
    }

    [Fact]
    public void DangerousValue_IsStripped()
    {
        var cut = Render<FlareSpacer>(p => p
            .Add(x => x.Height, FlareSpacing.Custom)
            .Add(x => x.HeightValue, "1rem;width:expression(alert(1))"));

        var style = cut.Find($".{Css.Classes.Spacer.Root}").GetAttribute("style");
        Assert.DoesNotContain("expression", style);
    }
}
