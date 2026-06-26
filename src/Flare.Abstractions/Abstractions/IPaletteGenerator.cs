using Flare.Core.Tokens;

// NOTE: namespace intentionally kept as Flare.Core.Services during the structural refactor so existing
// `using Flare.Core.Services;` references (themes, services) keep resolving. The port (IPaletteGenerator)
// and its input DTO (PaletteSeed) live in Flare.Abstractions; the DefaultPaletteGenerator implementation
// stays in the theming engine. The namespace is realigned to Flare.Abstractions in the rename phase.
namespace Flare.Core.Services;

/// <summary>Inputs for generating a palette: a brand color plus optional hints.</summary>
public readonly record struct PaletteSeed(string Main, string? Background = null, string? Secondary = null);

/// <summary>Generates a full <see cref="Palette"/> (light + dark) from a small seed.</summary>
public interface IPaletteGenerator
{
    /// <summary>Generates a full light+dark <see cref="Palette"/> from the given seed.</summary>
    Palette Generate(string id, string name, PaletteSeed seed, string? source = null);
}
