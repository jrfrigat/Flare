using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareFormFieldSizeParityTests : FlareTestContext
{
    [Theory]
    [InlineData(FieldSize.Xs, Css.Classes.Input.SizeXs)]
    [InlineData(FieldSize.Lg, Css.Classes.Input.SizeLg)]
    public void Autocomplete_Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Autocomplete.Root}").ClassName);
    }

    [Theory]
    [InlineData(FieldSize.Xs, Css.Classes.Input.SizeXs)]
    [InlineData(FieldSize.Lg, Css.Classes.Input.SizeLg)]
    public void TagInput_Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareTagField<string>>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.TagInput.Root}").ClassName);
    }
}
