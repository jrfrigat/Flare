using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareButtonLoadingTests : FlareTestContext
{
    [Fact]
    public void Loading_False_RendersChildContent()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, false)
            .AddChildContent("Click Me"));

        Assert.Contains("Click Me", cut.Find($".{Css.Classes.Button.Label}").TextContent);
    }

    [Fact]
    public void Loading_True_RendersSpinner()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true));

        Assert.NotEmpty(cut.FindAll($"span.{Css.Classes.Button.Spinner}"));
    }

    [Fact]
    public void Loading_True_AddsLoadingClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true));

        Assert.Contains(Css.Classes.Button.Loading, cut.Find("button").ClassName ?? "");
    }

    [Fact]
    public void Loading_True_DisablesButton()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Loading_True_SetsAriaBusy()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true));

        Assert.Equal("true", cut.Find("button").GetAttribute("aria-busy"));
    }

    [Fact]
    public void Loading_True_WithLoadingText_ShowsLoadingTextInLabel()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.LoadingText, "Saving..."));

        Assert.Contains("Saving...", cut.Find($".{Css.Classes.Button.Label}").TextContent);
    }

    [Fact]
    public void Loading_False_DoesNotShowSpinner()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, false));

        Assert.Empty(cut.FindAll($"span.{Css.Classes.Button.Spinner}"));
    }

    [Fact]
    public void DefaultState_RendersNormally()
    {
        var cut = Render<FlareButton>(p => p
            .AddChildContent("Submit"));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Button.Root}"));
        Assert.False(cut.Find("button").HasAttribute("disabled"));
    }
}

// ------------------------------------------------------------------------------
// FlareSplitButton  (4 tests)
// ------------------------------------------------------------------------------
