using System.Globalization;
using Flare.Components.IDE;
using Flare.Icons;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The editor breadcrumb/toolbar row is composition only: the host supplies the breadcrumb segments
/// and the action commands, while the row owns the surface, the slots and the labelled action area.
/// A segment with menu content is a FlareMenu activator; without it, a static icon-and-text leaf.
/// </summary>
public class IdeEditorBreadcrumbBarTests : FlareTestContext
{
    private static RenderFragment Menu(string item) => builder =>
    {
        builder.OpenComponent<FlareMenuItem>(0);
        builder.AddAttribute(1, nameof(FlareMenuItem.ChildContent),
            (RenderFragment)(content => content.AddContent(0, item)));
        builder.CloseComponent();
    };

    private sealed record SegmentSpec(string Text, FlareIcon? Icon = null, bool Active = false,
        RenderFragment? Menu = null, string? AriaLabel = null);

    private static RenderFragment Segment(SegmentSpec spec) => builder =>
    {
        var sequence = 0;
        builder.OpenComponent<FlareEditorBreadcrumbSegment>(sequence++);
        builder.AddAttribute(sequence++, nameof(FlareEditorBreadcrumbSegment.Text), spec.Text);
        if (spec.Icon is not null)
            builder.AddAttribute(sequence++, nameof(FlareEditorBreadcrumbSegment.Icon), spec.Icon);
        if (spec.Active)
            builder.AddAttribute(sequence++, nameof(FlareEditorBreadcrumbSegment.Active), true);
        if (spec.Menu is not null)
            builder.AddAttribute(sequence++, nameof(FlareEditorBreadcrumbSegment.MenuContent), spec.Menu);
        if (spec.AriaLabel is not null)
            builder.AddAttribute(sequence++, nameof(FlareEditorBreadcrumbSegment.AriaLabel), spec.AriaLabel);
        builder.CloseComponent();
    };

