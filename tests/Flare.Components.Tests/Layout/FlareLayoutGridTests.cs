using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareLayout reserves a grid track for each registered in-flow drawer and exposes the column template
// via the --flare-layout-cols custom property, so the content is pushed aside rather than covered.
public class FlareLayoutGridTests : FlareTestContext
{
    [Fact]
    public void Layout_ReservesRailTrackForMiniDrawer()
    {
        var cut = Render<FlareLayout>(p => p
            .Add(x => x.Responsive, false)
            .AddChildContent<FlareLayoutDrawer>(d => d
                .Add(x => x.Variant, DrawerVariant.Mini)
                .Add(x => x.RailWidth, "5rem")
                .Add(x => x.Open, false)));

        var style = cut.Find($".{Css.Classes.Layout.Root}").GetAttribute("style") ?? string.Empty;
        Assert.Contains(Css.Tokens.LocalVars.LayoutCols, style);
        Assert.Contains("5rem", style);          // the collapsed rail reserves a 5rem track
        Assert.Contains("minmax(0, 1fr)", style); // the content fills the rest
    }
}
