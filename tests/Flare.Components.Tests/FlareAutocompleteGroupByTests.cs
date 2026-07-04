// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareAutocomplete GroupBy  (5 tests)
// -----------------------------------------------------------------------------

public class FlareAutocompleteGroupByTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement_WithoutGroupBy()
    {
        var cut = Render<FlareAutocomplete<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-autocomplete"));
    }

    [Fact]
    public void GroupBy_Param_ExistsAndRendersWithoutError()
    {
        var items = new[] { "Apple", "Avocado", "Banana" };
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.GroupBy, (Func<string, string>)(item => item.StartsWith("A") ? "A" : "B")));

        Assert.NotEmpty(cut.FindAll(".flare-autocomplete"));
    }

    [Fact]
    public void RendersInputElement()
    {
        var cut = Render<FlareAutocomplete<string>>();

        Assert.NotEmpty(cut.FindAll("input.flare-input__control"));
    }

    [Fact]
    public void Disabled_RendersDisabledInput()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void Label_RendersLabel()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Label, "Search items"));

        Assert.Contains("Search items", cut.Find(".flare-input__label").TextContent);
    }
}
