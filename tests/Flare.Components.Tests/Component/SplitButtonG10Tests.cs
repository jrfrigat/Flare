using Flare.Components.Tests;

namespace Flare.Components;

// ------------------------------------------------------------------------------
// FlareSplitButton G10 follow-ups: Loading, FullWidth, Href (+rel), and the public
// Open()/Close() menu control.
// ------------------------------------------------------------------------------

public class C_FlareSplitButtonG10Tests : FlareTestContext
{
    [Fact]
    public void FullWidth_AddsFullClass()
    {
        var cut = Render<FlareSplitButton>(p => p.Add(x => x.FullWidth, true).AddChildContent("Save"));
        Assert.Contains("flare-split-btn--full", cut.Find(".flare-split-btn").ClassName);
    }

    [Fact]
    public void Href_RendersPrimaryAsLink()
    {
        var cut = Render<FlareSplitButton>(p => p.Add(x => x.Href, "/go").AddChildContent("Go"));
        var main = cut.Find("a.flare-split-btn__main");
        Assert.Equal("/go", main.GetAttribute("href"));
    }

    [Fact]
    public void Href_BlankTarget_DefaultsRelNoopener()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.Href, "https://example.com").Add(x => x.Target, "_blank").AddChildContent("Go"));
        Assert.Equal("noopener noreferrer", cut.Find("a.flare-split-btn__main").GetAttribute("rel"));
    }

    [Fact]
    public void Loading_ShowsSpinnerOnPrimary()
    {
        var cut = Render<FlareSplitButton>(p => p.Add(x => x.Loading, true).AddChildContent("Save"));
        Assert.Contains(Css.Classes.Button.Spinner, cut.Markup);
    }

    [Fact]
    public async Task OpenAndClose_TogglesMenuOpenState()
    {
        var cut = Render<FlareSplitButton>(p => p.AddChildContent("Save"));

        await cut.InvokeAsync(() => cut.Instance.Open());
        Assert.Contains("flare-split-btn--open", cut.Find(".flare-split-btn").ClassName);

        await cut.InvokeAsync(() => cut.Instance.Close());
        Assert.DoesNotContain("flare-split-btn--open", cut.Find(".flare-split-btn").ClassName);
    }
}
