namespace Flare.Components.Tests;

public class FlareTextAreaTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareTextArea>();

        // TextArea now renders the shared flare-input chrome; only the control keeps a textarea class.
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Root}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Textarea.Control}"));
    }

    [Fact]
    public void RendersTextareaElement()
    {
        var cut = Render<FlareTextArea>();

        Assert.NotEmpty(cut.FindAll("textarea"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Label, "Comments"));

        Assert.Contains("Comments", cut.Find($".{Css.Classes.Input.Label}").TextContent);
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Placeholder, "Enter text..."));

        Assert.Equal("Enter text...", cut.Find("textarea").GetAttribute("placeholder"));
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("textarea").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.HelperText, "Max 500 chars"));

        Assert.Contains("Max 500 chars", cut.Find($".{Css.Classes.Input.Helper}").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.ErrorText, "Field is required"));

        Assert.Contains("Field is required", cut.Find($".{Css.Classes.Input.HelperError}").TextContent);
    }

    [Fact]
    public void RendersRows()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Rows, 6));

        Assert.Equal("6", cut.Find("textarea").GetAttribute("rows"));
    }
}

// ------------------------------------------------------------------------------
// FlareNumericField  (9 tests from Wave4)
// ------------------------------------------------------------------------------
