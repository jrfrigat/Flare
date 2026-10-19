namespace Flare.Components.Tests;

public class ComboboxAsyncSourceTests : FlareTestContext
{
    private static string OptionSelector => $".{Css.Classes.Listbox.Option}";

    [Fact]
    public async Task ParentRender_KeepsAsyncResultsAndHighlight()
    {
        var gate = new TaskCompletionSource<IEnumerable<string>>();
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.DebounceMs, 0)
            .Add(x => x.SearchFunc, (string _, CancellationToken _) => gate.Task));

        cut.Find("input").Focus();
        cut.Find("input").Input("al");
        gate.SetResult(["alpha", "beta"]);
        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll(OptionSelector).Count));
        var active = cut.Find("input").GetAttribute("aria-activedescendant");
        Assert.False(string.IsNullOrEmpty(active));

        cut.Render(p => p.Add(x => x.Label, "Renamed"));

        Assert.Equal(2, cut.FindAll(OptionSelector).Count);
        Assert.Equal(active, cut.Find("input").GetAttribute("aria-activedescendant"));
    }

    [Fact]
    public async Task AsyncSearch_TakesPrecedenceOverItemsOnParentRender()
    {
        var gate = new TaskCompletionSource<IEnumerable<string>>();
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, new[] { "local" })
            .Add(x => x.DebounceMs, 0)
            .Add(x => x.SearchFunc, (string _, CancellationToken _) => gate.Task));

        cut.Find("input").Focus();
        cut.Find("input").Input("re");
        gate.SetResult(["remote"]);
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(OptionSelector)));

        cut.Render(p => p.Add(x => x.Label, "Renamed"));

        var option = Assert.Single(cut.FindAll(OptionSelector));
        Assert.Equal("remote", option.TextContent.Trim());
    }

    [Fact]
    public async Task StaleSearch_DoesNotReplaceTheNewerResults()
    {
        var first = new TaskCompletionSource<IEnumerable<string>>();
        var second = new TaskCompletionSource<IEnumerable<string>>();
        var call = 0;
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.DebounceMs, 0)
            .Add(x => x.SearchFunc, (string _, CancellationToken _) => ++call == 1 ? first.Task : second.Task));

        cut.Find("input").Focus();
        cut.Find("input").Input("a");
        cut.Find("input").Input("ab");
        second.SetResult(["ab-result"]);
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(OptionSelector)));

        first.SetResult(["a-result"]);

        var option = Assert.Single(cut.FindAll(OptionSelector));
        Assert.Equal("ab-result", option.TextContent.Trim());
    }
}
