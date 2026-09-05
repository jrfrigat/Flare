using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FieldVariantReuseTests : FlareTestContext
{
    [Fact]
    public void TagInput_Outlined_ReusesInputVariantClass()
    {
        var cut = Render<FlareTagField<string>>(p => p.Add(x => x.Variant, InputVariant.Outlined));
        Assert.Contains(Css.Classes.Input.VariantOutlined, cut.Find($".{Css.Classes.TagInput.Root}").ClassName);
    }

    [Fact]
    public void DatePicker_Filled_ReusesInputVariantClass()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Variant, InputVariant.Filled));
        Assert.Contains(Css.Classes.Input.VariantFilled, cut.Find($".{Css.Classes.DatePicker.Root}").ClassName);
    }
}
