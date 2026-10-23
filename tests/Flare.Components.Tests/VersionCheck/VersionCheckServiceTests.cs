using Flare.Abstractions;
using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// VersionCheckService polls a probe and tells the app once per distinct newer version. The tests
/// drive the probe directly (no network) and the service-worker half through a fake JS runtime
/// (no module, no timer, no sleep).
/// </summary>
public class VersionCheckServiceTests
{
    private static VersionCheckService Service(FlareVersionCheckOptions options, IJSRuntime? js = null)
        => new(options, js);

    private static FlareVersionCheckOptions Probe(Queue<string?> versions, string? current = "1.0.0")
        => new()
        {
            CurrentVersion = current,
            CheckForLatestVersion = _ => Task.FromResult(versions.Count > 0 ? versions.Dequeue() : null),
        };

    [Fact]
    public async Task NoProbe_SkipsTheTickWithoutRaising()
    {
        var service = Service(new FlareVersionCheckOptions { CurrentVersion = "1.0.0" });
        var raised = 0;
        service.NewVersionAvailable += _ => { raised++; return Task.CompletedTask; };

        await service.CheckNowAsync();

        Assert.Equal(0, raised);
        Assert.Null(service.LatestVersion);
    }

    [Fact]
    public async Task NullOrEmptyProbe_SkipsTheTick()
    {
        var service = Service(Probe(new Queue<string?>(new string?[] { null, "" })));
        var raised = 0;
        service.NewVersionAvailable += _ => { raised++; return Task.CompletedTask; };

        await service.CheckNowAsync();
        await service.CheckNowAsync();

        Assert.Equal(0, raised);
    }

    [Fact]
    public async Task TheRunningVersion_IsNotAnUpdate()
    {
        var service = Service(Probe(new Queue<string?>(new[] { "1.0.0" })));
        var raised = 0;
        service.NewVersionAvailable += _ => { raised++; return Task.CompletedTask; };

        await service.CheckNowAsync();

        Assert.Equal(0, raised);
        Assert.Null(service.LatestVersion);
    }

    [Fact]
    public async Task ANewerVersion_IsAnnouncedOnce()
    {
        var service = Service(Probe(new Queue<string?>(new[] { "2.0.0", "2.0.0" })));
        var infos = new List<VersionCheckInfo>();
        service.NewVersionAvailable += info => { infos.Add(info); return Task.CompletedTask; };

        await service.CheckNowAsync();
        await service.CheckNowAsync();

        var info = Assert.Single(infos);
        Assert.Equal("1.0.0", info.CurrentVersion);
        Assert.Equal("2.0.0", info.LatestVersion);
        Assert.Equal("2.0.0", service.LatestVersion);
    }

    [Fact]
    public async Task AnotherNewerVersion_IsAnnouncedAgain()
    {
        var service = Service(Probe(new Queue<string?>(new[] { "2.0.0", "3.0.0" })));
        var infos = new List<VersionCheckInfo>();
        service.NewVersionAvailable += info => { infos.Add(info); return Task.CompletedTask; };

        await service.CheckNowAsync();
        await service.CheckNowAsync();

        Assert.Equal(new[] { "2.0.0", "3.0.0" }, infos.Select(i => i.LatestVersion));
    }

    [Fact]
    public async Task AFaultySubscriber_DoesNotStopTheOthers()
    {
        var service = Service(Probe(new Queue<string?>(new[] { "2.0.0" })));
        var reached = 0;
        service.NewVersionAvailable += _ => throw new InvalidOperationException("subscriber");
        service.NewVersionAvailable += _ => { reached++; return Task.CompletedTask; };

        await service.CheckNowAsync();

        Assert.Equal(1, reached);
    }

    [Fact]
    public async Task SubscriberCancellation_Propagates()
    {
        var service = Service(Probe(new Queue<string?>(new[] { "2.0.0" })));
        service.NewVersionAvailable += _ => throw new OperationCanceledException();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(service.CheckNowAsync);
    }

