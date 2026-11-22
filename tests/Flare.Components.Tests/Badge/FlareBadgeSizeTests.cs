using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareBadgeSizeTests : FlareTestContext
{
    [Theory]
    [InlineData(FlareSize.Xs, Css.Classes.Badge.SizeXs)]
    [InlineData(FlareSize.Xl, Css.Classes.Badge.SizeXl)]
    public void Size_AppliesModifier(FlareSize size, string expected)
    {
        var cut = Render<FlareBadge>(p => p.Add(x => x.Count, 3).Add(x => x.Size, size));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Badge.Root}").ClassName);
    }
}
