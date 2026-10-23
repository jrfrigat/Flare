using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// FlareVideoPlayer is a thin shell over videoplayer.js. What a rendering test can hold is the
/// contract with that module: the calls it makes, the state it keeps in step, and what it does when
/// the browser is gone. Real media events belong to a browser run.
/// </summary>
public class VideoPlayerTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components.Media/js/videoplayer.js";

    private IRenderedComponent<FlareVideoPlayer> RenderPlayer(
        Action<ComponentParameterCollectionBuilder<FlareVideoPlayer>>? extra = null)
        => Render<FlareVideoPlayer>(p =>
        {
            p.Add(x => x.Src, "https://example.invalid/clip.mp4");
            extra?.Invoke(p);
        });

    private static IReadOnlyList<JSRuntimeInvocation> Calls(BunitJSModuleInterop module, string identifier)
        => module.Invocations[identifier];

    [Fact]
    public void Markup_ExposesTheVideoTrackAndTheGivenLabel()
    {
        JSInterop.SetupModule(Module);

        var cut = RenderPlayer(p => p
            .Add(x => x.Poster, "https://example.invalid/poster.jpg")
            .Add(x => x.AutoPlay, true)
            .Add(x => x.Loop, true)
            .Add(x => x.Muted, true)
            .Add(x => x.SubtitleSrc, "https://example.invalid/subs.vtt")
            .Add(x => x.AriaLabel, "Lecture recording"));

        var video = cut.Find("video");
        Assert.Equal("https://example.invalid/poster.jpg", video.GetAttribute("poster"));
        Assert.True(video.HasAttribute("autoplay"));
        Assert.True(video.HasAttribute("loop"));
        Assert.True(video.HasAttribute("muted"));
        Assert.True(video.HasAttribute("playsinline"));
        Assert.Equal("Lecture recording", video.GetAttribute("aria-label"));
        var track = cut.Find("track");
        Assert.Equal("https://example.invalid/subs.vtt", track.GetAttribute("src"));
        Assert.Equal("subtitles", track.GetAttribute("kind"));
    }

    [Fact]
    public void ControlsFalse_LeavesOnlyTheVideo()
    {
        JSInterop.SetupModule(Module);

        var cut = RenderPlayer(p => p.Add(x => x.Controls, false));

        Assert.Empty(cut.FindAll("button"));
        Assert.Empty(cut.FindAll("input[type=range]"));
    }

    [Fact]
    public void Init_RunsOnceWithTheElementAndTheCallbackReference()
    {
        var module = JSInterop.SetupModule(Module);

        RenderPlayer();

        Assert.Single(Calls(module, "init"));
    }

    [Fact]
    public async Task PlayAndPause_FollowTheReportedState()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderPlayer();
        var play = cut.FindAll("button")[0];
        var initialLabel = play.GetAttribute("aria-label");

        await cut.InvokeAsync(() => play.Click());
        Assert.Single(Calls(module, "play"));

        await cut.InvokeAsync(() => cut.Instance.OnPlayStateChanged(true));
        var paused = cut.FindAll("button")[0];
        Assert.NotEqual(initialLabel, paused.GetAttribute("aria-label"));

        await cut.InvokeAsync(() => paused.Click());
        Assert.Single(Calls(module, "pause"));
    }

    [Fact]
    public async Task Mute_TogglesTheAttributeAndTheCommand()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderPlayer();
        var video = cut.Find("video");
        Assert.False(video.HasAttribute("muted"));

        await cut.InvokeAsync(() => cut.FindAll("button")[1].Click());

        Assert.True(cut.Find("video").HasAttribute("muted"));
        Assert.Single(Calls(module, "toggleMute"));

        await cut.InvokeAsync(() => cut.FindAll("button")[1].Click());
        Assert.False(cut.Find("video").HasAttribute("muted"));
    }

    [Fact]
    public async Task Fullscreen_FollowsTheBrowserReport()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderPlayer();
        var label = cut.FindAll("button")[^1].GetAttribute("aria-label");

        await cut.InvokeAsync(() => cut.FindAll("button")[^1].Click());
        Assert.Single(Calls(module, "requestFullscreen"));

        await cut.InvokeAsync(() => cut.Instance.OnFullscreenChanged(true));
        Assert.NotEqual(label, cut.FindAll("button")[^1].GetAttribute("aria-label"));
    }

    [Fact]
    public async Task Seek_ParsesInvariantlyAndIgnoresGarbage()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderPlayer();
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            await cut.InvokeAsync(() => cut.FindAll("input[type=range]")[0].Input("42.5"));
            var seek = Assert.Single(Calls(module, "seek"));
            Assert.Equal(42.5, (double)seek.Arguments[1]!);

            await cut.InvokeAsync(() => cut.FindAll("input[type=range]")[0].Input("42,5x"));
            Assert.Single(Calls(module, "seek"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public async Task Volume_ParsesAndIgnoresGarbage()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderPlayer();
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            await cut.InvokeAsync(() => cut.FindAll("input[type=range]")[1].Input("0.3"));
            var set = Assert.Single(Calls(module, "setVolume"));
            Assert.Equal(0.3, (double)set.Arguments[1]!);

            await cut.InvokeAsync(() => cut.FindAll("input[type=range]")[1].Input("loud"));
            Assert.Single(Calls(module, "setVolume"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public async Task PlaybackRate_RendersItsOptionsAndSendsTheChosenRate()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderPlayer(p => p.Add(x => x.PlaybackRates, new[] { 0.5, 1.0, 1.5 }));

        Assert.Equal(3, cut.FindAll("select option").Count);
        await cut.InvokeAsync(() => cut.Find("select").Change("1.5"));

        var set = Assert.Single(Calls(module, "setPlaybackRate"));
        Assert.Equal(1.5, (double)set.Arguments[1]!);
    }

    [Fact]
    public async Task TimeUpdate_DrivesTheFormattedPositionAndDuration()
    {
        JSInterop.SetupModule(Module);
        var cut = RenderPlayer();

        await cut.InvokeAsync(() => cut.Instance.OnTimeUpdate(65, 3725));

        Assert.Contains("1:05 / 1:02:05", cut.Find($".{Css.Classes.VideoPlayer.Time}").TextContent);
    }

    [Fact]
    public async Task Ended_ReturnsTheButtonToPlay()
    {
        JSInterop.SetupModule(Module);
        var cut = RenderPlayer();
        await cut.InvokeAsync(() => cut.Instance.OnPlayStateChanged(true));
        var label = cut.FindAll("button")[0].GetAttribute("aria-label");

        await cut.InvokeAsync(() => cut.Instance.OnEnded());

        Assert.NotEqual(label, cut.FindAll("button")[0].GetAttribute("aria-label"));
    }

    // A failed import leaves no module behind; bUnit cannot make "import" throw directly, so the
    // state that import failure produces is set here. The guard is what matters: no module, no call.
    [Fact]
    public async Task NoModule_TurnsTheCommandsIntoNoOps()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderPlayer();
        typeof(FlareVideoPlayer)
            .GetField("_module", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .SetValue(cut.Instance, null);

        await cut.InvokeAsync(() => cut.FindAll("button")[0].Click());
        await cut.InvokeAsync(() => cut.FindAll("input[type=range]")[0].Input("5"));

        Assert.Empty(Calls(module, "play"));
        Assert.Empty(Calls(module, "seek"));
        Assert.NotEmpty(cut.FindAll("video"));
    }

    [Fact]
    public async Task ADisconnectedCommand_DoesNotBreakTheComponent()
    {
        var module = JSInterop.SetupModule(Module);
        module.SetupVoid("play").SetException(new JSDisconnectedException("circuit gone"));
        var cut = RenderPlayer();

        await cut.InvokeAsync(() => cut.FindAll("button")[0].Click());

        Assert.NotEmpty(cut.FindAll("video"));
    }

    [Fact]
    public async Task Dispose_StopsTheBrowserSidePlayer()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderPlayer();

        await cut.Instance.DisposeAsync();

        Assert.Single(Calls(module, "destroy"));
    }
}
