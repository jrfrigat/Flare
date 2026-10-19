using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class RadioAttributesTests : FlareTestContext
{
    [Fact]
    public void ClassStyleAndAdditionalAttributes_LandOnTheLabel()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.Class, "custom")
            .Add(x => x.Style, "color:red")
            .AddUnmatched("data-testid", "radio-a"));

        var label = cut.Find("label");
        Assert.Contains("custom", label.ClassName ?? string.Empty);
        Assert.Contains("color:red", label.GetAttribute("style"));
        Assert.Equal("radio-a", label.GetAttribute("data-testid"));
    }

    [Fact]
    public void InputAttributes_NameTheNativeRadioAndSetItsValue()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.InputAttributes, new Dictionary<string, object>
            {
                ["aria-label"] = "Option A",
                ["data-testid"] = "native-a",
            }));

        var input = cut.Find("input");
        Assert.Equal("Option A", input.GetAttribute("aria-label"));
        Assert.Equal("native-a", input.GetAttribute("data-testid"));
        Assert.Equal("a", input.GetAttribute("value"));
    }

    [Fact]
    public void GroupNameAndValue_StayOwnedByTheComponent()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .AddChildContent<FlareRadio<string>>(radio => radio
                .Add(x => x.Value, "a")
                .Add(x => x.Label, "A")
                .Add(x => x.InputAttributes, new Dictionary<string, object>
                {
                    ["name"] = "hijack",
                    ["value"] = "x",
                    ["type"] = "text",
                })));

        var input = cut.Find("input");
        Assert.StartsWith("flare-radio-", input.GetAttribute("name"));
        Assert.Equal("a", input.GetAttribute("value"));
        Assert.Equal("radio", input.GetAttribute("type"));
    }

    [Fact]
    public void IconOnlyRadio_CanBeNamedThroughInputAttributes()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.InputAttributes, new Dictionary<string, object> { ["aria-label"] = "Star" }));

        Assert.Equal("Star", cut.Find("input").GetAttribute("aria-label"));
    }
}
