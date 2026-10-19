namespace Flare.Components.Tests;

public class ComboboxDynamicPolicyTests : FlareTestContext
{
    private static string OptionSelector => $".{Css.Classes.Listbox.Option}";
    private static string HeaderSelector => $".{Css.Classes.Listbox.GroupHeader}";

    private static string[] Options(IRenderedComponent<FlareCombobox<string>> cut)
        => cut.FindAll(OptionSelector).Select(o => o.TextContent.Trim()).ToArray();

    [Fact]
    public void GroupBy_ChangedAfterFirstRender_RegroupsTheOptions()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, new[] { "a1", "a2", "b1" }));

        cut.Find("input").Focus();
        Assert.Empty(cut.FindAll(HeaderSelector));

        cut.Render(p => p.Add(x => x.GroupBy, (string s) => s[..1]));

        Assert.Equal(2, cut.FindAll(HeaderSelector).Count);
    }

    [Fact]
    public void RankFunc_ChangedAfterFirstRender_ReranksTheMatches()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, new[] { "beta", "alpha" }));

        cut.Find("input").Focus();
        cut.Find("input").Input("a");
        cut.WaitForAssertion(() => Assert.Equal(["beta", "alpha"], Options(cut)));

        cut.Render(p => p.Add(x => x.RankFunc,
            (string s, string q) => s.StartsWith(q, StringComparison.OrdinalIgnoreCase) ? 10 : 1));

        cut.WaitForAssertion(() => Assert.Equal(["alpha", "beta"], Options(cut)));
    }
}
