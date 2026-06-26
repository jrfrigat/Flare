namespace Flare.Components;

/// <summary>
/// Stable per-menu callbacks shared by <see cref="FlareMenu"/> / <see cref="FlareSubMenu"/> with their
/// <see cref="FlareMenuItem"/> children through a single cascading value: closing the menu and
/// registering an item for keyboard navigation. Replaces the previous stack of individually-named
/// cascades. The currently-focused item stays a separate cascade because it changes as focus moves,
/// whereas these two callbacks are constant for the menu's lifetime.
/// </summary>
/// <param name="Close">Closes the (sub)menu.</param>
/// <param name="Register">Registers a menu item so the parent can drive roving-tabindex focus.</param>
internal sealed record FlareMenuContext(Func<Task> Close, Action<FlareMenuItem> Register);
