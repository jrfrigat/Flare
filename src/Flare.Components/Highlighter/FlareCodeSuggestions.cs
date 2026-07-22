using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flare.Components;

/// <summary>
/// Where the caret is and what is around it, handed to whatever supplies suggestions.
/// </summary>
/// <param name="Text">The whole text as it currently stands, complete or not.</param>
/// <param name="Caret">Zero-based caret offset into that text.</param>
public sealed record FlareCodeContext(string Text, int Caret);

/// <summary>
/// One thing that could be written where the caret is.
/// <para>
/// The component knows nothing about what a suggestion means - only how to show it and how to put
/// it in. What is worth suggesting is a question about the language being edited, so it belongs to
/// whoever supplied the text.
/// </para>
/// </summary>
/// <param name="Text">What to insert.</param>
/// <param name="Label">What to show in the list.</param>
public sealed record FlareCodeSuggestion(string Text, string Label)
{
    /// <summary>A fuller caption shown beside the label, such as what a name refers to.</summary>
    public string? Detail { get; init; }

    /// <summary>
    /// What kind of thing this is, shown as a short tag. Free text, since the component has no
    /// opinion about the categories a language has.
    /// </summary>
    public string? Kind { get; init; }

    /// <summary>
    /// Where the text being replaced starts. Defaults to the caret, which appends rather than
    /// replaces; a provider completing a half-typed word points at the start of that word.
    /// </summary>
    public int ReplaceStart { get; init; } = -1;

    /// <summary>How many characters the insertion replaces.</summary>
    public int ReplaceLength { get; init; }
}

/// <summary>How badly wrong a marked span is.</summary>
public enum FlareCodeSeverity
{
    /// <summary>Something is wrong and has to be dealt with.</summary>
    Error,

    /// <summary>Something is probably not meant, but works.</summary>
    Warning,
}

/// <summary>
/// A span of text worth drawing attention to, such as something that does not parse.
/// </summary>
/// <param name="Start">Zero-based offset of the first character marked.</param>
/// <param name="Length">How many characters are marked. Zero marks a position rather than a range.</param>
/// <param name="Message">What is wrong, shown when the pointer rests on the span.</param>
/// <param name="Severity">How badly wrong it is.</param>
public sealed record FlareCodeMarker(
    int Start, int Length, string Message, FlareCodeSeverity Severity = FlareCodeSeverity.Error);

/// <summary>Supplies the suggestions offered at a caret.</summary>
/// <param name="context">Where the caret is and what surrounds it.</param>
public delegate ValueTask<IReadOnlyList<FlareCodeSuggestion>> FlareCodeSuggestionProvider(
    FlareCodeContext context);
