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
    /// Reads the config file, resolving a relative <see cref="SpecConfig.OutputRoot"/> against the root of
    /// the Flare checkout above the working directory, then above the config file, and only then against
    /// the config file's own directory.
    /// </summary>
    /// <param name="path">Path to the JSON config file.</param>
    public static SpecConfig Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Config file not found: {path}");

        var json = File.ReadAllText(path);
        // System.Text.Json deserializes into the closed SpecConfig POCO with no polymorphic
        // type resolution, and the source is a local config file the developer supplies to the
        // CLI -- not the gadget-chain insecure-deserialization vector V5611 guards against.
        var config = JsonSerializer.Deserialize<SpecConfig>(json, Options) //-V5611
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
        {
            // The working directory decides which checkout is meant, so a worktree writes into itself;
            // the config sits in the build output, so its folder alone would land the specs under bin.
            var baseDir = FindRepoRoot(Directory.GetCurrentDirectory()) ?? FindRepoRoot(configDir) ?? configDir;
            config.OutputRoot = Path.GetFullPath(Path.Combine(baseDir, config.OutputRoot));
        }

        return config;
    }

    private static string? FindRepoRoot(string start)
    {
        for (var dir = new DirectoryInfo(start); dir is not null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "Flare.slnx")))
                return dir.FullName;
        return null;
    }
}
