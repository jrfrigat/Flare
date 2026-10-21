namespace Flare.Components.Tests;

// Spacing and size are parameters on the layout boxes, so a page lays itself out without writing CSS;
// a consumer's Style still comes last and still wins.
public class FlareLayoutBoxTests : FlareTestContext
{
    private string? StyleOf(Bunit.IRenderedComponent<FlareStack> cut)
        => cut.Find($".{Css.Classes.Stack.Root}").GetAttribute("style");

    [Fact]
    public void NoBoxParameters_LeaveTheStyleEmpty()
    {
        var cut = Render<FlareStack>();

        Assert.Null(StyleOf(cut));
    }

    [Fact]
    public void PaddingStep_UsesTheSpacingToken()
    {
        var cut = Render<FlareStack>(p => p.Add(x => x.Padding, FlareSpacing.Large));

        Assert.Contains($"padding:var({Css.Tokens.Spacing.S12})", StyleOf(cut));
    }

    [Fact]
    public void CustomPaddingAndMargin_AcceptShorthands()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.Padding, FlareSpacing.Custom)
            .Add(x => x.PaddingValue, "1rem 2rem")
            .Add(x => x.Margin, FlareSpacing.Custom)
            .Add(x => x.MarginValue, "0 auto"));

        var style = StyleOf(cut);
        Assert.Contains("padding:1rem 2rem", style);
        Assert.Contains("margin:0 auto", style);
    }

    [Fact]
    public void SizeParameters_ReachTheBox()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.MaxWidth, "24rem")
            .Add(x => x.MinWidth, "12rem")
            .Add(x => x.Width, "100%")
            .Add(x => x.Height, "6rem")
            .Add(x => x.MinHeight, "4rem"));

        var style = StyleOf(cut);
        Assert.Contains("max-width:24rem", style);
        Assert.Contains("min-width:12rem", style);
        Assert.Contains("width:100%", style);
        Assert.Contains("height:6rem", style);
        Assert.Contains("min-height:4rem", style);
    }

    [Fact]
    public void ConsumerStyle_ComesLast()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.Padding, FlareSpacing.Small)
            .Add(x => x.Style, "background:red"));

        var style = StyleOf(cut)!;
        Assert.True(style.IndexOf("padding:", System.StringComparison.Ordinal)
                    < style.IndexOf("background:red", System.StringComparison.Ordinal));
    }

    [Fact]
    public void DangerousSizeValue_IsStripped()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.Width, "1px;background:expression(alert(1))"));

        Assert.DoesNotContain("expression", StyleOf(cut));
    }

    [Fact]
    public void GridAndColAndCard_AlsoTakeTheBoxParameters()
    {
        var grid = Render<FlareGrid>(p => p.Add(x => x.Padding, FlareSpacing.Small));
        Assert.Contains($"padding:var({Css.Tokens.Spacing.S6})", grid.Find($".{Css.Classes.Grid.Root}").GetAttribute("style"));

        var col = Render<FlareCol>(p => p.Add(x => x.MaxWidth, "10rem"));
        Assert.Contains("max-width:10rem", col.Find($".{Css.Classes.Col.Root}").GetAttribute("style"));

        var card = Render<FlareCard>(p => p
            .Add(x => x.Padding, FlareSpacing.Medium)
            .Add(x => x.MaxWidth, "18rem"));
        var cardStyle = card.Find($".{Css.Classes.Card.Root}").GetAttribute("style");
        Assert.Contains($"padding:var({Css.Tokens.Spacing.S8})", cardStyle);
        Assert.Contains("max-width:18rem", cardStyle);
    }
}
