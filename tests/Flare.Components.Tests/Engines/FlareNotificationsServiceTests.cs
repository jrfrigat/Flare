using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareNotificationsServiceTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-notifications.js";

    private readonly Bunit.BunitJSModuleInterop _module;
    private readonly FlareNotificationsService _notifications;

    public FlareNotificationsServiceTests()
    {
        _module = JSInterop.SetupModule(Module);
        _notifications = new FlareNotificationsService(JSInterop.JSRuntime);
    }

    [Theory]
    [InlineData("granted", NotificationPermission.Granted)]
    [InlineData("denied", NotificationPermission.Denied)]
    [InlineData("default", NotificationPermission.Default)]
    [InlineData("unsupported", NotificationPermission.Unsupported)]
    public async Task GetPermission_ReadsTheBrowserValue(string value, NotificationPermission expected)
    {
        _module.Setup<string?>("getPermission").SetResult(value);

        Assert.Equal(expected, await _notifications.GetPermissionAsync());
        Assert.Empty(_module.Invocations["requestPermission"]);
    }

    [Fact]
    public async Task RequestPermission_ReturnsTheAnswer()
    {
        _module.Setup<string?>("requestPermission").SetResult("granted");

        Assert.Equal(NotificationPermission.Granted, await _notifications.RequestPermissionAsync());
    }

    [Theory]
    [InlineData("shown", NotificationShowResult.Shown)]
    [InlineData("permission", NotificationShowResult.PermissionNotGranted)]
    [InlineData("unsupported", NotificationShowResult.Unsupported)]
    [InlineData("failed", NotificationShowResult.Failed)]
    public async Task Show_ReportsWhatHappened(string reply, NotificationShowResult expected)
    {
        _module.Setup<string?>("show", _ => true).SetResult(reply);

        Assert.Equal(expected, await _notifications.ShowAsync(new BrowserNotification { Title = "Build finished" }));
    }

    [Fact]
    public async Task Show_PassesEveryPartToTheBrowser()
    {
        _module.Setup<string?>("show", _ => true).SetResult("shown");

        await _notifications.ShowAsync(new BrowserNotification
        {
            Title = "Build finished", Body = "All green", Tag = "build", Icon = "/icon.png", Url = "/builds/42",
        });

        var args = _module.Invocations["show"].Single().Arguments;
        Assert.Equal(["Build finished", "All green", "build", "/icon.png", "/builds/42"], args.Cast<string?>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Show_RefusesAnEmptyTitle(string title)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _notifications.ShowAsync(new BrowserNotification { Title = title }).AsTask());
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("data:text/html,hi")]
    [InlineData("//elsewhere.example/page")]
    [InlineData("\\evil.example")]
    [InlineData("\\\\evil.example")]
    [InlineData("/\\evil.example")]
    [InlineData("\\/evil.example")]
    public async Task Show_RefusesAnAddressThatIsNotAWebAddress(string url)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _notifications.ShowAsync(new BrowserNotification { Title = "T", Url = url }).AsTask());
        Assert.Empty(_module.Invocations["show"]);
    }

    [Theory]
    [InlineData("/builds/42")]
    [InlineData("builds/42?tab=log#end")]
    [InlineData("https://ci.example/builds/42")]
    [InlineData("http://localhost:5000/")]
    public void WebAndRelativeAddressesAreAccepted(string url)
    {
        Assert.True(FlareNotificationsService.IsSafeUrl(url));
    }

    // A circuit that dropped mid-call, or a PWA serving an older copy of the module, must not throw into the page.
    [Fact]
    public async Task Show_DegradesWhenTheBrowserCannotBeReached()
    {
        _module.Setup<string?>("show", _ => true).SetException(new JSException("show is not a function"));

        Assert.Equal(NotificationShowResult.Unsupported,
            await _notifications.ShowAsync(new BrowserNotification { Title = "T" }));
    }
}
