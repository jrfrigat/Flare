using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareFileUploadZoneTests : FlareTestContext
{
    [Fact]
    public void RendersADropZone()
    {
        var cut = Render<FlareFileUploadZone>();

        Assert.NotEmpty(cut.FindAll($"label.{Css.Classes.FileUpload.Zone}"));
    }

    [Fact]
    public void TakesNoButtonVocabulary()
    {
        // A zone owns its footprint, so it must not carry the button classes the row-level button does.
        var cut = Render<FlareFileUploadZone>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Button.Root}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.FileUpload.Button}"));
    }

    [Fact]
    public void ShowsTheAcceptHint_WhenAcceptIsSet()
    {
        var cut = Render<FlareFileUploadZone>(p => p.Add(x => x.Accept, ".json"));

        Assert.Contains(".json", cut.Find($"span.{Css.Classes.FileUpload.Hint}").TextContent);
    }

    // --- absorbed from FlareDropZone ---

    [Fact]
    public void ChildContent_ReplacesTheWholeDefaultBody()
    {
        var cut = Render<FlareFileUploadZone>(p => p
            .Add(x => x.Accept, ".json")
            .AddChildContent("<span id=\"mine\">Drop an avatar</span>"));

        Assert.NotEmpty(cut.FindAll("#mine"));
        // The default icon, text and accept hint all give way - not just the text.
        Assert.DoesNotContain("upload_file", cut.Markup);
        Assert.Empty(cut.FindAll($"span.{Css.Classes.FileUpload.Hint}"));
    }

    [Fact]
    public void AriaLabel_IsApplied()
    {
        var cut = Render<FlareFileUploadZone>(p => p.Add(x => x.AriaLabel, "Avatar dropper"));

        Assert.Equal("Avatar dropper", cut.Find($"div.{Css.Classes.FileUpload.Root}").GetAttribute("aria-label"));
    }

    [Fact]
    public void MaxFileSize_IsUnlimitedByDefault()
    {
        // A silent cap discards the user's file with no explanation, so it must be opt-in. The old
        // FlareDropZone defaulted to 10MB and dropped anything larger without a word.
        var cut = Render<FlareFileUploadZone>();

        Assert.Equal(long.MaxValue, cut.Instance.MaxFileSize);
    }
}