    [Fact]
    public async Task AProbeFailure_SkipsTheTick()
    {
        var service = Service(new FlareVersionCheckOptions
        {
            CurrentVersion = "1.0.0",
            CheckForLatestVersion = _ => throw new InvalidOperationException("offline"),
        });
        var raised = 0;
        service.NewVersionAvailable += _ => { raised++; return Task.CompletedTask; };

        await service.CheckNowAsync();

        Assert.Equal(0, raised);
    }

    [Fact]
    public async Task ProbeCancellation_Propagates()
    {
        var service = Service(new FlareVersionCheckOptions
        {
            CurrentVersion = "1.0.0",
            CheckForLatestVersion = _ => throw new OperationCanceledException(),
        });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(service.CheckNowAsync);
    }

    [Fact]
    public async Task ServiceWorker_FirstReadingIsTheBaselineThenTheUpdate()
    {
        var js = new FakeJs();
        js.Module.Versions.Enqueue("1.0");
        js.Module.Versions.Enqueue("1.0");
        js.Module.Versions.Enqueue("2.0");
        var options = new FlareVersionCheckOptions { UseServiceWorker = true, ServiceWorkerPath = null };
        var service = Service(options, js);
        var infos = new List<VersionCheckInfo>();
        service.NewVersionAvailable += info => { infos.Add(info); return Task.CompletedTask; };

        await service.CheckNowAsync();
        Assert.Equal("1.0", service.CurrentVersion);
        Assert.Empty(infos);

        await service.CheckNowAsync();
        Assert.Empty(infos);

        await service.CheckNowAsync();
        var info = Assert.Single(infos);
        Assert.Equal("1.0", info.CurrentVersion);
        Assert.Equal("2.0", info.LatestVersion);
        Assert.Equal(3, js.Module.Count("check"));
    }

    [Fact]
    public async Task ServiceWorkerCheckFailure_SkipsTheTick()
    {
        var js = new FakeJs();
        js.Module.CheckError = new JSException("no worker");
        var service = Service(new FlareVersionCheckOptions
        {
            UseServiceWorker = true,
            ServiceWorkerPath = null,
        }, js);
        var raised = 0;
        service.NewVersionAvailable += _ => { raised++; return Task.CompletedTask; };

        await service.CheckNowAsync();

        Assert.Equal(0, raised);
    }

    [Fact]
    public async Task ApplyUpdate_CallsTheModuleAndSwallowsJsErrors()
    {
        var js = new FakeJs();
        var service = Service(new FlareVersionCheckOptions
        {
            UseServiceWorker = true,
            ServiceWorkerPath = null,
        }, js);

        await service.ApplyUpdateAsync();
        Assert.Equal(1, js.Module.Count("applyUpdate"));

        js.Module.ApplyError = new JSException("gone");
        await service.ApplyUpdateAsync();

        js.Module.ApplyError = new JSDisconnectedException("circuit gone");
        await service.ApplyUpdateAsync();
    }

    [Fact]
    public void Start_ChecksImmediatelyWhenConfigured()
    {
        var checks = 0;
        var service = Service(new FlareVersionCheckOptions
        {
            CheckImmediately = true,
            CheckForLatestVersion = _ => { checks++; return Task.FromResult<string?>(null); },
        });

        service.Start();
        service.Stop();

        Assert.Equal(1, checks);
    }

    [Fact]
    public async Task ApplyUpdate_WithoutJavascript_IsANoOp()
    {
        var service = Service(new FlareVersionCheckOptions());

        await service.ApplyUpdateAsync();
    }

    [Fact]
    public void StartIsIdempotentAndStopStops()
    {
        var service = Service(new FlareVersionCheckOptions { CheckImmediately = false });

        service.Start();
        Assert.True(service.IsRunning);
        service.Start();
        Assert.True(service.IsRunning);

        service.Stop();
        Assert.False(service.IsRunning);
        service.Stop();
        Assert.False(service.IsRunning);

        service.Start();
        Assert.True(service.IsRunning);
        service.Stop();
    }

