namespace Flare.Abstractions;

/// <summary>
/// Periodically polls for a newer application version and raises an event when one is found.
/// The service itself renders nothing - a consumer (e.g. a layout) subscribes to
/// <see cref="NewVersionAvailable"/> and decides how to surface it (toast, banner, dialog).
/// Configure it at registration via <c>AddFlareVersionCheck</c>.
/// </summary>
public interface IVersionCheckService : IAsyncDisposable
{
    /// <summary>
    /// Raised once per distinct newer version found by a check. Handlers run off the polling loop,
    /// so a Blazor subscriber should marshal UI work through <c>InvokeAsync</c>.
    /// </summary>
    event Func<VersionCheckInfo, Task>? NewVersionAvailable;

    /// <summary>The most recently detected version, or null if no check has found one yet.</summary>
    string? LatestVersion { get; }

    /// <summary>
    /// The version the app is currently running. In service-worker mode this is the first version read
    /// from the deployed assets manifest (null until that first successful probe); otherwise it is the
    /// configured <see cref="FlareVersionCheckOptions.CurrentVersion"/>. Useful for surfacing the live
    /// build in the UI - distinct from <see cref="LatestVersion"/>, which is only set once a newer
    /// build is detected.
    /// </summary>
    string? CurrentVersion { get; }

    /// <summary>Whether the periodic polling loop is currently running.</summary>
    bool IsRunning { get; }

    /// <summary>Starts the periodic polling loop. Idempotent - a second call is a no-op.</summary>
    void Start();

    /// <summary>Stops the polling loop. <see cref="Start"/> can be called again to resume.</summary>
    void Stop();

    /// <summary>Runs a single check immediately, regardless of the polling schedule.</summary>
    Task CheckNowAsync();

    /// <summary>
    /// Activates the waiting service worker (if any) and reloads onto the new version. Call this from
    /// the "Update" action of whatever you surface on <see cref="NewVersionAvailable"/>. No-op outside
    /// service-worker mode / when JS interop is unavailable.
    /// </summary>
    Task ApplyUpdateAsync();
}

/// <summary>Details passed to <see cref="IVersionCheckService.NewVersionAvailable"/>.</summary>
/// <param name="CurrentVersion">The version the app is currently running (may be null if unset).</param>
/// <param name="LatestVersion">The newer version that was detected.</param>
public readonly record struct VersionCheckInfo(string? CurrentVersion, string LatestVersion);

/// <summary>Configuration for the Flare version-check service (set at <c>AddFlareVersionCheck</c>).</summary>
public sealed class FlareVersionCheckOptions
{
    /// <summary>How often to poll for a newer version. Defaults to one hour.</summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// The version the app is currently running. A fetched version different from this (and not yet
    /// reported) raises <see cref="IVersionCheckService.NewVersionAvailable"/>.
    /// </summary>
    public string? CurrentVersion { get; set; }

    /// <summary>
    /// When true, the service polls the deployed app version itself by reading the Blazor PWA assets
    /// manifest (and nudging the service worker) on each tick - no <see cref="CheckForLatestVersion"/>
    /// needed. The first reading is taken as the running version; a later, different reading raises
    /// <see cref="IVersionCheckService.NewVersionAvailable"/>. Intended for WASM PWAs; the service
    /// worker registration stays an app concern. Takes precedence over <see cref="CheckForLatestVersion"/>.
    /// </summary>
    public bool UseServiceWorker { get; set; }

    /// <summary>
    /// When set (with <see cref="UseServiceWorker"/>), the service registers this service worker on
    /// start, so the app doesn't need its own registration script. Defaults to the Blazor PWA
    /// convention <c>service-worker.js</c>. Set to null if the app registers the worker itself.
    /// </summary>
    public string? ServiceWorkerPath { get; set; } = "service-worker.js";

    /// <summary>
    /// Returns the latest available version string (e.g. fetched from a <c>version.json</c> endpoint).
    /// Return null - or throw - to skip the current tick without raising. Used when
    /// <see cref="UseServiceWorker"/> is false; when both are unset, polling is a no-op.
    /// </summary>
    public Func<CancellationToken, Task<string?>>? CheckForLatestVersion { get; set; }

    /// <summary>
    /// When true (default), an initial check runs immediately on <see cref="IVersionCheckService.Start"/>
    /// instead of waiting a full <see cref="Interval"/>.
    /// </summary>
    public bool CheckImmediately { get; set; } = true;
}
