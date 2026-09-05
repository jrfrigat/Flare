using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareFileUploadButtonTests : FlareTestContext
{
    [Fact]
    public void TriggerIsALabel_SoItOpensThePicker()
    {
        var cut = Render<FlareFileUploadButton>();

        // A <button> here would silently stop opening the file dialog - the label/for pairing is load-bearing.
        var label = cut.Find($"label.{Css.Classes.FileUpload.ButtonTrigger}");
        var input = cut.Find("input[type=file]");
        Assert.Equal(input.Id, label.GetAttribute("for"));
    }

    [Fact]
    public void RootCarriesTheButtonModifier_SoTheInputIsNotAnOverlay()
    {
        // The zone overlays its label with the input - that overlay is what gives it drag-and-drop. The
        // button must NOT: while it did, the pointer landed on the input instead of the label, so hover,
        // press and the whole state layer were unreachable and the button looked dead next to a FlareButton.
        // The CSS that lifts the overlay keys off this modifier, so its absence is the bug coming back.
        var cut = Render<FlareFileUploadButton>();

        Assert.Contains(Css.Classes.FileUpload.Button, cut.Find($"div.{Css.Classes.FileUpload.Root}").ClassList);
    }

    [Fact]
    public void WearsTheRealButtonClasses()
    {
        var cut = Render<FlareFileUploadButton>();

        Assert.NotEmpty(cut.FindAll($"label.{Css.Classes.FileUpload.ButtonTrigger}.{Css.Classes.Button.Root}"));
    }

    [Theory]
    [InlineData(ButtonVariant.Filled, Css.Classes.Button.Filled)]
    [InlineData(ButtonVariant.Outlined, Css.Classes.Button.Outlined)]
    [InlineData(ButtonVariant.Text, Css.Classes.Button.Text)]
    [InlineData(ButtonVariant.Tonal, Css.Classes.Button.Tonal)]
    [InlineData(ButtonVariant.Elevated, Css.Classes.Button.Elevated)]
    public void VariantMapsToTheButtonFamilysClass(ButtonVariant variant, string expected)
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Variant, variant));

        Assert.Contains(expected, cut.Find($"label.{Css.Classes.FileUpload.ButtonTrigger}").ClassList);
    }

    [Theory]
    [InlineData(ButtonSize.Xs, Css.Classes.Button.Xs)]
    [InlineData(ButtonSize.Sm, Css.Classes.Button.Sm)]
    [InlineData(ButtonSize.Md, Css.Classes.Button.Md)]
    [InlineData(ButtonSize.Lg, Css.Classes.Button.Lg)]
    [InlineData(ButtonSize.Xl, Css.Classes.Button.Xl)]
    public void SizeMapsToTheButtonFamilysClass(ButtonSize size, string expected)
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Size, size));

        Assert.Contains(expected, cut.Find($"label.{Css.Classes.FileUpload.ButtonTrigger}").ClassList);
    }

    [Fact]
    public void SizeClassMatchesFlareButtonsForTheSameSize()
    {
        // The point of the shared ButtonCssClasses map: same input, same class, both components.
        var upload = Render<FlareFileUploadButton>(p => p.Add(x => x.Size, ButtonSize.Sm));
        var button = Render<FlareButton>(p => p.Add(x => x.Size, ButtonSize.Sm));

        var uploadSize = upload.Find($"label.{Css.Classes.FileUpload.ButtonTrigger}").ClassList.Single(c => c.StartsWith("flare-btn--") && c.Contains("sm"));
        var buttonSize = button.Find($"button.{Css.Classes.Button.Root}").ClassList.Single(c => c.StartsWith("flare-btn--") && c.Contains("sm"));
        Assert.Equal(buttonSize, uploadSize);
    }

    [Fact]
    public void LoadingBlocksThePickerAndMarksTheButton()
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Loading, true));

        Assert.Contains(Css.Classes.Button.Loading, cut.Find($"label.{Css.Classes.FileUpload.ButtonTrigger}").ClassList);
        Assert.True(cut.Find("input[type=file]").HasAttribute("disabled"));
    }

    [Fact]
    public void DisabledBlocksThePicker()
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Disabled, true));

        Assert.True(cut.Find("input[type=file]").HasAttribute("disabled"));
    }

    [Fact]
    public void LeadingIconReplacesTheDefaultGlyph()
    {
        var cut = Render<FlareFileUploadButton>(p => p
            .Add<RenderFragment>(x => x.LeadingIcon!, b => b.AddMarkupContent(0, "<span id=\"my-icon\"></span>")));

        Assert.NotEmpty(cut.FindAll("#my-icon"));
        Assert.DoesNotContain("upload_file", cut.Markup);
    }

    [Fact]
    public void TextSetsTheLabel()
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Text, "Import"));

        Assert.Contains("Import", cut.Find($"span.{Css.Classes.Button.Label}").TextContent);
    }

    [Fact]
    public void ShowFileList_False_HidesTheList()
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.ShowFileList, false));

        Assert.Empty(cut.FindAll($"ul.{Css.Classes.FileUpload.List}"));
    }

    [Fact]
    public void Typo_OverridesTheLabelScale_TheSameWayFlareButtonDoes()
    {
        var upload = Render<FlareFileUploadButton>(p => p
            .Add(x => x.Typo, TypographyScale.TitleLarge).AddChildContent("Import"));
        var button = Render<FlareButton>(p => p
            .Add(x => x.Typo, TypographyScale.TitleLarge).AddChildContent("Import"));

        var uploadLabel = upload.Find($"span.{Css.Classes.Button.Label}").ClassList.Single(c => c.StartsWith("flare-text--"));
        var buttonLabel = button.Find($"span.{Css.Classes.Button.Label}").ClassList.Single(c => c.StartsWith("flare-text--"));
        Assert.Equal(buttonLabel, uploadLabel);
    }

    [Fact]
    public void LoadingTemplate_ReplacesTheSpinnerAndLabel()
    {
        var cut = Render<FlareFileUploadButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.Text, "Import")
            .Add<RenderFragment>(x => x.LoadingTemplate!, b => b.AddMarkupContent(0, "<span id=\"mine\">Reading...</span>")));

        Assert.NotEmpty(cut.FindAll("#mine"));
        Assert.Empty(cut.FindAll($"span.{Css.Classes.Button.Spinner}"));
        Assert.DoesNotContain("Import", cut.Markup);
    }

    [Fact]
    public void CustomColor_TakesTheSameInlineTokensFlareButtonDoes()
    {
        // It used to set --fc-main by hand, which skipped the Dynamic palette and the filled label's
        // auto-contrast. Both components must resolve a custom colour through the same path.
        var upload = Render<FlareFileUploadButton>(p => p.Add(x => x.Color, FlareColor.Custom("#FF0000")));
        var button = Render<FlareButton>(p => p.Add(x => x.Color, FlareColor.Custom("#FF0000")));

        var uploadStyle = upload.Find($"label.{Css.Classes.FileUpload.ButtonTrigger}").GetAttribute("style");
        var buttonStyle = button.Find($"button.{Css.Classes.Button.Root}").GetAttribute("style");
        Assert.Equal(buttonStyle, uploadStyle);
    }
}
