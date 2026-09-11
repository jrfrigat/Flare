
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
