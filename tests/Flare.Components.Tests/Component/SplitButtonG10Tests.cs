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
        Assert.Contains(Css.Classes.SplitButton.Full, cut.Find($".{Css.Classes.SplitButton.Root}").ClassName);
    }

    [Fact]
    public void Href_RendersPrimaryAsLink()
    {
        var cut = Render<FlareSplitButton>(p => p.Add(x => x.Href, "/go").AddChildContent("Go"));
        var main = cut.Find($"a.{Css.Classes.SplitButton.Main}");
        Assert.Equal("/go", main.GetAttribute("href"));
    }

    [Fact]
    public void Href_BlankTarget_DefaultsRelNoopener()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.Href, "https://example.com").Add(x => x.Target, "_blank").AddChildContent("Go"));
        Assert.Equal("noopener noreferrer", cut.Find($"a.{Css.Classes.SplitButton.Main}").GetAttribute("rel"));
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
        Assert.Contains(Css.Classes.SplitButton.Opened, cut.Find($".{Css.Classes.SplitButton.Root}").ClassName);

        await cut.InvokeAsync(() => cut.Instance.Close());
        Assert.DoesNotContain(Css.Classes.SplitButton.Opened, cut.Find($".{Css.Classes.SplitButton.Root}").ClassName);
    }
}
