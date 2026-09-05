using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareCardKeyboardActivationTests : FlareTestContext
{
    [Theory]
    [InlineData("Enter")]
    [InlineData(" ")]
    public void ClickableCard_ActivatesOnKey(string key)
    {
        var clicks = 0;
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => clicks++)
            .AddChildContent("Card"));
        cut.Find($".{Css.Classes.Card.Root}").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = key });
        Assert.Equal(1, clicks);
    }

    [Fact]
    public void NonInteractiveCard_HasNoButtonRole()
    {
        var cut = Render<FlareCard>(p => p.AddChildContent("Card"));
        Assert.Null(cut.Find($".{Css.Classes.Card.Root}").GetAttribute("role"));
    }
}
