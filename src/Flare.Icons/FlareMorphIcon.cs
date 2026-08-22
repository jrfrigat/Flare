using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Icons;

/// <summary>
/// An inline-SVG icon whose OUTLINE transitions when <see cref="Data"/> changes: the same
/// <c>&lt;path&gt;</c> element stays in the document and its geometry is interpolated, rather than one
/// glyph being cross-faded against another. This is the literal reading of "morph", and it only works
/// between paths that were authored for each other - see the remarks.
/// </summary>
/// <remarks>
/// <para>
/// SVG path interpolation requires the two paths to be STRUCTURALLY IDENTICAL: the same list of commands
/// in the same order, differing only in coordinates. A pair that does not match is swapped discretely
/// half way through instead. That is why this is a separate icon type rather than a mode of the ordinary
/// cross-fade: the pair has to be authored, and an arbitrary pair of catalog icons never qualifies.
/// <see cref="FlareMorphIcons"/> holds the built-in matched pairs; author your own by drawing both
/// shapes with the same command list (degenerate, zero-length segments are how you pad the simpler one).
/// </para>
/// <para>
/// The interpolation is the CSS <c>d</c> property, so it costs no JavaScript. Where a browser does not
/// support that property the geometry still renders - it is emitted as the <c>d</c> ATTRIBUTE as well -
/// and the change simply lands in one frame, which is what the icon did before it was morphable.
/// </para>
/// <para>
/// SECURITY: as with <see cref="FlareSvgIcon"/>, pass only trusted, developer-authored path data.
/// </para>
/// </remarks>
public sealed record FlareMorphIcon : FlareIcon
{
    /// <summary>The current outline as SVG path data (a <c>d=</c> string). Changing it transitions the shape.</summary>
    public required string Data { get; init; }

    /// <summary>The <c>viewBox</c> that <see cref="Data"/> is authored on. Defaults to a 24x24 grid.</summary>
    public string ViewBox { get; init; } = "0 0 24 24";

    /// <inheritdoc/>
    protected override void Build(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "class", BuildClass(Css.Classes.Icon.PathMorph));
        builder.AddAttribute(2, "viewBox", ViewBox);
        builder.AddAttribute(3, "width", "1em");
        builder.AddAttribute(4, "height", "1em");
        builder.AddAttribute(5, "fill", "currentColor");
        var style = BuildStyle(null);
        if (style is not null) builder.AddAttribute(6, "style", style);
        AddAccessibility(builder, 7);
        AddExtraAttributes(builder, 9);
        builder.OpenElement(11, "path");
        // Both forms of the same geometry, deliberately. The CSS property is what the transition in
        // icon.css can animate, and it wins over the attribute wherever the browser understands it; the
        // attribute is what draws the shape everywhere else. Emitting only the property would leave the
        // icon invisible on a browser without `d` support, and only the attribute would never animate.
        builder.AddAttribute(12, "d", Data);
        builder.AddAttribute(13, "style", $"d:path('{Data}')");
        builder.CloseElement();
        builder.CloseElement();
    }
}
