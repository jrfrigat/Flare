using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareButtonTests : FlareTestContext
{
    [Fact]
    public void Renders_WithDefaultVariant_FilledCssClass()
    {
        var cut = Render<FlareButton>(p => p
            .AddChildContent("Click me"));

        var btn = cut.Find("button");
        Assert.Contains("flare-btn--filled", btn.ClassName);
    }

    [Fact]
    public void Renders_OutlinedVariant_OutlinedCssClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Variant, ButtonVariant.Outlined)
            .AddChildContent("Click me"));

        var btn = cut.Find("button");
        Assert.Contains("flare-btn--outlined", btn.ClassName);
        Assert.DoesNotContain("flare-btn--filled", btn.ClassName);
    }

    [Fact]
    public void Renders_TextVariant_TextCssClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Variant, ButtonVariant.Text)
            .AddChildContent("Click me"));

        var btn = cut.Find("button");
        Assert.Contains("flare-btn--text", btn.ClassName);
    }

    [Fact]
    public void Renders_ElevatedVariant_ElevatedCssClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Variant, ButtonVariant.Elevated)
            .AddChildContent("Elevated"));

        Assert.Contains("flare-btn--elevated", cut.Find("button").ClassName);
    }

    [Fact]
    public void Renders_TonalVariant_TonalCssClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Variant, ButtonVariant.Tonal)
            .AddChildContent("Tonal"));

        Assert.Contains("flare-btn--tonal", cut.Find("button").ClassName);
    }

    [Fact]
    public void Disabled_SetsDisabledAttribute()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Disabled, true)
            .AddChildContent("Disabled"));

        var btn = cut.Find("button");
        Assert.True(btn.HasAttribute("disabled"));
    }

    [Fact]
    public void NotDisabled_DoesNotSetDisabledAttribute()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Disabled, false)
            .AddChildContent("Enabled"));

        var btn = cut.Find("button");
        Assert.False(btn.HasAttribute("disabled"));
    }

    [Fact]
    public void OnClick_FiresWhenClicked()
    {
        var clicked = false;
        var cut = Render<FlareButton>(p => p
            .Add(c => c.OnClick, (MouseEventArgs _) => { clicked = true; })
            .AddChildContent("Click me"));

        cut.Find("button").Click();

        Assert.True(clicked);
    }

    [Fact]
    public void LeadingIcon_RendersLeadingIconSpan()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.LeadingIcon, b => b.AddMarkupContent(0, "<span>icon</span>"))
            .AddChildContent("With Icon"));

        var span = cut.Find(".flare-btn__icon--leading");
        Assert.NotNull(span);
        Assert.Contains("icon", span.InnerHtml);
    }

    [Fact]
    public void NoLeadingIcon_DoesNotRenderLeadingIconSpan()
    {
        var cut = Render<FlareButton>(p => p
            .AddChildContent("No Icon"));

        Assert.Empty(cut.FindAll(".flare-btn__icon--leading"));
    }

    [Fact]
    public void TrailingIcon_RendersTrailingIconSpan()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.TrailingIcon, b => b.AddMarkupContent(0, "<span>trailing</span>"))
            .AddChildContent("With Trailing"));

        var span = cut.Find(".flare-btn__icon--trailing");
        Assert.NotNull(span);
    }

    [Fact]
    public void SmallSize_AddsSizeCssClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Size, ButtonSize.Sm)
            .AddChildContent("Small"));

        Assert.Contains("flare-btn--sm", cut.Find("button").ClassName);
    }

    [Fact]
    public void LargeSize_AddsSizeCssClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Size, ButtonSize.Lg)
            .AddChildContent("Large"));

        Assert.Contains("flare-btn--lg", cut.Find("button").ClassName);
    }

    [Fact]
    public void ChildContent_RendersInsideLabelSpan()
    {
        var cut = Render<FlareButton>(p => p
            .AddChildContent("My Label"));

        var label = cut.Find(".flare-btn__label");
        Assert.Contains("My Label", label.TextContent);
    }

    [Fact]
    public void Type_ForwardedToButtonElement()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Type, ButtonType.Submit)
            .AddChildContent("Submit"));

        Assert.Equal("submit", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void Class_Parameter_AppendedToCssClasses()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Class, "my-custom-class")
            .AddChildContent("Custom"));

        Assert.Contains("my-custom-class", cut.Find("button").ClassName);
        Assert.Contains("flare-btn", cut.Find("button").ClassName);
    }

    [Fact]
    public void Typo_AppliesTypeScaleClassToLabel()
    {
        var cut = Render<FlareButton>(p => p
            .Add(c => c.Typo, TypographyScale.LabelSmall)
            .AddChildContent("Small"));

        var label = cut.Find(".flare-btn__label");
        Assert.Contains("flare-text--label-small", label.ClassName);
    }

    [Fact]
    public void Typo_NotSet_LabelHasNoTypeScaleClass()
    {
        var cut = Render<FlareButton>(p => p
            .AddChildContent("Default"));

        var label = cut.Find(".flare-btn__label");
        Assert.DoesNotContain("flare-text--", label.ClassName);
    }
}
