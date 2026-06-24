// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareDatePicker  (10 tests)
// ------------------------------------------------------------------------------

public class FlareDatePickerTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareDatePicker>();

        Assert.NotEmpty(cut.FindAll(".flare-datepicker"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Label, "Select date"));

        var label = cut.Find("label.flare-datepicker__label");
        Assert.Equal("Select date", label.TextContent);
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Placeholder, "YYYY-MM-DD"));

        Assert.Equal("YYYY-MM-DD", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void RendersCalendarIcon()
    {
        var cut = Render<FlareDatePicker>();

        var icon = cut.Find(".flare-input__icon--trailing");
        Assert.NotNull(icon);
    }

    [Fact]
    public void DoesNotShowCalendar_Initially()
    {
        var cut = Render<FlareDatePicker>();

        Assert.Empty(cut.FindAll(".flare-datepicker__panel"));
    }

    [Fact]
    public void RendersDisabledInput()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.ErrorText, "Date is required"));

        Assert.Contains("Date is required", cut.Find(".flare-datepicker__helper--error").TextContent);
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.HelperText, "Pick any date"));

        Assert.Contains("Pick any date", cut.Find(".flare-datepicker__helper").TextContent);
    }

    [Fact]
    public void RendersFormattedValue()
    {
        var date = new DateOnly(2026, 5, 23);
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Value, date)
            .Add(x => x.DateFormat, "yyyy-MM-dd"));

        Assert.Equal("2026-05-23", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void OpensCalendarOnToggleClick()
    {
        var cut = Render<FlareDatePicker>();

        cut.Find(".flare-datepicker__toggle").Click();

        Assert.NotEmpty(cut.FindAll(".flare-datepicker__panel"));
    }

    [Fact]
    public void TypingDateUpdatesValue()
    {
        DateOnly? value = null;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.DateFormat, "yyyy-MM-dd")
            .Add(x => x.ValueChanged, (DateOnly? v) => value = v));

        cut.Find("input").Change("2026-07-15");

        Assert.Equal(new DateOnly(2026, 7, 15), value);
    }
}
