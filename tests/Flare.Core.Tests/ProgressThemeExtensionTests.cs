using System.Globalization;
using System.Text.RegularExpressions;
using Flare.Abstractions;
using Flare.Theme.FluentUI2;
using Flare.Theme.MaterialDesign3;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>Verifies the progress wave tokens supplied by theme packages, and the ring geometry they encode.</summary>
public sealed class ProgressThemeExtensionTests
{
    [Fact]
    public void Md3Expressive_ProvidesItsWaveTokens()
    {
        ITheme theme = new MaterialDesign3ExpressiveTheme();
        var flat = theme.Design.FlattenDesign();

        Assert.Equal("40px", flat[Css.Tokens.Md3e.Progress.Length]);
        Assert.Equal("20px", flat[Css.Tokens.Md3e.Progress.IndeterminateLength]);
        Assert.Equal("3px", flat[Css.Tokens.Md3e.Progress.Amplitude]);
        Assert.Equal("1750ms", flat[Css.Tokens.ProgressField.LinearIndeterminateDuration]);
        Assert.Equal("1500ms", flat[Css.Tokens.ProgressField.CircularIndeterminateRotationDuration]);
        Assert.Equal("6000ms", flat[Css.Tokens.ProgressField.CircularIndeterminateProgressDuration]);
        Assert.NotEqual("0", flat[Css.Tokens.ProgressField.CircularGap].TrimEnd('p', 'x'));
    }

    [Fact]
    public void Md3Expressive_ShipsNoScriptModules()
    {
        // The wave is CSS. A module reappearing here means rendering logic left the stylesheet again.
        Assert.Empty(((ITheme)new MaterialDesign3ExpressiveTheme()).ScriptAssets);
    }

    /// <summary>
    /// The ring mask and the ring stroke are one measurement written in two places: progress.css
    /// sets <c>--_circ-width</c> to a share of the box, and the core derives the radius it strokes
    /// as <c>(size - width) / 2</c>. That radius has to land on the mask's mean radius, or the wave
    /// is drawn beside the ring instead of on it. Nothing in CSS can check that, so it is checked here.
    /// </summary>
    [Fact]
    public void Md3Expressive_RingMaskSitsOnTheRadiusTheCoreStrokes()
    {
        var flat = new MaterialDesign3ExpressiveTheme().Design.FlattenDesign();
        var mask = flat[Css.Tokens.Md3e.Progress.RingMask];

        var strokeWidth = double.Parse(
            Regex.Match(mask, @"stroke-width='([\d.]+)'").Groups[1].Value,
            CultureInfo.InvariantCulture);

        var radii = Regex.Matches(mask, @"[ML]([\d.]+),([\d.]+)")
            .Select(m => (
                X: double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) - 50,
                Y: double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture) - 50))
            .Select(p => Math.Sqrt(p.X * p.X + p.Y * p.Y))
            .ToArray();

        Assert.NotEmpty(radii);
        var mean = (radii.Max() + radii.Min()) / 2;
        var amplitude = (radii.Max() - radii.Min()) / 2;

        // What progress.css declares: --_circ-width: calc(var(--_circ-size) * 0.18).
        const double DeclaredWidthRatio = 0.18;

        // A quarter of a unit of slack: the path's coordinates are rounded to one decimal, so a
        // sampled crest lands a hair off the true one. A real mismatch is whole units wide.
        const double Slack = 0.25;

        // The stroked band has to cover crest, trough and the line's own thickness, or the mask
        // reaches past the ring and the wave's tips are clipped.
        Assert.InRange(amplitude * 2 + strokeWidth,
            DeclaredWidthRatio * 100 - Slack, DeclaredWidthRatio * 100 + Slack);
        // And the radius the core strokes has to be the radius the wave oscillates about.
        var coreRadius = (100 - DeclaredWidthRatio * 100) / 2;
        Assert.InRange(mean, coreRadius - Slack, coreRadius + Slack);
    }

    [Fact]
    public void FlatThemes_DoNotProvideMd3ExpressiveExtensions()
    {
        foreach (var theme in new ITheme[]
                 {
                     new MaterialDesign3Theme(),
                     new FluentUI2Theme(),
                 })
        {
            var design = theme.Design.FlattenDesign();
            Assert.False(design.ContainsKey(Css.Tokens.Md3e.Progress.Length));
            Assert.False(design.ContainsKey(Css.Tokens.Md3e.Progress.RingMask));
            Assert.Empty(theme.ScriptAssets);
            Assert.True(design.ContainsKey(Css.Tokens.ProgressField.LinearIndeterminateDuration));
            Assert.True(design.ContainsKey(Css.Tokens.ProgressField.LinearIndeterminateEasing));
            Assert.True(design.ContainsKey(Css.Tokens.ProgressField.CircularIndeterminateRotationDuration));
            Assert.True(design.ContainsKey(Css.Tokens.ProgressField.CircularIndeterminateProgressDuration));
        }
    }
}
