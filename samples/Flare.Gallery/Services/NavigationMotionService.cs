using Flare.Components;
using Microsoft.JSInterop;

namespace Flare.Gallery.Services;

/// <summary>
/// Stores the gallery user's choice of how the navigation drawers move: the animated track (A), no
/// motion (A0), or the surface animation (B). Persists the choice to localStorage so it survives
/// reloads (and works offline as a PWA) and raises <see cref="Changed"/> so the layout re-applies it
/// live, without reloading. Mirrors the <see cref="RailLabelService"/> pattern: a scoped service
/// initialized before the UI starts.
/// </summary>
public sealed class NavigationMotionService
{
    private const string StorageKey = "flare-layout-motion";
    private readonly IJSRuntime _js;

    public NavigationMotionService(IJSRuntime js) => _js = js;

    /// <summary>How the shell moves when a drawer changes its track. Defaults to the animated track (A).</summary>
    public LayoutMotion Motion { get; private set; } = LayoutMotion.Animated;

    /// <summary>Raised after the mode changes so subscribers (the layout) can re-render.</summary>
    public event Action? Changed;

    /// <summary>Sets the mode, persists it, and notifies subscribers. No-op when unchanged.</summary>
    public async Task SetMotionAsync(LayoutMotion motion)
    {
        if (motion == Motion) return;
        Motion = motion;
        try { await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, motion.ToString()); }
        catch (JSException) { }
        catch (InvalidOperationException) { }
        Changed?.Invoke();
    }

    /// <summary>Restores the saved mode from localStorage. Run once before the UI renders.</summary>
    public async Task InitializeAsync()
    {
        try
        {
            var stored = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            // The enum name is the stored value, so a renamed or unknown entry falls back to A
            // instead of moving the visitor into a mode the page does not implement.
            if (Enum.TryParse<LayoutMotion>(stored, out var motion) && Enum.IsDefined(motion))
                Motion = motion;
        }
        catch (JSException) { }
        catch (InvalidOperationException) { }
    }
}
