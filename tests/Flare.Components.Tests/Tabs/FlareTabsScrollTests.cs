using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTabsScrollTests : FlareTestContext
{
    [Fact]
    public void RendersBarWrapAroundBar()
    {
        var cut = Render<FlareTabs>();
        var wrap = cut.Find($".{Css.Classes.Tabs.BarWrap}");
        Assert.NotNull(wrap.QuerySelector($".{Css.Classes.Tabs.Bar}"));
    }

    [Fact]
    public void OnTabScrollState_ShowsArrowsWhenOverflowing()
    {
        var cut = Render<FlareTabs>();
        cut.InvokeAsync(() => cut.Instance.OnTabScrollState(overflowing: true, atStart: true, atEnd: false));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Tabs.Scroll}"));
        Assert.NotNull(cut.Find($".{Css.Classes.Tabs.ScrollPrev}").GetAttribute("disabled")); // atStart
    }
}
