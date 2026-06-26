using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Microsoft.JSInterop;

namespace Flare.Infrastructure;

/// <summary>
/// <see cref="IThemeStorageService"/> backed by the browser <c>localStorage</c>. Persists the
/// theme/palette/mode selection across reloads; degrades to a no-op during SSR/prerender when
/// no JS runtime is available.
/// </summary>
public sealed class LocalStorageThemeStorage : IThemeStorageService
{
    private readonly IJSRuntime _js;
    // Keys match the WASM anti-FOUC bootstrap script (see R25).
    private const string ThemeKey = "flare-theme";
    private const string PaletteKey = "flare-palette";
    private const string ModeKey = "flare-mode";

    /// <summary>Creates the storage over the given JS runtime.</summary>
    public LocalStorageThemeStorage(IJSRuntime js) => _js = js;

    /// <inheritdoc />
    public async Task<ThemeSelection?> GetAsync()
    {
        try
        {
            var theme = await _js.InvokeAsync<string?>("localStorage.getItem", ThemeKey);
            if (string.IsNullOrEmpty(theme)) return null;
            var palette = await _js.InvokeAsync<string?>("localStorage.getItem", PaletteKey);
            var modeStr = await _js.InvokeAsync<string?>("localStorage.getItem", ModeKey);
            var mode = Enum.TryParse<ThemeMode>(modeStr, ignoreCase: true, out var m) ? m : ThemeMode.Auto;
            return new ThemeSelection(theme, palette ?? string.Empty, mode);
        }
        catch (InvalidOperationException) { return null; } // SSR / prerender
        catch (JSDisconnectedException) { return null; }
    }

    /// <inheritdoc />
    public async Task SaveAsync(ThemeSelection selection)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", ThemeKey, selection.ThemeId);
            await _js.InvokeVoidAsync("localStorage.setItem", PaletteKey, selection.PaletteId);
            await _js.InvokeVoidAsync("localStorage.setItem", ModeKey, selection.Mode.ToString().ToLowerInvariant());
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
    }
}
