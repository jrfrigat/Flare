using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Shared box parameters for the layout containers: spacing inside and around the box, and a size it
/// may be held to. They exist so a page can lay itself out without writing CSS; the <c>Style</c> and
/// attribute pass-through every Flare component keeps still works for anyone who prefers to.
/// </summary>
public abstract class FlareLayoutBoxBase : FlareContainerBase
{
    /// <summary>Space inside the box on all sides, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? Padding { get; set; }

    /// <summary>Raw CSS padding used when <see cref="Padding"/> is <see cref="FlareSpacing.Custom"/> -
    /// a length or a shorthand such as <c>1rem 2rem</c>.</summary>
    [Parameter] public string? PaddingValue { get; set; }

    /// <summary>Space around the box on all sides, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? Margin { get; set; }

    /// <summary>Raw CSS margin used when <see cref="Margin"/> is <see cref="FlareSpacing.Custom"/> -
    /// a length or a shorthand such as <c>1rem auto</c>.</summary>
    [Parameter] public string? MarginValue { get; set; }

    /// <summary>Space above the box, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? MarginBlockStart { get; set; }

    /// <summary>Raw CSS margin used when <see cref="MarginBlockStart"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? MarginBlockStartValue { get; set; }

    /// <summary>Space below the box, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? MarginBlockEnd { get; set; }

    /// <summary>Raw CSS margin used when <see cref="MarginBlockEnd"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? MarginBlockEndValue { get; set; }

    /// <summary>Space above the box's content, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? PaddingBlockStart { get; set; }

    /// <summary>Raw CSS padding used when <see cref="PaddingBlockStart"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? PaddingBlockStartValue { get; set; }

    /// <summary>Space below the box's content, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? PaddingBlockEnd { get; set; }

    /// <summary>Raw CSS padding used when <see cref="PaddingBlockEnd"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? PaddingBlockEndValue { get; set; }

    /// <summary>Space before the box's content along the inline axis, as a step on the shared spacing
    /// scale. Logical, so it lands on the correct side in a right-to-left page.</summary>
    [Parameter] public FlareSpacing? PaddingInlineStart { get; set; }

    /// <summary>Raw CSS padding used when <see cref="PaddingInlineStart"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? PaddingInlineStartValue { get; set; }

    /// <summary>Space after the box's content along the inline axis, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? PaddingInlineEnd { get; set; }

    /// <summary>Raw CSS padding used when <see cref="PaddingInlineEnd"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? PaddingInlineEndValue { get; set; }

    /// <summary>Space before the box along the inline axis, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? MarginInlineStart { get; set; }

    /// <summary>Raw CSS margin used when <see cref="MarginInlineStart"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? MarginInlineStartValue { get; set; }

    /// <summary>Space after the box along the inline axis, as a step on the shared spacing scale.</summary>
    [Parameter] public FlareSpacing? MarginInlineEnd { get; set; }

    /// <summary>Raw CSS margin used when <see cref="MarginInlineEnd"/> is <see cref="FlareSpacing.Custom"/>.</summary>
    [Parameter] public string? MarginInlineEndValue { get; set; }

    /// <summary>How much of a row's spare space the box takes, relative to its siblings. Null leaves it
    /// to the layout; 1 makes it share the free space with every other growing child.</summary>
    [Parameter] public double? FlexGrow { get; set; }

    /// <summary>The box's starting size along the row's main axis before free space is shared. Null
    /// leaves it to the content (<c>auto</c>); <c>0</c> is the flex shorthand's basis, where growing
    /// children split the free space evenly instead of growing from their content width.</summary>
    [Parameter] public string? FlexBasis { get; set; }

    /// <summary>Upper bound for the box's width; any CSS length or percentage.</summary>
    [Parameter] public string? MaxWidth { get; set; }

    /// <summary>Lower bound for the box's width; any CSS length or percentage.</summary>
    [Parameter] public string? MinWidth { get; set; }

    /// <summary>The box's width; any CSS length or percentage.</summary>
    [Parameter] public string? Width { get; set; }

    /// <summary>The box's height; any CSS length or percentage.</summary>
    [Parameter] public string? Height { get; set; }

    /// <summary>Lower bound for the box's height; any CSS length or percentage.</summary>
    [Parameter] public string? MinHeight { get; set; }

    /// <summary>Upper bound for the box's height; any CSS length or percentage.</summary>
    [Parameter] public string? MaxHeight { get; set; }

    /// <summary>What the box does with content that does not fit: <c>visible</c> (the default when
    /// unset), <c>hidden</c>, <c>auto</c>, <c>scroll</c> or <c>clip</c>. A call site that wants a
    /// scrolling pane asks for <c>auto</c> here instead of writing an inline style.</summary>
    [Parameter] public string? Overflow { get; set; }

    /// <summary>The surface the box paints behind its content. A semantic role paints that role and,
    /// for the accent roles, pairs it with the role's readable on-color; a custom color paints itself.
    /// <see cref="FlareColor.Default"/> leaves the box transparent.</summary>
    [Parameter] public FlareColor Background { get; set; } = FlareColor.Default;

    /// <summary>The color of the box's content. <see cref="FlareColor.Default"/> inherits it from the
    /// surrounding surface, or takes the background role's on-color when one was set.</summary>
    [Parameter] public FlareColor Foreground { get; set; } = FlareColor.Default;

