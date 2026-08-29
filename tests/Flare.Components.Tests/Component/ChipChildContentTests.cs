using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// Content written between a chip's tags has to reach the label slot.
/// </summary>
/// <remarks>
/// It did not before, and nothing said so: every Flare component inherits an
/// <c>AdditionalAttributes</c> catch-all, so Razor emits the child fragment as an untyped attribute
/// instead of failing to compile, it lands in the unmatched dictionary, and splatting drops it. The
/// chip rendered empty with a clean build. These pin the slot that closes it for this component; the
/// other twenty are tracked in docs/issues/implicit-child-content.md.
/// </remarks>
public class C_ChipChildContentTests : FlareTestContext
{
    [Fact]
    public void Content_between_the_tags_reaches_the_label()
    {
        var cut = Render<FlareChip>(p => p
            .AddChildContent("<b>42</b>"));

        var label = cut.Find(".flare-chip__label");
        Assert.Equal("42", label.TextContent);
        Assert.NotEmpty(label.QuerySelectorAll("b"));
    }

    [Fact]
    public void Child_content_wins_over_the_string_shorthand()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "shorthand")
            .AddChildContent("markup"));

        var label = cut.Find(".flare-chip__label").TextContent;
        Assert.Equal("markup", label);
        Assert.DoesNotContain("shorthand", label);
    }

    [Fact]
    public void The_string_shorthand_still_renders_on_its_own()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Science"));

        Assert.Equal("Science", cut.Find(".flare-chip__label").TextContent);
    }

    [Fact]
    public void An_empty_chip_renders_an_empty_label_rather_than_throwing()
    {
        var cut = Render<FlareChip>();

        Assert.Equal(string.Empty, cut.Find(".flare-chip__label").TextContent);
    }

    [Fact]
    public void A_component_child_renders_too()
    {
        // The reported shape: a component, not markup, written directly between the tags.
        var cut = Render<FlareChip>(p => p
            .AddChildContent<FlareBadge>(b => b.Add(x => x.Text, "9")));

        Assert.NotEmpty(cut.Find(".flare-chip__label").QuerySelectorAll(".flare-badge"));
    }
}
