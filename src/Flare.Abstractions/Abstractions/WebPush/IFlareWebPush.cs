namespace Flare.Components;

/// <summary>
/// The browser side of Web Push: whether the browser can receive push messages, the notification
/// permission, and the push subscription an application hands to its server. Inject instead of using
/// IJSRuntime.
/// </summary>
/// <remarks>
/// <para>Nothing here asks the user anything on its own. Only <see cref="RequestPermissionAsync"/>
/// shows the browser's permission prompt, and <see cref="SubscribeAsync"/> refuses to subscribe until
/// the permission is granted instead of letting the browser prompt in its place.</para>
/// <para>The application owns everything around it: the service worker file and its registration, the
/// <c>push</c> and <c>notificationclick</c> handlers inside that worker, the VAPID key pair, storing
/// subscriptions and sending messages from the server.</para>
/// <para>On iOS and iPadOS the browser offers Web Push only to a web app added to the Home Screen
/// (16.4 and later); in an ordinary tab <see cref="IsSupportedAsync"/> reports false. Push also needs a
/// secure context - https or localhost.</para>
/// </remarks>
public interface IFlareWebPush
{
    /// <summary>
    /// Whether this browser, in this context, can subscribe to push messages: it has notifications, service
    /// workers and a push manager. False before the page is interactive (prerendering) as well.
    /// </summary>
    ValueTask<bool> IsSupportedAsync();

    /// <summary>
    /// Reads the notification permission without asking for it. Returns
    /// <see cref="NotificationPermission.Unsupported"/> when the browser has no notifications.
    /// </summary>
    ValueTask<NotificationPermission> GetPermissionAsync();

    /// <summary>
    /// Shows the browser's notification permission prompt and returns the user's answer. Call it from the
    /// handler of something the user did - a button's <c>OnClick</c> - never from
    /// <c>OnInitialized</c> or <c>OnAfterRender</c>: browsers ignore or reject the request without a user
    /// gesture, and once denied the prompt cannot be shown again from the page. When the permission is
    /// already decided, the browser returns it without prompting.
    /// </summary>
    ValueTask<NotificationPermission> RequestPermissionAsync();

    /// <summary>
    /// Reads the push subscription of the application's service worker, or null when there is none - no
    /// subscription yet, no service worker, or no support.
    /// </summary>
    ValueTask<WebPushSubscription?> GetSubscriptionAsync();

    /// <summary>
    /// Subscribes the application's service worker to push messages signed with
    /// <paramref name="applicationServerKey"/>, or returns the existing subscription made with the same key.
    /// Every outcome other than success comes back as a <see cref="WebPushSubscribeStatus"/>, not an
    /// exception.
    /// </summary>
    /// <param name="applicationServerKey">The public VAPID key of the application's server, base64url
    /// encoded - the form web-push libraries print it in. The private key never leaves the server.</param>
    /// <exception cref="ArgumentException"><paramref name="applicationServerKey"/> is null or
    /// whitespace.</exception>
    ValueTask<WebPushSubscribeResult> SubscribeAsync(string applicationServerKey);

    /// <summary>
    /// Cancels the current push subscription. Returns true when a subscription was cancelled, false when
    /// there was none or the browser refused. Tell the server to forget the subscription as well.
    /// </summary>
    ValueTask<bool> UnsubscribeAsync();
}
