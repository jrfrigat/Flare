using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Default <see cref="IFlareWebPush"/> over the browser's Notification and Push APIs.</summary>
/// <remarks>
/// A call that cannot reach the browser - prerendering, a dropped circuit, or a cached copy of the module
/// older than this service (a PWA serves library scripts from its own cache) - degrades to "unsupported"
/// or "nothing" instead of throwing into the page.
/// </remarks>
public sealed class WebPushService : FlareJsModule, IFlareWebPush
{
    /// <param name="js">The JS runtime (injected).</param>
    public WebPushService(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/flare-web-push.js") { }

    /// <inheritdoc />
    public async ValueTask<bool> IsSupportedAsync() =>
        await TryAsync(() => InvokeAsync<bool>("isSupported"), false);

    /// <inheritdoc />
    public async ValueTask<NotificationPermission> GetPermissionAsync() =>
        ParsePermission(await TryAsync(() => InvokeAsync<string?>("getPermission"), null));

    /// <inheritdoc />
    public async ValueTask<NotificationPermission> RequestPermissionAsync() =>
        ParsePermission(await TryAsync(() => InvokeAsync<string?>("requestPermission"), null));

    /// <inheritdoc />
    public async ValueTask<WebPushSubscription?> GetSubscriptionAsync() =>
        await TryAsync(() => InvokeAsync<WebPushSubscription?>("getSubscription"), null);

    /// <inheritdoc />
    public async ValueTask<WebPushSubscribeResult> SubscribeAsync(string applicationServerKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationServerKey);
        var reply = await TryAsync(
            () => InvokeAsync<SubscribeReply?>("subscribe", applicationServerKey.Trim()), null);
        return reply is null ? new(WebPushSubscribeStatus.Unsupported) : ToResult(reply);
    }

    /// <inheritdoc />
    public async ValueTask<bool> UnsubscribeAsync() =>
        await TryAsync(() => InvokeAsync<bool>("unsubscribe"), false);

    internal static NotificationPermission ParsePermission(string? value) => value switch
    {
        "granted" => NotificationPermission.Granted,
        "denied" => NotificationPermission.Denied,
        "default" => NotificationPermission.Default,
        _ => NotificationPermission.Unsupported,
    };

    private static WebPushSubscribeResult ToResult(SubscribeReply reply) => reply.Status switch
    {
        "subscribed" when reply.Subscription is not null => new(WebPushSubscribeStatus.Subscribed, reply.Subscription),
        "permission" => new(WebPushSubscribeStatus.PermissionNotGranted),
        "no-worker" => new(WebPushSubscribeStatus.NoServiceWorker),
        "key-mismatch" => new(WebPushSubscribeStatus.KeyMismatch),
        "unsupported" => new(WebPushSubscribeStatus.Unsupported),
        _ => new(WebPushSubscribeStatus.Failed, Error: reply.Error ?? "The browser returned no subscription."),
    };

    private static async ValueTask<T> TryAsync<T>(Func<ValueTask<T>> call, T fallback)
    {
        try { return await call(); }
        catch (JSDisconnectedException) { return fallback; }
        catch (JSException) { return fallback; }
        catch (InvalidOperationException) { return fallback; } // JS interop is unavailable while prerendering
        catch (TaskCanceledException) { return fallback; }
    }

    internal sealed record SubscribeReply(string? Status, WebPushSubscription? Subscription, string? Error);
}
