using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareBusy - the two timers are the whole component
// ------------------------------------------------------------------------------

/// <summary>
/// The delay and the minimum hold are the reason this is a component rather than a div with a spinner in
/// it, so they are what these test. Both are asserted through real waits: the timings ARE the behaviour,
/// and a test that mocked the clock would be testing a different component.
/// </summary>
public class C_FlareBusyTests : FlareTestContext
{
    [Fact]
    public void QuickWorkNeverFlashesASpinner()
    {
        var cut = Render<FlareBusy>(p => p.Add(x => x.Busy, true).Add(x => x.Delay, 5000));

        // Still inside the delay, so nothing has appeared yet.
        Assert.Empty(cut.FindAll(".flare-busy__veil"));
        Assert.Null(cut.Find(".flare-busy").GetAttribute("aria-busy"));
    }

    [Fact]
    public async Task TheOverlayAppearsOnceTheWorkOutlastsTheDelay()
    {
        var cut = Render<FlareBusy>(p => p.Add(x => x.Busy, true).Add(x => x.Delay, 20));

        await Task.Delay(200, Xunit.TestContext.Current.CancellationToken);
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll(".flare-busy__veil")));
        Assert.Equal("true", cut.Find(".flare-busy").GetAttribute("aria-busy"));
    }

    // Once the spinner is up it stays long enough to be read: without this, work that runs just past the
    // delay produces a flicker that reads as a glitch rather than as progress.
    [Fact]
    public async Task TheOverlayIsHeldForItsMinimumOnceShown()
    {
        var cut = Render<FlareBusy>(p => p
            .Add(x => x.Busy, true)
            .Add(x => x.Delay, 10)
            .Add(x => x.MinDuration, 3000));

        await Task.Delay(150, Xunit.TestContext.Current.CancellationToken);
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll(".flare-busy__veil")));

        cut.Render(p => p.Add(x => x.Busy, false));
        await Task.Delay(150, Xunit.TestContext.Current.CancellationToken);

        Assert.NotEmpty(cut.FindAll(".flare-busy__veil"));
    }

    // inert rather than a focus trap: the subtree keeps its layout and its scroll position, and the
    // browser takes it out of the tab order for us.
    [Fact]
    public async Task TheCoveredSubtreeIsTakenOutOfReach()
    {
        var cut = Render<FlareBusy>(p => p
            .Add(x => x.Busy, true)
            .Add(x => x.Delay, 10)
            .AddChildContent("<button>Save</button>"));

        await Task.Delay(150, Xunit.TestContext.Current.CancellationToken);
        cut.WaitForAssertion(() => Assert.NotNull(cut.Find(".flare-busy__content").GetAttribute("inert")));
    }

    [Fact]
    public void NotBusyRendersItsContentUncovered()
    {
        var cut = Render<FlareBusy>(p => p
            .Add(x => x.Busy, false)
            .AddChildContent("<button>Save</button>"));

        Assert.Empty(cut.FindAll(".flare-busy__veil"));
        Assert.Null(cut.Find(".flare-busy__content").GetAttribute("inert"));
        Assert.NotNull(cut.Find("button"));
    }
}

// ------------------------------------------------------------------------------
// Password strength - the rule, which is the part with a right answer
// ------------------------------------------------------------------------------

/// <summary>
/// The default rule scores on length first and variety second, which is the order that matters: a long
/// passphrase of one character class beats a short one of four, and a rule that says otherwise teaches
/// people to write <c>Passw0rd!</c>.
/// </summary>
public class C_PasswordStrengthTests
{
    private static int Score(string? password) => FlarePasswordRules.Evaluate(password).Score;

    [Fact]
    public void AnEmptyPasswordScoresNothingAndSaysWhy()
    {
        var result = FlarePasswordRules.Evaluate("");

        Assert.Equal(0, result.Score);
        Assert.NotEmpty(result.Suggestions);
    }

    [Fact]
    public void LengthCarriesMoreThanVariety()
    {
        // Four character classes in eight characters, against one class in twenty-four.
        var short4 = Score("Pa55w!rX");
        var longPhrase = Score("correcthorsebatterystaple");

        Assert.True(longPhrase > short4, $"passphrase {longPhrase} should beat {short4}");
    }

    [Fact]
    public void ASingleCharacterClassIsCappedHoweverLong()
    {
        Assert.True(Score("aaaaaaaaaaaaaaaaaaaaaaaa") <= 1);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("Password1")]
    [InlineData("qwerty")]
    public void AnObviousPasswordScoresZeroWhateverItsShape(string value)
    {
        var result = FlarePasswordRules.Evaluate(value);

        Assert.Equal(0, result.Score);
        Assert.Contains("Common", result.Suggestions);
    }

