using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareChip Variant
// ------------------------------------------------------------------------------
public class FlareChipVariantTests : FlareTestContext
{
    [Fact]
    public void FilledVariant_AddsFilledClass()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Variant, ChipVariant.Filled));

        Assert.Contains(Css.Classes.Chip.Filled, cut.Find($".{Css.Classes.Chip.Root}").ClassName);
    }

    [Fact]
    public void ElevatedVariant_AddsElevatedClass()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Variant, ChipVariant.Elevated));

        Assert.Contains(Css.Classes.Chip.Elevated, cut.Find($".{Css.Classes.Chip.Root}").ClassName);
    }

    [Fact]
    public void OutlinedVariant_IsDefault_NoVariantModifier()
    {
        var cut = Render<FlareChip>(p => p.Add(x => x.Label, "Tag"));

        var cls = cut.Find($".{Css.Classes.Chip.Root}").ClassName;
        Assert.DoesNotContain(Css.Classes.Chip.Filled, cls);
        Assert.DoesNotContain(Css.Classes.Chip.Elevated, cls);
    }

    [Fact]
    public void ElevatedBool_StillMapsToElevated()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Elevated, true));

        Assert.Contains(Css.Classes.Chip.Elevated, cut.Find($".{Css.Classes.Chip.Root}").ClassName);
    }
}
