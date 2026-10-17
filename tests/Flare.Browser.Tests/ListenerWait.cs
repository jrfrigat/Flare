using System.Text.Json;
using Microsoft.Playwright;

namespace Flare.Browser.Tests;

/// <summary>
/// Waits for a script module to take over an element. A marker in the markup proves only that Blazor rendered it: on
/// a slow machine a key or a pointer sent before the module attaches reaches an element nobody listens to yet.
/// </summary>
internal static class ListenerWait
{
    /// <summary>Returns once the first element matching <paramref name="selector"/> has a listener of <paramref name="type"/>.</summary>
    public static async Task ForAsync(IPage page, string selector, string type)
    {
        var cdp = await page.Context.NewCDPSessionAsync(page);
        try
        {
            var deadline = DateTime.UtcNow.AddSeconds(15);
            while (true)
            {
                if (await HasListenerAsync(cdp, selector, type)) return;
                if (DateTime.UtcNow > deadline) throw new TimeoutException($"No '{type}' listener on {selector}.");
                await Task.Delay(100);
            }
        }
        finally
        {
            await cdp.DetachAsync();
        }
    }

    private static async Task<bool> HasListenerAsync(ICDPSession cdp, string selector, string type)
    {
        var found = await cdp.SendAsync("Runtime.evaluate", new()
        {
            ["expression"] = $"document.querySelector({JsonSerializer.Serialize(selector)})",
        });
        if (found is not { } result || !result.GetProperty("result").TryGetProperty("objectId", out var id)) return false;
        var listeners = await cdp.SendAsync("DOMDebugger.getEventListeners", new() { ["objectId"] = id.GetString()! });
        return listeners is { } list
            && list.GetProperty("listeners").EnumerateArray().Any(l => l.GetProperty("type").GetString() == type);
    }
}
