using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// The height half of the field-family contract, at render time. Every control in the family draws its
/// own well differently - a native input, a combobox trigger, a chip well, a picker trigger - and the one
/// thing that makes them line up is that all of them carry the shared <c>.flare-input__field</c>, whose
/// height comes from the family ramp. A control that draws a well without that class opts out of the ramp
/// silently: it renders, it looks close enough at the default size, and it stands two to ten pixels off
/// the field beside it. That is how the trigger, the tag well and the pickers each drifted before.
/// The token side of the same contract is <c>FieldHeightRampTests</c> in the core suite.
/// </summary>
public sealed class FieldGeometryContractTests : FlareTestContext
{
    // Exactly one well per field: the height is a property of the well, so a field that renders two wells
    // (a nested frame, a second box around the control) has two heights and only one of them is the
    // family's.
    private static void AssertOneSharedWell(IRenderedComponent<IComponent> cut, bool grows)
    {
        var wells = cut.FindAll(".flare-input__field");
        Assert.Single(wells);

        var isGrow = wells[0].ClassList.Contains("flare-input__field--grow")
            || wells[0].ClassList.Contains("flare-tag-input__field");
        Assert.True(
            isGrow == grows,
            grows
                ? "This well's height is its content (rows of text, rows of chips), so it has to take the " +
                  "family step as a floor - it needs the grow modifier, or it is pinned to one line."
                : "This well is single-line, so its height is exactly the family step. Marking it as a " +
                  "grow well hands the height back to whatever sits inside it, which is the defect the " +
                  "ramp removed.");
    }

    [Fact]
    public void Field_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareField<string>>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void PasswordField_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlarePasswordField>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void MaskedField_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareMaskedField>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void NumericField_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareNumericField<double>>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void Select_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareSelect<string>>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void MultiSelect_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareMultiSelect<string>>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void Combobox_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareCombobox<string>>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void DatePicker_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareDatePicker>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void TimePicker_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareTimePicker>(p => p.Add(x => x.Label, "L")), grows: false);

    [Fact]
    public void DateTimePicker_CarriesTheSharedWell() =>
        AssertOneSharedWell(Render<FlareDateTimePicker>(p => p.Add(x => x.Label, "L")), grows: false);

    // The two wells whose height is legitimately their content.
    [Fact]
    public void TextArea_CarriesTheSharedWellAndGrows() =>
        AssertOneSharedWell(Render<FlareTextArea>(p => p.Add(x => x.Label, "L")), grows: true);

    [Fact]
    public void TagField_CarriesTheSharedWellAndGrows() =>
        AssertOneSharedWell(Render<FlareTagField<string>>(p => p.Add(x => x.Label, "L")), grows: true);

    // The trailing chrome is what used to set the height, so it is worth asserting it is still chrome
    // INSIDE the well rather than a sibling of it: a chevron outside the well is not centred by it and
    // not covered by its height either.
    [Fact]
    public void SelectArrow_SitsInsideTheWell()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Label, "L"));
        Assert.NotEmpty(cut.FindAll(".flare-input__field .flare-input__arrow"));
    }
}
