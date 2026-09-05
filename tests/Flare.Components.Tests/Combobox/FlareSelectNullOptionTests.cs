using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Combobox;

// The "no value" row: a filter select has to be able to say "All" without a sentinel item of type T
// leaking into the caller's model.
public class FlareSelectNullOptionTests : FlareTestContext
{
    private static readonly string[] _items = ["Flour", "Sugar", "Butter"];

    private IRenderedComponent<FlareSelect<string>> RenderSelect(
        string nullOption = "All ingredients", string? value = null, bool searchable = false) =>
        Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Value, value)
            .Add(x => x.Searchable, searchable)
            .Add(x => x.NullOption, nullOption));

    [Fact]
    public void NullOption_AddsARowAtTheTopOfTheList()
    {
        var cut = RenderSelect();
        cut.Find($".{Css.Classes.Select.Control}").Click();

        var options = cut.FindAll("[role=option]");
        Assert.Equal(4, options.Count);
        Assert.Contains("All ingredients", options[0].TextContent);
    }

    [Fact]
    public void ClosedField_ShowsTheNullOptionTextInsteadOfEmptiness()
    {
        var cut = RenderSelect();
        Assert.Contains("All ingredients", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    [Fact]
    public void NullOption_SuppressesThePlaceholder()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Placeholder, "Pick one")
            .Add(x => x.NullOption, "All ingredients"));

        Assert.Empty(cut.FindAll($".{Css.Classes.Input.Placeholder}"));
        Assert.Contains("All ingredients", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    [Fact]
    public void WithoutANullOption_ThePlaceholderStillShows()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Placeholder, "Pick one"));

        Assert.Single(cut.FindAll($".{Css.Classes.Input.Placeholder}"));
    }

    [Fact]
    public void SelectingTheNullRow_SetsTheValueToNull()
    {
        string? committed = "Sugar";
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Value, "Sugar")
            .Add(x => x.NullOption, "All ingredients")
            .Add(x => x.ValueChanged, (string? v) => committed = v));

        cut.Find($".{Css.Classes.Select.Control}").Click();
        cut.FindAll("[role=option]")[0].Click();

        Assert.Null(committed);
    }

    [Fact]
    public void TheNullRow_RendersAsSelectedWhenTheValueIsNull()
    {
        var cut = RenderSelect();
        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal("true", cut.FindAll("[role=option]")[0].GetAttribute("aria-selected"));
    }

    [Fact]
    public void TheNullRow_SurvivesASearchThatMatchesNothingElse()
    {
        var cut = RenderSelect(searchable: true);
        cut.Find($".{Css.Classes.Select.Control}").Click();
        cut.Find($"input.{Css.Classes.Select.Search}").Input("zzz");

        var options = cut.FindAll("[role=option]");
        Assert.Single(options);
        Assert.Contains("All ingredients", options[0].TextContent);
    }

    [Fact]
    public void TheNullRow_StaysAtTheTopWhileFiltering()
    {
        var cut = RenderSelect(searchable: true);
        cut.Find($".{Css.Classes.Select.Control}").Click();
        cut.Find($"input.{Css.Classes.Select.Search}").Input("ar");

        var options = cut.FindAll("[role=option]");
        Assert.Contains("All ingredients", options[0].TextContent);
        Assert.Contains(options.Skip(1), o => o.TextContent.Contains("Sugar"));
    }

    [Fact]
    public void NullOptionTemplate_RendersInBothTheListAndTheField()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.NullOption, "All")
            .Add(x => x.NullOptionTemplate, (RenderFragment)(b => b.AddMarkupContent(0, "<em class=\"all-row\">All</em>"))));

        Assert.Single(cut.FindAll($".{Css.Classes.Select.Value} em.all-row"));
        cut.Find($".{Css.Classes.Select.Control}").Click();
        Assert.Contains(cut.FindAll("[role=option] em.all-row"), _ => true);
    }

    [Fact]
    public void NullOption_OnANonNullableValueType_Throws()
    {
        // default(int) is 0, a real value: a "no value" row cannot exist, so this is a caller mistake
        // that has to surface immediately rather than silently selecting zero.
        var ex = Assert.Throws<InvalidOperationException>(() =>
            Render<FlareSelect<int>>(p => p
                .Add(x => x.Items, new[] { 1, 2, 3 })
                .Add(x => x.NullOption, "Any")));

        Assert.Contains("nullable TValue", ex.Message);
    }

    [Fact]
    public void NullOption_OnANullableValueType_Works()
    {
        var cut = Render<FlareSelect<int?>>(p => p
            .Add(x => x.Items, new int?[] { 1, 2, 3 })
            .Add(x => x.NullOption, "Any"));

        cut.Find($".{Css.Classes.Select.Control}").Click();
        Assert.Equal(4, cut.FindAll("[role=option]").Count);
    }
}
