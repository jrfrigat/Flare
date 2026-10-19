using System.Globalization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The transport bar used to carry English labels as literals, so a Russian application announced an
/// English player. Every name comes from the Media package resources now and the fullscreen button says
/// which way it will go.
/// </summary>
public class VideoPlayerLocalizationTests : FlareTestContext
{
    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _previous = CultureInfo.CurrentUICulture;

        public CultureScope(string name) => CultureInfo.CurrentUICulture = new CultureInfo(name);

        public void Dispose() => CultureInfo.CurrentUICulture = _previous;
    }

    private IRenderedComponent<FlareVideoPlayer> RenderPlayer() =>
        Render<FlareVideoPlayer>(p => p
            .Add(x => x.Src, "test.mp4")
            .Add(x => x.PlaybackRates, new[] { 1.0, 1.5, 2.0 }));

    private static string? ButtonLabel(IRenderedComponent<FlareVideoPlayer> cut, int index) =>
        cut.FindAll($"button.{Css.Classes.VideoPlayer.Button}")[index].GetAttribute("aria-label");

    [Fact]
    public void RussianCulture_LocalizesEveryControl()
    {
        using var scope = new CultureScope("ru");
        var cut = RenderPlayer();

        Assert.Equal("Видеоплеер", cut.Find($".{Css.Classes.VideoPlayer.Video}").GetAttribute("aria-label"));
        Assert.Equal("Воспроизвести", ButtonLabel(cut, 0));
        Assert.Equal("Выключить звук", ButtonLabel(cut, 1));
        Assert.Equal("Полноэкранный режим", ButtonLabel(cut, 2));

        var sliders = cut.FindAll($"input.{Css.Classes.VideoPlayer.Progress}");
        Assert.Equal("Перемотка", sliders[0].GetAttribute("aria-label"));
        Assert.Equal("Громкость", sliders[1].GetAttribute("aria-label"));

        Assert.Equal("Скорость воспроизведения",
            cut.Find($"select.{Css.Classes.VideoPlayer.Rate}").GetAttribute("aria-label"));
    }

    [Fact]
    public void EnglishCulture_KeepsTheEnglishControls()
    {
        using var scope = new CultureScope("en");
        var cut = RenderPlayer();

        Assert.Equal("Video player", cut.Find($".{Css.Classes.VideoPlayer.Video}").GetAttribute("aria-label"));
        Assert.Equal("Play", ButtonLabel(cut, 0));
        Assert.Equal("Mute", ButtonLabel(cut, 1));
        Assert.Equal("Fullscreen", ButtonLabel(cut, 2));
        Assert.Equal("Playback speed", cut.Find($"select.{Css.Classes.VideoPlayer.Rate}").GetAttribute("aria-label"));
    }

    [Fact]
    public void ThePlayButton_FollowsThePlaybackState()
    {
        using var scope = new CultureScope("ru");
        var cut = RenderPlayer();
        Assert.Equal("Воспроизвести", ButtonLabel(cut, 0));

        cut.InvokeAsync(() => cut.Instance.OnPlayStateChanged(true));
        cut.WaitForAssertion(() => Assert.Equal("Пауза", ButtonLabel(cut, 0)));

        cut.InvokeAsync(() => cut.Instance.OnPlayStateChanged(false));
        cut.WaitForAssertion(() => Assert.Equal("Воспроизвести", ButtonLabel(cut, 0)));
    }

    [Fact]
    public void TheFullscreenButton_OffersTheWayBackWhileFullscreen()
    {
        using var scope = new CultureScope("ru");
        var cut = RenderPlayer();
        Assert.Equal("Полноэкранный режим", ButtonLabel(cut, 2));

        cut.InvokeAsync(() => cut.Instance.OnFullscreenChanged(true));
        cut.WaitForAssertion(() => Assert.Equal("Выйти из полноэкранного режима", ButtonLabel(cut, 2)));

        cut.InvokeAsync(() => cut.Instance.OnFullscreenChanged(false));
        cut.WaitForAssertion(() => Assert.Equal("Полноэкранный режим", ButtonLabel(cut, 2)));
    }
}
