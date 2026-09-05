using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareMultiSelectDisabledA11yTests : FlareTestContext
{
    [Fact]
    public void Disabled_SetsAriaDisabledAndRemovesFromTabOrder()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Disabled, true));
        var combobox = cut.Find("[role=combobox]");
        Assert.Equal("true", combobox.GetAttribute("aria-disabled"));
        Assert.Equal("-1", combobox.GetAttribute("tabindex"));
    }

    [Fact]
    public void Enabled_IsTabbableWithoutAriaDisabled()
    {
        var cut = Render<FlareMultiSelect<string>>();
        var combobox = cut.Find("[role=combobox]");
        Assert.Null(combobox.GetAttribute("aria-disabled"));
        Assert.Equal("0", combobox.GetAttribute("tabindex"));
    }
}
