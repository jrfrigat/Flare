using System.Globalization;
using Flare.Abstractions;
using Flare.Components.IDE;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public sealed class IdeLiveLocalizationTests : FlareTestContext
{
    [Fact]
    public async Task FormulaBarDefaultsChangeWhileValuesAndExplicitCaptionsRemain()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(culture);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareFormulaBar>(f => f
            .Add(x => x.CellRef, "A1").Add(x => x.Value, "SUM(A1)").Add(x => x.FormulaLabel, "")));
        var bar = cut.FindComponent<FlareFormulaBar>().Instance;
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("ru")));
        cut.WaitForAssertion(() => Assert.Equal(culture.GetLocalizer<IdeStrings>()["FormulaBar_NameBox"].Value,
            cut.FindAll("input")[0].GetAttribute("aria-label")));
        Assert.Equal("", cut.FindAll("input")[1].GetAttribute("aria-label"));
        Assert.Equal("A1", cut.FindAll("input")[0].GetAttribute("value"));
        Assert.Equal("SUM(A1)", cut.FindAll("input")[1].GetAttribute("value"));
        Assert.Same(bar, cut.FindComponent<FlareFormulaBar>().Instance);
    }
}
