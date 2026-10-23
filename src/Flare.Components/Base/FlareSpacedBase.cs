using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Spacing parameters for a component that is content rather than a container - a run of text keeps
/// the space inside and around itself, but its size, overflow, paint and position belong to the box
/// it is placed in. The parameters are the same ones <see cref="FlareLayoutBoxBase"/> offers, so a
/// page spaces text and boxes with one vocabulary.
/// </summary>
public abstract class FlareSpacedBase : FlareComponentBase
{
    /// <summary>Space inside the component on all sides, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? Padding { get; set; }

    /// <summary>Raw CSS padding used when <see cref="Padding"/> is <see cref="FlareSpacing.Custom"/> -
    /// a length or a shorthand such as <c>1rem 2rem</c>.</summary>
    [Parameter] public string? PaddingValue { get; set; }

    /// <summary>Space around the component on all sides, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? Margin { get; set; }

    /// <summary>Raw CSS margin used when <see cref="Margin"/> is <see cref="FlareSpacing.Custom"/> -
    /// a length or a shorthand such as <c>1rem auto</c>.</summary>
    [Parameter] public string? MarginValue { get; set; }

    /// <summary>Space above the component, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? MarginBlockStart { get; set; }

    /// <summary>Raw CSS margin used when <see cref="MarginBlockStart"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? MarginBlockStartValue { get; set; }

    /// <summary>Space below the component, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? MarginBlockEnd { get; set; }

    /// <summary>Raw CSS margin used when <see cref="MarginBlockEnd"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? MarginBlockEndValue { get; set; }

    /// <summary>Space above the component's content, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? PaddingBlockStart { get; set; }

    /// <summary>Raw CSS padding used when <see cref="PaddingBlockStart"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? PaddingBlockStartValue { get; set; }

    /// <summary>Space below the component's content, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? PaddingBlockEnd { get; set; }

    /// <summary>Raw CSS padding used when <see cref="PaddingBlockEnd"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? PaddingBlockEndValue { get; set; }

    /// <summary>Space before the component's content along the inline axis, as a step on the shared
    /// spacing scale. Logical, so it lands on the correct side in a right-to-left page.</summary>
    [Parameter] public FlareSpacing? PaddingInlineStart { get; set; }

    /// <summary>Raw CSS padding used when <see cref="PaddingInlineStart"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? PaddingInlineStartValue { get; set; }

    /// <summary>Space after the component's content along the inline axis, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? PaddingInlineEnd { get; set; }

    /// <summary>Raw CSS padding used when <see cref="PaddingInlineEnd"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? PaddingInlineEndValue { get; set; }

    /// <summary>Space before the component along the inline axis, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? MarginInlineStart { get; set; }

    /// <summary>Raw CSS margin used when <see cref="MarginInlineStart"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? MarginInlineStartValue { get; set; }

    /// <summary>Space after the component along the inline axis, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? MarginInlineEnd { get; set; }

    /// <summary>Raw CSS margin used when <see cref="MarginInlineEnd"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? MarginInlineEndValue { get; set; }

    /// <summary>Writes the spacing declarations the parameters ask for, in the order a later one
    /// refines an earlier shorthand.</summary>
    private protected void WriteSpacing(ref BoxStyleWriter style)
    {
        style.Spacing("padding", Padding, PaddingValue);
        style.Spacing("padding-block-start", PaddingBlockStart, PaddingBlockStartValue);
        style.Spacing("padding-block-end", PaddingBlockEnd, PaddingBlockEndValue);
        style.Spacing("padding-inline-start", PaddingInlineStart, PaddingInlineStartValue);
        style.Spacing("padding-inline-end", PaddingInlineEnd, PaddingInlineEndValue);
        style.Spacing("margin", Margin, MarginValue);
        style.Spacing("margin-block-start", MarginBlockStart, MarginBlockStartValue);
        style.Spacing("margin-block-end", MarginBlockEnd, MarginBlockEndValue);
        style.Spacing("margin-inline-start", MarginInlineStart, MarginInlineStartValue);
        style.Spacing("margin-inline-end", MarginInlineEnd, MarginInlineEndValue);
    }

    /// <summary>Joins two declaration lists, skipping an empty one.</summary>
    protected static string? JoinStyle(string? first, string? second) => BoxStyleWriter.Join(first, second);

    /// <summary>Joins the declarations a component builds, in order, skipping the empty ones.</summary>
    protected static string? JoinStyle(params string?[] parts) => BoxStyleWriter.Join(parts);
}
