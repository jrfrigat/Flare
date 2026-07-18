using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

/// <summary>
/// An inline-SVG icon. <see cref="Data"/> is either SVG path data (a single <c>d=</c> string) or a full
/// SVG inner-markup fragment (one or more elements), rendered inside a <c>1em</c> <c>&lt;svg&gt;</c> that
/// inherits the current color. Depends on nothing external, so it backs the built-in <see cref="FlareIcons"/>
/// set as well as brand marks and custom art.
/// </summary>
/// <remarks>
/// SECURITY: a markup fragment is emitted verbatim without sanitization. Pass only trusted,
/// developer-authored values (e.g. <see cref="FlareIcons"/> members) - never untrusted or user input.
/// </remarks>
public sealed record FlareSvgIcon : FlareIcon
{
    /// <summary>SVG path data (a <c>d=</c> string) or a full inner-markup fragment (rendered verbatim).</summary>
    public required string Data { get; init; }

    /// <summary>The <c>viewBox</c> that <see cref="Data"/> is authored on. Defaults to a 24x24 grid.</summary>
    public string ViewBox { get; init; } = "0 0 24 24";

    // A leading '<' means the caller supplied full markup (e.g. "<path .../>" or nested elements);
    // otherwise Data is bare path data destined for a single <path d="...">.
    private bool IsMarkup => Data.TrimStart().StartsWith('<');

    /// <inheritdoc/>
    protected override void Build(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "class", BuildClass(Css.Classes.Icon.Svg));
        builder.AddAttribute(2, "viewBox", ViewBox);
        builder.AddAttribute(3, "width", "1em");
        builder.AddAttribute(4, "height", "1em");
        builder.AddAttribute(5, "fill", "currentColor");
        var style = BuildStyle(null);
        if (style is not null) builder.AddAttribute(6, "style", style);
        AddAccessibility(builder, 7);
        AddExtraAttributes(builder, 9);
        if (Data.Length == 0)
        {
            // Empty data -> an empty <svg> (e.g. the "unknown id" placeholder); no path.
        }
        else if (IsMarkup)
        {
            builder.AddMarkupContent(10, Data);
        }
        else
        {
            builder.OpenElement(11, "path");
            builder.AddAttribute(12, "d", Data);
            builder.CloseElement();
        }
        builder.CloseElement();
    }
}
