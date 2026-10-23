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
    public void PerSideSpacingAndGrow_ReachTheBox()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.MarginBlockStart, FlareSpacing.Custom)
            .Add(x => x.MarginBlockStartValue, "0.75rem")
            .Add(x => x.MarginBlockEnd, FlareSpacing.Small)
            .Add(x => x.PaddingBlockStart, FlareSpacing.Custom)
            .Add(x => x.PaddingBlockStartValue, "1rem")
            .Add(x => x.MarginInlineStart, FlareSpacing.Custom)
            .Add(x => x.MarginInlineStartValue, "0.5rem")
            .Add(x => x.PaddingInlineEnd, FlareSpacing.Small)
            .Add(x => x.FlexGrow, 1d));

        var style = StyleOf(cut);
        Assert.Contains("margin-block-start:0.75rem", style);
        Assert.Contains($"margin-block-end:var({Css.Tokens.Spacing.S6})", style);
        Assert.Contains("padding-block-start:1rem", style);
        Assert.Contains("margin-inline-start:0.5rem", style);
        Assert.Contains($"padding-inline-end:var({Css.Tokens.Spacing.S6})", style);
        Assert.Contains("flex-grow:1", style);
    }

    [Fact]
    public void OverflowPaintAndPosition_ReachTheBox()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.Overflow, "auto")
            .Add(x => x.Background, FlareColor.Primary)
            .Add(x => x.Foreground, FlareColor.OnSurface)
            .Add(x => x.Sticky, true)
            .Add(x => x.StickyTop, "1rem")
            .Add(x => x.Contain, "layout paint")
            .Add(x => x.FlexBasis, "0"));

        var style = StyleOf(cut);
        Assert.Contains("overflow:auto", style);
        Assert.Contains($"background:var({Css.Tokens.Color.Primary})", style);
        Assert.Contains($"color:var({Css.Tokens.Color.OnPrimary})", style);
        Assert.Contains($"color:var({Css.Tokens.Color.OnSurface})", style);
        Assert.Contains("position:sticky", style);
        Assert.Contains("top:1rem", style);
        Assert.Contains("contain:layout paint", style);
        Assert.Contains("flex-basis:0", style);
    }

    [Fact]
    public void Relative_MakesTheBoxAContainingBlock()
    {
        var cut = Render<FlareStack>(p => p.Add(x => x.Relative, true));

        Assert.Contains("position:relative", StyleOf(cut));
    }

    // Text keeps the space inside and around itself and the measure of its lines; everything else
    // about the box it sits in is the box's.
    [Fact]
    public void Text_TakesSpacingAndMeasure()
    {
        var plain = Render<FlareText>(p => p.AddChildContent("Body"));
        Assert.Null(plain.Find($".{Css.Classes.Text.Root}").GetAttribute("style"));

        var spaced = Render<FlareText>(p => p
            .Add(x => x.MarginBlockEnd, FlareSpacing.Small)
            .Add(x => x.PaddingInlineStart, FlareSpacing.Custom)
            .Add(x => x.PaddingInlineStartValue, "0.5rem")
            .Add(x => x.MaxWidth, "36rem")
            .Add(x => x.Style, "color:red")
            .AddChildContent("Body"));
        Assert.Equal(
            $"padding-inline-start:0.5rem;margin-block-end:var({Css.Tokens.Spacing.S6});max-width:36rem;color:red",
            spaced.Find($".{Css.Classes.Text.Root}").GetAttribute("style"));
    }

    // One spacing vocabulary for text and boxes: the same names, the same types.
    [Fact]
    public void SpacingParameters_MatchBetweenTextAndBoxes()
    {
        static string[] Spacing(Type type) => type
            .GetProperties()
            .Where(p => p.IsDefined(typeof(Microsoft.AspNetCore.Components.ParameterAttribute), true)
                        && (p.Name.StartsWith("Padding", StringComparison.Ordinal)
                            || p.Name.StartsWith("Margin", StringComparison.Ordinal)))
            .Select(p => $"{p.Name}:{p.PropertyType.Name}")
            .Order(StringComparer.Ordinal)
            .ToArray();

        var text = Spacing(typeof(FlareSpacedBase));
        Assert.Equal(20, text.Length);
        Assert.Equal(text, Spacing(typeof(FlareLayoutBoxBase)));
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
