using Flare.Components.IDE;
using Flare.Css;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareSettingsTests : FlareTestContext
{
    public FlareSettingsTests() => Services.AddFlareIde();

    private static readonly FlareSettingsCategory[] Categories =
    [
        new("general", "General"),
        new("editor", "Editor")
        {
            Selectable = false,
            Children =
            [
                new("appearance", "Appearance")
                {
                    Description = "Text and colors",
                    SearchTerms = ["Font size"]
                },
                new("behavior", "Behavior")
            ]
        }
    ];

    private static RenderFragment Content(FlareSettingsCategory category) =>
        builder => builder.AddContent(0, $"Page body for {category.Id}");

    [Fact]
    public void SearchKeepsMatchingParentsAndRestoresTheirExpansion()
    {
        var cut = Render<FlareSettings>(parameters => parameters
            .Add(x => x.Categories, Categories)
            .Add(x => x.PageContent, Content));
        var editor = "[role=treeitem][aria-label=Editor]";
        cut.Find($"{editor} > .{Css.Classes.Tree.ItemRow}").Click();

        cut.Find("input[type=search]").Input(" font SIZE ");

        Assert.Equal("true", cut.Find(editor).GetAttribute("aria-expanded"));
        Assert.Single(cut.FindAll("[role=treeitem][aria-label=Appearance]"));
        Assert.Empty(cut.FindAll("[role=treeitem][aria-label=Behavior]"));
        Assert.Contains("Page body for general", cut.Markup, StringComparison.Ordinal);

        cut.Find("input[type=search]").Input("");
        Assert.Equal("false", cut.Find(editor).GetAttribute("aria-expanded"));
        Assert.Empty(cut.FindAll("[role=treeitem][aria-label=Appearance]"));
    }

    [Fact]
    public void KeyboardRequestsSelectionAndTheHostControlsTheDisplayedPage()
    {
        string? requested = null;
        var cut = Render<FlareSettings>(parameters => parameters
            .Add(x => x.Categories, Categories)
            .Add(x => x.SelectedId, "general")
            .Add(x => x.PageContent, Content)
            .Add(x => x.SelectedIdChanged, id => requested = id));

        cut.Find("[role=treeitem][aria-label=Appearance]").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("appearance", requested);
        Assert.Contains("Page body for general", cut.Markup, StringComparison.Ordinal);
        cut.Render(parameters => parameters.Add(x => x.SelectedId, requested));

        Assert.Equal("Appearance", cut.Find(".flare-settings__page h3").TextContent);
        Assert.Contains("Text and colors", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Page body for appearance", cut.Markup, StringComparison.Ordinal);
    }
}
