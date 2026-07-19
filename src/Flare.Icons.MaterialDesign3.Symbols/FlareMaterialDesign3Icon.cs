using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Icons;

/// <summary>The Material Symbols variable-font family (glyph terminals). Each is a distinct font family.</summary>
public enum MaterialDesign3Family
{
    /// <summary>Rounded terminals - the Flare default and the family the Gallery bundles.</summary>
    Rounded,
    /// <summary>Straight, geometric terminals.</summary>
    Outlined,
    /// <summary>Sharp, square terminals.</summary>
    Sharp,
}

/// <summary>
/// An icon from the Material Symbols variable font, selected by <see cref="Name"/> (e.g. <c>"home"</c>).
/// Exposes the font axes - <see cref="Fill"/>, <see cref="Weight"/>, <see cref="Grade"/> and
/// <see cref="OpticalSize"/> - plus the <see cref="Family"/> family. Requires the Material Symbols font to be
/// loaded by the host app (a non-Rounded <see cref="Family"/> requires that family specifically). For a
/// dependency-free glyph prefer <see cref="FlareSvgIcon"/> or the built-in <see cref="FlareIcons"/> set.
/// </summary>
public sealed record FlareMaterialDesign3Icon : FlareIcon
{
    private const int DefaultWeight = 400;
    private const int DefaultGrade = 0;
    private const int DefaultOpticalSize = 24;

    /// <summary>The Material Symbols icon name (snake_case), e.g. <c>chevron_left</c>.</summary>
    public required string Name { get; init; }

    /// <summary>Fills the glyph (the <c>FILL</c> axis: <c>false</c> outline, <c>true</c> solid).</summary>
    public bool Fill { get; init; }

    /// <summary>Stroke weight (the <c>wght</c> axis, 100..700). Default 400.</summary>
    public int Weight { get; init; } = DefaultWeight;

    /// <summary>Grade (the <c>GRAD</c> axis, -25..200) - fine emphasis without changing the glyph size. Default 0.</summary>
    public int Grade { get; init; } = DefaultGrade;

    /// <summary>Optical size (the <c>opsz</c> axis, 20..48) the glyph is tuned for. Default 24.</summary>
    public int OpticalSize { get; init; } = DefaultOpticalSize;

    /// <summary>Glyph font family. Default <see cref="MaterialDesign3Family.Rounded"/>.</summary>
    public MaterialDesign3Family Family { get; init; } = MaterialDesign3Family.Rounded;

    /// <inheritdoc/>
    protected override void Build(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", BuildClass(FontClass()));
        var style = BuildStyle(VariationSettings());
        if (style is not null) builder.AddAttribute(2, "style", style);
        AddAccessibility(builder, 3);
        AddExtraAttributes(builder, 6);
        builder.AddContent(7, Name);
        builder.CloseElement();
    }

    private string FontClass() => Family switch
    {
        MaterialDesign3Family.Outlined => "material-symbols-outlined",
        MaterialDesign3Family.Sharp => "material-symbols-sharp",
        _ => "material-symbols-rounded",
    };

    // Emit the axes only when they diverge from the icon.css baseline, keeping the style string minimal.
    private string? VariationSettings()
    {
        if (!Fill && Weight == DefaultWeight && Grade == DefaultGrade && OpticalSize == DefaultOpticalSize)
            return null;
        return $"font-variation-settings:'FILL' {(Fill ? 1 : 0)},'wght' {Weight},'GRAD' {Grade},'opsz' {OpticalSize}";
    }
}
