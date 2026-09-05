namespace Flare.Components.Tests;

public class FlareSelectGroupByTests : FlareTestContext
{
    [Fact]
    public void RendersRootSelectElement_WithoutGroupBy()
    {
        var cut = Render<FlareSelect<string>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Select.Root}"));
    }

    [Fact]
    public void GroupBy_Param_ExistsAndRendersWithoutError()
    {
        var items = new[] { "Apple", "Avocado", "Banana" };
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.GroupBy, (Func<string, string>)(item => item.StartsWith("A") ? "A" : "B")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Select.Root}"));
    }

    [Fact]
    public void Items_Render_WithoutGroupBy()
    {
        var items = new[] { "One", "Two", "Three" };
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, items));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Select.Option}").Count);
    }

    [Fact]
    public void Disabled_RendersDisabledControl()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains(Css.Classes.Input.Disabled, cut.Find($".{Css.Classes.Select.Root}").ClassName);
    }

    [Fact]
    public void Label_RendersLabel()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Label, "Choose option"));

        Assert.Contains("Choose option", cut.Find($".{Css.Classes.Input.Label}").TextContent);
    }
}
