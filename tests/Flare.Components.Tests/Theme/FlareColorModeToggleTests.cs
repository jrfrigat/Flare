using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareColorModeToggleTests : FlareTestContext
{
    [Fact]
    public void RendersRoot()
    {
        var cut = Render<FlareColorModeToggle>();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Color.ModeToggle}"));
    }
}
