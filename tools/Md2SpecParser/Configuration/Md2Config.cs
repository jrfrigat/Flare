using System.Text.Json.Serialization;

namespace Flare.Tools.Md2SpecParser.Configuration;

/// <summary>
/// Root of the JSON configuration driving the parser. Where the Material 3 tool lists opaque
/// token-table urls, this one lists the routes of m2.material.io as a reader types them: the site
/// publishes a route index (see <c>SiteMetaIndex</c>) that turns each route into its page-data url,
/// so a new page is one human-readable line here.
/// </summary>
public sealed class Md2Config
{
    /// <summary>
    /// Root directory holding one folder per component type. Each type produces
    /// <c>&lt;OutputRoot&gt;/&lt;folder&gt;/&lt;OutputFileName&gt;</c>.
    /// </summary>
    [JsonPropertyName("outputRoot")]
    public string OutputRoot { get; set; } = @"C:\Job\Projects\FrigaT\Flare\docs\spec";

    /// <summary>Name of the file written per folder.</summary>
    [JsonPropertyName("outputFileName")]
    public string OutputFileName { get; set; } = "md2-spec.md";

    /// <summary>The site's route index; every page-data url is resolved through it.</summary>
    [JsonPropertyName("siteMetaUrl")]
    public string SiteMetaUrl { get; set; } = "https://m2.material.io/site_meta.js";

    /// <summary>Base url the resolved page-data paths hang off.</summary>
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; } = "https://m2.material.io";

    /// <summary>
    /// Where the Material Components stylesheets are fetched from, with <c>{version}</c> and
    /// <c>{path}</c> substituted. They are the second source: the guidelines site publishes no
    /// measurable value at all for several components, and Google's own implementation of the same
    /// specification does.
    /// </summary>
    [JsonPropertyName("mdcUrlTemplate")]
    public string MdcUrlTemplate { get; set; } = "https://cdn.jsdelivr.net/npm/@material/{path}";

    /// <summary>The Material Components release the Sass values are read from.</summary>
    [JsonPropertyName("mdcVersion")]
    public string MdcVersion { get; set; } = "14.0.0";

    /// <summary>The pages to render, grouped into output folders.</summary>
    [JsonPropertyName("pages")]
    public List<Md2Page> Pages { get; set; } = new();
}

/// <summary>One output file: a component (or foundation) type and the routes that describe it.</summary>
public sealed class Md2Page
{
    /// <summary>Logical type, e.g. <c>Button</c>. Titles the output and names it when no folder is set.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Output sub-folder; derived from <see cref="Type"/> by kebab-casing when empty.</summary>
    [JsonPropertyName("folder")]
    public string? Folder { get; set; }

    /// <summary>
    /// Site routes, e.g. <c>/components/buttons</c>. Several routes merge, in order, into one file -
    /// Material 2 splits a single Flare component across pages (buttons and the FAB, for instance).
    /// </summary>
    [JsonPropertyName("routes")]
    public List<string> Routes { get; set; } = new();

    /// <summary>
    /// Material Components packages whose Sass variables describe this component, as
    /// <c>&lt;package&gt;/&lt;file&gt;</c> - e.g. <c>tab/_variables.scss</c>. Rendered as a separate,
    /// clearly labelled section: an implementation is evidence of the specification, not the
    /// specification, and it only matters where the guidelines publish no number at all.
    /// </summary>
    [JsonPropertyName("mdc")]
    public List<string> Mdc { get; set; } = new();

    /// <summary>The folder this page writes into.</summary>
    public string ResolveFolder() =>
        string.IsNullOrWhiteSpace(Folder) ? Kebab(Type) : Folder!.Trim();

    private static string Kebab(string value) =>
        string.Join('-', value.Split([' ', ':', '/'], StringSplitOptions.RemoveEmptyEntries))
            .ToLowerInvariant();
}
