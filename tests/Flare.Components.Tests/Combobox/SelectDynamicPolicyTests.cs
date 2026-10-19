namespace Flare.Components.Tests;

public class SelectDynamicPolicyTests : FlareTestContext
{
    private static string HeaderSelector => $".{Css.Classes.Listbox.GroupHeader}";

    [Fact]
    public void GroupBy_ChangedAfterFirstRender_RegroupsTheOptions()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a1", "a2", "b1" }));

        cut.Find($".{Css.Classes.Select.Control}").Click();
        Assert.Empty(cut.FindAll(HeaderSelector));

        cut.Render(p => p.Add(x => x.GroupBy, (string s) => s[..1]));

        Assert.Equal(2, cut.FindAll(HeaderSelector).Count);
    }
}
