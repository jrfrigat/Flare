using Flare.Abstractions;
using Flare.Theme.FluentUI2;
using Flare.Theme.MaterialDesign3;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>Verifies the progress extension assets and tokens supplied by theme packages.</summary>
public sealed class ProgressThemeExtensionTests
{
    [Fact]
    public void Md3Expressive_ProvidesItsWaveRenderer()
    {
        var theme = new MaterialDesign3ExpressiveTheme();
        var flat = theme.Design.FlattenDesign();

        Assert.Equal("20px", flat[Css.Tokens.Md3e.Progress.IndeterminateLength]);
        Assert.Equal("15px", flat[Css.Tokens.Md3e.Progress.RingLength]);
        Assert.Equal("1750ms", flat[Css.Tokens.ProgressField.LinearIndeterminateDuration]);
        Assert.Equal("1500ms", flat[Css.Tokens.ProgressField.CircularIndeterminateRotationDuration]);
        Assert.Equal("6000ms", flat[Css.Tokens.ProgressField.CircularIndeterminateProgressDuration]);
        Assert.Contains("_content/Flare.Theme.MaterialDesign3Expressive/js/progress-wave.js", theme.ScriptAssets);
        Assert.NotEqual("0", flat[Css.Tokens.ProgressField.CircularGap].TrimEnd('p', 'x'));
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
            Assert.Empty(theme.ScriptAssets);
            Assert.True(design.ContainsKey(Css.Tokens.ProgressField.LinearIndeterminateDuration));
            Assert.True(design.ContainsKey(Css.Tokens.ProgressField.LinearIndeterminateEasing));
            Assert.True(design.ContainsKey(Css.Tokens.ProgressField.CircularIndeterminateRotationDuration));
            Assert.True(design.ContainsKey(Css.Tokens.ProgressField.CircularIndeterminateProgressDuration));
        }
    }
}
