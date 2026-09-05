// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareDateTimePicker  (8 tests)
// -----------------------------------------------------------------------------

public class FlareDateTimePickerTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareDateTimePicker>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DateTimePicker.Root}"));
    }

    [Fact]
    public void RendersLabel_WhenProvided()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Label, "Pick a date"));

        Assert.Contains("Pick a date", cut.Find($".{Css.Classes.Input.Label}").TextContent);
    }

    [Fact]
    public void NoLabel_NoLabelElement()
    {
        var cut = Render<FlareDateTimePicker>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Input.Label}"));
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Placeholder, "yyyy-MM-dd HH:mm"));

        Assert.Equal("yyyy-MM-dd HH:mm", cut.Find($".{Css.Classes.DateTimePicker.Root} .{Css.Classes.Input.Control}").GetAttribute("placeholder"));
    }

    [Fact]
    public void RendersDisabledInput()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find($".{Css.Classes.DateTimePicker.Root} .{Css.Classes.Input.Control}").HasAttribute("disabled"));
    }

    [Fact]
    public void PanelNotOpenInitially()
    {
        var cut = Render<FlareDateTimePicker>();

        Assert.Empty(cut.FindAll($".{Css.Classes.DateTimePicker.Panel}"));
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.HelperText, "Select date and time"));

        Assert.Contains("Select date and time", cut.Find($".{Css.Classes.Input.Helper}").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.ErrorText, "Date is required"));

        Assert.Contains("Date is required", cut.Find($".{Css.Classes.Input.HelperError}").TextContent);
    }
}
