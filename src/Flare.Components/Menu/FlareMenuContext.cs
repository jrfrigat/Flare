namespace Flare.Components;

/// <summary>
/// Stable per-menu callbacks shared by <see cref="FlareMenu"/> / <see cref="FlareSubMenu"/> with their
/// <see cref="FlareMenuItem"/> children through a single cascading value: closing the menu and keeping
/// the keyboard registry in step with the rendered items. The currently-focused item stays a separate
/// cascade because it changes as focus moves, whereas these callbacks are constant for the menu's
/// lifetime.
/// </summary>
/// <param name="Close">Closes the (sub)menu, restoring focus to its activator - or, for a menu opened by
/// code at a pointer position, to the element that held focus before the panel took it.</param>
/// <param name="CloseFromItem">Closes the (sub)menu after one of its items ran its action; focus returns to
/// the activator only if the action did not move it elsewhere.</param>
/// <param name="Register">Registers a menu item so the parent can drive roving-tabindex focus.</param>
/// <param name="Unregister">Removes an item that left the tree, so arrows and Enter never target a stale element.</param>
internal sealed record FlareMenuContext(
    Func<Task> Close, Func<Task> CloseFromItem, Action<IMenuEntry> Register, Action<IMenuEntry> Unregister);
