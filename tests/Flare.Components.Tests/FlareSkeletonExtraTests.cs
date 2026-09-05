// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// Extra coverage: FlareSkeleton pulse + no-animation, FlareAlert dismiss,
// FlareToggleButton size classes, FlarePaper padding, FlareRating readonly
// -----------------------------------------------------------------------------

public class FlareSkeletonExtraTests : FlareTestContext
{
    [Fact]
    public void AnimationPulse_HasPulseClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Animation, SkeletonAnimation.Pulse));

        Assert.Contains(Css.Classes.Skeleton.Pulse, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void AnimationNone_NoAnimClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Animation, SkeletonAnimation.None));

        var cls = cut.Find($".{Css.Classes.Skeleton.Root}").ClassName ?? string.Empty;
        Assert.DoesNotContain(Css.Classes.Skeleton.Wave, cls);
        Assert.DoesNotContain(Css.Classes.Skeleton.Pulse, cls);
    }

    [Fact]
    public void AriaAttributes_Present()
    {
        var cut = Render<FlareSkeleton>();

        var el = cut.Find($".{Css.Classes.Skeleton.Root}");
        Assert.Equal("true", el.GetAttribute("aria-busy"));
        Assert.Equal("status", el.GetAttribute("role"));
    }
}
