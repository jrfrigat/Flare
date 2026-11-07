using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Core.Tests;

public class BrowserViewportServiceTests
{
    [Fact]
    public async Task FailingImmediateCallbackDoesNotKeepWindowSubscription()
    {
        var module = new ViewportModule();
        await using var service = new BrowserViewportService(module);
        var calls = 0;

        await Assert.ThrowsAsync<FormatException>(async () => await service.SubscribeAsync(_ =>
        {
            calls++;
            return Task.FromException(new FormatException("Invalid callback state."));
        }, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains("stopViewportListener", module.Calls);
        await service.OnViewportResized(900, 600);
        Assert.Equal(1, calls);

        await using var live = await service.SubscribeAsync(_ => calls++, cancellationToken: TestContext.Current.CancellationToken);
        await service.OnViewportResized(1000, 600);
        Assert.Equal(3, calls);
    }

    [Fact]
    public async Task FailingElementCallbackUnobservesTheElement()
    {
        var module = new ViewportModule();
        await using var service = new BrowserViewportService(module);
        var calls = 0;

        await Assert.ThrowsAsync<FormatException>(async () => await service.ObserveElementAsync(default, _ =>
        {
            calls++;
            return Task.FromException(new FormatException("Invalid callback state."));
        }, cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains("unobserveElement", module.Calls);
        Assert.NotNull(module.ObservedId);
        await service.OnElementResized(module.ObservedId, ViewportModule.Rect);
        Assert.Equal(1, calls);
    }

    private sealed class ViewportModule : IJSRuntime, IJSObjectReference
    {
        public static readonly BrowserViewportService.ElementRectDto Rect = new(0, 0, 20, 20, 800, 600, 0, 0);
        public List<string> Calls { get; } = [];
        public string? ObservedId { get; private set; }

        public ValueTask<T> InvokeAsync<T>(string identifier, object?[]? args) =>
            InvokeAsync<T>(identifier, CancellationToken.None, args);

        public ValueTask<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Calls.Add(identifier);
            switch (identifier)
            {
                case "import":
                    return ValueTask.FromResult((T)(object)this);
                case "observeElement":
                    ObservedId = (string)args![0]!;
                    return ValueTask.FromResult((T)(object)Rect);
                case "getViewportSize":
                    return ValueTask.FromResult((T)(object)new BrowserViewportService.WindowSizeDto(800, 600));
                default:
                    return ValueTask.FromResult(default(T)!);
            }
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
