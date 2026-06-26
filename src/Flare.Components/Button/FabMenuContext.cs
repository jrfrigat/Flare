namespace Flare.Components;

/// <summary>
/// Speed-dial state shared by <see cref="FlareFloatingActionButton"/> with its child
/// <see cref="FlareFloatingActionMenu"/> / <see cref="FlareFloatingActionMenuItem"/> through a single
/// cascading value: whether the menu is open, and the callback that closes it. Replaces the previous
/// pair of individually-named cascades.
/// </summary>
/// <param name="Open">Whether the speed-dial menu is currently open.</param>
/// <param name="Close">Closes the speed-dial menu.</param>
internal sealed record FabMenuContext(bool Open, Action Close);
