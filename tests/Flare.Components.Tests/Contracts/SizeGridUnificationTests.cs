using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class SizeGridUnificationTests : FlareTestContext
{
    [Theory]
    [InlineData(ChipSize.Xs, Css.Classes.Chip.SizeXs)]
    [InlineData(ChipSize.Sm, Css.Classes.Chip.Sm)]
    [InlineData(ChipSize.Lg, Css.Classes.Chip.Lg)]
    [InlineData(ChipSize.Xl, Css.Classes.Chip.SizeXl)]
    public void Chip_Size_AppliesModifier(ChipSize size, string expected)
    {
        var cut = Render<FlareChip>(p => p.Add(x => x.Label, "x").Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Chip.Root}").ClassName);
    }

    [Fact]
    public void Avatar_Xs_AppliesModifier()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.Text, "AB").Add(x => x.Size, AvatarSize.Xs));
        Assert.Contains(Css.Classes.Avatar.SizeXs, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }

    [Fact]
    public void Slider_Xl_AppliesModifier()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Size, TrackSize.Xl));
        Assert.Contains(Css.Classes.Slider.Xl, cut.Find($".{Css.Classes.Slider.Root}").ClassName);
    }
}
