using Flare.Components;
using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class ColXxlTests : FlareTestContext
{
    [Fact]
    public void Xxl_Span_EmitsCssVariable()
    {
        var cut = Render<FlareCol>(p => p.Add(x => x.Xxl, 4).AddChildContent("<i>x</i>"));
        Assert.Contains($"{Css.Tokens.LocalVars.ColSpanXxl}:4", cut.Find($".{Css.Classes.Col.Root}").GetAttribute("style"));
    }
}
