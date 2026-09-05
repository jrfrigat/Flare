using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareEmptyStateCompactTests : FlareTestContext
{
    [Fact]
    public void Compact_AddsModifier()
    {
        var cut = Render<FlareEmptyState>(p => p.Add(x => x.Compact, true).Add(x => x.Title, "Empty"));
        Assert.Contains(Css.Classes.Empty.StateCompact, cut.Find($".{Css.Classes.Empty.State}").ClassName);
    }

    [Fact]
    public void Default_HasNoCompactModifier()
    {
        var cut = Render<FlareEmptyState>(p => p.Add(x => x.Title, "Empty"));
        Assert.DoesNotContain(Css.Classes.Empty.StateCompact, cut.Find($".{Css.Classes.Empty.State}").ClassName);
    }
}
