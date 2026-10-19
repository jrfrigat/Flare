using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareSelectAriaLabelContentTests : FlareTestContext
{
    private static readonly RenderFragment RichLabel = b => b.AddMarkupContent(0, "<span>Plan</span>");

    [Fact]
    public void SearchableSelect_LabelContentNamesTheInput()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Searchable, true)
            .Add(x => x.LabelContent, RichLabel));

        var input = cut.Find($".{Css.Classes.Select.Search}");
        var labelId = input.GetAttribute("aria-labelledby");
        Assert.False(string.IsNullOrEmpty(labelId));
        Assert.NotNull(cut.Find($"#{labelId}"));
    }

    [Fact]
    public void SelectOnlyLabelContentNamesTheTrigger()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.LabelContent, RichLabel));

        var control = cut.Find($".{Css.Classes.Select.Control}");
        var labelId = control.GetAttribute("aria-labelledby");
        Assert.False(string.IsNullOrEmpty(labelId));
        Assert.NotNull(cut.Find($"#{labelId}"));
    }

    [Fact]
    public void SearchableSelect_ErrorMarksTheInputInvalid()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Searchable, true)
            .Add(x => x.ErrorText, "Required"));

        Assert.Equal("true", cut.Find($".{Css.Classes.Select.Search}").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void SelectOnlyErrorMarksTheTriggerInvalid()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.ErrorText, "Required"));

        Assert.Equal("true", cut.Find($".{Css.Classes.Select.Control}").GetAttribute("aria-invalid"));
    }
}
