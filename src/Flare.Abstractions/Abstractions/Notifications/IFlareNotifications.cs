namespace Flare.Components;

/// <summary>
/// System notifications shown by the page through the browser's Notification API - a message that appears
/// outside the tab and, when clicked, brings the tab forward and opens an address. Inject instead of
/// writing a JS module.
/// </summary>
/// <remarks>
/// <para>This is the page showing a notification while it is open. Notifications that arrive while the
/// page is closed come from a server through Web Push - see <see cref="IFlareWebPush"/>.</para>
/// <para>Nothing here asks the user anything on its own: only <see cref="RequestPermissionAsync"/> shows
/// the permission prompt, and <see cref="ShowAsync"/> reports a missing permission instead of prompting.
/// Notifications need a secure context - https or localhost.</para>
/// <para>Where the browser forbids constructing a notification from the page (Chrome on Android), it is
/// shown through the page's service worker registration instead, with the address in
/// <c>notification.data.url</c>; a click there reaches the application's own <c>notificationclick</c>
/// handler, not this service. Without a registered worker such a browser reports
/// <see cref="NotificationShowResult.Unsupported"/>.</para>
/// </remarks>
public interface IFlareNotifications
{
    /// <summary>Whether this browser, in this context, has notifications. False before the page is
    /// interactive (prerendering) as well.</summary>
    ValueTask<bool> IsSupportedAsync();

    /// <summary>Reads the notification permission without asking for it.</summary>
    ValueTask<NotificationPermission> GetPermissionAsync();

    /// <summary>
    /// Shows the browser's notification permission prompt and returns the user's answer. Call it from the
    /// handler of something the user did - a button's <c>OnClick</c>: browsers ignore a request made without
    /// a user gesture, and once denied the prompt cannot be shown again from the page.
    /// </summary>
    ValueTask<NotificationPermission> RequestPermissionAsync();

    /// <summary>Shows a notification when the permission is granted, and says whether it was shown.</summary>
    /// <param name="notification">What to show and where a click leads.</param>
    /// <exception cref="ArgumentException">The title is empty, or the address is neither relative nor
    /// http(s).</exception>
    ValueTask<NotificationShowResult> ShowAsync(BrowserNotification notification);
}

/// <summary>A notification to show with <see cref="IFlareNotifications.ShowAsync"/>.</summary>
public sealed record BrowserNotification
{
    /// <summary>The notification's heading.</summary>
    public required string Title { get; init; }

    /// <summary>The text under the heading.</summary>
    public string? Body { get; init; }

    /// <summary>Groups notifications: a new one with the same tag replaces the one already shown instead of
    /// adding another.</summary>
    public string? Tag { get; init; }

    /// <summary>Address of an image shown beside the text, such as the application's icon.</summary>
    public string? Icon { get; init; }

    /// <summary>Address opened when the notification is clicked, relative to the application or an absolute
    /// http(s) address. An address in the application is opened without reloading it. When null, a click
    /// only brings the tab forward.</summary>
    public string? Url { get; init; }
}

/// <summary>What happened to a notification passed to <see cref="IFlareNotifications.ShowAsync"/>.</summary>
public enum NotificationShowResult
{
    /// <summary>The notification was handed to the system.</summary>
    Shown,
    /// <summary>Not shown: the user has not allowed notifications (or has blocked them).</summary>
    PermissionNotGranted,
    /// <summary>Not shown: the browser has no notifications here, or the page is not interactive yet.</summary>
    Unsupported,
    /// <summary>Not shown: the browser refused it for another reason.</summary>
    Failed,
}
