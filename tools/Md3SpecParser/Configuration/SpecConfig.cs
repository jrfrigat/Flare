using System.Text.Json.Serialization;

namespace Flare.Tools.Md3SpecParser.Configuration;

/// <summary>
/// Root of the JSON configuration file driving the parser.
/// </summary>
public sealed class SpecConfig
{
    /// <summary>
    /// Absolute (or relative-to-config) root directory where per-type spec
    /// folders live. Each type produces <c>&lt;OutputRoot&gt;/&lt;folder&gt;/md3-expressive-spec.md</c>.
    /// Defaults to the Flare repo's <c>docs/spec</c>.
    /// </summary>
    [JsonPropertyName("outputRoot")]
    public string OutputRoot { get; set; } =
        @"C:\Job\Projects\FrigaT\Flare\docs\spec";

    /// <summary>
    /// Name of the file written per component folder.
    /// </summary>
    [JsonPropertyName("outputFileName")]
    public string OutputFileName { get; set; } = "md3-expressive-spec.md";

    /// <summary>
    /// Flat list of sources. Multiple entries may share the same
    /// <see cref="SpecSource.Type"/>; they are grouped and merged, in file
    /// order, into a single output file for that type.
    /// </summary>
    [JsonPropertyName("sources")]
    public List<SpecSource> Sources { get; set; } = new();
}

/// <summary>
/// A single (component type, JSON url) mapping.
/// </summary>
public sealed class SpecSource
{
    /// <summary>
    /// Logical component type, e.g. <c>Button</c>, <c>Button Group</c>.
    /// Used both for grouping and (when <see cref="Folder"/> is empty) to
    /// derive the output sub-folder via kebab-casing.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Source JSON locations (<c>http(s)</c> urls or local file paths). Their parsed
    /// sections are merged, in order, into the type's output file.
    /// </summary>
    [JsonPropertyName("urls")]
    public List<string> Urls { get; set; } = new();

    /// <summary>
    /// Single-source convenience alias for <see cref="Urls"/>; when set it is appended
    /// to the effective url list.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>All source urls for this entry (<see cref="Urls"/> plus <see cref="Url"/>).</summary>
    [JsonIgnore]
    public IReadOnlyList<string> EffectiveUrls =>
        Urls.Concat(string.IsNullOrWhiteSpace(Url) ? Array.Empty<string>() : new[] { Url! })
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .ToList();

    /// <summary>
    /// Optional explicit output sub-folder name. When omitted, the folder is
    /// derived from <see cref="Type"/> (kebab-case, e.g. "Button Group" -> "button-group").
    /// </summary>
    [JsonPropertyName("folder")]
    public string? Folder { get; set; }

    /// <summary>
    /// Optional human label appended to the generated section heading; reserved
    /// for the parsing rule supplied later.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Section ordering for this source: <c>component</c> (default - follow the
    /// component's token-set order) or <c>category</c> (Color sets first, then Size
    /// sets ascending xs->xl, then the rest).
    /// </summary>
    [JsonPropertyName("sectionOrder")]
    public string? SectionOrder { get; set; }

    /// <summary>
    /// Optional output file name override for this source (e.g. <c>md3-spec.md</c> for
    /// baseline MD3). When omitted, <see cref="SpecConfig.OutputFileName"/> is used
    /// (<c>md3-expressive-spec.md</c>). The global palette is written under
    /// <c>_pallete/&lt;fileName&gt;</c> with the matching name.
    /// </summary>
    [JsonPropertyName("outputFileName")]
    public string? OutputFileName { get; set; }
}
