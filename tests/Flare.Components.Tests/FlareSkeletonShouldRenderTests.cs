// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// Wave 10 - ShouldRender optimizations, FlareFormField, FormLayout, FloatingLabel
// -----------------------------------------------------------------------------

// FlareSkeleton ShouldRender (8 tests)
public class FlareSkeletonShouldRenderTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareSkeleton>();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Skeleton.Root}"));
    }

    [Fact]
    public void DefaultVariantIsRect()
    {
        var cut = Render<FlareSkeleton>();
        Assert.Contains(Css.Classes.Skeleton.Rect, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void TextVariantAppliesClass()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Variant, SkeletonVariant.Text));
        Assert.Contains(Css.Classes.Skeleton.Text, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void CircleVariantAppliesClass()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Variant, SkeletonVariant.Circle));
        Assert.Contains(Css.Classes.Skeleton.Circle, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void WaveAnimationIsDefault()
    {
        var cut = Render<FlareSkeleton>();
        Assert.Contains(Css.Classes.Skeleton.Wave, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void PulseAnimationAppliesClass()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Animation, SkeletonAnimation.Pulse));
        Assert.Contains(Css.Classes.Skeleton.Pulse, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void WidthAppliedInlineStyle()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Width, "120px"));
        Assert.Contains("width:120px", cut.Find($".{Css.Classes.Skeleton.Root}").GetAttribute("style") ?? "");
    }

    [Fact]
    public void HeightAppliedInlineStyle()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Height, "40px"));
        Assert.Contains("height:40px", cut.Find($".{Css.Classes.Skeleton.Root}").GetAttribute("style") ?? "");
    }
}
