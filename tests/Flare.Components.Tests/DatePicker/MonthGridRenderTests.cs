using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class MonthGridRenderTests : FlareTestContext
{
    [Fact]
    public void UnchangedOutput_DoesNotRebuildCells()
    {
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.Culture, CultureInfo.GetCultureInfo("en-US")));
        var renders = cut.RenderCount;
        cut.Render(p => p.Add(x => x.Selected, _ => false).Add(x => x.Disabled, _ => false)
            .Add(x => x.DayClass, _ => string.Empty));
        Assert.Equal(renders, cut.RenderCount);
    }

    [Fact]
    public void MutablePredicates_RefreshSelectionAndAvailability()
    {
        var selected = new DateOnly(2026, 10, 15);
        var disabled = selected.AddDays(1);
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.Culture, CultureInfo.GetCultureInfo("en-US"))
            .Add(x => x.Selected, d => d == selected).Add(x => x.Disabled, d => d == disabled));
        selected = selected.AddDays(2);
        disabled = disabled.AddDays(2);
        cut.Render();
        Assert.Equal("17", cut.Find("button[aria-selected=true]").TextContent.Trim());
        Assert.Equal("18", cut.Find("button[disabled]").TextContent.Trim());
    }

    [Fact]
    public void MutableTemplate_IsEvaluatedOnEveryParameterUpdate()
    {
        var text = "before";
        RenderFragment<DateOnly> template = day => builder => builder.AddContent(0, text);
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.DayContent, template));
        text = "after";
        cut.Render();
        Assert.All(cut.FindAll("button[role=gridcell]"), cell => Assert.Equal("after", cell.TextContent));
    }

    [Fact]
    public void UnchangedRender_StillUsesUpdatedCallbacks()
    {
        var first = 0;
        var second = 0;
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.OnDayClick, _ => first++));
        cut.Render(p => p.Add(x => x.OnDayClick, _ => second++));
        cut.Find("button[tabindex='0']").Click();
        Assert.Equal(0, first);
        Assert.Equal(1, second);
    }
}
