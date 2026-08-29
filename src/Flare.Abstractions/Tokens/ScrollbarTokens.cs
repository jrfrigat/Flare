using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>
/// The scrollbar scale. Until this existed, every scroll container in a Flare application - the shell's
/// content panel, a data grid body, a listbox, a code block, a dialog - painted whatever the user agent
/// chose, which on a dark theme meant a light scrollbar and the loudest thing on the page.
/// </summary>
/// <remarks>
/// <see cref="Thumb"/> and <see cref="Track"/> feed both the standard <c>scrollbar-color</c> pair and
/// the WebKit pseudo-elements, so a theme states the intent once. A language that wants the browser's
/// own scrollbar back sets <see cref="Width"/> to <c>auto</c> and the colours to <c>auto</c>.
/// </remarks>
public sealed record ScrollbarTokens
{
    /// <summary>Standard <c>scrollbar-width</c> keyword: <c>auto</c>, <c>thin</c> or <c>none</c>. Not a
    /// length - the property does not take one.</summary>
    [CssVar(Scrollbar.Width)] public required string Width { get; init; }

    /// <summary>Thickness for the WebKit pseudo-element path, as a length. It answers the same design
    /// question as <see cref="Width"/> on the other mechanism, so the two have to agree - a thin
    /// scrollbar on one engine and a chunky one on the next is that question answered twice,
    /// differently.</summary>
    [CssVar(Scrollbar.Size)] public required string Size { get; init; }

    /// <summary>Thumb colour at rest.</summary>
    [CssVar(Scrollbar.Thumb)] public required string Thumb { get; init; }

    /// <summary>Thumb colour while the pointer is over the scroll container.</summary>
    [CssVar(Scrollbar.ThumbHover)] public required string ThumbHover { get; init; }

    /// <summary>Track colour. <c>transparent</c> gives the overlay look, and lets the surface the
    /// container sits on show through instead of a second tone competing with it.</summary>
    [CssVar(Scrollbar.Track)] public required string Track { get; init; }

    /// <summary>Corner radius of the thumb.</summary>
    [CssVar(Scrollbar.Radius)] public required string Radius { get; init; }
}
