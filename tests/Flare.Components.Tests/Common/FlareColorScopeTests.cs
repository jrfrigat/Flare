namespace Flare.Components.Tests;

/// <summary>
/// A dynamic color is only useful if the whole subtree can read its derived tones; the scope carries
/// them without the page writing a style of its own.
/// </summary>
public class FlareColorScopeTests : FlareTestContext
{
    [Fact]
    public void CustomColor_ExposesTheLocalTokenSet()
    {
        var cut = Render<FlareColorScope>(p => p
            .Add(x => x.Color, FlareColor.Custom("#ff0000"))
            .AddChildContent("content"));

        var style = cut.Find("div").GetAttribute("style") ?? string.Empty;
        Assert.Contains($"{Css.Tokens.LocalColor.Main}:#ff0000", style);
        Assert.Contains(Css.Tokens.LocalColor.On, style);
        Assert.Contains(Css.Tokens.LocalColor.Container, style);
        Assert.Contains(Css.Tokens.LocalColor.OnContainer, style);
    }

    [Fact]
    public void DefaultColor_AddsNoStyle()
    {
        var cut = Render<FlareColorScope>(p => p.AddChildContent("content"));

        Assert.Null(cut.Find("div").GetAttribute("style"));
    }
}
