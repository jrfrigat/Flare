using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareMaskedFieldTests : FlareTestContext
{
    [Fact]
    public void RendersInput()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "000-000")
            .Add(x => x.Value, "123456"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Root}"));
        Assert.NotEmpty(cut.FindAll("input"));
    }

    [Fact]
    public void LeadingLiteralMask_FirstDigit_AppearsWithLiteral()
    {
        // Regression: a mask that starts with a literal ('+') dropped the first
        // typed digit, so nothing appeared. Typing "7" must render "+7 (".
        string? bound = null;
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "+# (###) ###-##-##")
            .Add(x => x.ValueChanged, (string? v) => bound = v));

        cut.Find("input").Input("7");

        Assert.Equal("+7 (", cut.Find("input").GetAttribute("value"));
        Assert.Equal("+7 (", bound);
    }

    [Fact]
    public void LeadingLiteralMask_TypedGroups_ExtractAndReapply()
    {
        // The masked string round-trips: re-feeding it keeps only the digits and
        // re-applies the literals in place.
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "+# (###) ###-##-##"));

        cut.Find("input").Input("+7 (912");

        Assert.Equal("+7 (912) ", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void ParenLeadingMask_FirstDigit_AppearsWithLiteral()
    {
        // The same class of bug affected any leading literal, e.g. the classic
        // "(###) ###-####" phone shape.
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "(###) ###-####"));

        cut.Find("input").Input("5");

        Assert.Equal("(5", cut.Find("input").GetAttribute("value"));
    }
}
