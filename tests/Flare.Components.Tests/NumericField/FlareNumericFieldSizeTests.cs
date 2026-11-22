using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareNumericFieldSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(FlareSize.Xs, Css.Classes.Input.SizeXs)]
    [InlineData(FlareSize.Sm, Css.Classes.Input.SizeSm)]
    [InlineData(FlareSize.Lg, Css.Classes.Input.SizeLg)]
    [InlineData(FlareSize.Xl, Css.Classes.Input.SizeXl)]
    public void Size_AppliesSharedInputSizeClass(FlareSize size, string expected)
    {
        var cut = Render<FlareNumericField<int>>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }
}
