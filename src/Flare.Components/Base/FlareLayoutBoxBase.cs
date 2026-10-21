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

    /// <summary>The spacing and size declarations the box parameters ask for, or null when none is set.
    /// A component appends this before the consumer's <c>Style</c>, so the written style still wins.</summary>
    protected string? BoxStyle
    {
        get
        {
            var padding = ResolveSpacing(Padding, PaddingValue);
            var margin = ResolveSpacing(Margin, MarginValue);
            var parts = new List<string>(7);
            if (padding is not null) parts.Add($"padding:{padding}");
            if (margin is not null) parts.Add($"margin:{margin}");
            AddValue(parts, "max-width", MaxWidth);
            AddValue(parts, "min-width", MinWidth);
            AddValue(parts, "width", Width);
            AddValue(parts, "height", Height);
            AddValue(parts, "min-height", MinHeight);
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

    private static string? ResolveSpacing(FlareSpacing? spacing, string? raw) => spacing switch
    {
        null => null,
        FlareSpacing.Custom => Flare.Components.Security.CssValidator.StripDangerous(raw),
        _ => FlareSpacingVars.Var(spacing.Value),
    };
}
