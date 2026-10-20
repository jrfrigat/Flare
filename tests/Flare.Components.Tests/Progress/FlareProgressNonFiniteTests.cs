namespace Flare.Components.Tests;

/// <summary>
/// A percentage that is not a finite number cannot be drawn or announced. It is treated as "not given"
/// so the indicator is indeterminate, never NaN in ARIA or a NaN length in CSS/SVG.
/// </summary>
public class FlareProgressNonFiniteTests : FlareTestContext
{
    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void ANonFiniteValueIsIndeterminate(double value)
    {
        var linear = Render<FlareProgressLinear>(p => p.Add(x => x.Value, value));
        var bar = linear.Find("[role=progressbar]");
        Assert.Null(bar.GetAttribute("aria-valuenow"));
        Assert.Contains(Css.Classes.Progress.Indeterminate, bar.ClassName);
        Assert.DoesNotContain("NaN", linear.Markup);

        var circular = Render<FlareProgressCircular>(p => p.Add(x => x.Value, value));
        Assert.Null(circular.Find("[role=progressbar]").GetAttribute("aria-valuenow"));
        Assert.DoesNotContain("NaN", circular.Markup);
    }

    [Fact]
    public void ANonFiniteBufferValueFallsBackToZeroWithoutNaN()
    {
        var cut = Render<FlareProgressLinear>(p => p
            .Add(x => x.Variant, LinearProgressVariant.Buffer)
            .Add(x => x.Value, 50d)
            .Add(x => x.BufferValue, double.NaN));

        Assert.Equal("50", cut.Find("[role=progressbar]").GetAttribute("aria-valuenow"));
        var fill = cut.Find($".{Css.Classes.Progress.BufferFill}");
        Assert.Contains("width:0%", fill.GetAttribute("style"));
        Assert.DoesNotContain("NaN", cut.Markup);
    }

    [Fact]
    public void BufferWithoutAValueShowsTheIndeterminateStateItsAriaAlreadyReported()
    {
        var cut = Render<FlareProgressLinear>(p => p
            .Add(x => x.Variant, LinearProgressVariant.Buffer));

        var bar = cut.Find("[role=progressbar]");
        Assert.Null(bar.GetAttribute("aria-valuenow"));
        Assert.Contains(Css.Classes.Progress.Indeterminate, bar.ClassName);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.IndeterminateFirst}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Progress.BufferFill}"));
    }
}