    [Fact]
    public void ScoreNeverLeavesItsScale()
    {
        foreach (var value in new[] { "", "a", "Aa1!", new string('x', 500), "Tr0ub4dor&3xtra-long-passphrase" })
        {
            var score = Score(value);
            Assert.InRange(score, 0, FlarePasswordStrength.MaxScore);
        }
    }

    // The rule returns keys, not sentences, so the default rule stays free of display text and the field
    // is what turns them into localized strings.
    [Fact]
    public void SuggestionsAreKeysUntilACallerLocalizesThem()
    {
        var raw = FlarePasswordRules.Evaluate("abc");
        var localized = FlarePasswordRules.Evaluate("abc", k => $"[{k}]");

        Assert.Contains("Longer", raw.Suggestions);
        Assert.Contains("[Longer]", localized.Suggestions);
    }

    [Fact]
    public void TheMeterIsOffByDefault()
    {
        var context = new FlareTestContext();
        var cut = context.Render<FlarePasswordField>();

        Assert.Empty(cut.FindAll(".flare-input__strength"));
    }
}

// ------------------------------------------------------------------------------
// FlareTimeSpanPicker - a duration, not a clock
// ------------------------------------------------------------------------------

public class C_TimeSpanPickerTests : FlareTestContext
{
    [Fact]
    public void ShowsOneSegmentPerRequestedUnit()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p.Add(x => x.Units, TimeSpanUnits.All));

        Assert.Equal(4, cut.FindAll(".flare-timespan__input").Count);
    }

    [Fact]
    public void HoursMinutesShowsTwo()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p.Add(x => x.Units, TimeSpanUnits.HoursMinutes));

        Assert.Equal(2, cut.FindAll(".flare-timespan__input").Count);
    }

    // The largest shown segment absorbs everything above it: a field showing only hours on a two-day
    // duration must read 48, not 0.
    [Fact]
    public void TheLargestSegmentCarriesTheOverflow()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromHours(50)));

        var hours = cut.FindAll(".flare-timespan__input")[0];
        Assert.Equal("50", hours.GetAttribute("value"));
    }

    [Fact]
    public void SegmentsBelowTheLargestAreBoundedToTheirPlace()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p.Add(x => x.Units, TimeSpanUnits.All));
        var inputs = cut.FindAll(".flare-timespan__input");

        Assert.Null(inputs[0].GetAttribute("max"));      // days: no ceiling
        Assert.Equal("23", inputs[1].GetAttribute("max"));
        Assert.Equal("59", inputs[2].GetAttribute("max"));
        Assert.Equal("59", inputs[3].GetAttribute("max"));
    }

    [Fact]
    public void EditingASegmentRecomposesTheWholeDuration()
    {
        TimeSpan? captured = null;
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromMinutes(90))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeSpan?>(this, v => captured = v)));

        cut.FindAll(".flare-timespan__input")[1].Change("45");

        Assert.Equal(TimeSpan.FromMinutes(105), captured);
    }

    [Fact]
    public void ValuesClampIntoTheRange()
    {
        TimeSpan? captured = null;
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Max, TimeSpan.FromHours(8))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeSpan?>(this, v => captured = v)));

        cut.FindAll(".flare-timespan__input")[0].Change("40");

        Assert.Equal(TimeSpan.FromHours(8), captured);
    }

    [Fact]
    public void TheSignToggleAppearsOnlyWhenNegativesAreAllowed()
    {
        var plain = Render<FlareTimeSpanPicker>();
        var signed = Render<FlareTimeSpanPicker>(p => p.Add(x => x.Negatable, true));

        Assert.Empty(plain.FindAll(".flare-timespan__sign"));
        Assert.Single(signed.FindAll(".flare-timespan__sign"));
    }

    [Fact]
    public void TogglingTheSignFlipsTheDuration()
    {
        TimeSpan? captured = null;
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Value, TimeSpan.FromHours(2))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeSpan?>(this, v => captured = v)));

        cut.Find(".flare-timespan__sign").Click();

        Assert.Equal(TimeSpan.FromHours(-2), captured);
    }
}

// ------------------------------------------------------------------------------
// FlarePullToRefresh - the gesture
// ------------------------------------------------------------------------------

public class C_PullToRefreshTests : FlareTestContext
{
    [Fact]
    public void RendersItsContentWithNoIndicatorAtRest()
    {
        var cut = Render<FlarePullToRefresh>(p => p.AddChildContent("<p>rows</p>"));

        Assert.NotNull(cut.Find("p"));
        Assert.Equal("true", cut.Find(".flare-ptr__indicator").GetAttribute("aria-hidden"));
    }

