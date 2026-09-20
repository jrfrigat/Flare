namespace Flare.Components;

/// <summary>
/// What <see cref="FlareMenu"/> hands to its activator slot: the ARIA a menu button has to carry, plus
/// whether the menu is open. The attributes belong on the element that actually takes the focus - the
/// caller's own button - because ARIA on the wrapper around it announces nothing, and only the caller
/// knows which element that is. Spread them with <c>@attributes="context.Attributes"</c>.
/// </summary>
public sealed class FlareMenuActivatorContext
{
    // Built once per open/close rather than per render: the dictionary is handed to attribute splatting,
    // which only needs a new instance when one of the three values actually changes.
    internal FlareMenuActivatorContext(bool open, string panelId)
    {
        Open = open;
        Attributes = new Dictionary<string, object>(3)
        {
            ["aria-haspopup"] = "menu",
            ["aria-expanded"] = open ? "true" : "false",
            // Named even while the panel does not exist: a reference to a missing element is allowed
            // precisely when aria-expanded is false, and a key that comes and goes costs an attribute
            // diff on every open.
            ["aria-controls"] = panelId,
        };
    }

    /// <summary>
    /// Whether the menu panel is open, for an activator that shows it - a rotated chevron, a pressed
    /// look. The menu owns this state, so reading it here saves mirroring
    /// <see cref="FlareMenu.OnToggle"/> into a field of the caller's own.
    /// </summary>
    public bool Open { get; }

    /// <summary>
    /// The ARIA attributes to spread onto the focusable activator element: <c>aria-haspopup</c>,
    /// <c>aria-expanded</c> and <c>aria-controls</c>. An activator that does not spread them renders
    /// and works, but announces neither the popup nor its state.
    /// </summary>
    public IReadOnlyDictionary<string, object> Attributes { get; }
}
