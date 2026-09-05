namespace Flare.Components.Tests.Component;

/// <summary>
/// Which on-screen keyboard a phone raises is decided by <c>inputmode</c>, not by the input's html
/// type: <c>type="tel"</c> alone gets the phone keypad in some browsers and a full QWERTY in others,
/// and a numeric field whose keyboard has no decimal separator cannot be filled in at all in a locale
/// that uses one. The type is the semantic; <c>inputmode</c> is the keyboard, and the field is the only
/// party that knows both.
///
/// These are the part of the mobile audit that can be asserted without a device: the attribute is
/// either on the element or it is not.
/// </summary>
public sealed class OnScreenKeyboardTests : FlareTestContext
{
    [Theory]
    [InlineData("tel", "tel")]
    [InlineData("email", "email")]
    [InlineData("url", "url")]
    [InlineData("search", "search")]
    [InlineData("number", "decimal")]
    [InlineData("text", null)]
    [InlineData("password", null)]
    public void AFieldAsksForTheKeyboardItsTypeImplies(string type, string? expected)
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Type, type));

        Assert.Equal(expected, cut.Find("input").GetAttribute("inputmode"));
    }

    // The page still gets the last word: a masked field derives its keyboard from the MASK, which knows
    // more than either the type or the component.
    [Fact]
    public void AnExplicitInputModeWins()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.Type, "text")
            .Add(x => x.InputMode, "numeric"));

        Assert.Equal("numeric", cut.Find("input").GetAttribute("inputmode"));
    }

    // Only search gets an action-key label. Whether Enter means "go", "next" or "done" is the form's
    // question, and a wrong label is worse than the plain one - so the rest are left alone.
    [Theory]
    [InlineData("search", "search")]
    [InlineData("text", null)]
    [InlineData("email", null)]
    public void OnlyASearchFieldLabelsTheActionKey(string type, string? expected)
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Type, type));

        Assert.Equal(expected, cut.Find("input").GetAttribute("enterkeyhint"));
    }

    [Fact]
    public void TheActionKeyLabelCanBeSet()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.Type, "text")
            .Add(x => x.EnterKeyHint, "done"));

        Assert.Equal("done", cut.Find("input").GetAttribute("enterkeyhint"));
    }

    // A numeric field reads the keyboard off the type it is BOUND to: an integral field must not offer a
    // separator it cannot accept, and a decimal one must not hide the separator it needs.
    [Fact]
    public void ADecimalFieldOffersTheSeparator()
    {
        var cut = Render<FlareNumericField<decimal>>(p => p.Add(x => x.Value, 1.5m));

        Assert.Equal("decimal", cut.Find("input").GetAttribute("inputmode"));
    }

    [Fact]
    public void AnIntegralFieldDoesNot()
    {
        var whole = Render<FlareNumericField<int>>(p => p.Add(x => x.Value, 1));
        Assert.Equal("numeric", whole.Find("input").GetAttribute("inputmode"));

        // Nullable is the same question with a wrapper around it.
        var maybe = Render<FlareNumericField<long?>>(p => p.Add(x => x.Value, 1L));
        Assert.Equal("numeric", maybe.Find("input").GetAttribute("inputmode"));
    }

    // The grid's own search boxes are the ones a phone user actually types into most, so they are
    // asserted through the grid rather than through the field in isolation.
    [Fact]
    public void TheGridsQuickFilterIsASearchBox()
    {
        var cut = Render<FlareDataGridQuickFilter<string>>();

        var input = cut.Find("input");
        Assert.Equal("search", input.GetAttribute("inputmode"));
        Assert.Equal("search", input.GetAttribute("enterkeyhint"));
    }
}
