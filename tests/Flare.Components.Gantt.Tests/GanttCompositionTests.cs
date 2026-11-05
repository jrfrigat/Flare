using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Flare.Components.Gantt.Tests;

public sealed class GanttCompositionTests : GanttTestBase
{
    [Fact]
    public async Task InteractiveItemCanReuseTheStandardAxisKeyboardCommands()
    {
        Flare.Components.Gantt.GanttItemContext<Work>? item = null;
        Flare.Components.Gantt.GanttChangeEventArgs? change = null;
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r") })
            .Add(x => x.Items, new[] { new Work("w", "r", Day(2), Day(4)) })
            .Add(x => x.Editable, true).Add(x => x.OnChange, value => change = value)
            .Add(x => x.ItemInteractionTemplate, value => b => { item = value; b.AddContent(0, value.Item.Key); }));
        await cut.InvokeAsync(() => item!.HandleKeyAsync(new() { Key = "ArrowRight", CtrlKey = true }));
        Assert.NotNull(change);
        Assert.Equal(Flare.Components.Gantt.GanttEditAction.Move, change.Action);
        Assert.Equal(Day(3).ToDateTime(TimeOnly.MinValue), Assert.Single(change.Changes).Span.Start);
    }

    [Fact]
    public void InteractiveItemOwnsActivationAndRetainsTheBarGeometry()
    {
        var selected = 0;
        var edited = 0;
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r") })
            .Add(x => x.Items, new[] { new Work("w", "r", Day(2), Day(4)) })
            .Add(x => x.OnItemClick, _ => selected++)
            .Add(x => x.ItemInteractionTemplate, _ => b =>
            {
                b.OpenComponent<FlareButton>(0);
                b.AddAttribute(1, "OnClick", EventCallback.Factory.Create<MouseEventArgs>(this, () => edited++));
                b.AddAttribute(2, "ChildContent", (RenderFragment)(text => text.AddContent(0, "Edit")));
                b.CloseComponent();
            }));
        var bar = cut.Find(".flare-gantt__bar");
        Assert.Equal("group", bar.GetAttribute("role"));
        Assert.Null(bar.GetAttribute("aria-pressed"));
        Assert.Equal("left:80px;top:5px;width:120px;height:20px", cut.Find(".flare-gantt__bar-slot").GetAttribute("style"));
        bar.Click();
        cut.Find(".flare-gantt__bar button").Click();
        Assert.Equal(0, selected);
        Assert.Equal(1, edited);
    }
}
