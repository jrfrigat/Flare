using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

/// <summary>
/// Provider-agnostic description of a single icon. <see cref="FlareSvgIcon"/> (inline SVG) is the only
/// provider in core; optional add-on packages add their own (Material Symbols font, Material/Fluent SVG, Font
/// Awesome), each carrying that source's own options. Anywhere a component parameter is typed
/// <see cref="FlareIcon"/>, any subclass may be supplied, so the icon provider is a caller choice rather than
/// a component constraint. Render one standalone with <c>FlareIconView</c>.
/// </summary>
public abstract record FlareIcon
{
    /// <summary>Icon size as any CSS length (e.g. <c>"1.5rem"</c>, <c>"20px"</c>). When null the host/theme size applies.</summary>
    public string? Size { get; init; }

    /// <summary>Icon color. A role uses the shared color class; a custom color inlines a local token. Default inherits the current color.</summary>
    public FlareColor Color { get; init; } = FlareColor.Default;

    /// <summary>Accessible label. When set, the icon exposes <c>role="img"</c> + <c>aria-label</c>; otherwise it is <c>aria-hidden</c>.</summary>
    public string? AriaLabel { get; init; }

    /// <summary>Extra CSS class(es) appended to the icon element.</summary>
    public string? Class { get; init; }

    /// <summary>Inline style appended after the icon's own size/color declarations.</summary>
    public string? Style { get; init; }

    // Unmatched attributes forwarded by FlareIconView (its captured splat). Internal: a descriptor has no
    // attribute-capture surface of its own, so the render host supplies them.
    internal IReadOnlyDictionary<string, object>? Attributes { get; init; }

    /// <summary>Emits this icon's markup into the render tree. Implemented per provider.</summary>
    protected abstract void Build(RenderTreeBuilder builder);

    /// <summary>Returns a <see cref="RenderFragment"/> that renders this icon.</summary>
    public RenderFragment Render() => Build;

    /// <summary>
    /// Resolves a bare string to the dependency-free built-in SVG (<see cref="FlareIcons"/>) with that
    /// snake_case id (e.g. <c>"home"</c>, <c>"chevron_left"</c>). An id that is not built in resolves to an
    /// empty icon - the core never depends on a third-party icon set, so it does not fall back to a Material
    /// font. For provider icons use a typed value from an icon package (e.g. <c>MaterialDesign2Icons.Home</c>).
    /// </summary>
    public static implicit operator FlareIcon(string name) => FlareIcons.Find(name) ?? FlareIcons.Empty;

    // Note: there is deliberately no implicit FlareIcon -> RenderFragment conversion. It reads nicely
    // ("drop an icon into a RenderFragment slot") but makes overload resolution ambiguous wherever an API
    // has both a FlareIcon and a RenderFragment overload for the same argument (e.g. bUnit's .Add). Fill a
    // RenderFragment slot with the explicit @icon.Render() instead.

    // ---- Shared build helpers (for provider subclasses, including add-on packs) -----------------

    /// <summary>Joins the icon class list: root class, optional provider modifier, color role class, caller class.</summary>
    protected string BuildClass(string? modifier)
    {
        var parts = new List<string>(4) { Css.Classes.Icon.Root };
        if (!string.IsNullOrEmpty(modifier)) parts.Add(modifier);
        if (Color.CssClass is { } roleClass) parts.Add(roleClass);
        if (!string.IsNullOrEmpty(Class)) parts.Add(Class);
        return parts.Count == 1 ? Css.Classes.Icon.Root : string.Join(' ', parts);
    }

    /// <summary>Builds the inline style: font-size (Size), local color token (custom color), a provider extra, then caller Style.</summary>
    protected string? BuildStyle(string? extra)
    {
        var parts = new List<string>(4);
        if (!string.IsNullOrEmpty(Size)) parts.Add($"font-size:{Size}");
        if (Color.IsCustom) parts.Add($"{Css.Tokens.LocalColor.Main}:{Color.Value}");
        if (!string.IsNullOrEmpty(extra)) parts.Add(extra);
        if (!string.IsNullOrEmpty(Style)) parts.Add(Style);
        return parts.Count > 0 ? string.Join(';', parts) : null;
    }

    /// <summary>Adds the aria-hidden or role/aria-label attributes. Uses <paramref name="seq"/> and <paramref name="seq"/>+1.</summary>
    protected void AddAccessibility(RenderTreeBuilder builder, int seq)
    {
        if (string.IsNullOrEmpty(AriaLabel))
        {
            builder.AddAttribute(seq, "aria-hidden", "true");
        }
        else
        {
            builder.AddAttribute(seq, "role", "img");
            builder.AddAttribute(seq + 1, "aria-label", AriaLabel);
        }
    }

    /// <summary>Splats the host-forwarded unmatched attributes, if any.</summary>
    protected void AddExtraAttributes(RenderTreeBuilder builder, int seq)
    {
        if (Attributes is not null) builder.AddMultipleAttributes(seq, Attributes);
    }
}
