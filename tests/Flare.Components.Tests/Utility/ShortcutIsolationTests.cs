using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public sealed class ShortcutIsolationTests : FlareTestContext
{
    [Fact]
    public async Task DispatchInvokesOnlyItsOwningScope()
    {
        var firstCount = 0;
        var secondCount = 0;
        var first = Render<FlareShortcuts>(p => p.AddChildContent<FlareShortcutEntry>(e => e
            .Add(x => x.Keys, "ctrl+k").Add(x => x.OnActivated, () => firstCount++)));
        var second = Render<FlareShortcuts>(p => p.AddChildContent<FlareShortcutEntry>(e => e
            .Add(x => x.Keys, "ctrl+k").Add(x => x.OnActivated, () => secondCount++)));
        await first.InvokeAsync(() => first.Instance.HandleKeyDown("ctrl+k"));
        Assert.Equal(1, firstCount);
        Assert.Equal(0, secondCount);
        await second.InvokeAsync(() => second.Instance.HandleKeyDown("ctrl+k"));
        Assert.Equal(1, firstCount);
        Assert.Equal(1, secondCount);
    }

    [Fact]
    public async Task ChangedKeysAndDisposedEntriesDoNotLeaveStaleHandlers()
    {
        var count = 0;
        var cut = Render<FlareShortcuts>(p => p.AddChildContent<FlareShortcutEntry>(e => e
            .Add(x => x.Keys, "ctrl+k").Add(x => x.OnActivated, () => count++)));
        var entry = cut.FindComponent<FlareShortcutEntry>();
        entry.Render(p => p.Add(x => x.Keys, "ctrl+m"));
        await cut.InvokeAsync(() => cut.Instance.HandleKeyDown("ctrl+k"));
        Assert.Equal(0, count);
        await cut.InvokeAsync(() => cut.Instance.HandleKeyDown("ctrl+m"));
        Assert.Equal(1, count);
        cut.Render(p => p.AddChildContent(""));
        await cut.InvokeAsync(() => cut.Instance.HandleKeyDown("ctrl+m"));
        Assert.Equal(1, count);
    }
}
