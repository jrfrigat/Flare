using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// WCAG contrast math used by the ColorCustomizer's accessibility preview.
public class ColorMathContrastTests
{
    [Fact]
    public void BlackOnWhite_IsMaxRatio()
        => Assert.Equal(21.0, Flare.Theming.ColorMath.ContrastRatio("#000000", "#FFFFFF"), 1);

    [Fact]
    public void SameColor_IsOne()
        => Assert.Equal(1.0, Flare.Theming.ColorMath.ContrastRatio("#6750A4", "#6750A4"), 2);

    [Fact]
    public void IsSymmetric()
        => Assert.Equal(
            Flare.Theming.ColorMath.ContrastRatio("#6750A4", "#FFFFFF"),
            Flare.Theming.ColorMath.ContrastRatio("#FFFFFF", "#6750A4"), 4);

    [Fact]
    public void WhiteOnDarkPrimary_PassesAa()
        => Assert.True(Flare.Theming.ColorMath.ContrastRatio("#FFFFFF", "#6750A4") >= 4.5);
}
