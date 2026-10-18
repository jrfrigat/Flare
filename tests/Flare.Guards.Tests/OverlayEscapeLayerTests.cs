using System.Runtime.CompilerServices;

namespace Flare.Guards.Tests;

/// <summary>
/// Escape ordering is owned by the overlay engine: a popup registers itself as a dismissible layer when it opens,
/// and the topmost layer consumes the key. The rule used to be inferred from <c>aria-expanded</c> on the element
/// under the focus, which failed for a menu that moves focus into its own panel. This guard keeps a new popup from
/// shipping beside that mechanism: every component that renders a popup owner (<c>aria-haspopup</c>, a menu or a
/// listbox) must register through the shared path or be listed here with the reason it does not need to.
/// </summary>
public sealed class OverlayEscapeLayerTests
{
    // Components that render a popup-like role but are not independent dismissible layers, with the reason.
    private static readonly Dictionary<string, string> Allowed = new(StringComparer.Ordinal)
    {
        ["FlareSubMenu.razor"] = "part of a FlareMenu; the parent registers the layer",
        ["FlareOptionList.razor"] = "rendered inside FlarePopup, which registers the layer",
        ["FlareDataGrid.razor"] = "uses DataGridColumnPicker; tracked by its own card",
        ["DataGridColumnPicker.razor"] = "known gap: TASK-607",
        ["FlareFloatingActionMenu.razor"] = "known gap: TASK-607 (CSS menu owned by FlareFloatingActionButton)",
        ["FlareCodeBlock.razor"] = "role=menu is not a popup here",
    };

    [Fact]
    public void EveryPopupOwnerRegistersTheSharedEscapeLayer()
    {
        var root = FindRepoRoot();
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(Path.Combine(root, "src"), "*.razor", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                || file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                continue;

            var markup = File.ReadAllText(file);
            if (!RendersPopupOwner(markup)) continue;

            var name = Path.GetFileName(file);
            if (Allowed.ContainsKey(name)) continue;

            var source = markup + ReadCodeBehind(file);
            if (source.Contains("PushPopupLayerAsync", StringComparison.Ordinal)
                || source.Contains("<FlarePopup", StringComparison.Ordinal)
                || source.Contains("PickerPopup", StringComparison.Ordinal))
                continue;

            offenders.Add($"{Path.GetRelativePath(root, file)}");
        }

        Assert.True(offenders.Count == 0,
            "These components render a popup owner but never register an Escape layer, so Escape can close a "
            + "dialog underneath their popup (or be swallowed by it): " + string.Join(", ", offenders)
            + ". Register through IOverlayJsService.PushPopupLayerAsync in the open/close/dispose lifecycle "
            + "(or build on FlarePopup/PickerPopup), or add the file to Allowed with the reason.");
    }

    private static bool RendersPopupOwner(string markup) =>
        markup.Contains("aria-haspopup", StringComparison.Ordinal)
        || markup.Contains("role=\"menu\"", StringComparison.Ordinal)
        || markup.Contains("role=\"listbox\"", StringComparison.Ordinal);

    // A razor component's registration usually lives in its partial class beside the markup.
    private static string ReadCodeBehind(string razorFile)
    {
        var path = razorFile + ".cs";
        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    private static string FindRepoRoot([CallerFilePath] string thisFile = "")
    {
        var dir = Path.GetDirectoryName(thisFile);
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src")))
            dir = Path.GetDirectoryName(dir);
        Assert.False(dir is null, "Could not locate the repository root (no ancestor 'src' folder).");
        return dir!;
    }
}
