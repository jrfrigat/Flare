using Flare.Components;

namespace Flare.Core.Tests;

/// <summary>
/// The seven old size names are source-compatible constants, not separate scales: each must resolve to
/// the same FlareSize step, so existing markup keeps meaning the same height.
/// </summary>
public sealed class LegacySizeAliasTests
{
    [Fact]
    public void EveryLegacyNameResolvesToTheSameFlareSizeStep()
    {
        Assert.All(
            new[] { ButtonSize.Xs, FieldSize.Xs, AvatarSize.Xs, ChipSize.Xs, PaginationSize.Xs, SwitchSize.Xs, TrackSize.Xs },
            value => Assert.Equal(FlareSize.Xs, value));
        Assert.All(
            new[] { ButtonSize.Sm, FieldSize.Sm, AvatarSize.Sm, ChipSize.Sm, PaginationSize.Sm, SwitchSize.Sm, TrackSize.Sm },
            value => Assert.Equal(FlareSize.Sm, value));
        Assert.All(
            new[] { ButtonSize.Md, FieldSize.Md, AvatarSize.Md, ChipSize.Md, PaginationSize.Md, SwitchSize.Md, TrackSize.Md },
            value => Assert.Equal(FlareSize.Md, value));
        Assert.All(
            new[] { ButtonSize.Lg, FieldSize.Lg, AvatarSize.Lg, ChipSize.Lg, PaginationSize.Lg, SwitchSize.Lg, TrackSize.Lg },
            value => Assert.Equal(FlareSize.Lg, value));
        Assert.All(
            new[] { ButtonSize.Xl, FieldSize.Xl, AvatarSize.Xl, ChipSize.Xl, PaginationSize.Xl, SwitchSize.Xl, TrackSize.Xl },
            value => Assert.Equal(FlareSize.Xl, value));
    }
}
