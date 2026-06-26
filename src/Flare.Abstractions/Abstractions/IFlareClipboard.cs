namespace Flare.Components;

/// <summary>Clipboard access with a non-secure-context fallback. Inject instead of using IJSRuntime.</summary>
public interface IFlareClipboard
{
    /// <summary>Copies text to the clipboard (uses the async Clipboard API, falls back to execCommand).</summary>
    ValueTask CopyAsync(string text);
    /// <summary>Reads text from the clipboard; returns an empty string if unavailable/denied.</summary>
    ValueTask<string> ReadAsync();
}
