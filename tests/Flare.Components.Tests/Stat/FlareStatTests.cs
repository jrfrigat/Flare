namespace Flare.Components.Tests;

public class FlareStatTests : FlareTestContext
{
    private IRenderedComponent<FlareStat> RenderStat(Action<ComponentParameterCollectionBuilder<FlareStat>>? more = null) =>
        Render<FlareStat>(p =>
        {
            p.Add(x => x.Label, "Revenue").Add(x => x.Value, "1,200").Add(x => x.Unit, "USD");
            more?.Invoke(p);
        });

    [Fact]
    public void TheGroupIsNamedLabelColonValueAndUnit()
    {
        var root = RenderStat().Find($".{Css.Classes.Stat.Root}");

        Assert.Equal("group", root.GetAttribute("role"));
        Assert.Equal("Revenue: 1,200 USD", root.GetAttribute("aria-label"));
    }

    [Fact]
    public void LabelValueAndUnitAreRenderedInTheirOwnParts()
    {
        var cut = RenderStat();

        Assert.Equal("Revenue", cut.Find($".{Css.Classes.Stat.Label}").TextContent);
        Assert.Equal("1,200", cut.Find($".{Css.Classes.Stat.Value}").TextContent);
        Assert.Equal("USD", cut.Find($".{Css.Classes.Stat.Unit}").TextContent);
    }

    [Fact]
    public void OptionalPartsAreLeftOutWhenNotSet()
    {
        var cut = Render<FlareStat>(p => p.Add(x => x.Value, "42"));

        Assert.Empty(cut.FindAll($".{Css.Classes.Stat.Label}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Stat.Unit}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Stat.Delta}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Stat.Caption}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Stat.Icon}"));
        Assert.Equal("42", cut.Find($".{Css.Classes.Stat.Root}").GetAttribute("aria-label"));
    }

    [Fact]
    public void TheIconIsDecorative()
    {
        var cut = RenderStat(p => p.Add(x => x.Icon, FlareIcons.Group));

        Assert.Equal("true", cut.Find($".{Css.Classes.Stat.Icon}").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void ADeltaRolePaintsTheChangeInThatRolesColor()
    {
        var cut = RenderStat(p => p.Add(x => x.Delta, "+12%").Add(x => x.DeltaColor, FlareColor.Success));

        var delta = cut.Find($".{Css.Classes.Stat.Delta}");
        Assert.Equal("+12%", delta.TextContent);
        Assert.Contains(FlareColor.Success.CssValue!, delta.GetAttribute("style"));
    }

    [Fact]
    public void ADeltaWithoutAColorUsesTheMutedTextColor()
    {
        var cut = RenderStat(p => p.Add(x => x.Delta, "-3"));

        Assert.Null(cut.Find($".{Css.Classes.Stat.Delta}").GetAttribute("style"));
    }

    [Theory]
    [InlineData(FlareSize.Xs, Css.Classes.Stat.SizeSm)]
    [InlineData(FlareSize.Sm, Css.Classes.Stat.SizeSm)]
    [InlineData(FlareSize.Lg, Css.Classes.Stat.SizeLg)]
    [InlineData(FlareSize.Xl, Css.Classes.Stat.SizeLg)]
    public void SizesMapToTheThreeValueSteps(FlareSize size, string expected)
    {
        var cut = RenderStat(p => p.Add(x => x.Size, size));

        Assert.Contains(expected, cut.Find($".{Css.Classes.Stat.Root}").ClassName);
    }

    [Fact]
    public void TheDefaultSizeIsMedium()
    {
        var className = RenderStat().Find($".{Css.Classes.Stat.Root}").ClassName;

        Assert.Contains(Css.Classes.Stat.SizeMd, className);
        Assert.DoesNotContain(Css.Classes.Stat.SizeSm, className);
        Assert.DoesNotContain(Css.Classes.Stat.SizeLg, className);
    }

    [Fact]
    public void ALabelGivenAsMarkupNamesTheStatByReference()
    {
        var cut = RenderStat(p => p.Add(x => x.LabelContent,
            (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddMarkupContent(0, "Revenue <a href=\"#\">?</a>"))));

        var root = cut.Find($".{Css.Classes.Stat.Root}");
        Assert.Null(root.GetAttribute("aria-label"));
        var ids = root.GetAttribute("aria-labelledby")!.Split(' ');
        Assert.Equal(3, ids.Length);
        Assert.Contains("<a href=\"#\">?</a>", cut.Find($"#{ids[0]}").InnerHtml);
        Assert.Equal("1,200", cut.Find($"#{ids[1]}").TextContent);
        Assert.Equal("USD", cut.Find($"#{ids[2]}").TextContent);
    }

    [Fact]
    public void ACaptionGivenAsMarkupIsRendered()
    {
        var cut = RenderStat(p => p.Add(x => x.CaptionContent,
            (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddMarkupContent(0, "<a id=\"src\">Source</a>"))));

        Assert.NotNull(cut.Find($".{Css.Classes.Stat.Caption} #src"));
    }

    [Fact]
    public void TheCaptionIsRendered()
    {
        var cut = RenderStat(p => p.Add(x => x.Caption, "Last 30 days"));

        Assert.Equal("Last 30 days", cut.Find($".{Css.Classes.Stat.Caption}").TextContent);
    }
}
