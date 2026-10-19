namespace Flare.Components.Tests;

public class CheckboxInputAttributesTests : FlareTestContext
{
    [Fact]
    public void InputAttributes_ReachTheNativeCheckbox()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(x => x.Label, "Accept")
            .Add(x => x.InputAttributes, new Dictionary<string, object>
            {
                ["name"] = "consent",
                ["required"] = true,
                ["data-testid"] = "native",
            }));

        var input = cut.Find("input");
        Assert.Equal("consent", input.GetAttribute("name"));
        Assert.True(input.HasAttribute("required"));
        Assert.Equal("native", input.GetAttribute("data-testid"));
    }

    [Fact]
    public void IconOnlyCheckbox_CanBeNamedThroughInputAttributes()
    {
        var cut = Render<FlareCheckbox<bool?>>(p => p
            .Add(x => x.Value, null)
            .Add(x => x.InputAttributes, new Dictionary<string, object> { ["aria-label"] = "Select row" }));

        Assert.Equal("Select row", cut.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void OwnedAttributes_AreNotOverriddenByInputAttributes()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(x => x.Value, true)
            .Add(x => x.InputAttributes, new Dictionary<string, object>
            {
                ["type"] = "text",
                ["checked"] = false,
                ["disabled"] = true,
            }));

        var input = cut.Find("input");
        Assert.Equal("checkbox", input.GetAttribute("type"));
        Assert.True(input.HasAttribute("checked"));
        Assert.False(input.HasAttribute("disabled"));
    }

    [Fact]
    public void AdditionalAttributes_StayOnTheLabel()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(x => x.Label, "Accept")
            .AddUnmatched("data-testid", "wrapper"));

        Assert.Equal("wrapper", cut.Find("label").GetAttribute("data-testid"));
        Assert.Null(cut.Find("input").GetAttribute("data-testid"));
    }
}
