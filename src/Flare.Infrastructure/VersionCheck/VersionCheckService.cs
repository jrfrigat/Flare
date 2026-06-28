using Flare.Abstractions;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>
/// Default <see cref="IVersionCheckService"/>: a <see cref="PeriodicTimer"/> loop that probes for a
/// newer version and raises <see cref="NewVersionAvailable"/> once per distinct version. The probe is
/// either the built-in service-worker reader (<see cref="FlareVersionCheckOptions.UseServiceWorker"/>)
/// or the caller's <see cref="FlareVersionCheckOptions.CheckForLatestVersion"/> delegate. Renders nothing.
/// </summary>
public sealed class VersionCheckService : IVersionCheckService
{
    private const string ModulePath = "./_content/Flare.Components/js/flare-version-check.js";

    private readonly FlareVersionCheckOptions _options;
    private readonly IJSRuntime? _js;
    private CancellationTokenSource? _cts;
    private Task? _loop;
    private string? _lastNotified;
    private string? _baseline;          // first version observed in service-worker mode
    private IJSObjectReference? _module; // lazily imported SW probe module

    /// <summary>
    /// Creates the service. <paramref name="js"/> is required only for
    /// <see cref="FlareVersionCheckOptions.UseServiceWorker"/>; it is supplied automatically by
    /// <c>AddFlareVersionCheck</c>.
    /// </summary>
    public VersionCheckService(FlareVersionCheckOptions options, IJSRuntime? js = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
        _js = js;
    }

    /// <inheritdoc />
    public event Func<VersionCheckInfo, Task>? NewVersionAvailable;

    /// <inheritdoc />
    public string? LatestVersion { get; private set; }

    /// <inheritdoc />
    public string? CurrentVersion => _options.UseServiceWorker ? _baseline : _options.CurrentVersion;

    /// <inheritdoc />
    public bool IsRunning => _loop is { IsCompleted: false };

    /// <inheritdoc />
    public void Start()
    {
        if (IsRunning) return;
        _cts = new CancellationTokenSource();
        _loop = RunLoopAsync(_cts.Token);
    }

    /// <inheritdoc />
    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        _loop = null;
    }

    /// <inheritdoc />
    public Task CheckNowAsync() => CheckOnceAsync(_cts?.Token ?? CancellationToken.None);

    private async Task RunLoopAsync(CancellationToken ct)
    {
        try
        {
            // Register the app's service worker (so the app needs no registration script of its own).
            if (_options.UseServiceWorker && !string.IsNullOrEmpty(_options.ServiceWorkerPath) && _js is not null)
            {
                try
                {
                    var module = await GetModuleAsync(ct);
                    await module.InvokeVoidAsync("register", ct, _options.ServiceWorkerPath);
                }
                catch (OperationCanceledException) { throw; }
                catch { /* SW registration is best-effort */ }
            }

            if (_options.CheckImmediately)
                await CheckOnceAsync(ct);

            // PeriodicTimer keeps a steady cadence and is cancellation-aware (works in WASM too).
            using var timer = new PeriodicTimer(_options.Interval);
            while (await timer.WaitForNextTickAsync(ct))
                await CheckOnceAsync(ct);
        }
        catch (OperationCanceledException) { /* stopped */ }
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync(CancellationToken ct)
        => _module ??= await _js!.InvokeAsync<IJSObjectReference>("import", ct, ModulePath);

    private async Task<string?> ProbeAsync(CancellationToken ct)
    {
        if (_options.UseServiceWorker)
        {
            if (_js is null) return null;
            var module = await GetModuleAsync(ct);
            return await module.InvokeAsync<string?>("check", ct);
        }

        return _options.CheckForLatestVersion is not null
            ? await _options.CheckForLatestVersion(ct)
            : null;
    }

    /// <inheritdoc />
    public async Task ApplyUpdateAsync()
    {
        if (_js is null) return;
        try
        {
            var module = await GetModuleAsync(CancellationToken.None);
            await module.InvokeVoidAsync("applyUpdate");
        }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    private async Task CheckOnceAsync(CancellationToken ct)
    {
        string? latest;
        try
        {
            latest = await ProbeAsync(ct);
        }
        catch (OperationCanceledException) { throw; }
        catch
        {
            // A failed probe (offline, 404, JS disconnected, etc.) just skips this tick.
            return;
        }

        if (string.IsNullOrEmpty(latest)) return;

        // Service-worker mode: the first successful reading is the running version, not an update.
        if (_options.UseServiceWorker && _baseline is null)
        {
            _baseline = latest;
            _options.CurrentVersion ??= latest;
            return;
        }

        var current = _options.UseServiceWorker ? _baseline : _options.CurrentVersion;
        if (latest == current || latest == _lastNotified) return;

        _lastNotified = latest;
        LatestVersion = latest;

        var handler = NewVersionAvailable;
        if (handler is null) return;

        var info = new VersionCheckInfo(current, latest);
        foreach (var d in handler.GetInvocationList().Cast<Func<VersionCheckInfo, Task>>())
        {
            try { await d(info); }
            catch (OperationCanceledException) { throw; }
            catch { /* a faulty subscriber must not break the polling loop */ }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        Stop();
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); }
            catch (JSDisconnectedException) { }
            catch (OperationCanceledException) { }
            _module = null;
        }
    }
}
