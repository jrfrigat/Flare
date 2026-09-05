namespace Flare.Components.Tests;

public class FlareCardTests : FlareTestContext
{
    [Fact]
    public void Renders_BaseCardCssClass()
    {
        var cut = Render<FlareCard>();

        Assert.Single(cut.FindAll($".{Css.Classes.Card.Root}"));
    }

    [Fact]
    public void DefaultVariant_IsElevated()
    {
        var cut = Render<FlareCard>();

        var div = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Contains(Css.Classes.Card.Elevated, div.ClassName);
    }

    [Fact]
    public void FilledVariant_AddsFilledCssClass()
    {
        var cut = Render<FlareCard>(p => p
            .Add(c => c.Variant, CardVariant.Filled));

        var div = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Contains(Css.Classes.Card.Filled, div.ClassName);
        Assert.DoesNotContain(Css.Classes.Card.Elevated, div.ClassName);
    }

    [Fact]
    public void OutlinedVariant_AddsOutlinedCssClass()
    {
        var cut = Render<FlareCard>(p => p
            .Add(c => c.Variant, CardVariant.Outlined));

        var div = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Contains(Css.Classes.Card.Outlined, div.ClassName);
    }

    [Fact]
    public void ChildContent_RenderedInsideCard()
    {
        var cut = Render<FlareCard>(p => p
            .AddChildContent("<p class=\"inner\">Hello Card</p>"));

        var inner = cut.Find(".inner");
        Assert.Equal("Hello Card", inner.TextContent);
    }

    [Fact]
    public void Class_Parameter_AppendedToCssClasses()
    {
        var cut = Render<FlareCard>(p => p
            .Add(c => c.Class, "custom-card"));

        var div = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Contains("custom-card", div.ClassName);
        Assert.Contains(Css.Classes.Card.Root, div.ClassName);
    }

    [Fact]
    public void Style_ForwardedToCardDiv()
    {
        var cut = Render<FlareCard>(p => p
            .Add(c => c.Style, "margin: 8px;"));

        var div = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Equal("margin: 8px;", div.GetAttribute("style"));
    }

    [Fact]
    public void MultipleClasses_AllPresentInOutput()
    {
        var cut = Render<FlareCard>(p => p
            .Add(c => c.Variant, CardVariant.Outlined)
            .Add(c => c.Class, "my-class"));

        var div = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Contains(Css.Classes.Card.Root, div.ClassName);
        Assert.Contains(Css.Classes.Card.Outlined, div.ClassName);
        Assert.Contains("my-class", div.ClassName);
    }

    [Fact]
    public void NoChildContent_CardStillRenders()
    {
        var cut = Render<FlareCard>();

        // Should render the div without throwing
        Assert.Single(cut.FindAll($".{Css.Classes.Card.Root}"));
    }

    [Fact]
    public void TonalVariant_AddsTonalCssClass()
    {
        var cut = Render<FlareCard>(p => p.Add(c => c.Variant, CardVariant.Tonal));

        Assert.Contains(Css.Classes.Card.Tonal, cut.Find($".{Css.Classes.Card.Root}").ClassName);
    }

    [Fact]
    public void TextVariant_AddsTextCssClass()
    {
        var cut = Render<FlareCard>(p => p.Add(c => c.Variant, CardVariant.Text));

        Assert.Contains(Css.Classes.Card.Text, cut.Find($".{Css.Classes.Card.Root}").ClassName);
    }

    [Fact]
    public void DefaultSize_IsMd()
    {
        var cut = Render<FlareCard>();

        Assert.Contains(Css.Classes.Card.SizeMd, cut.Find($".{Css.Classes.Card.Root}").ClassName);
    }

    [Theory]
    [InlineData(CardSize.Sm, Css.Classes.Card.SizeSm)]
    [InlineData(CardSize.Lg, Css.Classes.Card.SizeLg)]
    public void Size_AddsSizeCssClass(CardSize size, string expected)
    {
        var cut = Render<FlareCard>(p => p.Add(c => c.Size, size));

        Assert.Contains(expected, cut.Find($".{Css.Classes.Card.Root}").ClassName);
    }

    [Fact]
    public void Compact_AddsCompactCssClass()
    {
        var cut = Render<FlareCard>(p => p.Add(c => c.Compact, true));

        Assert.Contains(Css.Classes.Card.Compact, cut.Find($".{Css.Classes.Card.Root}").ClassName);
    }

    [Fact]
    public void Selectable_RendersCheckboxRoleAndAriaChecked()
    {
        var cut = Render<FlareCard>(p => p.Add(c => c.Selectable, true));

        var div = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Equal("checkbox", div.GetAttribute("role"));
        Assert.Equal("false", div.GetAttribute("aria-checked"));
        Assert.Contains(Css.Classes.Card.Selectable, div.ClassName);
    }

    [Fact]
    public void SelectableSelected_AddsSelectedClassAndAriaChecked()
    {
        var cut = Render<FlareCard>(p => p
            .Add(c => c.Selectable, true)
            .Add(c => c.Selected, true));

        var div = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Equal("true", div.GetAttribute("aria-checked"));
        Assert.Contains(Css.Classes.Card.Selected, div.ClassName);
    }

    [Fact]
    public void ClickingSelectableCard_TogglesSelectionAndFiresEvents()
    {
        bool? bound = null;
        bool? changed = null;
        var cut = Render<FlareCard>(p => p
            .Add(c => c.Selectable, true)
            .Add(c => c.SelectedChanged, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<bool>(this, v => bound = v))
            .Add(c => c.OnSelectionChange, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<bool>(this, v => changed = v)));

        cut.Find($".{Css.Classes.Card.Root}").Click();

        Assert.True(bound);
        Assert.True(changed);
    }

    [Fact]
    public void NonSelectableCard_HasNoCheckboxRole()
    {
        var cut = Render<FlareCard>();

        Assert.Null(cut.Find($".{Css.Classes.Card.Root}").GetAttribute("role"));
    }
}
