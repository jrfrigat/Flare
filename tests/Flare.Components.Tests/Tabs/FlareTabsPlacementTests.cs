using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTabsPlacementTests : FlareTestContext
{
    [Theory]
    [InlineData(TabsPlacement.Top, "", false)]
    [InlineData(TabsPlacement.Bottom, Css.Classes.Tabs.Bottom, false)]
    [InlineData(TabsPlacement.Left, Css.Classes.Tabs.Vertical, true)]
    [InlineData(TabsPlacement.Right, Css.Classes.Tabs.Right, true)]
    public void Placement_AppliesClasses(TabsPlacement placement, string expected, bool vertical)
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.Placement, placement));
        var cls = cut.Find($".{Css.Classes.Tabs.Root}").ClassName ?? "";
        if (!string.IsNullOrEmpty(expected)) Assert.Contains(expected, cls);
        Assert.Equal(vertical, cls.Contains(Css.Classes.Tabs.Vertical));
    }

    [Theory]
    [InlineData(TabLabelRotation.None, "0deg")]
    [InlineData(TabLabelRotation.Rotate90, "90deg")]
    [InlineData(TabLabelRotation.Rotate180, "180deg")]
    [InlineData(TabLabelRotation.Rotate270, "270deg")]
    public void LabelRotation_SetsCssVariable(TabLabelRotation rot, string expected)
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.LabelRotation, rot));
        Assert.Contains($"{Css.Tokens.LocalVars.TabLabelRotation}:{expected}", cut.Find($".{Css.Classes.Tabs.Root}").GetAttribute("style"));
    }

    [Fact]
    public void Rotation90_AddsRotatedClass()
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.LabelRotation, TabLabelRotation.Rotate90));
        Assert.Contains(Css.Classes.Tabs.Rotated, cut.Find($".{Css.Classes.Tabs.Root}").ClassName);
    }
}
