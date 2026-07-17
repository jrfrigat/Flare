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
}
