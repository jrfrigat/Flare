namespace Flare.Components.Tests;

// FlareDivider ShouldRender (9 tests)
public class FlareDividerShouldRenderTests : FlareTestContext
{
    [Fact]
    public void RendersHrByDefault()
    {
        var cut = Render<FlareDivider>();
        Assert.NotEmpty(cut.FindAll($"hr.{Css.Classes.Divider.Root}"));
    }

    [Fact]
    public void VerticalRendersDivNotHr()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Vertical, true));
        Assert.Empty(cut.FindAll("hr"));
        Assert.NotEmpty(cut.FindAll($"div.{Css.Classes.Divider.Vertical}"));
    }

    [Fact]
    public void TextRendersTextVariant()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "OR"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Divider.Text}"));
        Assert.Contains("OR", cut.Markup);
    }

    [Fact]
    public void TextAlignLeftAppliesClass()
    {
        var cut = Render<FlareDivider>(p =>
        {
            p.Add(x => x.Text, "Start");
            p.Add(x => x.TextAlign, DividerTextAlign.Left);
        });
        Assert.Contains(Css.Classes.Divider.TextLeft, cut.Find($".{Css.Classes.Divider.Text}").ClassName);
    }

    [Fact]
    public void TextAlignRightAppliesClass()
    {
        var cut = Render<FlareDivider>(p =>
        {
            p.Add(x => x.Text, "End");
            p.Add(x => x.TextAlign, DividerTextAlign.Right);
        });
        Assert.Contains(Css.Classes.Divider.TextRight, cut.Find($".{Css.Classes.Divider.Text}").ClassName);
    }

    [Fact]
    public void TextAlignCenterIsDefault()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "Center"));
        Assert.Contains(Css.Classes.Divider.TextCenter, cut.Find($".{Css.Classes.Divider.Text}").ClassName);
    }

    [Fact]
    public void AnUnchangedRenderKeepsTheMarkup()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "OR"));
        var html1 = cut.Markup;
        cut.Render(p => p.Add(x => x.Text, "OR"));
        Assert.Equal(html1, cut.Markup);
    }

    [Fact]
    public void ATextDividerIsNotAriaHidden()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "OR"));

        Assert.Null(cut.Find($".{Css.Classes.Divider.Text}").GetAttribute("aria-hidden"));
    }

    // The old ShouldRender key ignored Class, Style, attributes and ChildContent, so after one unchanged
    // render those inputs changed the parameters and never reached the DOM.
    [Fact]
    public void InputsOutsideTheOldKeyStillReachTheDom()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Text, "OR")
            .Add(x => x.Class, "first")
            .AddUnmatched("data-testid", "one"));
        cut.Render(p => p.Add(x => x.Text, "OR").Add(x => x.Class, "first").AddUnmatched("data-testid", "one"));

        cut.Render(p => p
            .Add(x => x.Class, "second")
            .Add(x => x.Style, "color:red")
            .AddUnmatched("data-testid", "two")
            .AddChildContent("<b id=\"new-child\">new</b>"));

        var text = cut.Find($".{Css.Classes.Divider.Text}");
        Assert.Contains("second", text.ClassName);
        Assert.Equal("color:red", text.GetAttribute("style"));
        Assert.Equal("two", text.GetAttribute("data-testid"));
        Assert.NotEmpty(cut.FindAll("#new-child"));
    }
}
