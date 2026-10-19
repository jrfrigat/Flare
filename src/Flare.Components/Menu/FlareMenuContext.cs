namespace Flare.Components;

/// <summary>
/// Stable per-menu callbacks shared by <see cref="FlareMenu"/> / <see cref="FlareSubMenu"/> with their
/// <see cref="FlareMenuItem"/> children through a single cascading value: closing the menu and keeping
/// the keyboard registry in step with the rendered items. The currently-focused item stays a separate
/// cascade because it changes as focus moves, whereas these callbacks are constant for the menu's
/// lifetime.
/// </summary>
/// <param name="Close">Closes the (sub)menu.</param>
/// <param name="Register">Registers a menu item so the parent can drive roving-tabindex focus.</param>
/// <param name="Unregister">Removes an item that left the tree, so arrows and Enter never target a stale element.</param>
internal sealed record FlareMenuContext(
    Func<Task> Close, Action<IMenuEntry> Register, Action<IMenuEntry> Unregister);
