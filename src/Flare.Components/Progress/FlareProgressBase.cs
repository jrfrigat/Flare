using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Carries what a linear and a circular progress indicator genuinely share: the value they report,
/// the step they take on the size scale, and the color role they paint in. Everything past that -
/// the track, the arc, the buffer, the zones, the ring geometry - differs between the two, which is
/// why they are separate components rather than one with a variant switch.
/// </summary>
public abstract class FlareProgressBase : FlareComponentBase
{
    /// <summary>Percentage value (0-100); null for indeterminate mode.</summary>
    [Parameter] public double? Value { get; set; }

    /// <summary>
    /// Size step on the shared <see cref="TrackSize"/> scale, the same one <see cref="FlareSlider"/>
    /// and <see cref="FlareMeter"/> use. It selects which per-size theme token the CSS reads and
    /// carries no geometry of its own, so the theme owns what each step is worth.
    /// </summary>
    [Parameter] public TrackSize Size { get; set; } = TrackSize.Md;

    /// <summary>Semantic color applied to the progress indicator.</summary>
    [Parameter] public FlareColor Color { get; set; } = FlareColor.Default;

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Progress.Root;

    /// <summary>True when no value was given and the indicator reports only that work is happening.</summary>
    private protected bool Indeterminate => Value is null;

    /// <summary>The value held to 0-100, so an out-of-range one cannot overflow the track.</summary>
    private protected double ClampedValue => Math.Clamp(Value ?? 0, 0, 100);

    /// <summary>The value as ARIA wants it, or null while indeterminate.</summary>
    private protected string? AriaValueNow =>
        Indeterminate ? null : ClampedValue.ToString("F0", CultureInfo.InvariantCulture);

    /// <summary>Role colors arrive as a shared class that sets <c>--fc-main</c>.</summary>
    private protected string ColorClass => Color.CssClass ?? string.Empty;

    /// <summary>A custom color has no class, so it sets the same token inline instead.</summary>
    private protected string ColorTokens =>
        Color.IsCustom ? $"{Css.Tokens.LocalColor.Main}:{Color.Value};" : string.Empty;

    /// <summary>The size class only selects a theme token; it carries no measurement itself.</summary>
    private protected string SizeClass => Size switch
    {
        TrackSize.Sm => Css.Classes.Progress.Sm,
        TrackSize.Md => Css.Classes.Progress.Md,
        TrackSize.Lg => Css.Classes.Progress.Lg,
        TrackSize.Xl => Css.Classes.Progress.Xl,
        _ => Css.Classes.Progress.Xs,
    };
}
