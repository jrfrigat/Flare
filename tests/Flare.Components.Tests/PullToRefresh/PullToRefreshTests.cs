using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class PullToRefreshTests : FlareTestContext
{
    [Fact]
    public void RendersItsContentWithNoIndicatorAtRest()
    {
        var cut = Render<FlarePullToRefresh>(p => p.AddChildContent("<p>rows</p>"));

        Assert.NotNull(cut.Find("p"));
        Assert.Equal("true", cut.Find($".{Css.Classes.PullToRefresh.Indicator}").GetAttribute("aria-hidden"));
    }

    // A pull short of the threshold is an ordinary scroll and must not fire the refresh - the failure
    // mode this gesture has everywhere it goes wrong.
    [Fact]
    public void APullShortOfTheThresholdDoesNotRefresh()
    {
        var fired = 0;
        var cut = Render<FlarePullToRefresh>(p => p
            .Add(x => x.Threshold, 64)
            .Add(x => x.OnRefresh, EventCallback.Factory.Create(this, () => fired++)));

        var root = cut.Find($".{Css.Classes.PullToRefresh.Root}");
        root.PointerDown(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 0 });
        root.PointerMove(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 40 });
        root.PointerUp(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 40 });

        Assert.Equal(0, fired);
    }

    [Fact]
    public void DisabledIgnoresTheGestureWithoutUnwrappingTheContent()
    {
        var fired = 0;
        var cut = Render<FlarePullToRefresh>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnRefresh, EventCallback.Factory.Create(this, () => fired++))
            .AddChildContent("<p>rows</p>"));

        var root = cut.Find($".{Css.Classes.PullToRefresh.Root}");
        root.PointerDown(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 0 });
        root.PointerMove(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 400 });
        root.PointerUp(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 400 });

        Assert.Equal(0, fired);
        Assert.NotNull(cut.Find("p"));
    }
}
