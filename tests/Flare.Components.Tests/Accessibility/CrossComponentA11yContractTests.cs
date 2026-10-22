namespace Flare.Components.Tests.Accessibility;

/// <summary>
/// The rules every Flare component follows for assistive technology, stated once and checked on a
/// representative of each family: a control carries the name it was given, an icon-only control is
/// named, a field links its error to the input, an alert announces itself, and state attributes carry
/// text rather than HTML booleans. Component tests keep checking the same attributes in their own
/// scenarios; this class is the contract those scenarios rely on.
/// </summary>
public class CrossComponentA11yContractTests : FlareTestContext
{
    [Fact]
    public void AnExplicitAccessibleNameReachesTheButton()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.ChildContent, "Save")
            .Add(x => x.AriaLabel, "Save the document"));

        Assert.Equal("Save the document", cut.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void AnIconOnlyButtonIsNamed()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.Settings)
            .Add(x => x.AriaLabel, "Settings"));

        var button = cut.Find("button");
        Assert.Contains(Css.Classes.Button.IconOnly, button.ClassName);
        Assert.Equal("Settings", button.GetAttribute("aria-label"));
    }

    [Fact]
    public void AFieldWithAnErrorIsInvalidAndDescribesItsMessage()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.ErrorText, "This field is required"));

        var input = cut.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-invalid"));

        var describedBy = input.GetAttribute("aria-describedby");
        Assert.False(string.IsNullOrEmpty(describedBy));
        var message = cut.Find($"#{describedBy}");
        Assert.Contains("This field is required", message.TextContent);
    }

    [Fact]
    public void AFieldWithoutAnErrorDoesNotDescribeOne()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.HelperText, "Some hint"));

        Assert.Null(cut.Find("input").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void AnAlertAnnouncesItself()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.ChildContent, "Saved")
            .Add(x => x.Severity, AlertSeverity.Success));

        var alert = cut.Find($".{Css.Classes.Alert.Root}");
        Assert.Equal("alert", alert.GetAttribute("role"));
    }

    [Fact]
    public void AnAlertsCloseControlIsNamed()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.ChildContent, "Saved")
            .Add(x => x.Dismissible, true));

        var close = cut.Find($"button.{Css.Classes.Alert.Close}");
        Assert.False(string.IsNullOrWhiteSpace(close.GetAttribute("aria-label")));
    }

    [Fact]
    public void ADecorativeIconStaysOutOfTheAccessibilityTree()
    {
        var cut = Render<FlareAlert>(p => p.Add(x => x.ChildContent, "Saved"));

        var icon = cut.Find($".{Css.Classes.Alert.Icon}");
        Assert.Equal("true", icon.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void ACheckboxReportsItsStateAsText()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(x => x.Label, "Accept")
            .Add(x => x.Value, true));

        Assert.Equal("true", cut.Find("input[type='checkbox']").GetAttribute("aria-checked"));
    }

    [Fact]
    public void ABusyControlSaysItIsBusy()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.ChildContent, "Save")
            .Add(x => x.Loading, true));

        Assert.Equal("true", cut.Find("button").GetAttribute("aria-busy"));
    }

    [Fact]
    public void ADisabledControlIsActuallyDisabled()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.ChildContent, "Save")
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }
}
