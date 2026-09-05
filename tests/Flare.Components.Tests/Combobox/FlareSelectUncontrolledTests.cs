using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareSelectUncontrolledTests : FlareTestContext
{
    private static readonly string[] _items = ["Alpha", "Beta", "Gamma"];

    [Fact]
    public void Select_Uncontrolled_ShowsSelectionWithoutBinding()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Items, _items));

        cut.Find($".{Css.Classes.Select.Control}").Click();
        cut.FindAll($".{Css.Classes.Select.Option}")[1].Click();   // pick "Beta"

        Assert.Contains("Beta", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    [Fact]
    public void Select_Uncontrolled_SeedsFromOneWayValue()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Value, "Gamma"));

        Assert.Contains("Gamma", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    [Fact]
    public void MultiSelect_Uncontrolled_AccumulatesSelectionWithoutBinding()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Items, _items));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        cut.FindAll($".{Css.Classes.Multiselect.Option}")[0].Click();   // Alpha
        cut.FindAll($".{Css.Classes.Multiselect.Option}")[2].Click();   // Gamma

        var shown = cut.Find($".{Css.Classes.Multiselect.Value}").TextContent;
        Assert.Contains("Alpha", shown);
        Assert.Contains("Gamma", shown);
    }
}
