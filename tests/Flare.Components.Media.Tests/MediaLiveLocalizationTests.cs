using System.Globalization;
using Flare.Abstractions;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public sealed class MediaLiveLocalizationTests : FlareTestContext
{
    [Fact]
    public async Task MountedPlayerUpdatesControlsWithoutReplacingVideo()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(culture);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareVideoPlayer>(v => v.Add(x => x.Src, "test.mp4")));
        var player = cut.FindComponent<FlareVideoPlayer>().Instance;
        var video = cut.Find("video");
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("ru")));
        var text = culture.GetLocalizer<VideoPlayerStrings>();
        cut.WaitForAssertion(() => Assert.Equal(text["Play"].Value, cut.FindAll("button")[0].GetAttribute("aria-label")));
        Assert.Equal("test.mp4", cut.Find("video").GetAttribute("src"));
        Assert.Same(player, cut.FindComponent<FlareVideoPlayer>().Instance);
        Assert.Equal(video.Id, cut.Find("video").Id);
    }
}
