using System.Text.Json;

namespace Flare.Tools.Md3SpecParser.Configuration;

/// <summary>
/// Loads and validates the <see cref="SpecConfig"/> from disk.
/// </summary>
public static class ConfigLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>
    /// Reads the config file, resolving a relative <see cref="SpecConfig.OutputRoot"/>
    /// against the config file's own directory.
    /// </summary>
    /// <param name="path">Path to the JSON config file.</param>
    public static SpecConfig Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Config file not found: {path}");

        var json = File.ReadAllText(path);
        var config = JsonSerializer.Deserialize<SpecConfig>(json, Options)
                     ?? throw new InvalidDataException($"Config file is empty or invalid: {path}");

        if (config.Sources.Count == 0)
            throw new InvalidDataException($"Config has no 'sources' entries: {path}");

        foreach (var src in config.Sources)
        {
            if (string.IsNullOrWhiteSpace(src.Type))
                throw new InvalidDataException("A source entry is missing 'type'.");
            if (src.EffectiveUrls.Count == 0)
                throw new InvalidDataException($"Source '{src.Type}' has no 'urls' (or 'url').");
        }

        var configDir = Path.GetDirectoryName(Path.GetFullPath(path))!;
        if (!Path.IsPathRooted(config.OutputRoot))
            config.OutputRoot = Path.GetFullPath(Path.Combine(configDir, config.OutputRoot));

        return config;
    }
}
