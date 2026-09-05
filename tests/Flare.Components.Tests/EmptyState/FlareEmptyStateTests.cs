using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareEmptyStateTests : FlareTestContext
{
    [Fact]
    public void RendersIcon()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.IconContent, b => b.AddMarkupContent(0, "<span class=\"test-icon\">★</span>")));

        Assert.NotNull(cut.Find($".{Css.Classes.Empty.StateIcon}"));
        Assert.NotEmpty(cut.FindAll(".test-icon"));
    }

    [Fact]
    public void RendersTitle()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.Title, "Nothing here"));

        var title = cut.Find($".{Css.Classes.Empty.StateTitle}");
        Assert.Equal("Nothing here", title.TextContent);
    }

    [Fact]
    public void RendersDescription()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.Description, "Try adjusting your filters."));

        var desc = cut.Find($".{Css.Classes.Empty.StateDescription}");
        Assert.Equal("Try adjusting your filters.", desc.TextContent);
    }

    [Fact]
    public void RendersActionContent()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.ActionContent,
                b => b.AddMarkupContent(0, "<button class=\"action-btn\">Retry</button>")));

        Assert.NotNull(cut.Find($".{Css.Classes.Empty.StateAction}"));
        Assert.NotEmpty(cut.FindAll(".action-btn"));
    }

    [Fact]
    public void RendersMinimal_TitleOnly()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.Title, "Empty"));

        Assert.NotNull(cut.Find($".{Css.Classes.Empty.StateTitle}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Empty.StateIcon}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Empty.StateDescription}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Empty.StateAction}"));
    }

    [Fact]
    public void RendersWithAllSlots()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.IconContent, b => b.AddMarkupContent(0, "<span>icon</span>"))
            .Add(x => x.Title, "No Results")
            .Add(x => x.Description, "Clear your search to see results.")
            .Add(x => x.ActionContent, b => b.AddMarkupContent(0, "<button>Clear</button>")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Empty.StateIcon}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Empty.StateTitle}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Empty.StateDescription}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Empty.StateAction}"));
    }
}

// ------------------------------------------------------------------------------
// FlareConfirmDialogProvider  (8 tests from Wave6)
// ------------------------------------------------------------------------------
