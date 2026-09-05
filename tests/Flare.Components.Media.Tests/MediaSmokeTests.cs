namespace Flare.Components.Tests;

/// <summary>
/// The package shipped with no test at all. Both components lean on the browser - a canvas the pointer
/// draws on, a video element - so a render test is the floor rather than the coverage they deserve: it
/// says the markup the JS attaches to is still there and still named what the JS expects.
/// </summary>
public class MediaSmokeTests : FlareTestContext
{
    [Fact]
    public void SignaturePad_RendersTheCanvasItsScriptDrawsOn()
    {
        var cut = Render<FlareSignaturePad>();

        Assert.NotEmpty(cut.FindAll(".flare-signature-pad"));
        Assert.NotEmpty(cut.FindAll("canvas.flare-signature-pad__canvas"));
    }

    [Fact]
    public void VideoPlayer_RendersAVideoElementForItsSource()
    {
        var cut = Render<FlareVideoPlayer>(p => p.Add(x => x.Src, "https://example.invalid/clip.mp4"));

        Assert.NotEmpty(cut.FindAll(".flare-video-player"));
        var video = cut.Find("video.flare-video-player__video");
        Assert.Equal("https://example.invalid/clip.mp4", video.GetAttribute("src"));
    }
}
