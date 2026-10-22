using System.Globalization;
using Flare.Abstractions;
using Flare.Theme.Aero;
using Flare.Theme.FluentUI2;
using Flare.Theme.LiquidGlass;
using Flare.Theme.MaterialDesign2;
using Flare.Theme.MaterialDesign3;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.VisualStudio;
using Flare.Theming;

namespace Flare.Guards.Tests;

/// <summary>
/// A larger size step of a progress ring must not draw a thinner stroke than a smaller one. The ramp is
/// the theme's statement about emphasis: Xl with less stroke than Lg reads as a rendering mistake, and
/// MaterialDesign3 shipped exactly that (Lg 8px, Xl 5px) while the comment beside it said 4dp.
/// </summary>
public sealed class ProgressRingRampTests
{
    private static readonly Func<ITheme>[] Factories =
    [
        () => new AeroTheme(),
        () => new FluentUI2Theme(),
        () => new LiquidGlassTheme(),
        () => new MaterialDesign2Theme(),
        () => new MaterialDesign3Theme(),
        () => new MaterialDesign3ExpressiveTheme(),
        () => new VisualStudioTheme(),
    ];

    public static TheoryData<int> ThemeIndexes()
    {
        var d = new TheoryData<int>();
        for (var i = 0; i < Factories.Length; i++) d.Add(i);
        return d;
    }

    [Theory]
    [MemberData(nameof(ThemeIndexes))]
    public void CircularSizeAndStrokeNeverShrinkAsTheStepGrows(int index)
    {
        var theme = Factories[index]();
        var progress = theme.Design.Progress;

        AssertNonDecreasing(theme.Id, "size",
            progress.CircularSizeXs, progress.CircularSizeSm, progress.CircularSizeMd,
            progress.CircularSizeLg, progress.CircularSizeXl);
        AssertNonDecreasing(theme.Id, "stroke",
            progress.CircularWidthXs, progress.CircularWidthSm, progress.CircularWidthMd,
            progress.CircularWidthLg, progress.CircularWidthXl);
    }

    private static void AssertNonDecreasing(string themeId, string kind, params string[] values)
    {
        var numbers = values.Select(v => Pixels(v, themeId, kind)).ToArray();
        for (var i = 1; i < numbers.Length; i++)
        {
            Assert.True(numbers[i] >= numbers[i - 1],
                $"{themeId}: circular {kind} shrinks from step {i - 1} to {i} - " +
                $"{string.Join(" / ", values)}");
        }
    }

    private static double Pixels(string value, string themeId, string kind)
    {
        var text = value.Trim();
        Assert.True(text.EndsWith("px", StringComparison.Ordinal),
            $"{themeId}: circular {kind} '{value}' is not a px length");
        return double.Parse(text[..^2], CultureInfo.InvariantCulture);
    }
}
