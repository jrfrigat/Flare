using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareMeter rides the same scale, and deliberately the same tokens: a meter is the same rule of track as a
// linear progress bar, so they must not be able to drift apart.
public class FlareMeterSizeTests : FlareTestContext
{
    private static RenderFragment OneSegment => b =>
    {
        b.OpenComponent<FlareMeterSegment>(0);
        b.AddAttribute(1, nameof(FlareMeterSegment.Value), (double?)1.0);
        b.CloseComponent();
    };

    [Theory]
    [InlineData(TrackSize.Xs, Css.Classes.Meter.SizeXs)]
    [InlineData(TrackSize.Sm, Css.Classes.Meter.SizeSm)]
    [InlineData(TrackSize.Md, Css.Classes.Meter.SizeMd)]
    [InlineData(TrackSize.Lg, Css.Classes.Meter.SizeLg)]
    [InlineData(TrackSize.Xl, Css.Classes.Meter.SizeXl)]
    public void Size_AppliesSizeClass(TrackSize size, string expected)
    {
        var cut = Render<FlareMeter>(p => p.Add(x => x.Size, size).Add(x => x.ChildContent, OneSegment));

        Assert.Contains(expected, cut.Find($".{Css.Classes.Meter.Root}").ClassName);
    }

    [Fact]
    public void DefaultSize_IsMd_MatchingProgress()
    {
        var meter = Render<FlareMeter>(p => p.Add(x => x.ChildContent, OneSegment));
        var progress = Render<FlareProgressLinear>(p => p.Add(x => x.Value, 50d));

        Assert.Contains(Css.Classes.Meter.SizeMd, meter.Find($".{Css.Classes.Meter.Root}").ClassName);
        Assert.Contains(Css.Classes.Progress.SizeMd, progress.Find($".{Css.Classes.Progress.Root}").ClassName);
    }
}
