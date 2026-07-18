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

        Assert.NotEmpty(cut.FindAll(".flare-nav-group"));
    }

    [Fact]
    public void Label_RendersLabelText()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Navigation"));

        Assert.Contains("Navigation", cut.Find(".flare-nav-group__title").TextContent);
    }

    [Fact]
    public void Icon_RendersIconSpan()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Icon, FlareIcons.Home));

        // "home" is built in, so the icon renders as inline SVG (no Material font).
        Assert.Equal(FlareIcons.Home.Data, cut.Find(".flare-nav-group__icon path").GetAttribute("d"));
    }

    [Fact]
    public void Expanded_False_HidesChildren()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, false)
            .AddChildContent("<a id=\"nav-child\">Link</a>"));

        // Items always rendered in DOM (no state loss); hidden via CSS class when collapsed
        Assert.NotEmpty(cut.FindAll("#nav-child"));
        var items = cut.Find(".flare-nav-group__items");
        Assert.DoesNotContain("flare-nav-group__items--open", items.ClassName);
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

        cut.Find("button.flare-nav-group__header").Click();

        Assert.NotEmpty(cut.FindAll("#nav-link"));
    }

    [Fact]
    public void RendersChevronElement()
    {
        var cut = Render<FlareNavGroup>();

        Assert.NotEmpty(cut.FindAll(".flare-nav-group__chevron"));
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
