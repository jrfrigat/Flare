using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class SelectTypeaheadResetTests : FlareTestContext
{
    private static readonly string[] Cities = ["Berlin", "Paris", "Boston"];
    private static string Control => $".{Css.Classes.Select.Control}";
    private static string ActiveOption => $".{Css.Classes.Listbox.OptionActive}";

    private static KeyboardEventArgs Key(string key) => new() { Key = key };

    private static string Highlighted(IRenderedComponent<FlareSelect<string>> cut)
        => cut.Find(ActiveOption).TextContent.Trim();

    private IRenderedComponent<FlareSelect<string>> RenderCities()
        => Render<FlareSelect<string>>(p => p.Add(x => x.Items, Cities));

    [Fact]
    public void RapidPrefix_Accumulates()
    {
        var cut = RenderCities();

        cut.Find(Control).KeyDown(Key("b"));
        cut.Find(Control).KeyDown(Key("o"));

        Assert.Equal("Boston", Highlighted(cut));
    }

    [Fact]
    public void RepeatedLetter_CyclesWhileTheBufferIsFresh()
    {
        var cut = RenderCities();

        cut.Find(Control).KeyDown(Key("b"));
        cut.Find(Control).KeyDown(Key("b"));

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
    public async Task SameLetterAfterAPause_DoesNotCycle()
    {
        var cut = RenderCities();

        cut.Find(Control).KeyDown(Key("b"));
        await Task.Delay(1200, Xunit.TestContext.Current.CancellationToken);
        cut.Find(Control).KeyDown(Key("b"));

        Assert.Equal("Berlin", Highlighted(cut));
    }

    [Fact]
    public void Home_ResetsTheBuffer()
    {
        var cut = RenderCities();

        cut.Find(Control).KeyDown(Key("b"));
        cut.Find(Control).KeyDown(Key("Home"));
        cut.Find(Control).KeyDown(Key("p"));

        Assert.Equal("Paris", Highlighted(cut));
    }

    [Fact]
    public void End_ResetsTheBuffer()
    {
        var cut = RenderCities();

        cut.Find(Control).KeyDown(Key("b"));
        cut.Find(Control).KeyDown(Key("End"));
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
