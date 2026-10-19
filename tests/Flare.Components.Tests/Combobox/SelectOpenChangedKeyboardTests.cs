using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-505: opening a select with an arrow key is a real open transition, so a host binding OpenChanged
/// hears about it exactly once - like every other opening path - and a bound Open keeps driving the list.
/// </summary>
public class SelectOpenChangedKeyboardTests : FlareTestContext
{
    private static readonly string[] Items = ["Alpha", "Beta"];

    private static string SelectControl => $".{Css.Classes.Select.Control}";
    private static string MultiControl => $".{Css.Classes.Multiselect.Control}";
    private static string Listbox => $".{Css.Classes.Listbox.Root}";

    [Theory]
    [InlineData("ArrowDown")]
    [InlineData("ArrowUp")]
    public void Select_ArrowKeyThatOpensTheList_RaisesOpenChangedOnce(string key)
    {
        var opens = new List<bool>();
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Open, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, (bool v) => opens.Add(v))));

        cut.Find(SelectControl).KeyDown(key);

        Assert.Equal([true], opens);
        Assert.NotEmpty(cut.FindAll(Listbox));

        cut.Find(SelectControl).KeyDown(key);

        Assert.Equal([true], opens);
    }

    [Theory]
    [InlineData("ArrowDown")]
    [InlineData("ArrowUp")]
    public void MultiSelect_ArrowKeyThatOpensTheList_RaisesOpenChangedOnce(string key)
    {
        var opens = new List<bool>();
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Open, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, (bool v) => opens.Add(v))));

        cut.Find(MultiControl).KeyDown(key);

        Assert.Equal([true], opens);
        Assert.NotEmpty(cut.FindAll(Listbox));

        cut.Find(MultiControl).KeyDown(key);

        Assert.Equal([true], opens);
    }

    [Fact]
    public void Select_BoundOpenStillDrivesTheList()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Open, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, (bool _) => { })));

        Assert.Empty(cut.FindAll(Listbox));

        cut.Render(p => p.Add(x => x.Open, true));
        Assert.NotEmpty(cut.FindAll(Listbox));

        cut.Render(p => p.Add(x => x.Open, false));
        Assert.Empty(cut.FindAll(Listbox));
    }

    [Fact]
    public void MultiSelect_BoundOpenStillDrivesTheList()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Open, false)
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, (bool _) => { })));

        Assert.Empty(cut.FindAll(Listbox));

        cut.Render(p => p.Add(x => x.Open, true));
        Assert.NotEmpty(cut.FindAll(Listbox));

        cut.Render(p => p.Add(x => x.Open, false));
        Assert.Empty(cut.FindAll(Listbox));
    }
}
