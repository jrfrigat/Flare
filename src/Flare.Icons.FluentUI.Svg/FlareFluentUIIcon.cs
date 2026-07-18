using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

/// <summary>
/// A Fluent UI System Icons size grid. Fluent icons are drawn per size (each optimised for its box), so the
/// value picks the matching SVG <c>viewBox</c>.
/// </summary>
public enum FluentUIIconSize
{
    /// <summary>16x16 grid.</summary>
    Size16,
    /// <summary>20x20 grid.</summary>
    Size20,
    /// <summary>24x24 grid (the most common).</summary>
    Size24,
    /// <summary>28x28 grid.</summary>
    Size28,
    /// <summary>32x32 grid.</summary>
    Size32,
    /// <summary>48x48 grid.</summary>
    Size48,
}

/// <summary>
/// A Fluent UI System Icons icon, rendered as inline SVG with Fluent's own options - <see cref="Filled"/>
/// (filled vs regular) and the <see cref="GridSize"/> the artwork is authored on. Any
/// <see cref="FlareFluentUIIcon"/> drops into any parameter typed <see cref="FlareIcon"/>. Ships in the
/// optional <c>Flare.Icons.FluentUI.Svg</c> package.
/// </summary>
/// <remarks>
/// This provider renders the SVG artwork you pass in <see cref="Data"/>; source it from the Fluent icon
/// assets (the <c>@fluentui/svg-icons</c> package or the Fluent Blazor icon set) for the icon and size you
/// want. SECURITY: markup in <see cref="Data"/> is emitted verbatim - pass only trusted, developer-authored
/// values.
/// </remarks>
public sealed record FlareFluentUIIcon : FlareIcon
{
    /// <summary>The icon path data (a <c>d=</c> string) or full SVG inner markup, from the Fluent assets.</summary>
    public required string Data { get; init; }

    /// <summary>The icon name (Fluent id, e.g. <c>"Home"</c>). Optional - used only for accessibility/identity.</summary>
    public string? Name { get; init; }

    /// <summary>Whether this is the filled (vs regular/outline) variant. Metadata that matches the chosen <see cref="Data"/>.</summary>
    public bool Filled { get; init; }

    /// <summary>The Fluent size grid the artwork is authored on, which sets the SVG <c>viewBox</c>. Default <see cref="FluentUIIconSize.Size24"/>.</summary>
    public FluentUIIconSize GridSize { get; init; } = FluentUIIconSize.Size24;

    // A leading '<' means full markup; otherwise Data is bare path data for a single <path d="...">.
    private bool IsMarkup => Data.TrimStart().StartsWith('<');

    /// <inheritdoc/>
    protected override void Build(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "class", BuildClass(Css.Classes.Icon.Svg));
        builder.AddAttribute(2, "viewBox", ViewBox());
        builder.AddAttribute(3, "width", "1em");
        builder.AddAttribute(4, "height", "1em");
        builder.AddAttribute(5, "fill", "currentColor");
        var style = BuildStyle(null);
        if (style is not null) builder.AddAttribute(6, "style", style);
        AddAccessibility(builder, 7);
        AddExtraAttributes(builder, 9);
        if (IsMarkup)
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

    private string ViewBox() => GridSize switch
    {
        FluentUIIconSize.Size16 => "0 0 16 16",
        FluentUIIconSize.Size20 => "0 0 20 20",
        FluentUIIconSize.Size28 => "0 0 28 28",
        FluentUIIconSize.Size32 => "0 0 32 32",
        FluentUIIconSize.Size48 => "0 0 48 48",
        _ => "0 0 24 24",
    };
}
