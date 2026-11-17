namespace Flare.Components;

// The programmatic point-open half of the menu: a host that owns one shared context menu - a tree
// page opening it from a row's OnContextMenu - raises the panel at the pointer without an activator
// of its own. The lifecycle in FlareMenu.razor reads the two flags below to capture the element the
// user was on and to hand focus back to it when the panel closes.
/// <summary>
/// A dropdown menu: an activator slot opens a panel of <see cref="FlareMenuItem"/> entries with
/// arrow-key navigation and submenus, and a host can also open it at an explicit pointer position for
/// a context menu it owns itself.
/// </summary>
public partial class FlareMenu
{
    // Set by OpenAtAsync: the panel's point is explicit and there is no activator interaction behind it.
    private bool _openedAtPoint;
    // Set once the element that held focus before a point-opened panel took it has been remembered in
    // the browser. Its close hands focus back to that element instead of to the activator, which a
    // host-owned menu does not have.
    private bool _rememberedFocus;

    /// <summary>
    /// Opens the menu with its panel pinned to a point in viewport coordinates - a pointer event's
    /// <c>ClientX</c> and <c>ClientY</c> - so a host that owns a shared context menu (the tree row's
    /// <c>OnContextMenu</c>, for example) can raise it without an activator of its own. No-op when the
    /// menu is already open.
    /// <para>
    /// The point is honoured regardless of <see cref="PositionAtCursor"/>, which only governs the
    /// activator click. The panel otherwise opens like any other: the same placement engine flips and
    /// clamps it inside the viewport and raises it into the top layer, the backdrop and Escape close it,
    /// and a top-level menu in a <see cref="FlareMenuBarContext"/> joins the bar's coordination. Raises
    /// <see cref="OnToggle"/>.
    /// </para>
    /// <para>
    /// The panel takes focus like any other open. When it closes - unless an item's action moved focus
    /// elsewhere - focus returns to the element that held it before the menu opened, as long as that
    /// element is still in the document.
    /// </para>
    /// </summary>
    /// <param name="clientX">Horizontal pointer position in CSS pixels from the viewport's left edge; the
    /// same value as <c>MouseEventArgs.ClientX</c>.</param>
    /// <param name="clientY">Vertical pointer position in CSS pixels from the viewport's top edge.</param>
    /// <exception cref="ArgumentOutOfRangeException">A coordinate is not a finite number.</exception>
    public async Task OpenAtAsync(double clientX, double clientY)
    {
        if (!double.IsFinite(clientX))
            throw new ArgumentOutOfRangeException(nameof(clientX), clientX, "The coordinate must be a finite number.");
        if (!double.IsFinite(clientY))
            throw new ArgumentOutOfRangeException(nameof(clientY), clientY, "The coordinate must be a finite number.");
        if (_open) return;
        _cursorX = clientX;
        _cursorY = clientY;
        // Opened from code at an explicit point: no keyboard origin and no activator interaction.
        _openedByKeyboard = false;
        _openedByHover = false;
        _openedAtPoint = true;
        _open = true;
        if (MenuBarContext is { } bar) await bar.NotifyOpenedAsync(this);
        await OnToggle.InvokeAsync(true);
        StateHasChanged();
    }
}
