using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flare.Theming;

/// <summary>
/// JSON serialization helpers for themes and palettes.
/// Supports export (theme -> JSON) and import (JSON -> theme) for theme customization.
/// </summary>
public static class ThemeJsonSerializer
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// Serializes a theme to JSON. The theme's <see cref="ITheme.Base"/> is written as the parent's id
    /// (<c>baseId</c>); the asset lists are written complete, the inherited ones included.
    /// </summary>
    public static string ExportTheme(ITheme theme)
    {
        var model = new ThemeExportModel
        {
            Id = theme.Id,
            DisplayName = theme.DisplayName,
            DefaultPaletteId = theme.DefaultPaletteId,
            StyleAssets = theme.StyleAssets.ToArray(),
            ScriptAssets = theme.ScriptAssets.ToArray(),
            BaseId = theme.Base?.Id,
            Design = theme.Design,
        };
        return JsonSerializer.Serialize(model, s_options);
    }

    /// <summary>
    /// Deserializes a theme from JSON. Returns a theme that can be registered. A theme exported with a
    /// parent is rebuilt on the theme of that id from <paramref name="knownThemes"/> - typically the
    /// registered themes - with its asset lists exactly as exported. An export written by an older Flare
    /// with a <c>styleFamilyId</c> other than its own id names its parent the same way.
    /// </summary>
    /// <param name="json">The exported theme.</param>
    /// <param name="knownThemes">Themes a parent id is looked up in; only needed when the export names a parent.</param>
    /// <exception cref="InvalidOperationException">The JSON is not a theme, or it names a parent that is not among <paramref name="knownThemes"/>.</exception>
    public static ITheme ImportTheme(string json, IEnumerable<ITheme>? knownThemes = null)
    {
        var model = JsonSerializer.Deserialize<ThemeExportModel>(json, s_options)
            ?? throw new InvalidOperationException("Invalid theme JSON.");

        var builder = new Flare.Theming.FlareThemeBuilder(model.Id, model.DisplayName, model.Design)
            .WithDefaultPalette(model.DefaultPaletteId)
            .WithStyleAssets(model.StyleAssets)
            .WithScriptAssets(model.ScriptAssets);

        var baseId = !string.IsNullOrEmpty(model.BaseId) ? model.BaseId
            : model.StyleFamilyId is { Length: > 0 } family && family != model.Id ? family
            : null;
        if (baseId is not null)
        {
            var parent = knownThemes?.FirstOrDefault(t => t.Id == baseId)
                ?? throw new InvalidOperationException(
                    $"Theme '{model.Id}' is built on theme '{baseId}', which is not among the known themes. Register it and pass it in knownThemes.");
            // The exported lists already hold the parent's assets in order, so nothing is inherited twice.
            builder = builder.WithBase(parent, inheritStyleAssets: false);
        }

        return builder.BuildUnsafe();
    }

    /// <summary>Serializes a palette to JSON.</summary>
    public static string ExportPalette(Palette palette)
    {
        return JsonSerializer.Serialize(palette, s_options);
    }

    /// <summary>Deserializes a palette from JSON.</summary>
    public static Palette ImportPalette(string json)
    {
        return JsonSerializer.Deserialize<Palette>(json, s_options)
            ?? throw new InvalidOperationException("Invalid palette JSON.");
    }

    private sealed class ThemeExportModel
    {
        public string Id { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string DefaultPaletteId { get; set; } = "";
        public string[] StyleAssets { get; set; } = [];
        public string[] ScriptAssets { get; set; } = [];
        public string? BaseId { get; set; }
        // Only read, never written: exports from 0.39-0.41 named the theme whose stylesheets styled this one here.
        public string? StyleFamilyId { get; set; }
        public DesignTokens Design { get; set; } = null!;
    }
}
