using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareSelectVariantTests : FlareTestContext
{
    [Fact]
    public void OutlinedVariant_ReusesInputVariantClass()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Variant, InputVariant.Outlined));
        Assert.Contains(Css.Classes.Input.VariantOutlined, cut.Find($".{Css.Classes.Select.Root}").ClassName);
    }

    [Fact]
    public void MultiSelect_FilledVariant_ReusesInputVariantClass()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Variant, InputVariant.Filled));
        Assert.Contains(Css.Classes.Input.VariantFilled, cut.Find($".{Css.Classes.Multiselect.Root}").ClassName);
    }
}
