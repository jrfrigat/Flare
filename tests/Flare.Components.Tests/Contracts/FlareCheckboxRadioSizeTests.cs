using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareCheckboxRadioSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(FieldSize.Xs, Css.Classes.Checkbox.SizeXs)]
    [InlineData(FieldSize.Xl, Css.Classes.Checkbox.SizeXl)]
    public void Checkbox_Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareCheckbox>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Checkbox.Root}").ClassName);
    }

    [Fact]
    public void RadioGroup_Size_CascadesToRadios()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(g => g.Size, FieldSize.Lg)
            .AddChildContent<FlareRadio<string>>(r => r.Add(x => x.Value, "a")));
        Assert.Contains(Css.Classes.Radio.SizeLg, cut.Find($".{Css.Classes.Radio.Root}").ClassName);
    }
}
