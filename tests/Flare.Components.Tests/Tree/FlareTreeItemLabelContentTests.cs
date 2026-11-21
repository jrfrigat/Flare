using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// LabelContent is the non-interactive label slot a consumer uses for a mark that belongs to the
/// label itself. It renders inside the label span after the icon; Label stays the accessible name.
/// </summary>
public class FlareTreeItemLabelContentTests : FlareTestContext
{
    private static RenderFragment Markup(string markup) => builder => builder.AddMarkupContent(0, markup);

    [Fact]
    public void LabelContent_RendersInsideTheLabelSpan_AfterTheIcon()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Database")
            .Add(x => x.Icon, FlareIcons.Folder)
            .Add(x => x.LabelContent, Markup(
                "<span class=\"mark\">DB</span><span class=\"text\">Database</span>")));

        var row = cut.Find($".{Css.Classes.Tree.ItemRow}");
        var label = cut.Find($".{Css.Classes.TreeView.Label}");
        Assert.NotNull(label.QuerySelector(".mark"));
        Assert.NotNull(label.QuerySelector(".text"));

        var children = row.Children.ToList();
        var iconIndex = children.FindIndex(element => element.ClassList.Contains(Css.Classes.Tree.ItemIcon));
        var labelIndex = children.FindIndex(element => element.ClassList.Contains(Css.Classes.TreeView.Label));
        Assert.True(iconIndex >= 0 && iconIndex < labelIndex, "The icon must precede the label span.");
        Assert.Equal("Database", cut.Find("li").GetAttribute("aria-label"));
    }

    [Fact]
    public void WithoutLabelContent_TheLabelIsTheVisibleText()
    {
        var cut = Render<FlareTreeItem>(p => p.Add(x => x.Label, "Database"));

        var label = cut.Find($".{Css.Classes.TreeView.Label}");
        Assert.Equal("Database", label.TextContent.Trim());
        Assert.Null(cut.Find("li").GetAttribute("aria-label"));
    }

    [Fact]
    public void LabelContent_DoesNotHideTheIconOrAddTabStops()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Database")
            .Add(x => x.Icon, FlareIcons.Folder)
            .Add(x => x.LabelContent, Markup("<span class=\"mark\">DB</span>")));

        Assert.Equal(FlareIcons.Folder.Data, cut.Find($".{Css.Classes.Tree.ItemIcon} path").GetAttribute("d"));
        Assert.Null(cut.Find(".mark").GetAttribute("tabindex"));
        Assert.Equal("Database", cut.Find("li").GetAttribute("aria-label"));
    }

    [Fact]
    public void ExplicitAriaLabel_OverridesTheComponentName()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Database")
            .Add(x => x.LabelContent, Markup("<span>DB</span>"))
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object>
            {
                ["aria-label"] = "Connection: Database"
            }));

        Assert.Equal("Connection: Database", cut.Find("li").GetAttribute("aria-label"));
    }

    [Fact]
    public void LabelContent_WithNullLabel_HasNoAutomaticName()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.LabelContent, Markup("<span>Database</span>")));

        Assert.Null(cut.Find("li").GetAttribute("aria-label"));
    }

    [Fact]
    public void ExplicitDescription_PassesThrough()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Database")
            .Add(x => x.LabelContent, Markup("<span>DB</span>"))
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object>
            {
                ["aria-describedby"] = "category-database"
            }));

        Assert.Equal("category-database", cut.Find("li").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void LabelContent_AndTrailingContent_KeepTheRowOrder()
    {
        var cut = Render<FlareTreeItem>(p => p
            .Add(x => x.Label, "Database")
            .Add(x => x.LabelContent, Markup("<span id=\"mark\">DB</span>"))
            .Add(x => x.TrailingContent, Markup("<span id=\"status\">Connected</span>")));

        var row = cut.Find($".{Css.Classes.Tree.ItemRow}");
        var label = row.QuerySelector($".{Css.Classes.TreeView.Label}")!;
        var status = row.QuerySelector("#status")!;
        Assert.NotNull(label.QuerySelector("#mark"));
        Assert.True(row.QuerySelectorAll("*").ToList().IndexOf(status)
            > row.QuerySelectorAll("*").ToList().IndexOf(label), "The trailing slot must follow the label.");
    }
}
