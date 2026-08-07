using System.Globalization;
using Flare.Components.Resources;

namespace Flare.Components.Tests.QrCode;

// ---------------------------------------------------------------------------
// Component-level tests for FlareQrCode. QrCodeGeneratorTests proves the matrix
// is scannable; these prove the component puts the right matrix on the page and
// falls back gracefully when there isn't one.
// ---------------------------------------------------------------------------
public class FlareQrCodeTests : FlareTestContext
{
    /// <summary>Module count of the rendered symbol, read back off the SVG.</summary>
    private static int ModuleCountOf(IRenderedComponent<FlareQrCode> cut, int size)
    {
        // Every dark module is one rect; the first rect is the background. Module
        // size is the drawn size divided by the symbol plus its 4-module quiet zone.
        var rect = cut.FindAll("rect").Skip(1).First();
        double moduleSize = double.Parse(rect.GetAttribute("width")!, CultureInfo.InvariantCulture);
        return (int)Math.Round(size / moduleSize) - 8;
    }

    [Fact]
    public void ShortPayload_RendersTheSmallestSymbol()
    {
        var cut = Render<FlareQrCode>(p => p.Add(x => x.Value, "hi").Add(x => x.Size, 200));

        Assert.Equal(21, ModuleCountOf(cut, 200)); // version 1
    }

    [Fact]
    public void LongPayload_GrowsTheSymbolInsteadOfRefusing()
    {
        // 300 bytes is far past the old version-4 ceiling of 60 at level M, which
        // used to render the "value too long" placeholder instead of a code.
        var payload = new string('x', 300);

        var cut = Render<FlareQrCode>(p => p.Add(x => x.Value, payload).Add(x => x.Size, 400));

        Assert.DoesNotContain(FlareStrings.QrCode_ValueTooLong, cut.Markup);
        int modules = ModuleCountOf(cut, 400);
        Assert.True(modules > 33, $"expected a symbol past version 4 (33 modules), got {modules}");
        Assert.Equal(0, (modules - 21) % 4); // a real version: 21, 25, 29, ...
    }

    [Fact]
    public void PayloadBeyondEveryVersion_ShowsTheLocalizedNotice()
    {
        var cut = Render<FlareQrCode>(p => p
            .Add(x => x.Value, new string('x', 3000))
            .Add(x => x.ErrorCorrectionLevel, QrErrorCorrectionLevel.H));

        Assert.Contains(FlareStrings.QrCode_ValueTooLong, cut.Markup);
        Assert.Empty(cut.FindAll("rect").Skip(1)); // background only, no modules
    }

    [Fact]
    public void ChangingTheCorrectionLevel_ReEncodes()
    {
        // The component skips encoding when neither the payload nor the level moved.
        // A level change must still be picked up: H needs a larger symbol than L for
        // the same payload, so the module count is what proves the re-encode happened.
        var payload = new string('x', 100);
        var cut = Render<FlareQrCode>(p => p
            .Add(x => x.Value, payload)
            .Add(x => x.Size, 400)
            .Add(x => x.ErrorCorrectionLevel, QrErrorCorrectionLevel.L));
        int atLow = ModuleCountOf(cut, 400);

        cut.Render(p => p.Add(x => x.ErrorCorrectionLevel, QrErrorCorrectionLevel.H));

        Assert.True(ModuleCountOf(cut, 400) > atLow,
            "raising the correction level did not grow the symbol, so the cached matrix was reused");
    }

    [Fact]
    public void ChangingAnUnrelatedParameter_KeepsTheSameSymbol()
    {
        var cut = Render<FlareQrCode>(p => p.Add(x => x.Value, "hello").Add(x => x.Size, 200));
        int before = ModuleCountOf(cut, 200);

        cut.Render(p => p.Add(x => x.ForeColor, "#123456"));

        Assert.Equal(before, ModuleCountOf(cut, 200));
        Assert.Contains("#123456", cut.Markup);
    }
}
