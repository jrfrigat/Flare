using Microsoft.JSInterop;

namespace Flare.Gallery.Services;

/// <summary>
/// Stores the gallery user's preference for the primary navigation rail: whether each entry shows
/// a caption under its icon (a labeled rail) or just the icon. Persists the choice to localStorage
/// so it survives reloads (and works offline as a PWA) and raises <see cref="Changed"/> so the
/// layout re-applies it live. Mirrors the <see cref="LanguageService"/> pattern: a scoped service
/// initialized before the UI starts.
/// </summary>
public sealed class RailLabelService
{
    private const string StorageKey = "flare-rail-labels";
    private readonly IJSRuntime _js;

    public RailLabelService(IJSRuntime js) => _js = js;

    /// <summary>Whether the rail shows a caption under each icon. Defaults to a labeled rail.</summary>
    public bool ShowLabels { get; private set; } = true;

    /// <summary>Raised after the preference changes so subscribers (the layout) can re-render.</summary>
    public event Action? Changed;

    /// <summary>Sets the preference, persists it, and notifies subscribers. No-op when unchanged.</summary>
    public async Task SetShowLabelsAsync(bool showLabels)
    {
        if (showLabels == ShowLabels) return;
        ShowLabels = showLabels;
        try { await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, showLabels ? "true" : "false"); }
        catch (JSException) { }
        catch (InvalidOperationException) { }
        Changed?.Invoke();
    }

    /// <summary>Restores the saved preference from localStorage. Run once before the UI renders.</summary>
    public async Task InitializeAsync()
    {
        try
        {
            var stored = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(stored))
                ShowLabels = stored == "true";
        }
        catch (JSException) { }
        catch (InvalidOperationException) { }
    }
}
