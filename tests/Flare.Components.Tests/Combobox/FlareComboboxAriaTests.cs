using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareComboboxAriaTests : FlareTestContext
{
    private static readonly RenderFragment RichLabel = b => b.AddMarkupContent(0, "<span>City</span>");

    [Fact]
    public void LabelContent_NamesTheInput()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.LabelContent, RichLabel));

        var input = cut.Find("input");
        var labelId = input.GetAttribute("aria-labelledby");
        Assert.False(string.IsNullOrEmpty(labelId));
        Assert.NotNull(cut.Find($"#{labelId}"));
    }
}
