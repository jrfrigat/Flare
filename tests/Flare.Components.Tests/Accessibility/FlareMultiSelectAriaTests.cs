using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Accessibility;

public class FlareMultiSelectAriaTests : FlareTestContext
{
    private static readonly RenderFragment RichLabel = b => b.AddMarkupContent(0, "<span>Topics</span>");

    [Fact]
    public void SearchableMultiSelect_LabelContentNamesTheInput()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Searchable, true)
            .Add(x => x.LabelContent, RichLabel));

        var input = cut.Find($".{Css.Classes.Multiselect.Control} input");
        var labelId = input.GetAttribute("aria-labelledby");
        Assert.False(string.IsNullOrEmpty(labelId));
        Assert.NotNull(cut.Find($"#{labelId}"));
    }

    [Fact]
    public void MultiSelectOnlyLabelContentNamesTheTrigger()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.LabelContent, RichLabel));

        var control = cut.Find($".{Css.Classes.Multiselect.Control}");
        var labelId = control.GetAttribute("aria-labelledby");
        Assert.False(string.IsNullOrEmpty(labelId));
        Assert.NotNull(cut.Find($"#{labelId}"));
    }

    [Fact]
    public void SearchableMultiSelect_ErrorMarksTheInputInvalid()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Searchable, true)
            .Add(x => x.ErrorText, "Required"));

        Assert.Equal("true", cut.Find($".{Css.Classes.Multiselect.Control} input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void MultiSelectOnlyErrorMarksTheTriggerInvalid()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.ErrorText, "Required"));

        Assert.Equal("true", cut.Find($".{Css.Classes.Multiselect.Control}").GetAttribute("aria-invalid"));
    }
}
