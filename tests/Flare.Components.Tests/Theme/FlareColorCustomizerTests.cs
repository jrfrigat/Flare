using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareColorCustomizer shows a WCAG contrast preview once a primary color is chosen.
public class FlareColorCustomizerTests : FlareTestContext
{
    [Fact]
    public void SelectingPreset_ShowsContrastVerdict()
    {
        var cut = Render<FlareColorCustomizer>();
        Assert.Empty(cut.FindAll($".{Css.Classes.Color.CustomizerContrast}")); // nothing chosen yet

        cut.Find($"button.{Css.Classes.Color.CustomizerSwatch}").Click();      // pick the first preset

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Color.CustomizerContrast}"));
        var verdict = cut.Find($".{Css.Classes.Color.CustomizerContrastBadge}").TextContent.Trim();
        Assert.Contains(verdict, new[] { "AAA", "AA", "AA Large", "Fail" });
    }

    [Fact]
    public void ShowContrastFalse_HidesPreview()
    {
        var cut = Render<FlareColorCustomizer>(p => p.Add(c => c.ShowContrast, false));
        cut.Find($"button.{Css.Classes.Color.CustomizerSwatch}").Click();
        Assert.Empty(cut.FindAll($".{Css.Classes.Color.CustomizerContrast}"));
    }
}
