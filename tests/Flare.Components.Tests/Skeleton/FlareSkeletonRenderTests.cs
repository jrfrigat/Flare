namespace Flare.Components.Tests;

/// <summary>
/// A skeleton used to skip a render whose Variant, Animation, Width and Height were unchanged, which
/// also swallowed every later change to Class, Style, unmatched attributes and Announce. These are the
/// sequential renders that used to leave the first markup in place: the unchanged render warms the
/// component's key, and the change that follows it is the one that used to be dropped.
/// </summary>
public class FlareSkeletonRenderTests : FlareTestContext
{
    private static Dictionary<string, object> Data(string value) => new() { ["data-state"] = value };

    [Fact]
    public void ChangedClass_ReachesTheDomAndKeepsTheGeometry()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Width, "120px")
            .Add(x => x.Class, "first"));
        cut.Render(p => p
            .Add(x => x.Width, "120px")
            .Add(x => x.Class, "first"));

        cut.Render(p => p
            .Add(x => x.Width, "120px")
            .Add(x => x.Class, "second"));

        var root = cut.Find($".{Css.Classes.Skeleton.Root}");
        Assert.Contains("second", root.ClassName);
        Assert.Contains("width:120px", root.GetAttribute("style") ?? "");
    }

    [Fact]
    public void ChangedStyle_ReachesTheDom()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Style, "opacity:.4"));
        cut.Render(p => p.Add(x => x.Style, "opacity:.4"));

        cut.Render(p => p.Add(x => x.Style, "opacity:.9"));

        Assert.Contains("opacity:.9", cut.Find($".{Css.Classes.Skeleton.Root}").GetAttribute("style") ?? "");
    }

    [Fact]
    public void ChangedAdditionalAttributes_ReachTheDom()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.AdditionalAttributes, Data("a")));
        cut.Render(p => p.Add(x => x.AdditionalAttributes, Data("a")));

        cut.Render(p => p.Add(x => x.AdditionalAttributes, Data("b")));

        Assert.Equal("b", cut.Find($".{Css.Classes.Skeleton.Root}").GetAttribute("data-state"));
    }

    [Fact]
    public void ChangedAnnounce_ReachesTheDom()
    {
        var cut = Render<FlareSkeleton>(p => p.Add(x => x.Announce, true));
        cut.Render(p => p.Add(x => x.Announce, true));
        var root = cut.Find($".{Css.Classes.Skeleton.Root}");
        Assert.Equal("status", root.GetAttribute("role"));
        Assert.Null(root.GetAttribute("aria-hidden"));

        cut.Render(p => p.Add(x => x.Announce, false));

        root = cut.Find($".{Css.Classes.Skeleton.Root}");
        Assert.Null(root.GetAttribute("role"));
        Assert.Equal("true", root.GetAttribute("aria-hidden"));
    }
}