    /// <summary>Keeps the box in view while the page scrolls past it - the sticky rail or preview
    /// column of a two-pane page.</summary>
    [Parameter] public bool Sticky { get; set; }

    /// <summary>Establishes the box as the containing block for absolutely positioned children, such
    /// as a layer or backstage that should cover this box and nothing else.</summary>
    [Parameter] public bool Relative { get; set; }

    /// <summary>CSS containment for the box - <c>layout</c>, <c>paint</c>, <c>strict</c> or a
    /// combination. A frame around a fixed-position app bar uses <c>layout paint</c> so the bar is
    /// pinned to the frame instead of the viewport.</summary>
    [Parameter] public string? Contain { get; set; }

    /// <summary>Distance from the top of the scrollport at which a sticky box rests, as a CSS length.
    /// Null leaves it at zero.</summary>
    [Parameter] public string? StickyTop { get; set; }

    /// <summary>The spacing and size declarations the box parameters ask for, or null when none is set.
    /// A component appends this before the consumer's <c>Style</c>, so the written style still wins.</summary>
    protected string? BoxStyle
    {
        get
        {
            var padding = ResolveSpacing(Padding, PaddingValue);
            var paddingStart = ResolveSpacing(PaddingBlockStart, PaddingBlockStartValue);
            var paddingEnd = ResolveSpacing(PaddingBlockEnd, PaddingBlockEndValue);
            var paddingInlineStart = ResolveSpacing(PaddingInlineStart, PaddingInlineStartValue);
            var paddingInlineEnd = ResolveSpacing(PaddingInlineEnd, PaddingInlineEndValue);
            var margin = ResolveSpacing(Margin, MarginValue);
            var marginStart = ResolveSpacing(MarginBlockStart, MarginBlockStartValue);
            var marginEnd = ResolveSpacing(MarginBlockEnd, MarginBlockEndValue);
            var marginInlineStart = ResolveSpacing(MarginInlineStart, MarginInlineStartValue);
            var marginInlineEnd = ResolveSpacing(MarginInlineEnd, MarginInlineEndValue);
            var parts = new List<string>(14);
            if (padding is not null) parts.Add($"padding:{padding}");
            if (paddingStart is not null) parts.Add($"padding-block-start:{paddingStart}");
            if (paddingEnd is not null) parts.Add($"padding-block-end:{paddingEnd}");
            if (paddingInlineStart is not null) parts.Add($"padding-inline-start:{paddingInlineStart}");
            if (paddingInlineEnd is not null) parts.Add($"padding-inline-end:{paddingInlineEnd}");
            if (margin is not null) parts.Add($"margin:{margin}");
            if (marginStart is not null) parts.Add($"margin-block-start:{marginStart}");
            if (marginEnd is not null) parts.Add($"margin-block-end:{marginEnd}");
            if (marginInlineStart is not null) parts.Add($"margin-inline-start:{marginInlineStart}");
            if (marginInlineEnd is not null) parts.Add($"margin-inline-end:{marginInlineEnd}");
            AddValue(parts, "max-width", MaxWidth);
            AddValue(parts, "min-width", MinWidth);
            AddValue(parts, "width", Width);
            AddValue(parts, "height", Height);
            AddValue(parts, "min-height", MinHeight);
            AddValue(parts, "max-height", MaxHeight);
            AddValue(parts, "overflow", Overflow);
            if (Sticky) parts.Add("position:sticky");
            if (Relative) parts.Add("position:relative");
            AddValue(parts, "contain", Contain);
            AddValue(parts, "top", StickyTop);
            AddValue(parts, "flex-basis", FlexBasis);
            AddPaint(parts, Background, Foreground);
            if (FlexGrow is { } grow) parts.Add($"flex-grow:{grow.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            return parts.Count > 0 ? string.Join(';', parts) : null;
        }
    }

    /// <summary>Joins the declarations a component builds, in order, skipping the empty ones.</summary>
    protected static string? JoinStyle(params string?[] parts)
    {
        var kept = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
        return kept.Length == 0 ? null : string.Join(';', kept);
    }

    private static void AddValue(List<string> parts, string name, string? value)
    {
        var safe = Flare.Components.Security.CssValidator.StripDangerous(value);
        if (!string.IsNullOrWhiteSpace(safe)) parts.Add($"{name}:{safe}");
    }

    // A painted surface carries readable content: an accent role names its own on-color, and an
    // explicit Foreground overrides it. The on-color exists only for primary..info, so the neutral
    // roles paint themselves and leave the text alone.
    private static void AddPaint(List<string> parts, FlareColor background, FlareColor foreground)
    {
        if (!background.IsDefault && background.CssValue is { } fill)
        {
            AddValue(parts, "background", fill);
            if (background.RoleName is "primary" or "secondary" or "tertiary" or "success"
                or "warning" or "error" or "info")
                AddValue(parts, "color", $"var(--flare-color-on-{background.RoleName})");
        }

        if (!foreground.IsDefault && foreground.CssValue is { } ink)
            AddValue(parts, "color", ink);
    }

    private static string? ResolveSpacing(FlareSpacing? spacing, string? raw) => spacing switch
    {
        null => null,
        FlareSpacing.Custom => Flare.Components.Security.CssValidator.StripDangerous(raw),
        _ => FlareSpacingVars.Var(spacing.Value),
    };
}
