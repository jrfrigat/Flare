using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareRatingSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(FieldSize.Xs, Css.Classes.Rating.SizeXs)]
    [InlineData(FieldSize.Xl, Css.Classes.Rating.SizeXl)]
    public void Size_AppliesModifier(FieldSize size, string expected)
    {
        var cut = Render<FlareRating>(p => p.Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Rating.Root}").ClassName);
    }
}
