using Flare.Abstractions;
using Flare.Components;

namespace Flare.Components.Tests;

/// <summary>
/// FlareSection exposes two id paths: <c>Id</c> anchors the built-in heading, while an unmatched
/// <c>id</c> attribute (passed through <c>AdditionalAttributes</c>) belongs to the section element. The
/// Gallery points its table of contents at the section, so a markup <c>id="..."</c> - which binds to
/// <c>Id</c> case-insensitively - left the section without an anchor and the TOC empty.
/// </summary>
public sealed class FlareSectionAnchorTests : FlareTestContext
{
    [Fact]
    public void AdditionalAttributesId_LandsOnTheSection()
    {
        var cut = Render<FlareSection>(p => p
            .Add(x => x.Title, "Main")
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { ["id"] = "gallery-main" }));

        var section = cut.Find($"section.{Css.Classes.Section.Root}");
        Assert.Equal("gallery-main", section.GetAttribute("id"));
    }

    [Fact]
    public void IdParameter_AnchorsTheHeading_NotTheSection()
    {
        var cut = Render<FlareSection>(p => p
            .Add(x => x.Title, "Main")
            .Add(x => x.Id, "section-anchor"));

        var section = cut.Find($"section.{Css.Classes.Section.Root}");
        var heading = cut.Find("h2");
        Assert.Null(section.GetAttribute("id"));
        Assert.Equal("section-anchor", heading.GetAttribute("id"));
        Assert.Equal("section-anchor", section.GetAttribute("aria-labelledby"));
    }
}
