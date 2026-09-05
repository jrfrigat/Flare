using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareMenuGroupTests : FlareTestContext
{
    [Fact]
    public void RendersLabelAndChildren()
    {
        var cut = Render<FlareMenuGroup>(p => p
            .Add(x => x.Label, "Section")
            .AddChildContent("<li class=\"item\">a</li>"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Group}"));
        Assert.Contains("Section", cut.Markup);
        Assert.NotEmpty(cut.FindAll(".item"));
    }
}
