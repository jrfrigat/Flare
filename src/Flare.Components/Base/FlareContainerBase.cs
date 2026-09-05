using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Base class for components that HOLD other components - a card, a stack, a grid column, a tab set,
/// a data grid. It carries the one thing every such component needs and none of them can supply on
/// its own: a place in the chain of definite heights that a screen-fit page is made of.
/// </summary>
public abstract class FlareContainerBase : FlareComponentBase
{
    /// <summary>
    /// Makes the component spend the height it was given rather than growing to fit its content, and
    /// pass that height on to its children. This is the screen-fit case - a grid under a filter bar,
    /// a chart beside a table, a page that fills the window instead of scrolling with it.
    /// <para>
    /// It needs an ancestor with a height of its own, and EVERY link between that ancestor and this
    /// component needs the same switch: one link left at <c>auto</c> collapses the chain back to
    /// content height, silently. A page gets its first definite height from
    /// <c>FlareLayoutContent FillHeight</c>; anywhere else - a demo, a dialog - give the outermost box
    /// one explicitly.
    /// </para>
    /// </summary>
    [Parameter] public bool FillHeight { get; set; }

    /// <summary>The shared fill modifier when <see cref="FillHeight"/> is set, else <c>null</c>.
    /// Pass it to <c>BuildCssClass</c> alongside the component's own modifiers.</summary>
    protected string? FillClass => FillHeight ? Css.Classes.Fill.Root : null;
}
