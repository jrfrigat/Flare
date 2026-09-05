using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareMediaQueryTests : FlareTestContext
{
    [Fact]
    public void RendersChildContentWithInitialBreakpoint()
    {
        var cut = Render<FlareMediaQuery>(p => p
            .Add(x => x.InitialBreakpoint, Breakpoint.Md)
            .Add(x => x.ChildContent, (RenderFragment<Breakpoint>)(bp => b => b.AddContent(0, $"bp:{bp}"))));
        Assert.Contains("bp:Md", cut.Markup);
    }
}
