using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// Guards the field-family frame consolidation at render time: every converted field must compose
/// exactly ONE shared FlareFieldChrome frame (one <c>.flare-input</c> root), with the label and the
/// helper/error support row living INSIDE that single root. A field that double-wraps a second frame, or
/// re-emits its own label/helper beside the frame, breaks these assertions - which is exactly the
/// per-component chrome duplication this refactor removed. Complements the source-scan
/// <c>FieldChromeGuardTests</c> (which forbids the support-row markup outside the frame source file).
/// </summary>
public sealed class FieldChromeCompositionTests : FlareTestContext
{
    private static void AssertSingleFrameChrome(IRenderedComponent<IComponent> cut)
    {
        // Exactly one frame root: the field composes INTO the frame, it does not wrap a second one.
        Assert.Single(cut.FindAll(".flare-input"));
        // The (non-floating) label is rendered by the frame, inside that single root.
        Assert.Single(cut.FindAll(".flare-input label.flare-input__label"));
        // The helper text is inside the frame's support row (also inside the root).
        Assert.NotEmpty(cut.FindAll(".flare-input .flare-input__support .flare-input__helper"));
    }

    [Fact]
    public void Field_ComposesSingleFrameChrome() =>
        AssertSingleFrameChrome(Render<FlareField<string>>(p => p
            .Add(x => x.Label, "L").Add(x => x.HelperText, "H")));

    [Fact]
    public void TextArea_ComposesSingleFrameChrome() =>
        AssertSingleFrameChrome(Render<FlareTextArea>(p => p
            .Add(x => x.Label, "L").Add(x => x.HelperText, "H")));

    [Fact]
    public void MaskedField_ComposesSingleFrameChrome() =>
        AssertSingleFrameChrome(Render<FlareMaskedField>(p => p
            .Add(x => x.Label, "L").Add(x => x.HelperText, "H")));

    [Fact]
    public void NumericField_ComposesSingleFrameChrome() =>
        AssertSingleFrameChrome(Render<FlareNumericField<double>>(p => p
            .Add(x => x.Label, "L").Add(x => x.HelperText, "H")));

    [Fact]
    public void PasswordField_ComposesSingleFrameChrome() =>
        AssertSingleFrameChrome(Render<FlarePasswordField>(p => p
            .Add(x => x.Label, "L").Add(x => x.HelperText, "H")));

    [Fact]
    public void DatePicker_ComposesSingleFrameChrome() =>
        AssertSingleFrameChrome(Render<FlareDatePicker>(p => p
            .Add(x => x.Label, "L").Add(x => x.HelperText, "H")));

    [Fact]
    public void DateTimePicker_ComposesSingleFrameChrome() =>
        AssertSingleFrameChrome(Render<FlareDateTimePicker>(p => p
            .Add(x => x.Label, "L").Add(x => x.HelperText, "H")));

    [Fact]
    public void TimePicker_ComposesSingleFrameChrome() =>
        AssertSingleFrameChrome(Render<FlareTimePicker>(p => p
            .Add(x => x.Label, "L").Add(x => x.HelperText, "H")));

    [Fact]
    public void Field_ErrorReplacesHelperInsideFrame()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.Label, "L").Add(x => x.ErrorText, "boom"));

        // Error text renders as the support-row helper in its error form, inside the single frame root.
        Assert.NotEmpty(cut.FindAll(".flare-input .flare-input__support .flare-input__helper--error"));
        Assert.Single(cut.FindAll(".flare-input"));
    }
}
