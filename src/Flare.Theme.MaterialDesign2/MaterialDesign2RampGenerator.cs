using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Theme.MaterialDesign2;

/// <summary>
/// Material Design 2 palette generator. MD2 has no tonal system, so a brand seed simply replaces
/// the primary/info roles (with derived containers and on-colors) while the MD2 neutral surfaces,
/// teal secondary and the other accents stay from the MD2 base scheme.
/// </summary>
public sealed class MaterialDesign2RampGenerator : IPaletteGenerator
{
    /// <summary>Shared singleton instance.</summary>
    public static readonly MaterialDesign2RampGenerator Instance = new();

    /// <inheritdoc />
    public Palette Generate(string id, string name, PaletteSeed seed, string? source = null) =>
        PaletteFactory.Brand(id, name, MaterialDesign2Tokens.LightColors, MaterialDesign2Tokens.DarkColors, seed.Main, source);
}
