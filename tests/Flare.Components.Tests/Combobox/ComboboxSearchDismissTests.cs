namespace Flare.Components.Tests;

public class ComboboxSearchDismissTests : FlareTestContext
{
    private static string OptionSelector => $".{Css.Classes.Listbox.Option}";

    private IRenderedComponent<FlareCombobox<string>> RenderPending(TaskCompletionSource<IEnumerable<string>> gate)
        => Render<FlareCombobox<string>>(p => p
            .Add(x => x.DebounceMs, 0)
            .Add(x => x.SearchFunc, (string _, CancellationToken _) => gate.Task));

    [Fact]
    public async Task Escape_WhileSearchIsPending_DoesNotReopen()
    {
        var gate = new TaskCompletionSource<IEnumerable<string>>();
        var cut = RenderPending(gate);
        cut.Find("input").Focus();
        cut.Find("input").Input("al");
        cut.Find("input").KeyDown("Escape");

        gate.SetResult(["alpha"]);
        await Task.Delay(60, Xunit.TestContext.Current.CancellationToken);

        Assert.Empty(cut.FindAll(OptionSelector));
    }

    [Fact]
    public async Task ReadOnly_WhileSearchIsPending_DoesNotReopen()
    {
        var gate = new TaskCompletionSource<IEnumerable<string>>();
        var cut = RenderPending(gate);
        cut.Find("input").Focus();
        cut.Find("input").Input("al");
        cut.Render(p => p.Add(x => x.ReadOnly, true));

        gate.SetResult(["alpha"]);
        await Task.Delay(60, Xunit.TestContext.Current.CancellationToken);

        Assert.Empty(cut.FindAll(OptionSelector));
    }

    [Fact]
    public async Task Disabled_WhileSearchIsPending_DoesNotReopen()
    {
        var gate = new TaskCompletionSource<IEnumerable<string>>();
        var cut = RenderPending(gate);
        cut.Find("input").Focus();
        cut.Find("input").Input("al");
        cut.Render(p => p.Add(x => x.Disabled, true));

        gate.SetResult(["alpha"]);
        await Task.Delay(60, Xunit.TestContext.Current.CancellationToken);

        Assert.Empty(cut.FindAll(OptionSelector));
    }
}
