using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// Guards the two browser defaults that quietly break a field the moment it holds a number or an
/// adornment. Neither is reachable from a bUnit test - both are layout the browser performs - so a
/// text guard on the core CSS is the only automated defence there is.
///
/// <para><b>The spin button.</b> Chrome and Safari draw their own increment/decrement buttons inside
/// every <c>type=number</c> input and Firefox reserves room for them. They are unstyled, they cost
/// about 15px of the editable area, and they appear whether or not the component was asked for a
/// stepper - so a <c>FlareNumericField</c> with <c>ShowStepper</c> off still showed one, and with it
/// on showed two side by side. Three components render number inputs and only the colour picker had
/// remembered to suppress them.</para>
///
/// <para><b>The flex floor.</b> A form control's <c>min-width: auto</c> resolves to its intrinsic
/// width - roughly twenty characters - so as a flex item it refuses to shrink and pushes whatever
/// follows it out past the field's edge. That is what put the "kg" suffix outside the Weight field
/// while the shorter "%" happened to fit.</para>
/// </summary>
public sealed class NumberInputChromeTests
{
    /// <summary>
    /// Every class Flare puts on a <c>type=number</c> input, with the stylesheet that owns it.
    /// A new number input belongs here - and needs the suppression rule the test checks for.
    /// </summary>
    private static readonly (string Selector, string File)[] NumberInputSelectors =
    [
        // FlareNumericField, and any FlareField with Type="number" (the DataGrid's numeric filter).
        (".flare-input__control[type=number]", "input.css"),
        // FlareDateTimePicker's hour/minute boxes.
        (".flare-datetimepicker__time-input", "datetimepicker.css"),
        // FlareColorPicker's R/G/B and H/S/L boxes.
        (".flare-colorpicker__input[type=number]", "colorpicker.css"),
    ];

    [Theory]
    [MemberData(nameof(SelectorCases))]
    public void EveryNumberInput_SuppressesTheNativeSpinButton(string selector, string file)
    {
        var css = ReadCoreCss(file);

        // The pseudo-element must be neutralized, or the buttons are laid out regardless of the
        // `appearance` on the input itself - which is exactly how this shipped.
        Assert.True(
            Regex.IsMatch(css, Regex.Escape(selector) + @"::-webkit-inner-spin-button"),
            $"{file}: {selector} does not clear ::-webkit-inner-spin-button, so Chrome draws its own stepper inside the field.");
        Assert.True(
            Regex.IsMatch(css, Regex.Escape(selector) + @"::-webkit-outer-spin-button"),
            $"{file}: {selector} does not clear ::-webkit-outer-spin-button.");

        // Firefox needs the input itself switched to a plain text field.
        Assert.True(
            Regex.IsMatch(css, Regex.Escape(selector) + @"\s*\{[^}]*appearance:\s*textfield", RegexOptions.Singleline),
            $"{file}: {selector} is not set to appearance: textfield, so Firefox still reserves room for its spinner.");
    }

    public static IEnumerable<object[]> SelectorCases() =>
        NumberInputSelectors.Select(s => new object[] { s.Selector, s.File });

    [Fact]
    public void SharedFieldControl_CanShrinkBelowItsIntrinsicWidth()
    {
        var css = ReadCoreCss("input.css");
        var rule = Regex.Match(css, @"^\.flare-input__control\s*\{(.*?)\}", RegexOptions.Multiline | RegexOptions.Singleline);

        Assert.True(rule.Success, "input.css no longer declares a .flare-input__control rule.");
        Assert.True(
            Regex.IsMatch(rule.Groups[1].Value, @"min-width:\s*0"),
            ".flare-input__control must set min-width: 0. Without it the control keeps its intrinsic "
            + "width as a flex floor and pushes the prefix, suffix, clear button and stepper outside the field.");
    }

    private static string ReadCoreCss(string file)
    {
        var path = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css", file);
        Assert.True(File.Exists(path), $"Core stylesheet not found: {path}");
        return File.ReadAllText(path);
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Flare.slnx")))
            dir = dir.Parent;
        Assert.NotNull(dir);
        return dir!.FullName;
    }
}
