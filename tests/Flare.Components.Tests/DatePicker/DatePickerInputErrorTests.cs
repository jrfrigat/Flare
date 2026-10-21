using System.Globalization;
using AngleSharp.Dom;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-223: a refused entry stays in the field with a localized reason and the aria wiring that points
/// at it, instead of being silently replaced by the last committed value.
/// </summary>
public class DatePickerInputErrorTests : FlareTestContext
{
    private IRenderedComponent<FlareDatePicker> RenderPicker(
        List<DateOnly?> committed, DateOnly? value = null, DateOnly? min = null, DateOnly? max = null)
        => Render<FlareDatePicker>(p =>
        {
            p.Add(x => x.Culture, new CultureInfo("en-US"));
            if (value is not null) p.Add(x => x.Value, value);
            if (min is not null) p.Add(x => x.Min, min);
            if (max is not null) p.Add(x => x.Max, max);
            p.Add(x => x.ValueChanged, (DateOnly? v) => committed.Add(v));
        });

    private static string ErrorText(IElement input, IRenderedComponent<FlareDatePicker> cut)
    {
        var describedBy = input.GetAttribute("aria-describedby");
        Assert.False(string.IsNullOrEmpty(describedBy));
        return cut.Find("#" + describedBy).TextContent;
    }

    [Fact]
    public void ImpossibleDate_StaysInTheFieldWithAReason()
    {
        var committed = new List<DateOnly?>();
        var cut = RenderPicker(committed, new DateOnly(2026, 10, 18));

        cut.Find("input").Input("02/31/2026");
        cut.Find("input").Change("02/31/2026");

        var input = cut.Find("input");
        Assert.Empty(committed);
        Assert.Equal("02/31/2026", input.GetAttribute("value"));
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Contains("valid date", ErrorText(input, cut));
    }

    [Fact]
    public void IncompleteDate_AsksForTheFullDate()
    {
        var committed = new List<DateOnly?>();
        var cut = RenderPicker(committed, new DateOnly(2026, 10, 18));

        cut.Find("input").Input("10/2");
        cut.Find("input").Change("10/2");

        var input = cut.Find("input");
        Assert.Empty(committed);
        Assert.Equal("10/2", input.GetAttribute("value"));
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Contains("in full", ErrorText(input, cut));
    }

    [Fact]
    public void UnavailableDate_SaysSoAndKeepsTheValue()
    {
        var committed = new List<DateOnly?>();
        var cut = RenderPicker(committed, new DateOnly(2026, 10, 18),
            min: new DateOnly(2026, 10, 1), max: new DateOnly(2026, 10, 31));

        cut.Find("input").Input("11/05/2026");
        cut.Find("input").Change("11/05/2026");

        var input = cut.Find("input");
        Assert.Empty(committed);
        Assert.Equal("11/05/2026", input.GetAttribute("value"));
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Contains("not available", ErrorText(input, cut));
    }

    [Fact]
    public void CorrectingTheEntry_ClearsTheErrorAndCommits()
    {
        var committed = new List<DateOnly?>();
        var cut = RenderPicker(committed, new DateOnly(2026, 10, 18));

        cut.Find("input").Input("02/31/2026");
        cut.Find("input").Change("02/31/2026");
        cut.Find("input").Input("10/20/2026");

        var input = cut.Find("input");
        Assert.Equal([new DateOnly(2026, 10, 20)], committed);
        Assert.Null(input.GetAttribute("aria-invalid"));
        Assert.Null(input.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void ClearingTheEntry_ClearsTheError()
    {
        var committed = new List<DateOnly?>();
        var cut = RenderPicker(committed, new DateOnly(2026, 10, 18));

        cut.Find("input").Input("02/31/2026");
        cut.Find("input").Change("02/31/2026");
        cut.Find("input").Change(string.Empty);

        var input = cut.Find("input");
        Assert.Equal([null], committed);
        Assert.Null(input.GetAttribute("aria-invalid"));
    }

    [Fact]
    public void ExternalValue_ReplacesTheRefusedEntry()
    {
        var committed = new List<DateOnly?>();
        var cut = RenderPicker(committed, new DateOnly(2026, 10, 18));

        cut.Find("input").Input("02/31/2026");
        cut.Find("input").Change("02/31/2026");

        cut.Render(p =>
        {
            p.Add(x => x.Culture, new CultureInfo("en-US"));
            p.Add(x => x.Value, new DateOnly(2026, 11, 1));
            p.Add(x => x.ValueChanged, (DateOnly? v) => committed.Add(v));
        });

        var input = cut.Find("input");
        Assert.Null(input.GetAttribute("aria-invalid"));
        Assert.Equal("11/1/2026", input.GetAttribute("value"));
    }
}
