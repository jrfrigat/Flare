using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for pointer ergonomics - read by the core's <c>@media (pointer: coarse)</c> rules.
/// </summary>
public sealed record TouchTokens
{
    /// <summary>
    /// Smallest tappable size a control may present to a coarse pointer.
    /// </summary>
    /// <remarks>
    /// A theme value because the design languages each publish their own minimum and they do not
    /// agree - so the core states none, and reads this only inside its
    /// <c>@media (pointer: coarse)</c> rules. It applies to the controls small enough to fall under
    /// it (the square icon-sized ones), not to every control.
    /// </remarks>
    [CssVar(TouchField.TargetMin)] public required string TargetMin { get; init; }
}
