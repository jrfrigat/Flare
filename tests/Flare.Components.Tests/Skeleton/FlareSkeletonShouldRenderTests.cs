namespace Flare.Components.Tests;

// FlareSkeleton ShouldRender (4 tests)
public class FlareSkeletonShouldRenderTests : FlareTestContext
{
    [Fact]
    public void DefaultVariantIsRect()
    {
        var cut = Render<FlareSkeleton>();
        Assert.Contains(Css.Classes.Skeleton.Rect, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void WaveAnimationIsDefault()
    {
        var cut = Render<FlareSkeleton>();
        Assert.Contains(Css.Classes.Skeleton.Wave, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
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
