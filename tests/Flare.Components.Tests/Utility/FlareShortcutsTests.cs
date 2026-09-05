using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareShortcutsTests : FlareTestContext
{
    [Fact]
    public void RendersWithoutError()
    {
        var cut = Render<FlareShortcuts>();

        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareShortcuts>(p => p
            .AddChildContent("<span id=\"shortcut-child\">Help</span>"));

        Assert.NotEmpty(cut.FindAll("#shortcut-child"));
    }

    [Fact]
    public void RendersMultipleChildren()
    {
        var cut = Render<FlareShortcuts>(p => p
            .AddChildContent("<p id=\"a\">A</p><p id=\"b\">B</p>"));

        Assert.NotEmpty(cut.FindAll("#a"));
        Assert.NotEmpty(cut.FindAll("#b"));
    }
}

// ------------------------------------------------------------------------------
// FlareScrollTop  (6 tests from Wave6)
// ------------------------------------------------------------------------------
