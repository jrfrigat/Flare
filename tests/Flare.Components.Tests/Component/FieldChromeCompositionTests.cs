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
        Assert.Single(cut.FindAll($".{Css.Classes.Input.Root}"));
        // The (non-floating) label is rendered by the frame, inside that single root.
        Assert.Single(cut.FindAll($".{Css.Classes.Input.Root} label.{Css.Classes.Input.Label}"));
        // The helper text is inside the frame's support row (also inside the root).
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Root} .{Css.Classes.Input.Support} .{Css.Classes.Input.Helper}"));
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
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Root} .{Css.Classes.Input.Support} .{Css.Classes.Input.HelperError}"));
        Assert.Single(cut.FindAll($".{Css.Classes.Input.Root}"));
    }

    // No helper, no error, no counter asked for => no support row at all. The frame decides the row
    // exists by testing CounterContent for null, and these two fields used to hand it a named fragment
    // whose BODY was conditional: non-null, renders nothing, still buys a row and the column gap above
    // it. That is the 4px by which a text field outgrew the select standing beside it.
    [Fact]
    public void BareField_RendersNoSupportRow() =>
        Assert.Empty(Render<FlareField<string>>(p => p.Add(x => x.Label, "L"))
            .FindAll($".{Css.Classes.Input.Support}"));

    [Fact]
    public void BareTextArea_RendersNoSupportRow() =>
        Assert.Empty(Render<FlareTextArea>(p => p.Add(x => x.Label, "L"))
            .FindAll($".{Css.Classes.Input.Support}"));

    // A select never had the problem, and is the control the two above are measured against: all three
    // must agree on when the row exists, or the family stops lining up.
    [Fact]
    public void BareSelect_RendersNoSupportRow() =>
        Assert.Empty(Render<FlareSelect<string>>(p => p.Add(x => x.Label, "L"))
            .FindAll($".{Css.Classes.Input.Support}"));

    // ...and the row comes back the moment there is something to put in it.
    [Fact]
    public void FieldWithACounter_RendersTheSupportRow()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Label, "L").Add(x => x.MaxLength, 10));
        Assert.Single(cut.FindAll($".{Css.Classes.Input.Support}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Support} .{Css.Classes.Input.Counter}"));
    }

    [Fact]
    public void TextAreaWithACounter_RendersTheSupportRow()
    {
        var cut = Render<FlareTextArea>(p => p.Add(x => x.Label, "L").Add(x => x.MaxLength, 10));
        Assert.Single(cut.FindAll($".{Css.Classes.Input.Support}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.Support} .{Css.Classes.Input.Counter}"));
    }
}
