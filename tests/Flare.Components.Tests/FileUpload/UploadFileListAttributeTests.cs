namespace Flare.Components.Tests;

/// <summary>
/// The list inherited the component base but painted its ul with a literal class and no attribute
/// support, so a caller's Class, Style, id, aria-label and data-* were dropped. They belong on the ul,
/// and the drop guards that keep the browser from navigating away to a dropped file stay with them.
/// </summary>
public class UploadFileListAttributeTests : FlareTestContext
{
    private IRenderedComponent<FlareUploadFileList> RenderList() =>
        Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, Array.Empty<FlareUploadFile>())
            .Add(x => x.Class, "custom-list")
            .Add(x => x.Style, "max-height: 4rem")
            .AddUnmatched("id", "my-list")
            .AddUnmatched("aria-label", "Chosen files")
            .AddUnmatched("data-testid", "upload-list"));

    [Fact]
    public void TheClassKeepsTheListClassAndAddsTheCallersOwn()
    {
        var ul = RenderList().Find("ul");

        Assert.Contains(Css.Classes.FileUpload.List, ul.ClassList);
        Assert.Contains("custom-list", ul.ClassList);
    }

    [Fact]
    public void Style_LandsOnTheList()
    {
        Assert.Equal("max-height: 4rem", RenderList().Find("ul").GetAttribute("style"));
    }

    [Fact]
    public void IdAriaAndDataAttributes_LandOnTheList()
    {
        var ul = RenderList().Find("ul");

        Assert.Equal("my-list", ul.GetAttribute("id"));
        Assert.Equal("Chosen files", ul.GetAttribute("aria-label"));
        Assert.Equal("upload-list", ul.GetAttribute("data-testid"));
    }

    [Fact]
    public void TheDropGuards_StayOnTheList()
    {
        var ul = RenderList().Find("ul");

        Assert.Contains("ondragover", ul.OuterHtml, StringComparison.Ordinal);
        Assert.Contains("ondrop", ul.OuterHtml, StringComparison.Ordinal);
    }
}
