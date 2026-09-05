using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTableDenseTests : FlareTestContext
{
    [Fact]
    public void Dense_AppliesDenseModifier()
    {
        var cut = Render<FlareTable<int>>(p => p.Add(x => x.Dense, true));
        Assert.Contains(Css.Classes.Table.Dense, cut.Find($".{Css.Classes.Table.Root}").ClassName);
    }

    [Fact]
    public void Default_HasNoDenseModifier()
    {
        var cut = Render<FlareTable<int>>();
        Assert.DoesNotContain(Css.Classes.Table.Dense, cut.Find($".{Css.Classes.Table.Root}").ClassName);
    }
}
