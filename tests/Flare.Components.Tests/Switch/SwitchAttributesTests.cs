namespace Flare.Components.Tests;

public class SwitchAttributesTests : FlareTestContext
{
    [Fact]
    public void InputAttributes_ReachTheNativeSwitch()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "Wifi")
            .Add(x => x.InputAttributes, new Dictionary<string, object>
            {
                ["name"] = "wifi",
                ["required"] = true,
            }));
        var input = cut.Find("input");
        Assert.Equal("wifi", input.GetAttribute("name"));
        Assert.True(input.HasAttribute("required"));
    }

    [Fact]
    public void OwnedAttributes_AreNotOverriddenByInputAttributes()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Value, true)
            .Add(x => x.InputAttributes, new Dictionary<string, object>
            {
                ["type"] = "text",
                ["role"] = "button",
                ["aria-checked"] = "false",
                ["checked"] = false,
            }));

        var input = cut.Find("input");
        Assert.Equal("checkbox", input.GetAttribute("type"));
        Assert.Equal("switch", input.GetAttribute("role"));
        Assert.Equal("true", input.GetAttribute("aria-checked"));
        Assert.True(input.HasAttribute("checked"));
    }

    [Fact]
    public void ASwitchWithoutALabel_CanBeNamedThroughInputAttributes()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.InputAttributes, new Dictionary<string, object> { ["aria-label"] = "Enable wifi" }));

        Assert.Equal("Enable wifi", cut.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void AnError_IsDescribedAndMarkedInvalid()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "Wifi")
            .Add(x => x.ErrorText, "Required"));

        var input = cut.Find("input");
        var error = cut.Find("[role=alert]");
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal(error.Id, input.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void TheAccessibleName_DoesNotContainTheError()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "Wifi")
            .Add(x => x.ErrorText, "Required"));

        var labelId = cut.Find("input").GetAttribute("aria-labelledby");
        Assert.Equal("Wifi", cut.Find($"#{labelId}").TextContent.Trim());
    }

    [Fact]
    public void ClearingTheError_ClearsTheAssociations()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "Wifi")
            .Add(x => x.ErrorText, "Required"));

        cut.Render(p => p
            .Add(x => x.Label, "Wifi")
            .Add(x => x.ErrorText, (string?)null));

        Assert.Null(cut.Find("input").GetAttribute("aria-invalid"));
        Assert.Empty(cut.FindAll("[role=alert]"));
    }

    [Fact]
    public void AdditionalAttributes_StayOnTheLabel()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "Wifi")
            .AddUnmatched("data-testid", "wrapper"));

        Assert.Equal("wrapper", cut.Find("label").GetAttribute("data-testid"));
        Assert.Null(cut.Find("input").GetAttribute("data-testid"));
    }
}
