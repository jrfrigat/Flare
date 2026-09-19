using Flare.Css;
namespace Flare.Abstractions.Tokens;

/// <summary>The type scale (display, headline, title, body and label styles) and the font-family roles
/// that sit beside it.</summary>
public sealed record TypographyTokens
{
    /// <summary>
    /// The monospace face, used wherever characters have to line up in a column - <c>FlareText</c> in
    /// mono, inline <c>FlareCode</c>, a code block, code inside markdown. Not a step of the scale: the
    /// steps carry size, weight and spacing, and this carries only the family they cannot express.
    /// <para>
    /// Name a real family before the generic (<c>"JetBrains Mono", monospace</c>). A generic family on
    /// its own makes several engines use their "monospace default size" rather than the size the rule
    /// sets, so the editor's two layers end up measured in one size and painted in another; naming any
    /// family first avoids it, which is why the fallback value repeats the generic twice.
    /// </para>
    /// </summary>
    [CssVar(Flare.Css.Tokens.Typography.MonoFont)] public required string MonoFont { get; init; }

    /// <summary>Display large token.</summary>
    public required TypeStyle DisplayLarge { get; init; }
    /// <summary>Display medium token.</summary>
    public required TypeStyle DisplayMedium { get; init; }
    /// <summary>Display small token.</summary>
    public required TypeStyle DisplaySmall { get; init; }
    /// <summary>Headline large token.</summary>
    public required TypeStyle HeadlineLarge { get; init; }
    /// <summary>Headline medium token.</summary>
    public required TypeStyle HeadlineMedium { get; init; }
    /// <summary>Headline small token.</summary>
    public required TypeStyle HeadlineSmall { get; init; }
    /// <summary>Title large token.</summary>
    public required TypeStyle TitleLarge { get; init; }
    /// <summary>Title medium token.</summary>
    public required TypeStyle TitleMedium { get; init; }
    /// <summary>Title small token.</summary>
    public required TypeStyle TitleSmall { get; init; }
    /// <summary>Body large token.</summary>
    public required TypeStyle BodyLarge { get; init; }
    /// <summary>Body medium token.</summary>
    public required TypeStyle BodyMedium { get; init; }
    /// <summary>Body small token.</summary>
    public required TypeStyle BodySmall { get; init; }
    /// <summary>Label large token.</summary>
    public required TypeStyle LabelLarge { get; init; }
    /// <summary>Label medium token.</summary>
    public required TypeStyle LabelMedium { get; init; }
    /// <summary>Label small token.</summary>
    public required TypeStyle LabelSmall { get; init; }
}
