using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class MultiSelectTypeaheadResetTests : FlareTestContext
{
    private static readonly string[] Cities = ["Berlin", "Paris", "Boston"];
    private static string Control => $".{Css.Classes.Multiselect.Control}";
    private static string ActiveOption => $".{Css.Classes.Listbox.OptionActive}";

    private static KeyboardEventArgs Key(string key) => new() { Key = key };

    private static string Highlighted(IRenderedComponent<FlareMultiSelect<string>> cut)
        => cut.Find(ActiveOption).TextContent.Trim();

    private IRenderedComponent<FlareMultiSelect<string>> RenderCities()
        => Render<FlareMultiSelect<string>>(p => p.Add(x => x.Items, Cities));

    [Fact]
    public void RapidPrefix_Accumulates()
    {
        var cut = RenderCities();

        cut.Find(Control).KeyDown(Key("b"));
        cut.Find(Control).KeyDown(Key("o"));

        Assert.Equal("Boston", Highlighted(cut));
    }

    [Fact]
    public async Task PauseLongerThanTheTimeout_StartsANewBuffer()
    {
        var cut = RenderCities();

        cut.Find(Control).KeyDown(Key("b"));
        Assert.Equal("Berlin", Highlighted(cut));

        await Task.Delay(1200, Xunit.TestContext.Current.CancellationToken);
        cut.Find(Control).KeyDown(Key("p"));

        Assert.Equal("Paris", Highlighted(cut));
    }

    [Fact]
    public void Escape_ResetsTheBuffer()
    {
        var cut = RenderCities();

        cut.Find(Control).KeyDown(Key("b"));
        cut.Find(Control).KeyDown(Key("Escape"));
        cut.Find(Control).KeyDown(Key("p"));

        Assert.Equal("Paris", Highlighted(cut));
    }
}
