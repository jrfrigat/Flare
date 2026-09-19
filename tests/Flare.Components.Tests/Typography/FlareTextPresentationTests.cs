using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The two presentation requests a type scale cannot express on its own: what case the text is set in,
/// and how far it is allowed to run. Both were reachable only through an inline style or a class of the
/// caller's own - an eyebrow label became a stylesheet, and an app-bar title that had to fit became
/// three CSS declarations written at the call site.
/// </summary>
public class FlareTextPresentationTests : FlareTestContext
{
    private static string ClassOf(IRenderedComponent<FlareText> cut) => cut.Find("*").ClassName ?? string.Empty;
    private static string StyleOf(IRenderedComponent<FlareText> cut) => cut.Find("*").GetAttribute("style") ?? string.Empty;

    [Theory]
    [InlineData(TextTransform.Uppercase, Css.Classes.Text.TransformUppercase)]
    [InlineData(TextTransform.Lowercase, Css.Classes.Text.TransformLowercase)]
    [InlineData(TextTransform.Capitalize, Css.Classes.Text.TransformCapitalize)]
    public void Transform_AppliesAClassNotAnInlineStyle(TextTransform transform, string expected)
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.Transform, transform)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Workspace flow"))));

        Assert.Contains(expected, ClassOf(cut));
        Assert.DoesNotContain("text-transform", StyleOf(cut));
    }

    [Fact]
    public void Transform_DefaultsToNothing()
    {
        var cut = Render<FlareText>(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "plain"))));

        Assert.DoesNotContain("transform", ClassOf(cut));
    }

    // One line keeps the element's own display type, so a heading stays a heading; the clamp does not,
    // which is why one parameter still picks two mechanisms.
    [Fact]
    public void MaxLinesOne_TruncatesWithoutClamping()
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.MaxLines, 1)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "A family name that will not fit"))));

        Assert.Contains(Css.Classes.Text.TruncateLine, ClassOf(cut));
        Assert.DoesNotContain(Css.Classes.Text.Clamp, ClassOf(cut));
        Assert.DoesNotContain(Css.Tokens.LocalVars.TextMaxLines, StyleOf(cut));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(10)]
    public void MaxLinesAboveOne_ClampsAndCarriesTheDepth(int lines)
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.MaxLines, lines)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "a long description"))));

        Assert.Contains(Css.Classes.Text.Clamp, ClassOf(cut));
        Assert.DoesNotContain(Css.Classes.Text.TruncateLine, ClassOf(cut));
        Assert.Contains($"{Css.Tokens.LocalVars.TextMaxLines}:{lines}", StyleOf(cut));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MaxLinesUnsetOrNegative_LeavesTheTextAlone(int lines)
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.MaxLines, lines)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "free to wrap"))));

        Assert.DoesNotContain(Css.Classes.Text.TruncateLine, ClassOf(cut));
        Assert.DoesNotContain(Css.Classes.Text.Clamp, ClassOf(cut));
    }

    // The clamp depth and a custom colour are two channels on one style attribute; adding the first
    // must not drop the second, and neither may drop the caller's own Style.
    [Fact]
    public void ClampDepth_CustomColourAndCallerStyle_AllSurvive()
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.MaxLines, 3)
            .Add(x => x.Color, FlareColor.Custom("#ff0000"))
            .Add(x => x.Style, "margin:0;")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "x"))));

        var style = StyleOf(cut);
        Assert.Contains($"{Css.Tokens.LocalVars.TextMaxLines}:3", style);
        Assert.Contains("#ff0000", style);
        Assert.Contains("margin:0", style);
    }

    [Fact]
    public void NoPresentationRequested_EmitsNoStyleAttribute()
    {
        var cut = Render<FlareText>(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "x"))));

        Assert.False(cut.Find("*").HasAttribute("style"));
    }

    // Casing and clamping are independent of everything else the component already does.
    [Fact]
    public void Transform_And_MaxLines_ComposeWithTheRest()
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.Typo, TypographyScale.LabelSmall)
            .Add(x => x.Transform, TextTransform.Uppercase)
            .Add(x => x.MaxLines, 1)
            .Add(x => x.Mono, true)
            .Add(x => x.Align, TextAlign.Center)
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "ORCHESTRATION LOAD"))));

        var cls = ClassOf(cut);
        Assert.Contains(Css.Classes.Text.TransformUppercase, cls);
        Assert.Contains(Css.Classes.Text.TruncateLine, cls);
        Assert.Contains(Css.Classes.Text.Mono, cls);
        Assert.Contains(Css.Classes.Text.AlignCenter, cls);
    }
}
