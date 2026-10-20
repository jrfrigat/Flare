namespace Flare.Components.Tests;

/// <summary>
/// TASK-594: five date/time fields used to put the placeholder on aria-label even when a rich
/// LabelContent was shown, so a screen reader read "MM/dd/yyyy" instead of the visible label.
/// </summary>
public class RichLabelAriaTests : FlareTestContext
{
    private const string Label = "Start";

    [Fact]
    public void DateTimePicker_ReadsTheRichLabel()
    {
        var rich = Render<FlareDateTimePicker>(p => p.Add(x => x.Label, Label)
            .Add(x => x.LabelContent, b => b.AddContent(0, "Rich")));
        Assert.Null(rich.Find("input").GetAttribute("aria-label"));

        var plain = Render<FlareDateTimePicker>(p => p.Add(x => x.Label, Label));
        Assert.Equal(Label, plain.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void MonthPicker_ReadsTheRichLabel()
    {
        var rich = Render<FlareMonthPicker>(p => p.Add(x => x.Label, Label)
            .Add(x => x.LabelContent, b => b.AddContent(0, "Rich")));
        Assert.Null(rich.Find("input").GetAttribute("aria-label"));

        var plain = Render<FlareMonthPicker>(p => p.Add(x => x.Label, Label));
        Assert.Equal(Label, plain.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void WeekPicker_ReadsTheRichLabel()
    {
        var rich = Render<FlareWeekPicker>(p => p.Add(x => x.Label, Label)
            .Add(x => x.LabelContent, b => b.AddContent(0, "Rich")));
        Assert.Null(rich.Find("input").GetAttribute("aria-label"));

        var plain = Render<FlareWeekPicker>(p => p.Add(x => x.Label, Label));
        Assert.Equal(Label, plain.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void MultiDatePicker_ReadsTheRichLabel()
    {
        var rich = Render<FlareMultiDatePicker>(p => p.Add(x => x.Label, Label)
            .Add(x => x.LabelContent, b => b.AddContent(0, "Rich")));
        Assert.Null(rich.Find("input").GetAttribute("aria-label"));

        var plain = Render<FlareMultiDatePicker>(p => p.Add(x => x.Label, Label));
        Assert.Equal(Label, plain.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void TimePicker_ReadsTheRichLabel()
    {
        var rich = Render<FlareTimePicker>(p => p.Add(x => x.Label, Label)
            .Add(x => x.LabelContent, b => b.AddContent(0, "Rich")));
        Assert.Null(rich.Find("input").GetAttribute("aria-label"));

        var plain = Render<FlareTimePicker>(p => p.Add(x => x.Label, Label));
        Assert.Equal(Label, plain.Find("input").GetAttribute("aria-label"));
    }
}
