using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class TagFieldAriaTests : FlareTestContext
{
    private static readonly RenderFragment RichLabel = b => b.AddMarkupContent(0, "<span>Tags</span>");

    private static void AssertNamedByItsLabel(IRenderedComponent<FlareTagField<string>> cut)
    {
        var input = cut.Find("input");
        var labelId = input.GetAttribute("aria-labelledby");
        Assert.False(string.IsNullOrEmpty(labelId));
        Assert.NotNull(cut.Find($"#{labelId}"));
    }

    [Fact]
    public void LabelContent_NamesTheInput()
    {
        var cut = Render<FlareTagField<string>>(p => p.Add(x => x.LabelContent, RichLabel));
        AssertNamedByItsLabel(cut);
    }

    [Fact]
    public void TextLabel_NamesTheInput()
    {
        var cut = Render<FlareTagField<string>>(p => p.Add(x => x.Label, "Tags"));
        AssertNamedByItsLabel(cut);
    }

    [Fact]
    public void ErrorText_MarksTheInputInvalid()
    {
        var cut = Render<FlareTagField<string>>(p => p.Add(x => x.ErrorText, "Invalid"));

        Assert.Equal("true", cut.Find("input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void NoLabel_LeavesTheInputUnlabelled()
    {
        var cut = Render<FlareTagField<string>>();

        Assert.False(cut.Find("input").HasAttribute("aria-labelledby"));
    }
}
