using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareToggleGroup cascade (Size / Color / Disabled)
// ------------------------------------------------------------------------------
public class FlareToggleGroupCascadeTests : FlareTestContext
{
    [Fact]
    public void GroupSize_CascadesToButtons()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Size, ButtonSize.Lg)
            .AddChildContent<FlareToggleButton>(b => b
                .Add(x => x.Value, (object?)"a")
                .AddChildContent("A")));

        Assert.Contains(Css.Classes.Button.Lg, cut.Find($"button.{Css.Classes.Button.Root}").ClassName);
    }

    [Fact]
    public void GroupColor_CascadesColorClassToButtons()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Color, FlareColor.Tertiary)
            .AddChildContent<FlareToggleButton>(b => b
                .Add(x => x.Value, (object?)"a")
                .AddChildContent("A")));

        Assert.Contains(Css.Classes.Color.Tertiary, cut.Find($"button.{Css.Classes.Button.Root}").ClassName);
    }

    [Fact]
    public void GroupDisabled_DisablesButtons()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Disabled, true)
            .AddChildContent<FlareToggleButton>(b => b
                .Add(x => x.Value, (object?)"a")
                .AddChildContent("A")));

        Assert.True(cut.Find($"button.{Css.Classes.Button.Root}").HasAttribute("disabled"));
    }
}
