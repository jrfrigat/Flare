using System.Text.Json;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareWebPushServiceTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-web-push.js";
    private const string Key = "BExampleServerKey_base64url-123";

    private readonly Bunit.BunitJSModuleInterop _module;
    private readonly FlareWebPushService _push;

    public FlareWebPushServiceTests()
    {
        _module = JSInterop.SetupModule(Module);
        _push = new FlareWebPushService(JSInterop.JSRuntime);
    }

    private static WebPushSubscription Sample(long? expires = null) => new()
    {
        Endpoint = "https://push.example/send/abc",
        ExpirationTime = expires,
        Keys = new WebPushSubscriptionKeys { P256dh = "BPub", Auth = "secret" },
    };

    [Theory]
    [InlineData("granted", NotificationPermission.Granted)]
    [InlineData("denied", NotificationPermission.Denied)]
    [InlineData("default", NotificationPermission.Default)]
    [InlineData("unsupported", NotificationPermission.Unsupported)]
    public async Task GetPermission_ReadsTheBrowserValue(string value, NotificationPermission expected)
    {
        _module.Setup<string?>("getPermission").SetResult(value);

        Assert.Equal(expected, await _push.GetPermissionAsync());
    }

    [Fact]
    public async Task RequestPermission_ReturnsTheAnswer()
    {
        _module.Setup<string?>("requestPermission").SetResult("granted");

        Assert.Equal(NotificationPermission.Granted, await _push.RequestPermissionAsync());
    }

    // Reading the permission must never be the thing that asks for it.
    [Fact]
    public async Task GetPermission_DoesNotCallRequest()
    {
        _module.Setup<string?>("getPermission").SetResult("default");

        await _push.GetPermissionAsync();

        Assert.Empty(_module.Invocations["requestPermission"]);
    }

    [Theory]
    [InlineData("permission", WebPushSubscribeStatus.PermissionNotGranted)]
    [InlineData("no-worker", WebPushSubscribeStatus.NoServiceWorker)]
    [InlineData("key-mismatch", WebPushSubscribeStatus.KeyMismatch)]
    [InlineData("unsupported", WebPushSubscribeStatus.Unsupported)]
    public async Task Subscribe_ReportsWhyItDidNotSubscribe(string status, WebPushSubscribeStatus expected)
    {
        _module.Setup<FlareWebPushService.SubscribeReply?>("subscribe", _ => true)
            .SetResult(new FlareWebPushService.SubscribeReply(status, null, null));

        var result = await _push.SubscribeAsync(Key);

        Assert.Equal(expected, result.Status);
        Assert.Null(result.Subscription);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Subscribe_ReturnsTheSubscription_AndPassesTheKey()
    {
        _module.Setup<FlareWebPushService.SubscribeReply?>("subscribe", _ => true)
            .SetResult(new FlareWebPushService.SubscribeReply("subscribed", Sample(), null));

        var result = await _push.SubscribeAsync($"  {Key} ");

        Assert.True(result.Succeeded);
        Assert.Equal("https://push.example/send/abc", result.Subscription!.Endpoint);
        Assert.Equal(Key, _module.Invocations["subscribe"].Single().Arguments[0]);
    }

    [Fact]
    public async Task Subscribe_CarriesTheBrowsersReason()
    {
        _module.Setup<FlareWebPushService.SubscribeReply?>("subscribe", _ => true)
            .SetResult(new FlareWebPushService.SubscribeReply("failed", null, "AbortError: push service unreachable"));

        var result = await _push.SubscribeAsync(Key);

        Assert.Equal(WebPushSubscribeStatus.Failed, result.Status);
        Assert.Equal("AbortError: push service unreachable", result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Subscribe_RejectsAMissingKey(string key)
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(async () => await _push.SubscribeAsync(key));
    }

    // A PWA can serve a cached module older than the service; the page must not fall over.
    [Fact]
    public async Task AModuleWithoutTheFunction_DegradesInsteadOfThrowing()
    {
        _module.Setup<bool>("isSupported").SetException(new JSException("isSupported is not a function"));
        _module.Setup<FlareWebPushService.SubscribeReply?>("subscribe", _ => true)
            .SetException(new JSException("subscribe is not a function"));
        _module.Setup<string?>("getPermission").SetException(new JSException("getPermission is not a function"));

        Assert.False(await _push.IsSupportedAsync());
        Assert.Equal(NotificationPermission.Unsupported, await _push.GetPermissionAsync());
        Assert.Equal(WebPushSubscribeStatus.Unsupported, (await _push.SubscribeAsync(Key)).Status);
    }

    [Fact]
    public async Task GetSubscription_ReturnsNull_WhenThereIsNone()
    {
        _module.Setup<WebPushSubscription?>("getSubscription").SetResult(null);

        Assert.Null(await _push.GetSubscriptionAsync());
    }

    [Fact]
    public async Task Unsubscribe_ReturnsWhetherOneWasCancelled()
    {
        _module.Setup<bool>("unsubscribe").SetResult(true);

        Assert.True(await _push.UnsubscribeAsync());
    }

    // Server-side web-push libraries take the browser's PushSubscription.toJSON() shape as it is.
    [Fact]
    public void Subscription_SerializesToTheBrowsersJsonShape()
    {
        var json = JsonSerializer.Serialize(Sample());

        Assert.Equal(
            """{"endpoint":"https://push.example/send/abc","expirationTime":null,"keys":{"p256dh":"BPub","auth":"secret"}}""",
            json);
    }

    [Fact]
    public void Subscription_ReadsTheBrowsersJson()
    {
        const string json = """{"endpoint":"https://e","expirationTime":1767225600000,"keys":{"p256dh":"k","auth":"a"}}""";

        var sub = JsonSerializer.Deserialize<WebPushSubscription>(json)!;

        Assert.Equal("k", sub.Keys.P256dh);
        Assert.Equal(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), sub.ExpiresAt);
    }
}
