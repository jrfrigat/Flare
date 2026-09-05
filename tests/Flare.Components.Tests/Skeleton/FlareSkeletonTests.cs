using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareSkeletonTests : FlareTestContext
{

    [Fact]
    public void VariantRect_HasRectClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Variant, SkeletonVariant.Rect));

        Assert.Contains(Css.Classes.Skeleton.Rect, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void VariantCircle_HasCircleClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Variant, SkeletonVariant.Circle));

        Assert.Contains(Css.Classes.Skeleton.Circle, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void VariantText_HasTextClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Variant, SkeletonVariant.Text));

        Assert.Contains(Css.Classes.Skeleton.Text, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void AnimationWave_HasWaveClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Animation, SkeletonAnimation.Wave));

        Assert.Contains(Css.Classes.Skeleton.Wave, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void WidthAndHeightAppliedAsStyle()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Width, "200px")
            .Add(x => x.Height, "50px"));

        var style = cut.Find($".{Css.Classes.Skeleton.Root}").GetAttribute("style") ?? string.Empty;
        Assert.Contains("200px", style);
        Assert.Contains("50px", style);
    }
}

// ------------------------------------------------------------------------------
// FlareOverlay  (7 tests from Wave5)
// ------------------------------------------------------------------------------
