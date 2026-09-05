// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareNavGroup  (8 tests)
// -----------------------------------------------------------------------------

public class FlareNavGroupTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareNavGroup>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Navigation.NavGroup}"));
    }

    [Fact]
    public void Label_RendersLabelText()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Navigation"));

        Assert.Contains("Navigation", cut.Find($".{Css.Classes.Navigation.NavGroupTitle}").TextContent);
    }

    [Fact]
    public void Icon_RendersIconSpan()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Icon, FlareIcons.Home));

        // "home" is built in, so the icon renders as inline SVG (no Material font).
        Assert.Equal(FlareIcons.Home.Data, cut.Find($".{Css.Classes.Navigation.GroupIcon} path").GetAttribute("d"));
    }

    [Fact]
    public void Expanded_False_HidesChildren()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, false)
            .AddChildContent("<a id=\"nav-child\">Link</a>"));

        // Items always rendered in DOM (no state loss); hidden via CSS class when collapsed
        Assert.NotEmpty(cut.FindAll("#nav-child"));
        var items = cut.Find($".{Css.Classes.Navigation.GroupItems}");
        Assert.DoesNotContain(Css.Classes.Navigation.GroupItemsOpen, items.ClassName);
    }

    [Fact]
    public void Expanded_True_ShowsChildren()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, true)
            .AddChildContent("<a id=\"nav-child-visible\">Link</a>"));

        Assert.NotEmpty(cut.FindAll("#nav-child-visible"));
    }

    [Fact]
    public void HeaderButton_IsClickable()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, false)
            .AddChildContent("<a id=\"nav-link\">Link</a>"));

        cut.Find($"button.{Css.Classes.Navigation.NavGroupHeader}").Click();

        Assert.NotEmpty(cut.FindAll("#nav-link"));
    }

    [Fact]
    public void RendersChevronElement()
    {
        var cut = Render<FlareNavGroup>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Navigation.GroupChevron}"));
    }

    [Fact]
    public void Expanded_True_RendersChildContent()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, true)
            .AddChildContent("<span id=\"group-content\">Content</span>"));

        Assert.NotEmpty(cut.FindAll("#group-content"));
    }
}
