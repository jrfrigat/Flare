using System.Text.RegularExpressions;
using Flare.Components.IDE;

namespace Flare.Components.Tests;

/// <summary>
/// A tab strip and its panel point at each other by id, and an id is the one kind of name a compiler
/// never checks. The ribbon had exactly that hole: the panel claimed <c>aria-labelledby</c> on an id no
/// element carried, so a screen reader announced a tab panel with no name and nothing failed - the
/// markup was still valid, the class names were all real, and every rendering test still passed.
/// <para>
/// So this asserts the link itself rather than either end of it: every id an ARIA attribute references
/// resolves to an element in the same tree.
/// </para>
/// </summary>
public class TabPanelLabellingTests : FlareTestContext
{
    public TabPanelLabellingTests() => Services.AddFlareIde();

    [Fact]
    public void Ribbon_PanelAndItsTabPointAtEachOther()
    {
        var cut = Render<FlareRibbon>(p => p.AddChildContent<FlareRibbonTab>(t => t
            .Add(x => x.Title, "Home")
            .AddChildContent<FlareRibbonGroup>(g => g.Add(x => x.Label, "Clipboard"))));

        AssertEveryAriaReferenceResolves(cut.Markup);
    }

    [Fact]
    public void DocumentTabs_PanelAndItsTabPointAtEachOther()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .AddChildContent<FlareDocumentTab>(t => t.Add(x => x.Title, "readme.md")));

        AssertEveryAriaReferenceResolves(cut.Markup);
    }

    // Read from the rendered markup rather than through the DOM query API: what matters is that the id
    // written into one attribute is the id written into the other, and both are plain text by then.
    private static void AssertEveryAriaReferenceResolves(string markup)
    {
        var ids = new HashSet<string>(
            Regex.Matches(markup, @"\bid=""([^""]+)""").Select(m => m.Groups[1].Value),
            StringComparer.Ordinal);

        var references = Regex.Matches(markup, @"\baria-(labelledby|controls)=""([^""]+)""")
            .Select(m => (Attribute: m.Groups[1].Value, Id: m.Groups[2].Value))
            .ToList();

        Assert.NotEmpty(references);

        var dangling = references.Where(r => !ids.Contains(r.Id)).ToList();
        Assert.True(dangling.Count == 0,
            "These ARIA references point at an id nothing carries, so the element they name does not "
            + "exist: " + string.Join(", ", dangling.Select(d => $"aria-{d.Attribute}=\"{d.Id}\""))
            + ". Ids present: " + string.Join(", ", ids));
    }
}
