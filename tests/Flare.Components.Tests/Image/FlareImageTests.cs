using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareImageTests : FlareTestContext
{
    [Fact]
    public void RendersImgWithSrcAndAlt()
    {
        var cut = Render<FlareImage>(p => p
            .Add(x => x.Src, "/logo.png")
            .Add(x => x.Alt, "Logo"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Image.Root}"));
        var img = cut.Find("img");
        Assert.Equal("/logo.png", img.GetAttribute("src"));
        Assert.Equal("Logo", img.GetAttribute("alt"));
    }
}
