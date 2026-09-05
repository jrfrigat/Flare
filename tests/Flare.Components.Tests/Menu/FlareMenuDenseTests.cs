using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareMenuDenseTests : FlareTestContext
{
    [Fact]
    public void Dense_AddsDenseModifier()
    {
        var cut = Render<FlareMenu>(p => p.Add(x => x.Dense, true));
        Assert.Contains(Css.Classes.Menu.Dense, cut.Find($".{Css.Classes.Menu.Root}").ClassName);
    }

    [Fact]
    public void Default_HasNoDenseModifier()
    {
        var cut = Render<FlareMenu>();
        Assert.DoesNotContain(Css.Classes.Menu.Dense, cut.Find($".{Css.Classes.Menu.Root}").ClassName);
    }
}
