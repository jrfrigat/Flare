using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareFileUploadButton / FlareFileUploadZone
//
// The button's whole reason to exist is that it wears the SAME classes FlareButton does, so it lines up
// with the buttons beside it in a toolbar. It cannot nest a FlareButton (a <button> inside its <label for>
// would not open the picker), so nothing but a test keeps the two from drifting apart.
// ------------------------------------------------------------------------------

public class C_FlareFileUploadButtonTests : FlareTestContext
{
    [Fact]
    public void TriggerIsALabel_SoItOpensThePicker()
    {
        var cut = Render<FlareFileUploadButton>();

        // A <button> here would silently stop opening the file dialog - the label/for pairing is load-bearing.
        var label = cut.Find("label.flare-file-upload__button");
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

        Assert.Contains("flare-file-upload--button", cut.Find("div.flare-file-upload").ClassList);
    }

    [Fact]
    public void WearsTheRealButtonClasses()
    {
        var cut = Render<FlareFileUploadButton>();

        Assert.NotEmpty(cut.FindAll("label.flare-file-upload__button.flare-btn"));
    }

    [Theory]
    [InlineData(ButtonVariant.Filled, "flare-btn--filled")]
    [InlineData(ButtonVariant.Outlined, "flare-btn--outlined")]
    [InlineData(ButtonVariant.Text, "flare-btn--text")]
    [InlineData(ButtonVariant.Tonal, "flare-btn--tonal")]
    [InlineData(ButtonVariant.Elevated, "flare-btn--elevated")]
    public void VariantMapsToTheButtonFamilysClass(ButtonVariant variant, string expected)
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Variant, variant));

        Assert.Contains(expected, cut.Find("label.flare-file-upload__button").ClassList);
    }

    [Theory]
    [InlineData(ButtonSize.Xs, "flare-btn--xs")]
    [InlineData(ButtonSize.Sm, "flare-btn--sm")]
    [InlineData(ButtonSize.Md, "flare-btn--md")]
    [InlineData(ButtonSize.Lg, "flare-btn--lg")]
    [InlineData(ButtonSize.Xl, "flare-btn--xl")]
    public void SizeMapsToTheButtonFamilysClass(ButtonSize size, string expected)
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Size, size));

        Assert.Contains(expected, cut.Find("label.flare-file-upload__button").ClassList);
    }

    [Fact]
    public void SizeClassMatchesFlareButtonsForTheSameSize()
    {
        // The point of the shared ButtonCssClasses map: same input, same class, both components.
        var upload = Render<FlareFileUploadButton>(p => p.Add(x => x.Size, ButtonSize.Sm));
        var button = Render<FlareButton>(p => p.Add(x => x.Size, ButtonSize.Sm));

        var uploadSize = upload.Find("label.flare-file-upload__button").ClassList.Single(c => c.StartsWith("flare-btn--") && c.Contains("sm"));
        var buttonSize = button.Find("button.flare-btn").ClassList.Single(c => c.StartsWith("flare-btn--") && c.Contains("sm"));
        Assert.Equal(buttonSize, uploadSize);
    }

    [Fact]
    public void LoadingBlocksThePickerAndMarksTheButton()
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Loading, true));

        Assert.Contains("flare-btn--loading", cut.Find("label.flare-file-upload__button").ClassList);
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
            .Add<RenderFragment>(x => x.LeadingIcon, b => b.AddMarkupContent(0, "<span id=\"my-icon\"></span>")));

        Assert.NotEmpty(cut.FindAll("#my-icon"));
        Assert.DoesNotContain("upload_file", cut.Markup);
    }

    [Fact]
    public void TextSetsTheLabel()
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.Text, "Import"));

        Assert.Contains("Import", cut.Find("span.flare-btn__label").TextContent);
    }

    [Fact]
    public void ShowFileList_False_HidesTheList()
    {
        var cut = Render<FlareFileUploadButton>(p => p.Add(x => x.ShowFileList, false));

        Assert.Empty(cut.FindAll("ul.flare-file-upload__list"));
    }

    [Fact]
    public void Typo_OverridesTheLabelScale_TheSameWayFlareButtonDoes()
    {
        var upload = Render<FlareFileUploadButton>(p => p
            .Add(x => x.Typo, TypographyScale.TitleLarge).AddChildContent("Import"));
        var button = Render<FlareButton>(p => p
            .Add(x => x.Typo, TypographyScale.TitleLarge).AddChildContent("Import"));

        var uploadLabel = upload.Find("span.flare-btn__label").ClassList.Single(c => c.StartsWith("flare-text--"));
        var buttonLabel = button.Find("span.flare-btn__label").ClassList.Single(c => c.StartsWith("flare-text--"));
        Assert.Equal(buttonLabel, uploadLabel);
    }

    [Fact]
    public void LoadingTemplate_ReplacesTheSpinnerAndLabel()
    {
        var cut = Render<FlareFileUploadButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.Text, "Import")
            .Add<RenderFragment>(x => x.LoadingTemplate, b => b.AddMarkupContent(0, "<span id=\"mine\">Reading...</span>")));

        Assert.NotEmpty(cut.FindAll("#mine"));
        Assert.Empty(cut.FindAll("span.flare-btn__spinner"));
        Assert.DoesNotContain("Import", cut.Markup);
    }

    [Fact]
    public void CustomColor_TakesTheSameInlineTokensFlareButtonDoes()
    {
        // It used to set --fc-main by hand, which skipped the Dynamic palette and the filled label's
        // auto-contrast. Both components must resolve a custom colour through the same path.
        var upload = Render<FlareFileUploadButton>(p => p.Add(x => x.Color, FlareColor.Custom("#FF0000")));
        var button = Render<FlareButton>(p => p.Add(x => x.Color, FlareColor.Custom("#FF0000")));

        var uploadStyle = upload.Find("label.flare-file-upload__button").GetAttribute("style");
        var buttonStyle = button.Find("button.flare-btn").GetAttribute("style");
        Assert.Equal(buttonStyle, uploadStyle);
    }
}

public class C_FlareFileUploadZoneTests : FlareTestContext
{
    [Fact]
    public void RendersADropZone()
    {
        var cut = Render<FlareFileUploadZone>();

        Assert.NotEmpty(cut.FindAll("label.flare-file-upload__zone"));
    }

    [Fact]
    public void TakesNoButtonVocabulary()
    {
        // A zone owns its footprint, so it must not carry the button classes the row-level button does.
        var cut = Render<FlareFileUploadZone>();

        Assert.Empty(cut.FindAll(".flare-btn"));
        Assert.Empty(cut.FindAll(".flare-file-upload--button"));
    }

    [Fact]
    public void ShowsTheAcceptHint_WhenAcceptIsSet()
    {
        var cut = Render<FlareFileUploadZone>(p => p.Add(x => x.Accept, ".json"));

        Assert.Contains(".json", cut.Find("span.flare-file-upload__hint").TextContent);
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
        Assert.Empty(cut.FindAll("span.flare-file-upload__hint"));
    }

    [Fact]
    public void AriaLabel_IsApplied()
    {
        var cut = Render<FlareFileUploadZone>(p => p.Add(x => x.AriaLabel, "Avatar dropper"));

        Assert.Equal("Avatar dropper", cut.Find("div.flare-file-upload").GetAttribute("aria-label"));
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
