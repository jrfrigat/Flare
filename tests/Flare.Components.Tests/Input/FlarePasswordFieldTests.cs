using Microsoft.AspNetCore.Components;
namespace Flare.Components.Tests;

public class FlarePasswordFieldTests : FlareTestContext
{
    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Label, "Password"));

        var label = cut.Find($"label.{Css.Classes.Input.Label}");
        Assert.Equal("Password", label.TextContent);
    }

    [Fact]
    public void RendersPasswordTypeInitially()
    {
        var cut = Render<FlarePasswordField>();

        Assert.Equal("password", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void RendersToggleButton()
    {
        var cut = Render<FlarePasswordField>();

        var toggleBtn = cut.Find($"button.{Css.Classes.Button.Root}");
        Assert.NotNull(toggleBtn);
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.HelperText, "At least 8 characters"));

        var helper = cut.Find($".{Css.Classes.Input.Helper}");
        Assert.Contains("At least 8 characters", helper.TextContent);
    }

    [Fact]
    public void RendersErrorState()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.ErrorText, "Password too short"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.HelperError}"));
    }

    [Fact]
    public void ValueChanged_FiresOnChange()
    {
        // Regression: the inner @bind-Value used to only assign the local Value field and never
        // invoked the component's own ValueChanged, so a consumer's @bind-Value stayed empty forever.
        string? captured = null;
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Value, "")
            .Add(x => x.ValueChanged, v => { captured = v; }));

        cut.Find("input").Change("s3cret");

        Assert.Equal("s3cret", captured);
    }

    [Fact]
    public void Immediate_CommitsValueOnKeystroke()
    {
        string? captured = null;
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.ValueChanged, v => { captured = v; }));

        cut.Find("input").Input("typing");

        Assert.Equal("typing", captured);
    }

    [Fact]
    public void NotImmediate_DoesNotCommitOnKeystroke()
    {
        string? captured = null;
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.ValueChanged, v => { captured = v; }));

        cut.Find("input").Input("typing"); // oninput is gated behind Immediate

        Assert.Null(captured);
    }

    [Fact]
    public void Required_EmitsRequiredAttribute()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Required, true));

        Assert.True(cut.Find("input").HasAttribute("required"));
    }

    [Fact]
    public void NotRequired_NoRequiredAttribute()
    {
        var cut = Render<FlarePasswordField>();

        Assert.False(cut.Find("input").HasAttribute("required"));
    }

    [Fact]
    public void Variant_Outlined_ForwardsToField()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Variant, InputVariant.Outlined));

        Assert.Contains(Css.Classes.Input.VariantOutlined, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void FullWidth_False_ForwardsAutoClass()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.FullWidth, false));

        Assert.Contains(Css.Classes.Input.Auto, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void Margin_Dense_ForwardsToField()
    {
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.Margin, FieldMargin.Dense));

        Assert.Contains(Css.Classes.Input.MarginDense, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }
}

// The reveal button is a control the caller reaches with the keyboard, so it has to exist for a screen
// reader too. FlareField wrapped its whole trailing slot in aria-hidden, which removed the button from
// the accessibility tree while leaving it in the tab order: focus landed on something that announced
// nothing, and its state could not be read at all.
public class FlarePasswordFieldAccessibilityTests : FlareTestContext
{
    // Anything that takes focus. A negative tabindex is deliberate removal, not a stop.
    private static bool IsFocusable(AngleSharp.Dom.IElement el)
    {
        var tabindex = el.GetAttribute("tabindex");
        if (tabindex is not null && tabindex.StartsWith('-')) return false;
        return el.TagName is "BUTTON" or "A" or "INPUT" or "SELECT" or "TEXTAREA" || tabindex is not null;
    }

    [Fact]
    public void NoFocusableElementSitsUnderAriaHidden()
    {
        var cut = Render<FlarePasswordField>(p => p.Add(x => x.Label, "Password"));

        var trapped = cut.FindAll("[aria-hidden=true]")
            .SelectMany(h => h.QuerySelectorAll("*").Where(IsFocusable))
            .Select(e => e.TagName + " " + (e.GetAttribute("aria-label") ?? e.GetAttribute("class")))
            .ToList();

        Assert.True(trapped.Count == 0,
            "These focusable elements are inside an aria-hidden subtree, so they take focus without "
            + "existing for a screen reader: " + string.Join(", ", trapped));
    }

    [Fact]
    public void RevealButton_CarriesALabelAndItsState()
    {
        var cut = Render<FlarePasswordField>(p => p.Add(x => x.Label, "Password"));

        var toggle = cut.FindAll("button").Single(b => b.GetAttribute("aria-pressed") is not null);
        Assert.False(string.IsNullOrWhiteSpace(toggle.GetAttribute("aria-label")));
        Assert.Equal("false", toggle.GetAttribute("aria-pressed"));

        toggle.Click();

        Assert.Equal("true", cut.FindAll("button")
            .Single(b => b.GetAttribute("aria-pressed") is not null)
            .GetAttribute("aria-pressed"));
    }

    // The slot still has to hide a decorative icon, which is what the wrapper's aria-hidden was doing by
    // accident: the icon hides itself when it carries no label, so the guarantee survives without it.
    [Fact]
    public void DecorativeTrailingIconStaysHidden()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.Label, "Search")
            .Add(x => x.TrailingIcon, (RenderFragment)(b =>
            {
                b.OpenComponent<FlareIconView>(0);
                b.AddAttribute(1, nameof(FlareIconView.Value), FlareIcons.Visibility);
                b.CloseComponent();
            })));

        var icon = cut.Find($".{Css.Classes.Input.IconTrailing}");
        Assert.Null(icon.GetAttribute("aria-hidden"));                    // the slot no longer hides
        Assert.NotEmpty(icon.QuerySelectorAll("[aria-hidden=true]"));     // the glyph inside still does
    }
}
