namespace Flare.Core.Tokens;

/// <summary>The full type scale (display, headline, title, body and label styles).</summary>
public sealed record TypographyTokens
{
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
