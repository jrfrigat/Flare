using System.Globalization;
using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// The MD3 Expressive circular wave is a mask laid over the ring the core already strokes, so two
/// drawings have to agree on one radius. The core takes its radius from <c>--_circ-width</c>; the
/// mask carries its own. Nothing in CSS can compare them, and when they drift the indicator breaks
/// into disconnected pieces rather than failing outright - so they are compared here.
/// </summary>
public sealed class ProgressRingWaveTests
{
    // The size ramp as Material states it: diameter, then active-indicator thickness.
    private static readonly (string Step, double Size, double Line)[] Ramp =
    [
        ("xs", 24, 3), ("sm", 32, 3), ("md", 40, 4), ("lg", 52, 4), ("xl", 64, 5),
    ];

    // MD3's wave is 1.6dp against a 4dp indicator.
    private const double AmplitudeOfLine = 0.4;

    [Theory]
    [InlineData("xs")]
    [InlineData("sm")]
    [InlineData("md")]
    [InlineData("lg")]
    [InlineData("xl")]
    public void EachStepsMaskSitsOnTheRadiusTheCoreStrokes(string step)
    {
        var (_, size, line) = Ramp.Single(r => r.Step == step);
        var rule = RuleFor(step);

        var lineShare = Share(rule, "--_ring-line");
        var widthShare = Share(rule, "--_circ-width");

        var amplitude = line * AmplitudeOfLine;

        // The shares are of --_circ-size, so at the step's own diameter they must reproduce the
        // tokens. That is what keeps a plain ring and a wavy one the same weight at the same size.
        Assert.Equal(line, lineShare * size, 2);
        Assert.Equal(2 * amplitude + line, widthShare * size, 2);

        // Now the mask itself, drawn in a 100-unit box that is scaled onto the element.
        var mask = MaskFor(rule);
        var radii = Radii(mask);
        var mean = (radii.Max() + radii.Min()) / 2;
        var maskAmplitude = (radii.Max() - radii.Min()) / 2;
        var stroke = double.Parse(
            Regex.Match(mask, @"stroke-width='([\d.]+)'").Groups[1].Value, CultureInfo.InvariantCulture);

        // Quarter of a unit of slack: path coordinates are rounded to one decimal, so a sampled
        // crest lands a hair off the true one. A real mismatch is whole units wide.
        const double Slack = 0.25;

        // The radius the core strokes, as a share of the box, is (100 - width) / 2.
        Assert.InRange(mean, (100 - widthShare * 100) / 2 - Slack, (100 - widthShare * 100) / 2 + Slack);
        // And crest, trough and line together have to fit inside that stroke, or the mask reaches
        // past the paint and the wave's tips are cut off.
        Assert.InRange(2 * maskAmplitude + stroke, widthShare * 100 - Slack, widthShare * 100 + Slack);
        // The line the viewer sees is the mask's stroke, so that is what has to match the token.
        Assert.InRange(stroke / 100 * size, line - Slack, line + Slack);
    }

    [Fact]
    public void TheWaveClosesOnItselfSoTheRingHasNoSeam()
    {
        foreach (var (step, _, _) in Ramp)
        {
            var radii = Radii(MaskFor(RuleFor(step)));
            // A whole number of waves is what makes the path's end meet its start; a fractional one
            // leaves a step in the ring at the twelve o'clock position.
            var crossings = 0;
            for (var i = 0; i < radii.Length; i++)
            {
                var mean = (radii.Max() + radii.Min()) / 2;
                if (radii[i] >= mean && radii[(i + 1) % radii.Length] < mean) crossings++;
            }
            Assert.True(crossings >= 5, $"{step}: only {crossings} wave(s) found");
        }
    }

    private static double[] Radii(string mask) =>
        Regex.Matches(mask, @"[ML]([\d.]+),([\d.]+)")
            .Select(m => (
                X: double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) - 50,
                Y: double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture) - 50))
            .Select(p => Math.Sqrt(p.X * p.X + p.Y * p.Y))
            .ToArray();

    private static string RuleFor(string step)
    {
        var css = File.ReadAllText(Path.Combine(FindRepoRoot(),
            "src", "Flare.Theme.MaterialDesign3Expressive", "wwwroot", "css", "components", "progress.css"));
        var match = Regex.Match(css,
            @"\.flare-progress--md3e-wavy\.flare-progress--" + step + @"\s*\{(.*?)\}",
            RegexOptions.Singleline);
        Assert.True(match.Success, $"no wave rule for the {step} size step in progress.css");
        return match.Groups[1].Value;
    }

    private static double Share(string rule, string property)
    {
        var match = Regex.Match(rule,
            Regex.Escape(property) + @":\s*calc\(var\(--_circ-size\)\s*\*\s*([\d.]+)\)");
        Assert.True(match.Success, $"{property} is not a share of --_circ-size; the mask cannot follow it");
        return double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
    }

    private static string MaskFor(string rule)
    {
        var match = Regex.Match(rule, @"--_ring-mask:\s*(url\(.*?\));", RegexOptions.Singleline);
        Assert.True(match.Success, "the size step declares no --_ring-mask");
        return match.Groups[1].Value;
    }

    private static string FindRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src", "Flare.Components")))
            dir = Path.GetDirectoryName(dir);
        return dir ?? throw new DirectoryNotFoundException("Could not locate the repo root.");
    }
}
