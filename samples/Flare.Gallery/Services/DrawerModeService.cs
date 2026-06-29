using Flare.Components;
using Microsoft.JSInterop;

namespace Flare.Gallery.Services;

/// <summary>
/// Stores the gallery user's preferred navigation <see cref="DrawerMode"/>, persists it to
/// localStorage so it survives reloads (and works offline as a PWA), and raises
/// <see cref="DrawerModeChanged"/> so the layout re-applies it live. Mirrors the
/// <see cref="LanguageService"/> pattern: a scoped service initialized before the UI starts.
/// </summary>
public sealed class DrawerModeService
{
    private const string StorageKey = "flare-drawer-mode";
    private readonly IJSRuntime _js;

    public DrawerModeService(IJSRuntime js) => _js = js;

    /// <summary>The active drawer mode. Defaults to the icon rail that expands on hover.</summary>
    public DrawerMode CurrentMode { get; private set; } = DrawerMode.RailHoverExpand;

    /// <summary>Raised after the mode changes so subscribers (the layout) can re-render.</summary>
    public event Action? DrawerModeChanged;

    /// <summary>Sets the mode, persists it, and notifies subscribers. No-op when unchanged.</summary>
    public async Task SetModeAsync(DrawerMode mode)
    {
        if (mode == CurrentMode) return;
        CurrentMode = mode;
        try { await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, mode.ToString()); }
        catch (JSException) { }
        catch (InvalidOperationException) { }
        DrawerModeChanged?.Invoke();
    }

    /// <summary>Restores the saved mode from localStorage. Run once before the UI renders.</summary>
    public async Task InitializeAsync()
    {
        try
        {
            var stored = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(stored) && Enum.TryParse<DrawerMode>(stored, ignoreCase: true, out var mode))
                CurrentMode = mode;
        }
        catch (JSException) { }
        catch (InvalidOperationException) { }
    }
}
