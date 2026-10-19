using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class RadioGroupContextTests : FlareTestContext
{
    private static RenderFragment TwoRadios() => b =>
    {
        b.OpenComponent<FlareRadio<string>>(0);
        b.AddAttribute(1, nameof(FlareRadio<string>.Value), "a");
        b.AddAttribute(2, nameof(FlareRadio<string>.Label), "A");
        b.CloseComponent();
        b.OpenComponent<FlareRadio<string>>(3);
        b.AddAttribute(4, nameof(FlareRadio<string>.Value), "b");
        b.AddAttribute(5, nameof(FlareRadio<string>.Label), "B");
        b.CloseComponent();
    };

    [Fact]
    public void GroupDisabled_ReachesRenderedRadios()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p.Add(x => x.ChildContent, TwoRadios()));

        cut.Render(p => p.Add(x => x.Disabled, true));

        Assert.All(cut.FindAll("input"), input => Assert.True(input.HasAttribute("disabled")));
    }

    [Fact]
    public void GroupSize_ReachesRenderedRadios()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p.Add(x => x.ChildContent, TwoRadios()));

        cut.Render(p => p.Add(x => x.Size, FieldSize.Lg));

        Assert.All(cut.FindAll($"label.{Css.Classes.Radio.Root}"),
            label => Assert.Contains(Css.Classes.Radio.SizeLg, label.ClassName ?? string.Empty));
    }
}
