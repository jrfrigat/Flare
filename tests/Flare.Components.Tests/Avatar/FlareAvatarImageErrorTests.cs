using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// A URL that is safe to put in <c>src</c> can still fail to load. The avatar has to answer the
/// browser's error event with its own fallback instead of leaving a broken image, and a later valid URL
/// has to be tried again - otherwise the failure would be permanent.
/// </summary>
public class FlareAvatarImageErrorTests : FlareTestContext
{
    private static void FailImage(IRenderedComponent<FlareAvatar> cut)
        => cut.Find("img").TriggerEvent("onerror", new EventArgs());

    private static string AvatarSelector => $".{Css.Classes.Avatar.Root}";

    [Fact]
    public void AFailedImage_FallsBackToInitials()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Src, "/missing-avatar.png")
            .Add(x => x.Text, "Ann Lee"));

        Assert.Single(cut.FindAll("img"));
        FailImage(cut);

        Assert.Empty(cut.FindAll("img"));
        Assert.Contains("AL", cut.Markup);
    }

    [Fact]
    public void AFailedImage_UsesTheFallbackContent()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Src, "/missing-avatar.png")
            .Add(x => x.FallbackContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span id=\"avatar-fallback\">F</span>"))));

        FailImage(cut);

        Assert.Empty(cut.FindAll("img"));
        Assert.Single(cut.FindAll("#avatar-fallback"));
    }

    [Fact]
    public void ANewSource_LoadsAgainAfterAFailure()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Src, "/missing-avatar.png")
            .Add(x => x.Text, "Ann Lee"));
        FailImage(cut);
        Assert.Empty(cut.FindAll("img"));

        cut.Render(p => p
            .Add(x => x.Src, "/present-avatar.png")
            .Add(x => x.Text, "Ann Lee"));

        Assert.Equal("/present-avatar.png", cut.Find("img").GetAttribute("src"));
    }

    [Fact]
    public void TheAccessibleName_SurvivesTheFailure()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Src, "/missing-avatar.png")
            .Add(x => x.Alt, "Ann Lee portrait"));

        FailImage(cut);

        Assert.Equal("Ann Lee portrait", cut.Find(AvatarSelector).GetAttribute("aria-label"));
    }

    private int _max = 5;

    private void AddGroupParameters(ComponentParameterCollectionBuilder<FlareAvatarGroup> p)
    {
        p.Add(x => x.Max, _max);
        p.AddChildContent(builder =>
        {
            builder.OpenComponent<FlareAvatar>(0);
            builder.AddAttribute(1, nameof(FlareAvatar.Src), "/missing-avatar.png");
            builder.AddAttribute(2, nameof(FlareAvatar.Text), "Ann Lee");
            builder.CloseComponent();
        });
    }

    [Fact]
    public void AGroupVersionRerender_DoesNotRetryADeadUrl()
    {
        var cut = Render<FlareAvatarGroup>(AddGroupParameters);
        FailImage(cut.FindComponent<FlareAvatar>());
        Assert.Empty(cut.FindAll("img"));

        // The group version cascades through the avatar's OnParametersSet; the failure must survive it
        // because the URL itself did not change.
        _max = 4;
        cut.Render(AddGroupParameters);

        Assert.Empty(cut.FindAll("img"));
    }
}
