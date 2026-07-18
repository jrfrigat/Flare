using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

/// <summary>
/// A Font Awesome icon style, mapped to the matching <c>fa-*</c> family class.
/// <see cref="Light"/>, <see cref="Thin"/> and <see cref="Duotone"/> require Font Awesome Pro.
/// </summary>
public enum FaStyle
{
    /// <summary>Solid (<c>fa-solid</c>). The default; included in Font Awesome Free.</summary>
    Solid,
    /// <summary>Regular / outline (<c>fa-regular</c>). Limited in Free; full set is Pro.</summary>
    Regular,
    /// <summary>Light (<c>fa-light</c>). Font Awesome Pro.</summary>
    Light,
    /// <summary>Thin (<c>fa-thin</c>). Font Awesome Pro.</summary>
    Thin,
    /// <summary>Duotone (<c>fa-duotone</c>). Font Awesome Pro.</summary>
    Duotone,
    /// <summary>Brand marks (<c>fa-brands</c>), e.g. <c>github</c>, <c>microsoft</c>.</summary>
    Brands,
}

/// <summary>
/// A Font Awesome icon, selected by <see cref="Name"/> (the icon id without the <c>fa-</c> prefix, e.g.
/// <c>"github"</c>, <c>"house"</c>) and <see cref="Variant"/>. Renders the standard Font Awesome
/// <c>&lt;i class="fa-solid fa-github"&gt;</c> markup, so the host app must load a Font Awesome stylesheet
/// (Free, Pro, or a kit). Any <see cref="FlareFontAwesomeIcon"/> drops into any parameter typed
/// <see cref="FlareIcon"/>. Ships in the optional <c>Flare.Icons.FontAwesome</c> package.
/// </summary>
public sealed record FlareFontAwesomeIcon : FlareIcon
{
    /// <summary>The Font Awesome icon id without the <c>fa-</c> prefix, e.g. <c>"house"</c> or <c>"github"</c>.</summary>
    public required string Name { get; init; }

    /// <summary>The Font Awesome style family. Default <see cref="FaStyle.Solid"/>.</summary>
    public FaStyle Variant { get; init; } = FaStyle.Solid;

    /// <summary>Creates a brand icon (<see cref="FaStyle.Brands"/>), e.g. <c>FlareFontAwesomeIcon.Brand("github")</c>.</summary>
    public static FlareFontAwesomeIcon Brand(string name) => new() { Name = name, Variant = FaStyle.Brands };

    /// <inheritdoc/>
    protected override void Build(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "i");
        builder.AddAttribute(1, "class", BuildClass($"{StyleClass()} fa-{Name}"));
        var style = BuildStyle(null);
        if (style is not null) builder.AddAttribute(2, "style", style);
        AddAccessibility(builder, 3);
        AddExtraAttributes(builder, 6);
        builder.CloseElement();
    }

    private string StyleClass() => Variant switch
    {
        FaStyle.Regular => "fa-regular",
        FaStyle.Light => "fa-light",
        FaStyle.Thin => "fa-thin",
        FaStyle.Duotone => "fa-duotone",
        FaStyle.Brands => "fa-brands",
        _ => "fa-solid",
    };
}
