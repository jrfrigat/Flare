using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareFieldCharacterCountTests : FlareTestContext
{
    [Fact]
    public void ShowCharacterCount_WithMaxLength_RendersCurrentOverMax()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.Value, "abc")
            .Add(x => x.MaxLength, 10)
            .Add(x => x.ShowCharacterCount, true));
        Assert.Equal("3/10", cut.Find($".{Css.Classes.Input.Counter}").TextContent);
    }

    [Fact]
    public void NoCounter_ByDefault()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Value, "abc"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Input.Counter}"));
    }
}
