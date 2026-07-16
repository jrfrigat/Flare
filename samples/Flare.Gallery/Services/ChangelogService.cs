using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Flare.Gallery.Services;

/// <summary>One release entry parsed from the changelog: version, date and the markdown body.</summary>
/// <param name="Version">The semantic version, e.g. "0.1.7".</param>
/// <param name="Date">The release date text, if present on the header line.</param>
/// <param name="Markdown">The section body (everything below the version header, as markdown).</param>
public sealed record ChangelogEntry(string Version, string? Date, string Markdown);

/// <summary>
/// Single source of truth for release notes. Reads the embedded <c>CHANGELOG.md</c> (and
/// <c>CHANGELOG.ru.md</c>) once, parses them into per-version entries, and serves the /changelog page - so a
/// release only touches the changelog file(s) and the git tag, not a page or a resx.
/// </summary>
public sealed class ChangelogService
{
    private readonly List<ChangelogEntry> _en;
    private readonly Dictionary<string, ChangelogEntry> _ru;

    public ChangelogService()
    {
        _en = Parse(ReadEmbedded("CHANGELOG.md"));
        _ru = Parse(ReadEmbedded("CHANGELOG.ru.md")).ToDictionary(e => e.Version);
    }

    /// <summary>All release entries, newest first, in the given culture. Russian entries fall back to
    /// English for versions that have not been translated.</summary>
    public IReadOnlyList<ChangelogEntry> Entries(CultureInfo? culture = null)
    {
        var ru = (culture ?? CultureInfo.CurrentUICulture).TwoLetterISOLanguageName == "ru";
        if (!ru) return _en;
        return _en.Select(e => _ru.TryGetValue(e.Version, out var r) ? r : e).ToList();
    }

    private static string ReadEmbedded(string logicalName)
    {
        using var stream = typeof(ChangelogService).Assembly.GetManifestResourceStream(logicalName);
        if (stream is null) return string.Empty;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    // Split on "## [version] - date" headers; a section's body is everything up to the next "## ".
    private static readonly Regex _header = new(@"^##\s*\[([^\]]+)\](?:\s*-\s*(.+))?\s*$", RegexOptions.Compiled);

    private static List<ChangelogEntry> Parse(string raw)
    {
        var entries = new List<ChangelogEntry>();
        if (string.IsNullOrEmpty(raw)) return entries;

        string? version = null, date = null;
        var body = new StringBuilder();
        void Flush()
        {
            if (version is not null)
                entries.Add(new ChangelogEntry(version, date, body.ToString().Trim('\n')));
            body.Clear();
        }

        foreach (var line in raw.Replace("\r\n", "\n").Split('\n'))
        {
            var m = _header.Match(line);
            if (m.Success)
            {
                Flush();
                version = m.Groups[1].Value.Trim();
                date = m.Groups[2].Success ? m.Groups[2].Value.Trim() : null;
            }
            else if (version is not null)
            {
                body.Append(line).Append('\n');
            }
        }
        Flush();
        return entries;
    }
}
