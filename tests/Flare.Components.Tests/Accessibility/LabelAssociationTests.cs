using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public sealed class LabelAssociationTests : FlareTestContext
{
    private static IReadOnlyDictionary<string, object> ConsumerId(string id) =>
        new Dictionary<string, object> { ["id"] = id };

    private static void AssertLabelReferences<TComponent>(
        IRenderedComponent<TComponent> cut, AngleSharp.Dom.IElement control) where TComponent : IComponent
    {
        var label = cut.Find("label");
        Assert.Equal(control.Id, label.GetAttribute("for"));
        var referenced = cut.Find("#" + control.Id);
        Assert.Equal(control.Id, referenced.Id);
        Assert.Equal(control.TagName, referenced.TagName);
    }

    [Fact]
    public void SelectOnlyUsesANonLabelCaptionForItsDivTrigger()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Label, "City"));
        var combo = cut.Find("[role=combobox]");
        var caption = cut.Find("#" + combo.GetAttribute("aria-labelledby"));
        Assert.Equal("SPAN", caption.TagName);
        Assert.Equal("City", caption.TextContent);
        Assert.Empty(cut.FindAll("label"));
    }

    [Fact]
    public void SearchableSelectHasANativeLabelAndKeepsIdentityAcrossRenders()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Searchable, true).Add(x => x.Label, "City"));
        var id = cut.Find("input").Id;
        Assert.False(string.IsNullOrEmpty(id));
        Assert.Equal(id, cut.Find("label").GetAttribute("for"));
        cut.Render(p => p.Add(x => x.Label, "Destination"));
        Assert.Equal(id, cut.Find("input").Id);
        cut.Render(p => p.Add(x => x.Searchable, false));
        Assert.Empty(cut.FindAll("label"));
        Assert.NotNull(cut.Find("#" + cut.Find("[role=combobox]").GetAttribute("aria-labelledby")));
    }

    [Fact]
    public void EditableComboboxAndTagFieldAssociateNativeInputs()
    {
        var combo = Render<FlareCombobox<string>>(p => p.Add(x => x.Label, "City"));
        Assert.Equal(combo.Find("input").Id, combo.Find("label").GetAttribute("for"));
        var tags = Render<FlareTagField<string>>(p => p.Add(x => x.Label, "Tags"));
        Assert.Equal(tags.Find("input").Id, tags.Find("label").GetAttribute("for"));
        Assert.NotEqual(combo.Find("input").Id, tags.Find("input").Id);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MultiSelectUsesTheCorrectCaptionForItsCurrentMode(bool searchable)
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Searchable, searchable).Add(x => x.Label, "Cities"));
        var target = cut.Find("[role=combobox]");
        var caption = cut.Find("#" + target.GetAttribute("aria-labelledby"));
        Assert.Equal(searchable ? "LABEL" : "SPAN", caption.TagName);
        if (searchable) Assert.Equal(target.Id, caption.GetAttribute("for"));
        else Assert.False(caption.HasAttribute("for"));
    }

    [Fact]
    public void RangeCaptionNamesTheGroupInsteadOfAnUnassociatedLabel()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.Label, "Booking"));
        var group = cut.Find("[role=group]");
        var caption = cut.Find("#" + group.GetAttribute("aria-labelledby"));
        Assert.Equal("Booking", caption.TextContent);
        Assert.Equal("SPAN", caption.TagName);
    }

    [Fact]
    public void SliderCaptionNamesBothRangeEndpointsWithoutAnUnassociatedLabel()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Label, "Budget").Add(x => x.Range, true));
        Assert.Empty(cut.FindAll("label"));
        foreach (var input in cut.FindAll("input[type=range]"))
            Assert.All(input.GetAttribute("aria-labelledby")!.Split(' '), id => Assert.NotNull(cut.Find("#" + id)));
    }

    [Fact]
    public void NativeCaptionFollowsConsumerInputId()
    {
        var attrs = new Dictionary<string, object> { ["id"] = "application-city" };
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Label, "City").Add(x => x.Searchable, true)
            .Add(x => x.InputAttributes, attrs));
        Assert.Equal("application-city", cut.Find("input").Id);
        Assert.Equal("application-city", cut.Find("label").GetAttribute("for"));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void TagFieldWithNoEditableInputDoesNotRenderANativeLabel(bool disabled, bool readOnly)
    {
        var cut = Render<FlareTagField<string>>(p => p.Add(x => x.Label, "Tags")
            .Add(x => x.Disabled, disabled).Add(x => x.ReadOnly, readOnly));
        Assert.Empty(cut.FindAll("input"));
        Assert.Empty(cut.FindAll("label"));
        Assert.Contains("Tags", cut.Markup);
    }

    [Fact]
    public void ColorCustomizerNamesItsGroupAndAssociatesTheNativePicker()
    {
        var cut = Render<FlareColorCustomizer>();
        var input = cut.Find("input[type=color]");
        Assert.Equal(input.Id, cut.Find("label").GetAttribute("for"));
        Assert.NotNull(cut.Find("#" + cut.Find("[role=group]").GetAttribute("aria-labelledby")));
    }

    [Fact]
    public async Task TextField_LabelFollowsConsumerIdAndFocusesTheControl()
    {
        var focused = false;
        var cut = Render<FlareTextField>(p => p
            .Add(x => x.Label, "Name")
            .Add(x => x.InputAttributes, ConsumerId("consumer-name"))
            .Add(x => x.OnFocus, EventCallback.Factory.Create<FocusEventArgs>(this, _ => focused = true)));

        var input = cut.Find("input");
        Assert.Equal("consumer-name", input.Id);
        AssertLabelReferences(cut, input);

        await input.TriggerEventAsync("onfocus", new FocusEventArgs());
        Assert.True(focused);
    }

    [Fact]
    public async Task TextArea_LabelFollowsConsumerIdAndFocusesTheControl()
    {
        var focused = false;
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Label, "Notes")
            .Add(x => x.InputAttributes, ConsumerId("consumer-notes"))
            .Add(x => x.OnFocus, EventCallback.Factory.Create<FocusEventArgs>(this, _ => focused = true)));

        var textarea = cut.Find("textarea");
        Assert.Equal("consumer-notes", textarea.Id);
        AssertLabelReferences(cut, textarea);

        await textarea.TriggerEventAsync("onfocus", new FocusEventArgs());
        Assert.True(focused);
    }

    [Fact]
    public async Task NumericField_LabelFollowsConsumerIdAndFocusesTheControl()
    {
        var focused = false;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Label, "Amount")
            .Add(x => x.InputAttributes, ConsumerId("consumer-amount"))
            .Add(x => x.OnFocus, EventCallback.Factory.Create<FocusEventArgs>(this, _ => focused = true)));

        var input = cut.Find("input");
        Assert.Equal("consumer-amount", input.Id);
        AssertLabelReferences(cut, input);

        await input.TriggerEventAsync("onfocus", new FocusEventArgs());
        Assert.True(focused);
    }

    [Fact]
    public void FieldFamily_WithoutConsumerIdKeepsTheGeneratedId()
    {
        var text = Render<FlareTextField>(p => p.Add(x => x.Label, "Name"));
        var textInput = text.Find("input");
        Assert.StartsWith("flare-field-", textInput.Id);
        AssertLabelReferences(text, textInput);

        var area = Render<FlareTextArea>(p => p.Add(x => x.Label, "Notes"));
        var textarea = area.Find("textarea");
        Assert.StartsWith("flare-field-", textarea.Id);
        AssertLabelReferences(area, textarea);

        var numeric = Render<FlareNumericField<int>>(p => p.Add(x => x.Label, "Amount"));
        var numberInput = numeric.Find("input");
        Assert.StartsWith("flare-field-", numberInput.Id);
        AssertLabelReferences(numeric, numberInput);
    }
}
