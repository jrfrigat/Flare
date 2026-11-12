using Flare.Abstractions;
using Flare.Components;

namespace Flare.Components.Tests;

// FlareLayout.AnimateTrack is opt-in. The default shell changes its grid tracks in one frame: a
// grid-template-columns transition is a layout animation and re-lays out the whole content column
// every frame, which is the difference between a smooth drawer and dropped frames on a heavy page.
public class FlareLayoutAnimationTests : FlareTestContext
{
    private static string RootClasses(IRenderedComponent<FlareLayout> cut) =>
        cut.Find($".{Css.Classes.Layout.Root}").GetAttribute("class") ?? string.Empty;

    [Fact]
    public void TrackAnimation_IsOffByDefault()
    {
        var cut = Render<FlareLayout>(p => p.AddChildContent("<div>content</div>"));

        Assert.DoesNotContain(Css.Classes.Layout.AnimateTrack, RootClasses(cut));
    }

    [Fact]
    public void TrackAnimation_IsOptedInto()
    {
        var cut = Render<FlareLayout>(p => p
            .Add(x => x.AnimateTrack, true)
            .AddChildContent("<div>content</div>"));

        Assert.Contains(Css.Classes.Layout.AnimateTrack, RootClasses(cut));
    }
}
