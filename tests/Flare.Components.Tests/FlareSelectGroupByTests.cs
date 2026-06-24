// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareSelect GroupBy  (5 tests)
// -----------------------------------------------------------------------------

public class FlareSelectGroupByTests : FlareTestContext
{
    [Fact]
    public void RendersRootSelectElement_WithoutGroupBy()
    {
        var cut = RenderComponent<FlareSelect<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-select"));
    }

    [Fact]
    public void GroupBy_Param_ExistsAndRendersWithoutError()
    {
        var items = new[] { "Apple", "Avocado", "Banana" };
        var cut = RenderComponent<FlareSelect<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.GroupBy, (Func<string, string>)(item => item.StartsWith("A") ? "A" : "B")));

        Assert.NotEmpty(cut.FindAll(".flare-select"));
    }

    [Fact]
    public void Items_Render_WithoutGroupBy()
    {
        var items = new[] { "One", "Two", "Three" };
        var cut = RenderComponent<FlareSelect<string>>(p => p
            .Add(x => x.Items, items));

        cut.Find(".flare-select__control").Click();

        Assert.Equal(3, cut.FindAll(".flare-select__option").Count);
    }

    [Fact]
    public void Disabled_RendersDisabledControl()
    {
        var cut = RenderComponent<FlareSelect<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains("flare-select--disabled", cut.Find(".flare-select").ClassName);
    }

    [Fact]
    public void Label_RendersLabel()
    {
        var cut = RenderComponent<FlareSelect<string>>(p => p
            .Add(x => x.Label, "Choose option"));

        Assert.Contains("Choose option", cut.Find(".flare-select__label").TextContent);
    }
}
