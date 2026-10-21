using Flare.Components.IDE;

namespace Flare.Components.Tests;

/// <summary>
/// A layout embedded in a page frame asks for a height instead of writing it as an inline style; the
/// root style carries the request, and leaving it unset keeps the viewport-filling default.
/// </summary>
public class IdeLayoutHeightTests : FlareTestContext
{
    public IdeLayoutHeightTests() => Services.AddFlareIde();

    [Fact]
    public void Height_ReachesTheRootStyle()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.Height, "520px"));

        Assert.Contains("height:520px", cut.Find($".{Css.Classes.Ide.Layout.Root}").GetAttribute("style"));
    }

    [Fact]
    public void NoHeight_LeavesTheRootStyleWithoutOne()
    {
        var cut = Render<FlareIdeLayout>();

        Assert.DoesNotContain(";height:", cut.Find($".{Css.Classes.Ide.Layout.Root}").GetAttribute("style"));
    }
}
