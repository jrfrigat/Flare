using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class TagFieldStateChangeTests : FlareTestContext
{
    private static string OptionSelector => $".{Css.Classes.Listbox.Option}";

    [Fact]
    public void SwitchingReadOnly_ClosesLocalSuggestions()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, new[] { "alpha", "beta" })
            .Add(x => x.MinChars, 0));

        cut.Find("input").Focus();
        Assert.NotEmpty(cut.FindAll(OptionSelector));

        cut.Render(p => p.Add(x => x.ReadOnly, true));

        Assert.Empty(cut.FindAll(OptionSelector));
    }

    [Fact]
    public async Task AsyncResults_AfterSwitchingReadOnly_AreDiscarded()
    {
        var gate = new TaskCompletionSource<IEnumerable<string>>();
        var changes = 0;
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.DebounceMs, 0)
            .Add(x => x.SearchFunc, (string _, CancellationToken _) => gate.Task)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, (IReadOnlyList<string> _) => changes++)));

        cut.Find("input").Focus();
        cut.Find("input").Input("al");
        cut.Render(p => p.Add(x => x.ReadOnly, true));
        gate.SetResult(["alpha"]);
        await Task.Delay(50);

        Assert.Empty(cut.FindAll(OptionSelector));
        Assert.Equal(0, changes);
    }

    [Fact]
    public async Task AsyncResults_AfterSwitchingDisabled_AreDiscarded()
    {
        var gate = new TaskCompletionSource<IEnumerable<string>>();
        var changes = 0;
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.DebounceMs, 0)
            .Add(x => x.SearchFunc, (string _, CancellationToken _) => gate.Task)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, (IReadOnlyList<string> _) => changes++)));

        cut.Find("input").Focus();
        cut.Find("input").Input("al");
        cut.Render(p => p.Add(x => x.Disabled, true));
        gate.SetResult(["alpha"]);
        await Task.Delay(50);

        Assert.Empty(cut.FindAll(OptionSelector));
        Assert.Equal(0, changes);
    }
}
