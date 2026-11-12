using Flare.Abstractions;
using Flare.Components;

namespace Flare.Components.Tests;

// FlareLayout.Motion chooses how the shell moves when an in-flow drawer changes its track.
// Animated (A) is the default and keeps the old track transition; Instant (A0) removes the motion;
// Surface (B) moves the drawer surface and steps the track once, recording the closing direction.
public class FlareLayoutMotionTests : FlareTestContext
{
    private static string RootClasses(IRenderedComponent<FlareLayout> cut) =>
        cut.Find($".{Css.Classes.Layout.Root}").GetAttribute("class") ?? string.Empty;

    private IRenderedComponent<FlareLayout> RenderWithDrawer(LayoutMotion motion, bool open = false) =>
        Render<FlareLayout>(p => p
            .Add(x => x.Responsive, false)
            .Add(x => x.Motion, motion)
            .AddChildContent<FlareLayoutDrawer>(d => d
                .Add(x => x.Variant, DrawerVariant.Persistent)
                .Add(x => x.Width, "16rem")
                .Add(x => x.Open, open)));

    [Fact]
    public void Motion_DefaultsToAnimated()
    {
        var cut = Render<FlareLayout>(p => p.AddChildContent("<div>content</div>"));

        Assert.Contains(Css.Classes.Layout.AnimateTrack, RootClasses(cut));
        Assert.DoesNotContain(Css.Classes.Layout.MotionSurface, RootClasses(cut));
    }

    [Fact]
    public void Instant_EmitsNoMotionClass()
    {
        var cut = RenderWithDrawer(LayoutMotion.Instant);

        Assert.DoesNotContain(Css.Classes.Layout.AnimateTrack, RootClasses(cut));
        Assert.DoesNotContain(Css.Classes.Layout.MotionSurface, RootClasses(cut));
    }

    [Fact]
    public void Surface_EmitsTheSurfaceClass()
    {
        var cut = RenderWithDrawer(LayoutMotion.Surface);

        Assert.Contains(Css.Classes.Layout.MotionSurface, RootClasses(cut));
        Assert.DoesNotContain(Css.Classes.Layout.AnimateTrack, RootClasses(cut));
    }

    [Fact]
    public async Task Surface_ClosingDrawer_RecordsTheClosingDirection()
    {
        var cut = RenderWithDrawer(LayoutMotion.Surface, open: true);
        Assert.DoesNotContain(Css.Classes.Layout.TrackClosing, RootClasses(cut));

        await cut.InvokeAsync(() => cut.FindComponent<FlareLayoutDrawer>().Instance.ToggleAsync());
        cut.WaitForAssertion(() =>
            Assert.Contains(Css.Classes.Layout.TrackClosing, RootClasses(cut)));
    }

    [Fact]
    public async Task Surface_OpeningDrawer_DropsTheClosingDirection()
    {
        var cut = RenderWithDrawer(LayoutMotion.Surface);
        await cut.InvokeAsync(() => cut.FindComponent<FlareLayoutDrawer>().Instance.ToggleAsync());
        cut.WaitForAssertion(() =>
            Assert.DoesNotContain(Css.Classes.Layout.TrackClosing, RootClasses(cut)));
    }
}
