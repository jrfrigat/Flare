namespace Flare.Components.Tests;

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
    public void DecorativeByDefault_IsHiddenFromAssistiveTechnology()
    {
        var cut = Render<FlareSkeleton>();

        var el = cut.Find($".{Css.Classes.Skeleton.Root}");
        Assert.Equal("true", el.GetAttribute("aria-hidden"));
        Assert.Null(el.GetAttribute("role"));
        Assert.Null(el.GetAttribute("aria-busy"));
        Assert.Null(el.GetAttribute("aria-label"));
    }

    [Fact]
    public void Announce_BecomesTheSingleStatus()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Announce, true));

        var el = cut.Find($".{Css.Classes.Skeleton.Root}");
        Assert.Equal("status", el.GetAttribute("role"));
        Assert.Equal("true", el.GetAttribute("aria-busy"));
        Assert.False(string.IsNullOrEmpty(el.GetAttribute("aria-label")));
        Assert.Null(el.GetAttribute("aria-hidden"));
    }
}
