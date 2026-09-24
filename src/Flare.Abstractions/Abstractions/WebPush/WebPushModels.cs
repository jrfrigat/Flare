using System.Text.Json.Serialization;

namespace Flare.Components;

/// <summary>The user's answer to the browser's notification permission prompt.</summary>
public enum NotificationPermission
{
    /// <summary>Not decided yet: the prompt has not been answered, so it can still be shown.</summary>
    Default,
    /// <summary>Notifications are allowed.</summary>
    Granted,
    /// <summary>Notifications are blocked. The page cannot prompt again; only the browser's site settings
    /// can change it.</summary>
    Denied,
    /// <summary>The browser has no notifications, or the page is not interactive yet.</summary>
    Unsupported,
}

/// <summary>How a call to <see cref="IFlareWebPush.SubscribeAsync"/> ended.</summary>
public enum WebPushSubscribeStatus
{
    /// <summary>The service worker is subscribed; <see cref="WebPushSubscribeResult.Subscription"/> holds
    /// the subscription to send to the server.</summary>
    Subscribed,
    /// <summary>The browser cannot receive push messages in this context.</summary>
    Unsupported,
    /// <summary>The notification permission is not granted. Ask for it with
    /// <see cref="IFlareWebPush.RequestPermissionAsync"/> from a user action first.</summary>
    PermissionNotGranted,
    /// <summary>The page has no registered service worker to receive the messages.</summary>
    NoServiceWorker,
    /// <summary>A subscription made with a different server key already exists. Unsubscribe first -
    /// and let the server forget the old subscription - then subscribe with the new key.</summary>
    KeyMismatch,
    /// <summary>The browser or its push service rejected the subscription;
    /// <see cref="WebPushSubscribeResult.Error"/> says why.</summary>
    Failed,
}

/// <summary>The outcome of <see cref="IFlareWebPush.SubscribeAsync"/>.</summary>
/// <param name="Status">How the call ended.</param>
/// <param name="Subscription">The subscription when <paramref name="Status"/> is
/// <see cref="WebPushSubscribeStatus.Subscribed"/>; otherwise null.</param>
/// <param name="Error">The browser's reason when <paramref name="Status"/> is
/// <see cref="WebPushSubscribeStatus.Failed"/>; otherwise null.</param>
public sealed record WebPushSubscribeResult(
    WebPushSubscribeStatus Status,
    WebPushSubscription? Subscription = null,
    string? Error = null)
{
    /// <summary>Whether the service worker is subscribed.</summary>
    [JsonIgnore]
    public bool Succeeded => Status == WebPushSubscribeStatus.Subscribed && Subscription is not null;
}

/// <summary>
/// A push subscription as the server needs it to send messages. Serialized with System.Text.Json it
/// produces exactly the browser's <c>PushSubscription.toJSON()</c> shape -
/// <c>{"endpoint": ..., "expirationTime": ..., "keys": {"p256dh": ..., "auth": ...}}</c> - which
/// server-side web-push libraries accept as it is.
/// </summary>
public sealed record WebPushSubscription
{
    /// <summary>The push service URL the server posts messages to. Unique per subscription.</summary>
    [JsonPropertyName("endpoint")]
    public required string Endpoint { get; init; }

    /// <summary>When the subscription expires, in milliseconds since the Unix epoch, or null when the
    /// push service set no expiry - which is the usual case.</summary>
    [JsonPropertyName("expirationTime")]
    public long? ExpirationTime { get; init; }

    /// <summary>The keys the server encrypts each message with.</summary>
    [JsonPropertyName("keys")]
    public required WebPushSubscriptionKeys Keys { get; init; }

    /// <summary><see cref="ExpirationTime"/> as a point in time; not serialized.</summary>
    [JsonIgnore]
    public DateTimeOffset? ExpiresAt =>
        ExpirationTime is { } ms ? DateTimeOffset.FromUnixTimeMilliseconds(ms) : null;
}

/// <summary>The encryption keys of a <see cref="WebPushSubscription"/>, base64url encoded.</summary>
public sealed record WebPushSubscriptionKeys
{
    /// <summary>The browser's P-256 elliptic-curve public key.</summary>
    [JsonPropertyName("p256dh")]
    public required string P256dh { get; init; }

    /// <summary>The shared authentication secret.</summary>
    [JsonPropertyName("auth")]
    public required string Auth { get; init; }
}
