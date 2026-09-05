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
    /// <para>
    /// The box that SUPPLIES a height does not fill: when <c>Style</c> declares a <c>height</c> of its
    /// own, that height wins and this switch is ignored, because the two are contradictory instructions
    /// and the written number is the one you can see in the markup.
    /// </para>
    /// </summary>
    [Parameter] public bool FillHeight { get; set; }

    /// <summary>The shared fill modifier when <see cref="FillHeight"/> is set, else <c>null</c>.
    /// Pass it to <c>BuildCssClass</c> alongside the component's own modifiers.</summary>
    protected string? FillClass => FillHeight && !DeclaresOwnHeight(Style) ? Css.Classes.Fill.Root : null;

    // "Fill your parent" and "be this tall" are contradictory instructions, and the contradiction used
    // to resolve into a third thing that is neither: filling sets `flex-basis: 0`, which REPLACES the
    // height on a flex parent's main axis, so a box told to be 24rem tall came out at its content
    // height and every link below it collapsed with it. Measured in the Gallery on exactly that
    // mistake - a 24rem box that rendered 104px tall.
    //
    // A written number is the more specific instruction and the one the author can see in the markup,
    // so it wins and the fill is dropped. The alternative - basing the flex on the height instead of on
    // zero - fixes this case and breaks a commoner one, because an item whose basis is its full height
    // takes its shrink out of its siblings, squashing the toolbar or header it sits next to.
    private static bool DeclaresOwnHeight(string? style)
    {
        if (string.IsNullOrWhiteSpace(style)) return false;

        foreach (var declaration in style.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var colon = declaration.IndexOf(':');
            if (colon < 0) continue;

            // The property name, exactly: `min-height`, `max-height`, `line-height` and a custom
            // property called `--height` all contain "height" and none of them is one.
            var property = declaration.AsSpan(0, colon).Trim();
            if (property.Equals("height", StringComparison.OrdinalIgnoreCase)
                || property.Equals("block-size", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
