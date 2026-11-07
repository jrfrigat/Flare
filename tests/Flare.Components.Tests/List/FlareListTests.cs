using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareListTests : FlareTestContext
{
    [Fact]
    public void VirtualizeUsesALiveIndexedViewOfReadOnlyItems()
    {
        var source = new List<int> { 1, 2, 3 };
        var cut = Render<FlareList<int>>(p => p
            .Add(x => x.Virtual, true)
            .Add(x => x.Items, new ReadOnlyRows(source)));
        var child = cut.FindComponent<Virtualize<int>>();
        var view = Assert.IsAssignableFrom<IList<int>>(child.Instance.Items);

        source[1] = 20;
        source.Add(4);
        Assert.Equal(20, view[1]);
        Assert.Equal(4, view.Count);

        int[] replacement = [5, 6];
        cut.Render(p => p.Add(x => x.Items, replacement));
        Assert.Same(replacement, child.Instance.Items);
    }

    private sealed class ReadOnlyRows(List<int> rows) : IReadOnlyList<int>
    {
        public int Count => rows.Count;
        public int this[int index] => rows[index];
        public IEnumerator<int> GetEnumerator() => rows.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Fact]
    public void RendersRootUl()
    {
        var cut = Render<FlareList<object>>();

        Assert.NotEmpty(cut.FindAll($"ul.{Css.Classes.List.Root}"));
    }

    [Fact]
    public void HasRoleList()
    {
        var cut = Render<FlareList<object>>();

        Assert.Equal("list", cut.Find($"ul.{Css.Classes.List.Root}").GetAttribute("role"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareList<object>>(p => p
            .AddChildContent("<li id=\"custom-li\">Item</li>"));

        Assert.NotEmpty(cut.FindAll("#custom-li"));
    }

    [Fact]
    public void Dense_HasDenseClass()
    {
        var cut = Render<FlareList<object>>(p => p
            .Add(x => x.Dense, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Dense}"));
    }

    [Fact]
    public void NotDense_NoDenseClass()
    {
        var cut = Render<FlareList<object>>(p => p
            .Add(x => x.Dense, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.List.Dense}"));
    }

    [Fact]
    public void RendersListItems()
    {
        var cut = Render<FlareList<object>>(p => p
            .AddChildContent<FlareListItem>(li =>
                li.Add(x => x.Primary, "First Item")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Item}"));
    }
}
