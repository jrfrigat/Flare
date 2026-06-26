using Flare.Abstractions.Tokens;

namespace Flare.Abstractions;

/// <summary>A persisted theme selection across the three axes.</summary>
public readonly record struct ThemeSelection(string ThemeId, string PaletteId, ThemeMode Mode);

/// <summary>Persists and restores the user's theme selection (e.g. via localStorage).</summary>
public interface IThemeStorageService
{
    /// <summary>Returns the saved selection, or null if nothing is stored / storage is unavailable.</summary>
    Task<ThemeSelection?> GetAsync();
    /// <summary>Persists the current selection (theme, palette, mode).</summary>
    Task SaveAsync(ThemeSelection selection);
}
