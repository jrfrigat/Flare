using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Default <see cref="IFlareClipboard"/> over the browser clipboard.</summary>
/// <remarks>
/// Two JS surfaces, deliberately: <c>navigator.clipboard</c> is a browser global reached through the
/// runtime, while the fallback and the read helper live in the Flare module and are reached through it.
/// </remarks>
public sealed class FlareClipboardService : FlareJsModule, IFlareClipboard
{
    private readonly IJSRuntime _js;

    /// <param name="js">The JS runtime (injected).</param>
    public FlareClipboardService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-components.js") => _js = js;

    /// <summary>Copies the given text to the system clipboard.</summary>
    public async ValueTask CopyAsync(string text)
    {
        try { await _js.InvokeVoidAsync("navigator.clipboard.writeText", text); }
        catch { await InvokeVoidAsync("FlareClipboardFallback.copy", text); }
    }

    /// <summary>Reads the current text contents of the system clipboard.</summary>
    public async ValueTask<string> ReadAsync()
    {
        try { return await InvokeAsync<string>("flareOtp.getClipboardText"); }
        catch { return string.Empty; }
    }
}
