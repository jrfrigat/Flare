using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for form - component-specific geometry read by form.css.</summary>
public sealed record FormTokens
{
    /// <summary>Horizontal Columns.</summary>
    [CssVar(FormField.HorizontalColumns)] public required string HorizontalColumns { get; init; }
}
