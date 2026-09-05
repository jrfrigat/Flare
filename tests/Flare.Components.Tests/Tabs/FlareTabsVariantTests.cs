using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTabsVariantTests : FlareTestContext
{
    [Theory]
    [InlineData(TabsVariant.Underline, Css.Classes.Tabs.Underline)]
    [InlineData(TabsVariant.Primary, Css.Classes.Tabs.Primary)]
    [InlineData(TabsVariant.Text, Css.Classes.Tabs.Text)]
    [InlineData(TabsVariant.Tonal, Css.Classes.Tabs.Tonal)]
    [InlineData(TabsVariant.Filled, Css.Classes.Tabs.Filled)]
    [InlineData(TabsVariant.Outlined, Css.Classes.Tabs.Outlined)]
    public void Variant_AddsModifierClass(TabsVariant variant, string expected)
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.Variant, variant)
            .AddChildContent<FlareTab>(t => t.Add(x => x.Label, "A")));
        Assert.Contains(expected, cut.Find($".{Css.Classes.Tabs.Root}").ClassName);
    }

    [Fact]
    public void Default_AddsNoVariantModifier()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent<FlareTab>(t => t.Add(x => x.Label, "A")));
        var cls = cut.Find($".{Css.Classes.Tabs.Root}").ClassName;
        Assert.DoesNotContain(Css.Classes.Tabs.Underline, cls);
        Assert.DoesNotContain(Css.Classes.Tabs.Primary, cls);
        Assert.DoesNotContain(Css.Classes.Tabs.Filled, cls);
    }
}
