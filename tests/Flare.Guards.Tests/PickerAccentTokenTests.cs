using Flare.Css.Tokens;

namespace Flare.Guards.Tests;

/// <summary>
/// TASK-661: the selection surfaces of the time and colour pickers are a theme decision. The stylesheets
/// paint through their own tokens, and the roles the core used before remain the defaults.
/// </summary>
public class PickerAccentTokenTests
{
    private static readonly string TimePicker = File.ReadAllText(
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css", "timepicker.css"));

    private static readonly string ColorPicker = File.ReadAllText(
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css", "colorpicker.css"));

    [Fact]
    public void TimePicker_PaintsSelectionThroughItsTokens()
    {
        Assert.Contains($"var({TimePickerField.SelectedBg})", TimePicker);
        Assert.Contains($"var({TimePickerField.SelectedColor})", TimePicker);
        Assert.Contains($"var({TimePickerField.PeriodSelectedBg})", TimePicker);
        Assert.Contains($"var({TimePickerField.PeriodSelectedColor})", TimePicker);

        Assert.DoesNotContain($"var({Color.PrimaryContainer})", TimePicker);
        Assert.DoesNotContain($"var({Color.OnPrimaryContainer})", TimePicker);
        Assert.DoesNotContain($"var({Color.TertiaryContainer})", TimePicker);
        Assert.DoesNotContain($"var({Color.OnTertiaryContainer})", TimePicker);
    }

    [Fact]
    public void ColorPicker_PaintsItsAccentAndFocusRingThroughItsTokens()
    {
        Assert.Contains($"var({ColorPickerField.Accent})", ColorPicker);
        Assert.Contains($"var({ColorPickerField.FocusRing})", ColorPicker);

        Assert.DoesNotContain($"var({Color.Primary})", ColorPicker);
        Assert.DoesNotContain($"var({Color.PrimaryContainer})", ColorPicker);
    }

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;

        throw new DirectoryNotFoundException("Could not find the repository root from " + AppContext.BaseDirectory);
    }
}
