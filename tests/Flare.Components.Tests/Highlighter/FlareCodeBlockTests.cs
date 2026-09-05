using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareCodeBlockTests : FlareTestContext
{
    [Fact]
    public void RendersRootWithValue()
    {
        var cut = Render<FlareCodeBlock>(p => p
            .Add(x => x.Value, "var x = 1;")
            .Add(x => x.Language, "csharp")
            .Add(x => x.ReadOnly, false));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Codeblock.Root}"));
        // Content, not a "value" attribute: a textarea has no such attribute in HTML, and the editor
        // deliberately stops rendering its own text after the first pass - assigning a textarea's
        // value moves the caret to the end, which made typing in the middle of a line jump.
        Assert.Equal("var x = 1;", cut.Find("textarea").TextContent);
    }
}
