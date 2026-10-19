using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-381: with a percent Format the field shows 0.25 as "25%" when blurred and edits 0.25 when focused.
/// Pasting or committing the formatted text returns the fraction, in any culture and either number path,
/// while text without the percent symbol keeps the underlying value and focus/blur changes nothing.
/// </summary>
public class NumericFieldPercentTests : FlareTestContext
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");

    [Fact]
    public void PercentFormat_ShowsFormattedValueAndEditsTheRawValue()
    {
        var changes = 0;
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Format, "P0")
            .Add(x => x.Culture, En)
            .Add(x => x.Value, 0.25m)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal>(this, (decimal _) => { changes++; })));

        Assert.Equal("text", cut.Find("input").GetAttribute("type"));
        Assert.Equal("25%", cut.Find("input").GetAttribute("value"));

        cut.Find("input").Focus();
        Assert.Equal("0.25", cut.Find("input").GetAttribute("value"));

        cut.Find("input").Blur();
        Assert.Equal("25%", cut.Find("input").GetAttribute("value"));
        Assert.Equal(0, changes);
    }

    [Theory]
    [InlineData("25%", 0.25)]
    [InlineData("-25%", -0.25)]
    [InlineData("1,234%", 12.34)]
    public void PastedPercent_CommitsTheFraction_Decimal(string text, double expected)
    {
        decimal? committed = null;
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Format, "P0")
            .Add(x => x.Culture, En)
            .Add(x => x.Value, 0.5m)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal>(this, (decimal v) => committed = v)));

        cut.Find("input").Focus();
        cut.Find("input").Change(text);

        Assert.Equal((decimal)expected, committed);
    }

    [Fact]
    public void PastedPercent_CommitsTheFraction_Double()
    {
        double? committed = null;
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Format, "P0")
            .Add(x => x.Culture, En)
            .Add(x => x.Value, 0.5d)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, (double v) => committed = v)));

        cut.Find("input").Focus();
        cut.Find("input").Change("25%");

        Assert.Equal(0.25d, committed);
    }

    [Theory]
    [InlineData("25,5")]
    [InlineData("-1,5")]
    [InlineData("1234,5")]
    public void PastedCultureFormattedPercent_CommitsTheFraction(string rawNumber)
    {
        // The culture's own text, including its thousands separator and the space before the symbol.
        var de = CultureInfo.GetCultureInfo("de-DE");
        var value = decimal.Parse(rawNumber, NumberStyles.Number, de) / 100m;
        var formatted = value.ToString("P1", de);

        decimal? committed = null;
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Format, "P1")
            .Add(x => x.Culture, de)
            .Add(x => x.Value, 1m)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal>(this, (decimal v) => committed = v)));

        cut.Find("input").Focus();
        cut.Find("input").Change(formatted);

        Assert.Equal(value, committed);
    }

    [Fact]
    public void PercentSymbolBeforeTheNumber_CommitsTheFraction()
    {
        var tr = CultureInfo.GetCultureInfo("tr-TR");
        var formatted = 0.25m.ToString("P0", tr);
        Assert.StartsWith("%", formatted);

        decimal? committed = null;
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Format, "P0")
            .Add(x => x.Culture, tr)
            .Add(x => x.Value, 1m)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal>(this, (decimal v) => committed = v)));

        cut.Find("input").Focus();
        cut.Find("input").Change(formatted);

        Assert.Equal(0.25m, committed);
    }

    [Fact]
    public void CustomPercentFormat_CommitsTheFraction()
    {
        decimal? committed = null;
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Format, "0.0%")
            .Add(x => x.Culture, En)
            .Add(x => x.Value, 0.5m)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal>(this, (decimal v) => committed = v)));

        Assert.Equal("50.0%", cut.Find("input").GetAttribute("value"));
        cut.Find("input").Focus();
        cut.Find("input").Change("25.5%");

        Assert.Equal(0.255m, committed);
    }

    [Fact]
    public void EscapedPercent_IsNotAPercentFormat()
    {
        decimal? committed = null;
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Format, "0.0\\%")
            .Add(x => x.Culture, En)
            .Add(x => x.Value, 25m)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal>(this, (decimal v) => committed = v)));

        cut.Find("input").Focus();
        cut.Find("input").Change("25%");

        Assert.Equal(25m, committed);
    }
}
