using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>
/// The stacking ladder - which of Flare's detached surfaces paints over which. Only surfaces that escape
/// their parent's flow belong here: a pinned bar, a drawer, an anchored panel, a modal, a toast. A
/// z-index used inside a component's own stacking context (a state layer at -1, a frozen grid column over
/// its body cells) is local to that component and stays a plain number in its stylesheet, because nothing
/// outside can collide with it.
///
/// <para>
/// The order carries the meaning, and it is the same order in every design language - none of them
/// disagrees that a dialog covers a navigation bar. What a theme owns is where the ladder sits and how
/// far apart the rungs are, which is what an application embedding Flare beside chrome of its own has to
/// move. Leave room between rungs: a component lifts one of its own parts with
/// <c>calc(var(--flare-z-drawer) + 1)</c> - a drawer panel over its own scrim, a submenu over its parent
/// menu - and that offset must not reach the rung above.
/// </para>
/// </summary>
public sealed record LayerTokens
{
    /// <summary>Pinned shell chrome - the layout app bar, a bottom navigation bar, a scroll-to-top
    /// button. The lowest rung: it covers page content and nothing else.</summary>
    [CssVar(Layer.Chrome)] public required string Chrome { get; init; }

    /// <summary>A navigation drawer and the scrim that dims the page behind it. Above chrome, because a
    /// temporary drawer is meant to cover the bar it was opened from.</summary>
    [CssVar(Layer.Drawer)] public required string Drawer { get; init; }

    /// <summary>A panel anchored to the control that owns it - a select listbox, a menu, a date picker,
    /// a popover. Above a drawer, so a control sitting in one still opens over it.</summary>
    [CssVar(Layer.Dropdown)] public required string Dropdown { get; init; }

    /// <summary>A modal surface and its scrim - dialog, message box, confirm. Above every anchored
    /// panel, so opening a dialog visually takes the page.</summary>
    [CssVar(Layer.Modal)] public required string Modal { get; init; }

    /// <summary>A transient notification. Above a modal: it reports the result of what the modal just
    /// did and must not be buried by it.</summary>
    [CssVar(Layer.Toast)] public required string Toast { get; init; }

    /// <summary>A tooltip, which describes whatever is under the pointer and therefore sits above all
    /// of it.</summary>
    [CssVar(Layer.Tooltip)] public required string Tooltip { get; init; }

    /// <summary>The element being dragged. The top rung: while a drag is in flight the dragged thing is
    /// what the pointer is holding, so nothing may cover it.</summary>
    [CssVar(Layer.Drag)] public required string Drag { get; init; }
}
