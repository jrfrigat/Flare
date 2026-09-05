using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareField / FlareTextField Error + FullWidth + Margin
// ------------------------------------------------------------------------------
public class FlareFieldErrorLayoutTests : FlareTestContext
{
    [Fact]
    public void Error_AddsErrorState_WithoutMessage()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.Error, true));

        Assert.Contains(Css.Classes.Input.Error, cut.Find($".{Css.Classes.Input.Root}").ClassName);
        // No error message row is forced when there is no ErrorText.
        Assert.Empty(cut.FindAll($".{Css.Classes.Input.HelperError}"));
        Assert.Equal("true", cut.Find($"input.{Css.Classes.Input.Control}").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void Invalid_Alias_AddsErrorState()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.Invalid, true));

        Assert.Contains(Css.Classes.Input.Error, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void FullWidthFalse_AddsAutoClass()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.FullWidth, false));

        Assert.Contains(Css.Classes.Input.Auto, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void FullWidthTrue_IsDefault_NoAutoClass()
    {
        var cut = Render<FlareTextField>();

        Assert.DoesNotContain(Css.Classes.Input.Auto, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void MarginDense_AddsMarginClass()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.Margin, FieldMargin.Dense));

        Assert.Contains(Css.Classes.Input.MarginDense, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }
}
