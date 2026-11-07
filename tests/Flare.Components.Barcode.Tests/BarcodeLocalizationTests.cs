using System.Globalization;
using Flare.Abstractions;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public sealed class BarcodeLocalizationTests : FlareTestContext
{
    [Theory]
    [InlineData("", BarcodeSymbology.Code128, "Empty")]
    [InlineData("\u0430", BarcodeSymbology.Code128, "AsciiOnly", '\u0430')]
    [InlineData("12x", BarcodeSymbology.Ean13, "DigitsOnly")]
    [InlineData("12", BarcodeSymbology.Ean13, "DigitCount", 12, 13, 2)]
    [InlineData("A&B", BarcodeSymbology.Code39, "Code39Character", '&')]
    [InlineData("12x", BarcodeSymbology.Itf14, "ItfDigitsOnly")]
    [InlineData("12", BarcodeSymbology.Itf14, "DigitCount", 13, 14, 2)]
    [InlineData("123", BarcodeSymbology.Codabar, "CodabarDelimiters")]
    [InlineData("A?B", BarcodeSymbology.Codabar, "CodabarCharacter", '?')]
    public async Task FailureRelocalizesAndTheNeutralDiagnosticIsReportedOnce(string value,
        BarcodeSymbology symbology, string failure, params object[] arguments)
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(culture);
        var diagnostics = new List<string>();
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareBarcode>(b => b
            .Add(x => x.Value, value).Add(x => x.Symbology, symbology).Add(x => x.OnInvalid, diagnostics.Add)));
        var original = cut.Find("[role=status]").TextContent;
        var diagnostic = Assert.Single(diagnostics);
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("ru")));
        var expected = culture.GetLocalizer<FlareStrings>()["Barcode_Error_" + failure, arguments].Value;
        cut.WaitForAssertion(() => Assert.Equal(expected, cut.Find("[role=status]").TextContent));
        Assert.NotEqual(original, expected);
        Assert.Equal(diagnostic, Assert.Single(diagnostics));
        BarcodeEncoder.TryEncode(value, symbology, out var legacy);
        Assert.Equal(legacy, diagnostic);
    }
}