    // A pull short of the threshold is an ordinary scroll and must not fire the refresh - the failure
    // mode this gesture has everywhere it goes wrong.
    [Fact]
    public void APullShortOfTheThresholdDoesNotRefresh()
    {
        var fired = 0;
        var cut = Render<FlarePullToRefresh>(p => p
            .Add(x => x.Threshold, 64)
            .Add(x => x.OnRefresh, EventCallback.Factory.Create(this, () => fired++)));

        var root = cut.Find(".flare-ptr");
        root.PointerDown(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 0 });
        root.PointerMove(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 40 });
        root.PointerUp(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 40 });

        Assert.Equal(0, fired);
    }

    [Fact]
    public void DisabledIgnoresTheGestureWithoutUnwrappingTheContent()
    {
        var fired = 0;
        var cut = Render<FlarePullToRefresh>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnRefresh, EventCallback.Factory.Create(this, () => fired++))
            .AddChildContent("<p>rows</p>"));

        var root = cut.Find(".flare-ptr");
        root.PointerDown(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 0 });
        root.PointerMove(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 400 });
        root.PointerUp(new Microsoft.AspNetCore.Components.Web.PointerEventArgs { ClientY = 400 });

        Assert.Equal(0, fired);
        Assert.NotNull(cut.Find("p"));
    }
}

// The sign is a property OF the value, not state beside it: a field handed a negative duration rendered
// a "+" until this was fixed, because only the commit path ever wrote the flag.
public class C_TimeSpanSignTests : FlareTestContext
{
    [Fact]
    public void ANegativeValueRendersANegativeSign()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromMinutes(-75)));

        Assert.Equal("-", cut.Find(".flare-timespan__sign").TextContent.Trim());
        Assert.Equal("true", cut.Find(".flare-timespan__sign").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void SegmentsShowTheMagnitudeOfANegativeDuration()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromMinutes(-75)));

        var inputs = cut.FindAll(".flare-timespan__input");
        Assert.Equal("1", inputs[0].GetAttribute("value"));
        Assert.Equal("15", inputs[1].GetAttribute("value"));
    }

    [Fact]
    public void APositiveValueRendersAPositiveSign()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Value, TimeSpan.FromHours(3)));

        Assert.Equal("+", cut.Find(".flare-timespan__sign").TextContent.Trim());
    }

    // Editing a segment of a negative duration must keep it negative - the sign is not something the
    // user has to re-apply after every keystroke.
    [Fact]
    public void EditingKeepsTheSign()
    {
        TimeSpan? captured = null;
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromMinutes(-75))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeSpan?>(this, v => captured = v)));

        cut.FindAll(".flare-timespan__input")[1].Change("30");

        Assert.Equal(TimeSpan.FromMinutes(-90), captured);
    }
}

// The mobile keyboard. Every mask preset is digits, and a text keyboard for a credit-card number is a
// phone-sized mistake that never shows up on a desktop.
public class C_MaskedFieldKeyboardTests : FlareTestContext
{
    private static string? Mode(IRenderedComponent<FlareMaskedField> cut) =>
        cut.Find("input").GetAttribute("inputmode");

    [Theory]
    [InlineData(MaskPreset.CreditCard, "numeric")]
    [InlineData(MaskPreset.Ssn, "numeric")]
    [InlineData(MaskPreset.Date, "numeric")]
    [InlineData(MaskPreset.Time, "numeric")]
    [InlineData(MaskPreset.IpAddress, "numeric")]
    public void ADigitOnlyPresetAsksForTheNumericKeyboard(MaskPreset preset, string expected)
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Preset, preset));

        Assert.Equal(expected, Mode(cut));
    }

    // A phone number is tel, not numeric: the tel keypad carries +, * and # as well as the digits.
    [Fact]
    public void APhoneAsksForTheTelKeypad()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Preset, MaskPreset.Phone));

        Assert.Equal("tel", Mode(cut));
    }

    [Fact]
    public void ACustomDigitMaskAlsoGetsTheNumericKeyboard()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Mask, "###-###"));

        Assert.Equal("numeric", Mode(cut));
    }

    // A mask that accepts letters needs the full keyboard, so it must NOT be narrowed.
    [Fact]
    public void AMaskWithLettersKeepsTheFullKeyboard()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Mask, "AA-####"));

        Assert.Null(Mode(cut));
    }

    [Fact]
    public void AnExplicitInputModeWins()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Preset, MaskPreset.CreditCard)
            .Add(x => x.InputMode, "decimal"));

        Assert.Equal("decimal", Mode(cut));
    }
}
