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
        Assert.NotEmpty(cut.FindAll(".flare-skeleton"));
    }

    [Fact]
    public void DefaultVariantIsRect()
    {
        var cut = Render<FlareSkeleton>();
        Assert.Contains("flare-skeleton--rect", cut.Find(".flare-skeleton").ClassName);
    }

    [Fact]
    public void TextVariantAppliesClass()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Variant, SkeletonVariant.Text));
        Assert.Contains("flare-skeleton--text", cut.Find(".flare-skeleton").ClassName);
    }

    [Fact]
    public void CircleVariantAppliesClass()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Variant, SkeletonVariant.Circle));
        Assert.Contains("flare-skeleton--circle", cut.Find(".flare-skeleton").ClassName);
    }

    [Fact]
    public void WaveAnimationIsDefault()
    {
        var cut = Render<FlareSkeleton>();
        Assert.Contains("flare-skeleton--wave", cut.Find(".flare-skeleton").ClassName);
    }

    [Fact]
    public void PulseAnimationAppliesClass()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Animation, SkeletonAnimation.Pulse));
        Assert.Contains("flare-skeleton--pulse", cut.Find(".flare-skeleton").ClassName);
    }

    [Fact]
    public void WidthAppliedInlineStyle()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Width, "120px"));
        Assert.Contains("width:120px", cut.Find(".flare-skeleton").GetAttribute("style") ?? "");
    }

    [Fact]
    public void HeightAppliedInlineStyle()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Height, "40px"));
        Assert.Contains("height:40px", cut.Find(".flare-skeleton").GetAttribute("style") ?? "");
    }
}
