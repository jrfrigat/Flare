namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for the stacking ladder - which of Flare's detached surfaces paints over which.
/// Only surfaces that escape their parent's flow belong here: a pinned bar, a drawer, an anchored panel,
/// a modal, a toast. A z-index used inside a component's own stacking context (a state layer at -1, a
/// frozen grid column over its body cells) is local to that component and stays a plain number, because
/// nothing outside can collide with it.
///
/// The ladder is a structural contract rather than a design opinion - no design language disagrees that
/// a dialog covers a navigation bar - so core declares the whole of it in <c>layers.css</c> and a theme
/// inherits a correct order for free. Themes and applications may still repoint any rung: an application
/// embedding Flare beside its own fixed chrome moves the whole ladder by redefining these variables.
/// Keep them ordered as declared; <c>LayerLadderTests</c> holds that.
/// </summary>
public static class Layer
{
    /// <summary>CSS custom-property name for pinned shell chrome - the layout app bar, a bottom
    /// navigation bar, a scroll-to-top button. It covers page content and nothing else.</summary>
    public const string Chrome = "--flare-z-chrome";

    /// <summary>CSS custom-property name for a navigation drawer and the scrim that dims the page
    /// behind it. Above chrome, since a temporary drawer is meant to cover the bar it was opened from.</summary>
    public const string Drawer = "--flare-z-drawer";

    /// <summary>CSS custom-property name for a panel anchored to the control that owns it - a select
    /// listbox, a menu, a date picker, a popover. Above a drawer so a control inside one still opens
    /// over it.</summary>
    public const string Dropdown = "--flare-z-dropdown";

    /// <summary>CSS custom-property name for a modal surface and its scrim - dialog, message box,
    /// confirm. Above every anchored panel, so opening a dialog visually takes the page.</summary>
    public const string Modal = "--flare-z-modal";

    /// <summary>CSS custom-property name for a transient notification (snackbar). Above a modal: it
    /// reports the result of what the modal just did and must not be buried by it.</summary>
    public const string Toast = "--flare-z-toast";

    /// <summary>CSS custom-property name for a tooltip, which describes whatever is under the pointer
    /// and therefore sits above all of it.</summary>
    public const string Tooltip = "--flare-z-tooltip";

    /// <summary>CSS custom-property name for the element being dragged. The top rung: while a drag is
    /// in flight the dragged thing is what the pointer is holding, so nothing may cover it.</summary>
    public const string Drag = "--flare-z-drag";
}
