namespace Flare.Components;

/// <summary>How a <see cref="FlarePopover"/> is opened.</summary>
public enum PopoverTrigger
{
    /// <summary>The consumer drives <c>Open</c>/<c>OpenChanged</c> (default) -- no built-in handler.</summary>
    Manual,

    /// <summary>Clicking the anchor toggles the popover open and closed.</summary>
    Click,

    /// <summary>Hovering the anchor opens the popover (after <see cref="FlarePopover.Delay"/>) and leaving it
    /// closes it (after <see cref="FlarePopover.HideDelay"/>). Not modal - no scrim or focus trap.</summary>
    Hover,
}
