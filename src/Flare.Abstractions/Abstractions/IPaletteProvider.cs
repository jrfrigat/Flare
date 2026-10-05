using Flare.Abstractions.Tokens;

namespace Flare.Abstractions;

/// <summary>
/// Supplies a set of <see cref="Palette"/>s in a standalone palette pack.
/// Register its palettes explicitly through <c>AddFlarePalette</c>, or expose them through
/// <see cref="ITheme.Palettes"/> when they belong to a theme.
/// </summary>
public interface IPaletteProvider
{
    /// <summary>The palettes to register.</summary>
    IReadOnlyList<Palette> Palettes { get; }
}
