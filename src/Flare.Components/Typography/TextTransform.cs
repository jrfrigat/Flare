namespace Flare.Components;

/// <summary>Letter-casing applied to a <c>FlareText</c> on top of its type-scale step.</summary>
/// <remarks>
/// Casing is a typographic level in most design languages - a section eyebrow, a group heading, a
/// service label are set in capitals - but no step of the scale carries it, so it was reachable only
/// through an inline style or a stylesheet of the caller's own.
/// </remarks>
public enum TextTransform
{
    /// <summary>Leave the text as written, which is what the type scale alone gives.</summary>
    None,

    /// <summary>Set in capitals.</summary>
    Uppercase,

    /// <summary>Set in lower case.</summary>
    Lowercase,

    /// <summary>Capitalize the first letter of every word.</summary>
    Capitalize,
}
