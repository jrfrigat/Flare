namespace Flare.Components;

/// <summary>How a <see cref="FlarePopover"/> is opened.</summary>
public enum PopoverTrigger
{
    /// <summary>The consumer drives <c>Open</c>/<c>OpenChanged</c> (default) -- no built-in handler.</summary>
    Manual,

    /// <summary>Clicking the anchor toggles the popover open and closed.</summary>
    Click,
}
