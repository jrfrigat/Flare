using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// Speed-dial state shared by <see cref="FlareFloatingActionButton"/> with its child
/// <see cref="FlareFloatingActionMenu"/> / <see cref="FlareFloatingActionMenuItem"/> through a single
/// cascading value: whether the menu is open, how it closes, and the stable ids and item order that
/// connect the trigger to the list and drive its keyboard focus.
/// </summary>
/// <param name="Open">Whether the speed-dial menu is currently open.</param>
/// <param name="Close">Closes the speed-dial menu; the argument asks for focus to return to the trigger.</param>
/// <param name="MenuId">Stable id of the action list, referenced by the trigger's <c>aria-controls</c>.</param>
/// <param name="TriggerId">Stable id of the trigger, which names the action list through
/// <c>aria-labelledby</c>.</param>
/// <param name="Items">The actions in declaration order, through which focus moves.</param>
internal sealed record FabMenuContext(
    bool Open, Action<bool> Close, string MenuId, string TriggerId, FabMenuItems Items);

/// <summary>
/// The actions of a speed dial in declaration order. Focus moves over the enabled ones only, wrapping
/// at either end, so a disabled action is never a hidden stop on the way through the menu.
/// </summary>
internal sealed class FabMenuItems
{
    private readonly List<FlareFloatingActionMenuItem> _items = [];

    /// <summary>Adds an action that has entered the tree.</summary>
    /// <param name="item">The action to add.</param>
    internal void Register(FlareFloatingActionMenuItem item)
    {
        if (!_items.Contains(item)) _items.Add(item);
    }

    /// <summary>Removes an action that left the tree, so focus never targets a stale element.</summary>
    /// <param name="item">The action to remove.</param>
    internal void Unregister(FlareFloatingActionMenuItem item) => _items.Remove(item);

    /// <summary>Focuses the first or last enabled action.</summary>
    /// <param name="first">True for the first enabled action, false for the last.</param>
    internal ValueTask FocusEdgeAsync(bool first)
    {
        var enabled = Enabled();
        return enabled.Count == 0
            ? ValueTask.CompletedTask
            : FocusAsync(first ? enabled[0] : enabled[^1]);
    }

    /// <summary>Focuses the enabled action after (or before) <paramref name="from"/>, wrapping at the
    /// ends. An action that is no longer in the list enters at the matching end.</summary>
    /// <param name="from">The action that currently holds focus.</param>
    /// <param name="step">+1 for the next action, -1 for the previous one.</param>
    internal ValueTask MoveAsync(FlareFloatingActionMenuItem from, int step)
    {
        var enabled = Enabled();
        if (enabled.Count == 0) return ValueTask.CompletedTask;

        var index = enabled.IndexOf(from);
        var next = index < 0
            ? (step > 0 ? 0 : enabled.Count - 1)
            : (index + step + enabled.Count) % enabled.Count;
        return FocusAsync(enabled[next]);
    }

    private List<FlareFloatingActionMenuItem> Enabled() => _items.Where(item => !item.Disabled).ToList();

    private static async ValueTask FocusAsync(FlareFloatingActionMenuItem item)
    {
        try { await item.FocusAsync(); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
        catch (InvalidOperationException) { }
    }
}
