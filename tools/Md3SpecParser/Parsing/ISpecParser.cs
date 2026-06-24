using Flare.Tools.Md3SpecParser.Configuration;

namespace Flare.Tools.Md3SpecParser.Parsing;

/// <summary>
/// Transforms a single source JSON document into a markdown fragment plus the color
/// palette data extracted from it.
/// </summary>
public interface ISpecParser
{
    /// <summary>
    /// Parses <paramref name="json"/> into a component markdown fragment and a palette snapshot.
    /// </summary>
    /// <param name="json">Raw JSON downloaded for the source.</param>
    /// <param name="source">The source descriptor (type, url, label).</param>
    ParseResult Parse(string json, SpecSource source);
}

/// <summary>The output of parsing one source.</summary>
/// <param name="Markdown">Component markdown fragment (sections + tables), without a trailing blank line.</param>
/// <param name="Palette">Palette data for the per-component and global palette outputs.</param>
public sealed record ParseResult(string Markdown, PaletteSnapshot Palette);
