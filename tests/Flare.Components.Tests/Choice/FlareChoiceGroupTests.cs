using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareChoiceGroupTests : FlareTestContext
{
    private static readonly RenderFragment TwoCards = b =>
    {
        b.OpenComponent<FlareChoiceCard<string>>(0);
        b.AddAttribute(1, "Value", "deny");
        b.AddAttribute(2, "Title", "Block the run");
        b.CloseComponent();

        b.OpenComponent<FlareChoiceCard<string>>(10);
        b.AddAttribute(11, "Value", "ask");
        b.AddAttribute(12, "Title", "Warn and ask");
        b.CloseComponent();
    };

    [Fact]
    public void RendersRadiogroupRoot()
    {
        var cut = Render<FlareChoiceGroup<string>>();

        var root = cut.Find($".{Css.Classes.ChoiceGroup.Root}");
        Assert.Equal("radiogroup", root.GetAttribute("role"));
    }

    [Fact]
    public void RendersLabelAndNamesTheGroupWithIt()
    {
        var cut = Render<FlareChoiceGroup<string>>(p => p
            .Add(x => x.Label, "Scope overlap policy"));

        var label = cut.Find($".{Css.Classes.ChoiceGroup.Label}");
        Assert.Contains("Scope overlap policy", label.TextContent);
        Assert.Equal(label.Id, cut.Find($".{Css.Classes.ChoiceGroup.Root}").GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void OmitsTheLabelledByWhenThereIsNoLabel()
    {
        var cut = Render<FlareChoiceGroup<string>>();

        Assert.Null(cut.Find($".{Css.Classes.ChoiceGroup.Root}").GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void RendersTheCardsInsideTheOptionsGrid()
    {
        var cut = Render<FlareChoiceGroup<string>>(p => p.Add(x => x.ChildContent, TwoCards));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.ChoiceGroup.Options}"));
        Assert.Equal(2, cut.FindAll($".{Css.Classes.ChoiceCard.Root}").Count);
    }

    [Fact]
    public void RendersTheColumnCountAsAPrivateVariable()
    {
        var cut = Render<FlareChoiceGroup<string>>(p => p.Add(x => x.Columns, 2));

        Assert.Contains("--_cc-columns:2", cut.Find($".{Css.Classes.ChoiceGroup.Root}").GetAttribute("style"));
    }

    [Fact]
    public void ColumnsDefaultsToThree()
    {
        var cut = Render<FlareChoiceGroup<string>>();

        Assert.Contains("--_cc-columns:3", cut.Find($".{Css.Classes.ChoiceGroup.Root}").GetAttribute("style"));
    }

    [Fact]
    public void RendersDisabledClass()
    {
        var cut = Render<FlareChoiceGroup<string>>(p => p.Add(x => x.Disabled, true));

        Assert.Contains(Css.Classes.ChoiceGroup.Disabled, cut.Find($".{Css.Classes.ChoiceGroup.Root}").ClassName);
    }
}
