using Flare.Abstractions.Tokens;

namespace Flare.Abstractions;

/// <summary>
/// Supplies a set of <see cref="Palette"/>s for assembly auto-discovery. Implement this on a class
/// with a public parameterless constructor to ship a standalone palette pack that
/// <c>RegisterAllBuiltInThemes</c> picks up from a referenced assembly -- the palette counterpart of
/// <see cref="ITheme"/>. (A <see cref="Palette"/> is a sealed record, so palette instances cannot be
/// discovered by subclassing; a provider type is the scannable equivalent.)
/// </summary>
public interface IPaletteProvider
{
    /// <summary>The palettes to register.</summary>
    IReadOnlyList<Palette> Palettes { get; }
}