    private static RenderFragment Actions() => builder =>
    {
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "aria-label", "Split right");
        builder.AddContent(3, "Split");
        builder.CloseElement();
    };

    private IRenderedComponent<FlareEditorBreadcrumbBar> RenderBar(
        RenderFragment? breadcrumb = null, RenderFragment? actions = null, string? actionsLabel = null) =>
        Render<FlareEditorBreadcrumbBar>(p =>
        {
            if (breadcrumb is not null) p.Add(x => x.BreadcrumbContent, breadcrumb);
            if (actions is not null) p.Add(x => x.ToolbarContent, actions);
            if (actionsLabel is not null) p.Add(x => x.ActionsLabel, actionsLabel);
        });

    private static AngleSharp.Dom.IElement FirstSegment(IRenderedComponent<FlareEditorBreadcrumbBar> cut) =>
        cut.Find($".{Css.Classes.Ide.EditorBreadcrumbBar.Segment}");

    private static AngleSharp.Dom.IElement SegmentButton(IRenderedComponent<FlareEditorBreadcrumbBar> cut) =>
        cut.Find($".{Css.Classes.Ide.EditorBreadcrumbBar.SegmentButton}");

    [Fact]
    public void Bar_RendersTheLeadingBreadcrumbAndTheLabelledActionArea()
    {
        var cut = RenderBar(Segment(new SegmentSpec("Program.cs")), Actions());

        var root = cut.Find($".{Css.Classes.Ide.EditorBreadcrumbBar.Root}");
        Assert.Equal(2, root.Children.Length);
        Assert.Equal(Css.Classes.Ide.EditorBreadcrumbBar.Breadcrumb, root.Children[0].ClassName);
        Assert.Equal(Css.Classes.Ide.EditorBreadcrumbBar.Actions, root.Children[1].ClassName);
        Assert.Equal("toolbar", root.Children[1].GetAttribute("role"));
        Assert.Equal("Editor actions", root.Children[1].GetAttribute("aria-label"));
        Assert.Single(root.Children[1].QuerySelectorAll("button[aria-label='Split right']"));
    }

    [Fact]
    public void Bar_WithoutActionsRendersNoActionAreaAndKeepsTheBreadcrumb()
    {
        var cut = RenderBar(Segment(new SegmentSpec("Program.cs")));

        var root = cut.Find($".{Css.Classes.Ide.EditorBreadcrumbBar.Root}");
        Assert.Single(root.Children);
        Assert.Equal(Css.Classes.Ide.EditorBreadcrumbBar.Breadcrumb, root.Children[0].ClassName);
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.EditorBreadcrumbBar.Actions}"));
    }

    [Fact]
    public void Bar_TakesACustomActionsLabel()
    {
        var cut = RenderBar(Segment(new SegmentSpec("Program.cs")), Actions(), "Editor commands");

        Assert.Equal("Editor commands",
            cut.Find($".{Css.Classes.Ide.EditorBreadcrumbBar.Actions}").GetAttribute("aria-label"));
    }

    [Fact]
    public void Bar_UsesTheLocalizedActionsLabel()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

            var cut = RenderBar(Segment(new SegmentSpec("Program.cs")), Actions());

            Assert.Equal("Действия редактора",
                cut.Find($".{Css.Classes.Ide.EditorBreadcrumbBar.Actions}").GetAttribute("aria-label"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public void Segment_WithoutMenuRendersAStaticIconAndLabel()
    {
        var cut = RenderBar(Segment(new SegmentSpec("Program.cs", FlareIcons.Description)));

        var segment = FirstSegment(cut);
        Assert.Equal("span", SegmentButton(cut).LocalName);
        Assert.Empty(segment.QuerySelectorAll("button"));
        Assert.Single(segment.QuerySelectorAll("svg"));
        Assert.NotNull(segment.QuerySelector($"svg.{Css.Classes.Ide.EditorBreadcrumbBar.SegmentIcon}"));
        Assert.Empty(segment.QuerySelectorAll($".{Css.Classes.Ide.EditorBreadcrumbBar.SegmentChevron}"));
        Assert.Equal("Program.cs",
            segment.QuerySelector($".{Css.Classes.Ide.EditorBreadcrumbBar.SegmentLabel}")!.TextContent.Trim());
    }

    [Fact]
    public async Task Segment_WithMenuRendersAnActivatorChevronAndTheHostItems()
    {
        var cut = RenderBar(Segment(new SegmentSpec("Quas.Core", FlareIcons.Folder, Menu: Menu("Quas.Core"))));

        var segment = FirstSegment(cut);
        var button = segment.QuerySelector("button");
        Assert.NotNull(button);
        Assert.Equal("menu", button.GetAttribute("aria-haspopup"));
        Assert.Equal(2, segment.QuerySelectorAll("svg").Length);
        Assert.NotNull(segment.QuerySelector($"svg.{Css.Classes.Ide.EditorBreadcrumbBar.SegmentChevron}"));

        var activator = cut.Find(
            $".{Css.Classes.Ide.EditorBreadcrumbBar.Segment} .{Css.Classes.Menu.Activator}");
        await cut.InvokeAsync(() => activator.Click());

        var panel = cut.Find($".{Css.Classes.Menu.Panel}");
        Assert.Equal("Quas.Core", panel.GetAttribute("aria-label"));
        Assert.Contains("Quas.Core", panel.TextContent);
    }

    [Fact]
    public void Segment_AriaLabelNamesTheButtonAndDropdown()
    {
        var cut = RenderBar(Segment(new SegmentSpec("", FlareIcons.Description, Menu: Menu("Members"),
            AriaLabel: "Current type")));

        var button = SegmentButton(cut);
        Assert.Equal("button", button.LocalName);
        Assert.Equal("Current type", button.GetAttribute("aria-label"));
    }

    [Fact]
    public void Segment_ActiveCarriesTheActiveClass()
    {
        var cut = RenderBar(Segment(new SegmentSpec("Program.cs", Active: true)));

        Assert.Contains(Css.Classes.Ide.EditorBreadcrumbBar.SegmentActive, FirstSegment(cut).ClassList);
    }

    [Fact]
    public void Segment_NotActiveHasNoActiveClass()
    {
        var cut = RenderBar(Segment(new SegmentSpec("Program.cs")));

        Assert.DoesNotContain(Css.Classes.Ide.EditorBreadcrumbBar.SegmentActive, FirstSegment(cut).ClassList);
    }

    [Fact]
    public void Bar_RendersSegmentsInHostOrder()
    {
        var cut = RenderBar((RenderFragment)(builder =>
        {
            builder.AddContent(0, Segment(new SegmentSpec("One")));
            builder.AddContent(1, Segment(new SegmentSpec("Two")));
            builder.AddContent(2, Segment(new SegmentSpec("Three")));
        }));

        Assert.Equal(["One", "Two", "Three"],
            cut.FindAll($".{Css.Classes.Ide.EditorBreadcrumbBar.SegmentLabel}")
                .Select(label => label.TextContent.Trim()));
    }
}
