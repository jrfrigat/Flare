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

    /// <summary>Serializes a theme to JSON.</summary>
    public static string ExportTheme(ITheme theme)
    {
        var model = new ThemeExportModel
        {
            Id = theme.Id,
            DisplayName = theme.DisplayName,
            DefaultPaletteId = theme.DefaultPaletteId,
            StyleAssets = theme.StyleAssets.ToArray(),
            Design = theme.Design,
        };
        return JsonSerializer.Serialize(model, s_options);
    }

    /// <summary>Deserializes a theme from JSON. Returns a BuiltTheme that can be registered.</summary>
    public static ITheme ImportTheme(string json)
    {
        var model = JsonSerializer.Deserialize<ThemeExportModel>(json, s_options)
            ?? throw new InvalidOperationException("Invalid theme JSON.");

        return new Flare.Theming.FlareThemeBuilder(model.Id, model.DisplayName)
            .WithDesign(model.Design)
            .WithDefaultPalette(model.DefaultPaletteId)
            .WithStyleAssets(model.StyleAssets)
            .BuildUnsafe();
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
        public DesignTokens Design { get; set; } = null!;
    }
}
