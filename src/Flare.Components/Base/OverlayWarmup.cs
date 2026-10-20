using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// Gets the overlay module ready when the app starts, so the first popup, dialog, menu or picker to open does not
/// wait for it. Importing it is asynchronous, and started by the first popup it resolved only after the frame that
/// popup opened in. Renders nothing; <see cref="FlareThemeProvider"/> places one at the root of the app.
/// </summary>
internal sealed class OverlayWarmup : ComponentBase
{
    [Inject] private IOverlayJsService Overlay { get; set; } = default!;
    [Inject] private ILogger<OverlayWarmup> Logger { get; set; } = default!;

    /// <inheritdoc />
    protected override bool ShouldRender() => false;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        try { await Overlay.PrepareAsync(); }
        catch (JSDisconnectedException) { }
        catch (JSException error) { Logger.LogWarning(error, "The overlay module could not be prepared; the next popup imports it again."); }
        catch (InvalidOperationException error) { Logger.LogWarning(error, "The overlay module cannot be imported here; the next popup imports it again."); }
    }
}
