using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class ComboboxOpenChangedTests : FlareTestContext
{
    private static string Listbox => $".{Css.Classes.Listbox.Root}";

    private IRenderedComponent<FlareCombobox<string>> RenderTracked(List<bool> opens)
        => Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, new[] { "alpha", "beta" })
            .Add(x => x.Open, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, (bool v) => opens.Add(v))));

    [Fact]
    public void Focus_RaisesOpenChangedOnce()
    {
        var opens = new List<bool>();
        var cut = RenderTracked(opens);

        cut.Find("input").Focus();

        Assert.Equal([true], opens);
        Assert.NotEmpty(cut.FindAll(Listbox));
    }

    [Fact]
    public void ArrowKey_RaisesOpenChangedOnceAndNotAgainWhileOpen()
    {
        var opens = new List<bool>();
        var cut = RenderTracked(opens);

        cut.Find("input").KeyDown("ArrowDown");
        Assert.Equal([true], opens);
        Assert.NotEmpty(cut.FindAll(Listbox));

        cut.Find("input").KeyDown("ArrowDown");
        Assert.Equal([true], opens);
    }

    [Fact]
    public void ExternalOpen_DrivesTheList()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, new[] { "alpha" })
            .Add(x => x.Open, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, (bool _) => { })));

        Assert.Empty(cut.FindAll(Listbox));

        cut.Render(p => p.Add(x => x.Open, true));
        Assert.NotEmpty(cut.FindAll(Listbox));

        cut.Render(p => p.Add(x => x.Open, false));
        Assert.Empty(cut.FindAll(Listbox));
    }
}
