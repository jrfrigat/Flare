using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Default <see cref="IFlareNotifications"/> over the browser's Notification API.</summary>
/// <remarks>
/// A call that cannot reach the browser - prerendering, a dropped circuit, or a cached copy of the module
/// older than this service (a PWA serves library scripts from its own cache) - degrades to "unsupported"
/// instead of throwing into the page.
/// </remarks>
public sealed class FlareNotificationsService : FlareJsModule, IFlareNotifications
{
    /// <param name="js">The JS runtime (injected).</param>
    public FlareNotificationsService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-notifications.js") { }

    /// <inheritdoc />
    public async ValueTask<bool> IsSupportedAsync() =>
        await TryAsync(() => InvokeAsync<bool>("isSupported"), false);

    /// <inheritdoc />
    public async ValueTask<NotificationPermission> GetPermissionAsync() =>
        FlareWebPushService.ParsePermission(await TryAsync(() => InvokeAsync<string?>("getPermission"), null));

    /// <inheritdoc />
    public async ValueTask<NotificationPermission> RequestPermissionAsync() =>
        FlareWebPushService.ParsePermission(await TryAsync(() => InvokeAsync<string?>("requestPermission"), null));

    /// <inheritdoc />
    public async ValueTask<NotificationShowResult> ShowAsync(BrowserNotification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        ArgumentException.ThrowIfNullOrWhiteSpace(notification.Title, nameof(notification));
        if (notification.Url is { } url && !IsSafeUrl(url))
            throw new ArgumentException("The address must be relative or an absolute http(s) address.", nameof(notification));

        var reply = await TryAsync(() => InvokeAsync<string?>("show",
            notification.Title, notification.Body, notification.Tag, notification.Icon, notification.Url), null);
        return ParseResult(reply);
    }

    internal static NotificationShowResult ParseResult(string? value) => value switch
    {
        "shown" => NotificationShowResult.Shown,
        "permission" => NotificationShowResult.PermissionNotGranted,
        "failed" => NotificationShowResult.Failed,
        _ => NotificationShowResult.Unsupported,
    };

    // A relative address stays in the application; an absolute one must be a web address, so a
    // notification can never carry a javascript: or data: URL into location.assign.
    internal static bool IsSafeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (Uri.TryCreate(url, UriKind.Absolute, out var absolute) && !url.StartsWith('/'))
            return absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps;
        return Uri.TryCreate(url, UriKind.Relative, out _) && !url.StartsWith("//", StringComparison.Ordinal);
    }

    private static async ValueTask<T> TryAsync<T>(Func<ValueTask<T>> call, T fallback)
    {
        try { return await call(); }
        catch (JSDisconnectedException) { return fallback; }
        catch (JSException) { return fallback; }
        catch (InvalidOperationException) { return fallback; } // JS interop is unavailable while prerendering
        catch (TaskCanceledException) { return fallback; }
    }
}
