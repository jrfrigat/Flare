using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class MultiSelectDynamicPolicyTests : FlareTestContext
{
    private static string OptionSelector => $".{Css.Classes.Multiselect.Option}";

    [Fact]
    public void MaxSelections_ChangedAfterFirstRender_AppliesToNewPicks()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b", "c" })
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, (IReadOnlyList<string> v) => captured.Add(v))));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        cut.FindAll(OptionSelector)[0].Click();
        cut.Render(p => p.Add(x => x.Values, captured[^1]));

        cut.FindAll(OptionSelector)[1].Click();
        cut.Render(p => p
            .Add(x => x.Values, captured[^1])
            .Add(x => x.MaxSelections, 2));
        Assert.Equal(2, captured[^1].Count);

        var calls = captured.Count;
        cut.FindAll(OptionSelector)[2].Click();

        Assert.Equal(calls, captured.Count);
        Assert.Equal(2, captured[^1].Count);
    }
}
