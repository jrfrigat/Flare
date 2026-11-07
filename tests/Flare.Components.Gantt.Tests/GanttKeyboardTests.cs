using Xunit;

namespace Flare.Components.Gantt.Tests;

public sealed class GanttKeyboardTests : GanttTestBase
{
    [Fact]
    public async Task ArrowsFollowVisibleRowsAndTreeParents()
    {
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("a", new Node("a1"), new Node("a2", new Node("a21"))), new Node("b") }));
        var panel = cut.FindComponent<FlareGanttPanel<Node, Work>>().Instance;
        string Active() => cut.Find("[role=row][tabindex='0']").GetAttribute("data-row")!;
        async Task Press(int index, string key) => await cut.InvokeAsync(() => panel.HandleTreeKeyAsync(index, key));

        await Press(0, "ArrowDown");
        Assert.Equal("a1", Active());
        await Press(1, "ArrowUp");
        Assert.Equal("a", Active());
        await Press(0, "ArrowLeft");
        Assert.Equal(2, cut.FindAll("[role=row]").Count);
        await Press(0, "ArrowRight");
        Assert.Equal(5, cut.FindAll("[role=row]").Count);
        await Press(0, "ArrowRight");
        Assert.Equal("a1", Active());
        await Press(1, "ArrowLeft");
        Assert.Equal("a", Active());
        await Press(0, "End");
        Assert.Equal("b", Active());
        await Press(4, "Home");
        Assert.Equal("a", Active());
        await Press(3, "ArrowLeft");
        Assert.Equal("a2", Active());
    }
}
