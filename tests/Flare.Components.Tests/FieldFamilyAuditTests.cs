using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

// Field-family cross-framework audit follow-ups (items 1-9). Render/attribute + API-surface level;
// the JS-backed focus/select/blur paths are not invoked here (they only fire on user interaction).
public class FieldFamilyAuditTests : FlareTestContext
{
    // --- Item 1/2: intermediate editable base carries the imperative focus surface, select-family does not.
    [Fact]
    public void EditableFields_Derive_From_EditableBase_And_Expose_FocusAsync()
    {
        Assert.True(typeof(FlareEditableFieldBase).IsAssignableFrom(typeof(FlareField<string>)));
        Assert.True(typeof(FlareEditableFieldBase).IsAssignableFrom(typeof(FlareNumericField<int>)));
        Assert.True(typeof(FlareEditableFieldBase).IsAssignableFrom(typeof(FlareTextArea)));
        Assert.True(typeof(FlareEditableFieldBase).IsAssignableFrom(typeof(FlareMaskedField)));
        Assert.True(typeof(FlareEditableFieldBase).IsAssignableFrom(typeof(FlarePasswordField)));

        Assert.NotNull(typeof(FlareField<string>).GetMethod("FocusAsync"));
        Assert.NotNull(typeof(FlareNumericField<int>).GetMethod("FocusAsync"));
        Assert.NotNull(typeof(FlareTextArea).GetMethod("FocusAsync"));
        Assert.NotNull(typeof(FlareMaskedField).GetMethod("FocusAsync"));
        Assert.NotNull(typeof(FlarePasswordField).GetMethod("FocusAsync"));
        Assert.NotNull(typeof(FlareOtpField).GetMethod("FocusAsync"));
    }

    [Fact]
    public void SelectFamily_Stays_On_FieldBase_NotEditableBase()
    {
        // The div-trigger select family keeps its own focus model - it must NOT inherit the editable base.
        Assert.False(typeof(FlareEditableFieldBase).IsAssignableFrom(typeof(FlareSelect<string>)));
        Assert.True(typeof(FlareFieldBase).IsAssignableFrom(typeof(FlareSelect<string>)));
    }

    [Fact]
    public void EditableFields_Have_Autofocus_Parameter()
    {
        Assert.NotNull(typeof(FlareField<string>).GetProperty("Autofocus"));
        Assert.NotNull(typeof(FlareOtpField).GetProperty("Autofocus"));
    }

    // --- Item 3 + 8: Pattern / InputMode / Autocomplete / Spellcheck render as native attributes.
    [Fact]
    public void Field_Renders_Pattern_InputMode_Autocomplete_Spellcheck()
    {
        var cut = Render<FlareTextField>(p => p
            .Add(c => c.Pattern, "[0-9]+")
            .Add(c => c.InputMode, "numeric")
            .Add(c => c.Autocomplete, "email")
            .Add(c => c.Spellcheck, false));

        var input = cut.Find("input");
        Assert.Equal("[0-9]+", input.GetAttribute("pattern"));
        Assert.Equal("numeric", input.GetAttribute("inputmode"));
        Assert.Equal("email", input.GetAttribute("autocomplete"));
        Assert.Equal("false", input.GetAttribute("spellcheck"));
    }

    // --- Item 4 + 9: HelperTextOnFocus hides the helper until the input is focused.
    [Fact]
    public void Field_HelperTextOnFocus_HidesHelper_UntilFocus()
    {
        var cut = Render<FlareTextField>(p => p
            .Add(c => c.HelperText, "hint")
            .Add(c => c.HelperTextOnFocus, true));

        Assert.Empty(cut.FindAll(".flare-input__helper"));
        cut.Find("input").Focus();
        Assert.NotEmpty(cut.FindAll(".flare-input__helper"));
    }

    // --- Item 6: Clearable is now on every editable field.
    [Fact]
    public void Numeric_Clearable_ShowsClear_AndResetsValue_AndFiresCallback()
    {
        int? captured = 5;
        var cleared = false;
        var cut = Render<FlareNumericField<int?>>(p => p
            .Add(c => c.Value, 5)
            .Add(c => c.ValueChanged, v => captured = v)
            .Add(c => c.Clearable, true)
            .Add(c => c.OnClearButtonClick, () => cleared = true));

        cut.Find("button.flare-input__clear").Click();

        Assert.Null(captured);
        Assert.True(cleared);
    }

    [Fact]
    public void TextArea_Clearable_ShowsClearButton_WhenHasValue()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(c => c.Value, "text")
            .Add(c => c.Clearable, true));

        Assert.NotEmpty(cut.FindAll("button.flare-input__clear"));
    }

    [Fact]
    public void Masked_Clearable_ShowsClearButton_WhenHasValue()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(c => c.Mask, "###")
            .Add(c => c.Value, "12")
            .Add(c => c.Clearable, true));

        Assert.NotEmpty(cut.FindAll("button.flare-input__clear"));
    }

    // --- Item 7: public Increment()/Decrement().
    [Fact]
    public async Task Numeric_Increment_Decrement_ChangeValue_ViaStep()
    {
        int captured = 5;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(c => c.Value, 5)
            .Add(c => c.Step, 1.0)
            .Add(c => c.ValueChanged, v => captured = v));

        await cut.InvokeAsync(() => cut.Instance.Increment());
        Assert.Equal(6, captured);

        await cut.InvokeAsync(() => cut.Instance.Decrement());
        Assert.Equal(4, captured); // both computed from the bound Value (5)
    }

    // --- Item 9: TextArea Resize maps to the CSS resize property.
    [Fact]
    public void TextArea_Resize_None_SetsResizeStyle()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(c => c.Resize, TextAreaResize.None));

        var style = cut.Find("textarea").GetAttribute("style") ?? string.Empty;
        Assert.Contains("resize:none", style);
    }
}
