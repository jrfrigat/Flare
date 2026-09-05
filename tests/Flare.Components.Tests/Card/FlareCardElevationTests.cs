using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareCard numeric Elevation
// ------------------------------------------------------------------------------
public class FlareCardElevationTests : FlareTestContext
{
    [Fact]
    public void Elevation_EmitsInlineElevationVariable()
    {
        var cut = Render<FlareCard>(p => p.Add(x => x.Elevation, 3));

        // Driven through the --flare-card-elevation variable (not the final box-shadow) so the
        // clickable :hover lift rule can still override the resting shadow.
        Assert.Contains($"{Css.Tokens.CardField.Elevation}:var({Css.Tokens.Elevation.Level3})", cut.Find($".{Css.Classes.Card.Root}").GetAttribute("style"));
    }

    [Fact]
    public void ElevationZero_IsFlat()
    {
        var cut = Render<FlareCard>(p => p.Add(x => x.Elevation, 0));

        Assert.Contains($"{Css.Tokens.CardField.Elevation}:var({Css.Tokens.Elevation.Level0})", cut.Find($".{Css.Classes.Card.Root}").GetAttribute("style"));
    }

    [Fact]
    public void Elevation_ClampedToScale()
    {
        var cut = Render<FlareCard>(p => p.Add(x => x.Elevation, 99));

        Assert.Contains($"{Css.Tokens.CardField.Elevation}:var({Css.Tokens.Elevation.Level5})", cut.Find($".{Css.Classes.Card.Root}").GetAttribute("style"));
    }
}
