using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests.Accessibility;

/// <summary>
/// A toggle group is one choice spread over several buttons, so it has to reach the accessibility tree
/// as a named group while each button keeps its own pressed state. A radio contract would be wrong here:
/// a single selection can be cleared and a MultiSelect group allows several.
/// </summary>
public class FlareToggleGroupAriaTests : FlareTestContext
{
    private static void AddButton(RenderTreeBuilder builder, int seq, string value, string label)
    {
        builder.OpenComponent<FlareToggleButton>(seq);
        builder.AddAttribute(seq + 1, nameof(FlareToggleButton.Value), value);
        builder.AddAttribute(seq + 2, nameof(FlareToggleButton.ChildContent), (RenderFragment)(b => b.AddContent(0, label)));
        builder.CloseComponent();
    }

    private IRenderedComponent<FlareToggleGroup<string>> RenderGroup(
        Action<ComponentParameterCollectionBuilder<FlareToggleGroup<string>>> parameters)
        => Render<FlareToggleGroup<string>>(p =>
        {
            parameters(p);
            p.AddChildContent(builder =>
            {
                AddButton(builder, 0, "a", "A");
                AddButton(builder, 10, "b", "B");
            });
        });

    [Fact]
    public void TheRootIsANamedGroup()
    {
        var cut = RenderGroup(p => p.Add(x => x.AriaLabel, "Text alignment"));

        var root = cut.Find($".{Css.Classes.ToggleGroup.Root}");
        Assert.Equal("group", root.GetAttribute("role"));
        Assert.Equal("Text alignment", root.GetAttribute("aria-label"));
        Assert.Null(root.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void AnUnmatchedAriaLabelNamesTheGroup()
    {
        var cut = RenderGroup(p => p
            .AddUnmatched("aria-label", "Text alignment")
            .AddUnmatched("data-testid", "group"));

        var root = cut.Find($".{Css.Classes.ToggleGroup.Root}");
        Assert.Equal("group", root.GetAttribute("role"));
        Assert.Equal("Text alignment", root.GetAttribute("aria-label"));
        Assert.Equal("group", root.GetAttribute("data-testid"));
    }

    [Fact]
    public void AriaLabelledBy_NamesTheGroupFromEitherSource()
    {
        var cut = RenderGroup(p => p.Add(x => x.AriaLabelledBy, "alignment-heading"));
        Assert.Equal("alignment-heading",
            cut.Find($".{Css.Classes.ToggleGroup.Root}").GetAttribute("aria-labelledby"));

        var withAttribute = RenderGroup(p => p.AddUnmatched("aria-labelledby", "filter-heading"));
        Assert.Equal("filter-heading",
            withAttribute.Find($".{Css.Classes.ToggleGroup.Root}").GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void AnExplicitAriaLabel_WinsOverAnUnmatchedOneAndIsNotPaintedTwice()
    {
        var cut = RenderGroup(p => p
            .Add(x => x.AriaLabel, "Explicit")
            .AddUnmatched("aria-label", "Unmatched"));

        var roots = cut.FindAll($".{Css.Classes.ToggleGroup.Root}");
        Assert.Single(roots);
        Assert.Equal("Explicit", roots[0].GetAttribute("aria-label"));
    }

    [Fact]
    public void TheButtonsKeepTheirPressedStateAndTheGroupIsNotARadioGroup()
    {
        var cut = RenderGroup(p => p
            .Add(x => x.MultiSelect, true)
            .Add(x => x.Values, new[] { "a" }));

        Assert.Empty(cut.FindAll("[role=radio]"));
        var buttons = cut.FindAll($"button.{Css.Classes.Button.Root}");
        Assert.Equal(2, buttons.Count);
        Assert.Equal("true", buttons[0].GetAttribute("aria-pressed"));
        Assert.Equal("false", buttons[1].GetAttribute("aria-pressed"));
    }
}
