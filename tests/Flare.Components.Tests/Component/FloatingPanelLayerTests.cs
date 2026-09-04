using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// What a floating panel has to render for the placement engine to be able to do its half. The engine
/// itself needs a browser, so these assert the contract between the two: the anchor it is pointed at,
/// and the side the panel says it wants (which the engine rewrites with the side it actually used, and
/// which the arrow follows either way).
/// </summary>
public class FloatingPanelLayerTests : FlareTestContext
{
    private static RenderFragment Markup(string html) => b => b.AddMarkupContent(0, html);

    [Theory]
    [InlineData(TooltipPlacement.Top, "top")]
    [InlineData(TooltipPlacement.Bottom, "bottom")]
    [InlineData(TooltipPlacement.Left, "left")]
    [InlineData(TooltipPlacement.Right, "right")]
    public void Tooltip_BubbleNamesItsSide(TooltipPlacement placement, string expected)
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(t => t.Content, "hello")
            .Add(t => t.Placement, placement)
            .Add(t => t.ChildContent, Markup("<span>x</span>")));

        Assert.Equal(expected, cut.Find(".flare-tooltip__content").GetAttribute("data-flare-side"));
    }

    [Fact]
    public void Tooltip_ExplicitOffset_OverridesTheThemeTokenOnThatInstance()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(t => t.Content, "hello")
            .Add(t => t.Offset, 16)
            .Add(t => t.ChildContent, Markup("<span>x</span>")));

        // One source for the distance: the token both the resting CSS and the placement engine read.
        Assert.Contains("--flare-tooltip-offset:16px", cut.Find(".flare-tooltip").GetAttribute("style"));
    }

    [Fact]
    public void Tooltip_WithoutOffset_LeavesTheThemeTokenAlone()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(t => t.Content, "hello")
            .Add(t => t.ChildContent, Markup("<span>x</span>")));

        Assert.DoesNotContain("--flare-tooltip-offset", cut.Find(".flare-tooltip").GetAttribute("style") ?? "");
    }

    [Fact]
    public void Popover_PaperNamesItsSide()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Placement, PopoverPlacement.RightEnd)
            .Add(x => x.AnchorContent, Markup("<span>x</span>"))
            .Add(x => x.ChildContent, Markup("<span>body</span>")));

        Assert.Equal("right", cut.Find(".flare-popover__paper").GetAttribute("data-flare-side"));
    }

    [Fact]
    public void DataGridFilterMenu_TriggersCarryDistinctIdsForTheEngineToAnchorTo()
    {
        // The panel is anchored by the trigger's DOM id rather than by a captured reference: the
        // triggers are rendered one per column, and a single captured reference would hold the last
        // column's button rather than the one that was clicked.
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(g => g.Items, new[] { new Row("a", "b") })
            .Add(g => g.FilterMode, DataGridFilterMode.Menu)
            .Add(g => g.Columns, b =>
            {
                b.OpenComponent<FlareColumn<Row>>(0);
                b.AddComponentParameter(1, nameof(FlareColumn<Row>.Title), "First");
                b.AddComponentParameter(2, nameof(FlareColumn<Row>.Field), (Func<Row, object?>)(r => r.First));
                b.AddComponentParameter(3, nameof(FlareColumn<Row>.Filterable), true);
                b.CloseComponent();
                b.OpenComponent<FlareColumn<Row>>(4);
                b.AddComponentParameter(5, nameof(FlareColumn<Row>.Title), "Second");
                b.AddComponentParameter(6, nameof(FlareColumn<Row>.Field), (Func<Row, object?>)(r => r.Second));
                b.AddComponentParameter(7, nameof(FlareColumn<Row>.Filterable), true);
                b.CloseComponent();
            }));

        var ids = cut.FindAll(".flare-datagrid__filter-trigger")
            .Select(e => e.GetAttribute("id"))
            .ToList();

        Assert.Equal(2, ids.Count);
        Assert.All(ids, id => Assert.False(string.IsNullOrEmpty(id)));
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    private sealed record Row(string First, string Second);
}