    [Fact]
    public void ServiceWorkerRegistrationFailure_DoesNotStopTheLoop()
    {
        var js = new FakeJs();
        js.Module.RegisterError = new JSException("no worker");
        var service = Service(new FlareVersionCheckOptions
        {
            UseServiceWorker = true,
            ServiceWorkerPath = "service-worker.js",
            CheckImmediately = false,
        }, js);

        service.Start();

        Assert.True(service.IsRunning);
        service.Stop();
    }

    [Fact]
    public void ServiceWorkerRegistrationCancellation_StopsTheLoop()
    {
        var js = new FakeJs();
        js.Module.RegisterError = new OperationCanceledException();
        var service = Service(new FlareVersionCheckOptions
        {
            UseServiceWorker = true,
            ServiceWorkerPath = "service-worker.js",
            CheckImmediately = false,
        }, js);

        service.Start();

        Assert.False(service.IsRunning);
    }

    [Fact]
    public async Task Dispose_StopsTheLoopAndDisposesTheModule()
    {
        var js = new FakeJs();
        js.Module.Versions.Enqueue("1.0");
        var service = Service(new FlareVersionCheckOptions
        {
            UseServiceWorker = true,
            ServiceWorkerPath = null,
        }, js);
        await service.CheckNowAsync();

        await service.DisposeAsync();

        Assert.False(service.IsRunning);
        Assert.True(js.Module.Disposed);
        await service.DisposeAsync();
    }

    [Theory]
    [InlineData("js")]
    [InlineData("disconnected")]
    [InlineData("cancelled")]
    public async Task Dispose_SwallowsModuleDisposalFailures(string kind)
    {
        var js = new FakeJs();
        js.Module.Versions.Enqueue("1.0");
        var service = Service(new FlareVersionCheckOptions
        {
            UseServiceWorker = true,
            ServiceWorkerPath = null,
        }, js);
        await service.CheckNowAsync();
        js.Module.DisposeError = kind switch
        {
            "js" => new JSException("gone"),
            "disconnected" => new JSDisconnectedException("circuit gone"),
            _ => new OperationCanceledException(),
        };

        await service.DisposeAsync();

        Assert.True(js.Module.Disposed);
    }

    [Fact]
    public void CurrentVersion_IsTheConfiguredValueOutsideServiceWorkerMode()
    {
        Assert.Equal("1.2.3", Service(new FlareVersionCheckOptions { CurrentVersion = "1.2.3" }).CurrentVersion);
    }

    [Fact]
    public void OptionsAreRequired()
    {
        Assert.Throws<ArgumentNullException>(() => new VersionCheckService(null!));
    }

    private sealed class FakeJs : IJSRuntime
    {
        public FakeModule Module { get; } = new();

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            if (identifier != "import") throw new InvalidOperationException(identifier);
            return ValueTask.FromResult((TValue)(object)Module);
        }
    }

    private sealed class FakeModule : IJSObjectReference
    {
        public Queue<string?> Versions { get; } = new();
        private readonly List<string> _calls = new();
        public Exception? RegisterError { get; set; }
        public Exception? CheckError { get; set; }
        public Exception? ApplyError { get; set; }
        public Exception? DisposeError { get; set; }
        public bool Disposed { get; private set; }

        public int Count(string identifier) => _calls.Count(c => c == identifier);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            _calls.Add(identifier);
            switch (identifier)
            {
                case "check":
                    if (CheckError is not null) throw CheckError;
                    var value = Versions.Count > 0 ? Versions.Dequeue() : null;
                    return ValueTask.FromResult((TValue)(object?)value);
                case "register":
                    if (RegisterError is not null) throw RegisterError;
                    return ValueTask.FromResult(default(TValue)!);
                case "applyUpdate":
                    if (ApplyError is not null) throw ApplyError;
                    return ValueTask.FromResult(default(TValue)!);
                default:
                    throw new InvalidOperationException(identifier);
            }
        }

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            if (DisposeError is not null) throw DisposeError;
            return ValueTask.CompletedTask;
        }
    }
}
