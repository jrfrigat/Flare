// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareShortcutEntry  (3 tests)
// -----------------------------------------------------------------------------

public class FlareShortcutEntryTests : FlareTestContext
{
    [Fact]
    public void RendersInsideShortcutsParent_WithKeys()
    {
        // FlareShortcutEntry is code-only, verify it mounts without throwing
        var cut = Render<FlareShortcuts>(p => p
            .AddChildContent<FlareShortcutEntry>(ep => ep
                .Add(x => x.Keys, "ctrl+k")
                .Add(x => x.OnActivated, EventCallback.Factory.Create(this, () => { }))));

        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void RendersWithKeysAndDescription()
    {
        // FlareShortcutEntry itself renders no HTML (code-only) - verify no throw
        var cut = Render<FlareShortcuts>(p => p
            .AddChildContent<FlareShortcutEntry>(ep => ep
                .Add(x => x.Keys, "ctrl+s")
                .Add(x => x.Description, "Save file")
                .Add(x => x.OnActivated, EventCallback.Factory.Create(this, () => { }))));

        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void DescriptionParam_DoesNotThrow()
    {
        // Verify Description parameter is accepted and component renders
        var cut = Render<FlareShortcuts>(p => p
            .AddChildContent<FlareShortcutEntry>(ep => ep
                .Add(x => x.Keys, "ctrl+d")
                .Add(x => x.Description, "Delete selected")
                .Add(x => x.OnActivated, EventCallback.Factory.Create(this, () => { }))));

        // ShortcutEntry is renderless - the parent Shortcuts renders its child content
        var entry = cut.FindComponent<FlareShortcutEntry>();
        Assert.Equal("Delete selected", entry.Instance.Description);
    }
}
