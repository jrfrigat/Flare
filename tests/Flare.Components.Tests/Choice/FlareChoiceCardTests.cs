using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

public class FlareChoiceCardTests : FlareTestContext
{
    /// <summary>A group with two cards, so a card renders with the context it never renders without.</summary>
    private sealed class ChoiceHost : ComponentBase
    {
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public bool GroupDisabled { get; set; }
        [Parameter] public bool CardDisabled { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            b.OpenComponent<FlareChoiceGroup<string>>(0);
            b.AddAttribute(1, "Value", (object?)Value);
            b.AddAttribute(2, "ValueChanged", ValueChanged);
            b.AddAttribute(3, "Disabled", GroupDisabled);
            b.AddAttribute(4, "ChildContent", (RenderFragment)(c =>
            {
                c.OpenComponent<FlareChoiceCard<string>>(0);
                c.AddAttribute(1, "Value", "deny");
                c.AddAttribute(2, "Title", "Block the run");
                c.AddAttribute(3, "Hint", "The overlapping card waits");
                c.CloseComponent();

                c.OpenComponent<FlareChoiceCard<string>>(10);
                c.AddAttribute(11, "Value", "ask");
                c.AddAttribute(12, "Title", "Warn and ask");
                c.AddAttribute(13, "Disabled", CardDisabled);
                c.CloseComponent();
            }));
            b.CloseComponent();
        }
    }

    [Fact]
    public void RendersTitleAndHint()
    {
        var cut = Render<ChoiceHost>();

        Assert.Contains("Block the run", cut.FindAll($".{Css.Classes.ChoiceCard.Title}")[0].TextContent);
        Assert.Contains("The overlapping card waits", cut.Find($".{Css.Classes.ChoiceCard.Hint}").TextContent);
    }

    [Fact]
    public void RendersNativeRadiosSharingTheGroupName()
    {
        var cut = Render<ChoiceHost>();

        var inputs = cut.FindAll("input[type=radio]");
        Assert.Equal(2, inputs.Count);

        var name = inputs[0].GetAttribute("name");
        Assert.False(string.IsNullOrEmpty(name));
        Assert.All(inputs, i => Assert.Equal(name, i.GetAttribute("name")));
    }

    [Fact]
    public void MarksTheCardWhoseValueMatchesTheGroupSelected()
    {
        var cut = Render<ChoiceHost>(p => p.Add(x => x.Value, "ask"));

        var inputs = cut.FindAll("input[type=radio]");
        Assert.False(inputs[0].HasAttribute("checked"));
        Assert.True(inputs[1].HasAttribute("checked"));

        var cards = cut.FindAll($".{Css.Classes.ChoiceCard.Root}");
        Assert.DoesNotContain(Css.Classes.ChoiceCard.Selected, cards[0].ClassName);
        Assert.Contains(Css.Classes.ChoiceCard.Selected, cards[1].ClassName);
    }

    [Fact]
    public void RendersNoCheckedInputWhenNothingIsSelected()
    {
        var cut = Render<ChoiceHost>();

        Assert.All(cut.FindAll("input[type=radio]"), i => Assert.False(i.HasAttribute("checked")));
    }

    [Fact]
    public void GroupDisabledDisablesEveryCard()
    {
        var cut = Render<ChoiceHost>(p => p.Add(x => x.GroupDisabled, true));

        Assert.All(cut.FindAll("input[type=radio]"), i => Assert.True(i.HasAttribute("disabled")));
        Assert.All(cut.FindAll($".{Css.Classes.ChoiceCard.Root}"),
            c => Assert.Contains(Css.Classes.ChoiceCard.Disabled, c.ClassName));
    }

    [Fact]
    public void CardDisabledDisablesOnlyThatCard()
    {
        var cut = Render<ChoiceHost>(p => p.Add(x => x.CardDisabled, true));

        var inputs = cut.FindAll("input[type=radio]");
        Assert.False(inputs[0].HasAttribute("disabled"));
        Assert.True(inputs[1].HasAttribute("disabled"));
    }

    [Fact]
    public void ChoosingACardRaisesTheGroupsValueChanged()
    {
        var raised = new List<string?>();
        var cut = Render<ChoiceHost>(p => p
            .Add(x => x.Value, "deny")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => raised.Add(v))));

        cut.FindAll("input[type=radio]")[1].Change();

        Assert.Equal(new string?[] { "ask" }, raised);
    }
}
