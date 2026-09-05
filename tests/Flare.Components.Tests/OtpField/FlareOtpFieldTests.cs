using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareOtpFieldTests : FlareTestContext
{
    [Fact]
    public void RendersOneInputPerDigit()
    {
        var cut = Render<FlareOtpField>(p => p.Add(x => x.Length, 5));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Otp.Root}"));
        Assert.Equal(5, cut.FindAll("input").Count);
    }
}
