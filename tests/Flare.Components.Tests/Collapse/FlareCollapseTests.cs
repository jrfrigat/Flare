using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareCollapse (standalone expand/collapse)
// ------------------------------------------------------------------------------
public class FlareCollapseTests : FlareTestContext
{
    [Fact]
    public void Collapsed_ByDefault()
    {
        var cut = Render<FlareCollapse>(p => p
            .AddChildContent("<p class=\"inner\">Body</p>"));

        Assert.DoesNotContain(Css.Classes.Collapse.Expanded, cut.Find($".{Css.Classes.Collapse.Root}").ClassName);
    }

    [Fact]
    public void Expanded_AddsExpandedClass()
    {
        var cut = Render<FlareCollapse>(p => p
            .Add(x => x.Expanded, true)
            .AddChildContent("<p class=\"inner\">Body</p>"));

        Assert.Contains(Css.Classes.Collapse.Expanded, cut.Find($".{Css.Classes.Collapse.Root}").ClassName);
    }

    [Fact]
    public void Header_RendersToggleButton_AndExpands()
    {
        var cut = Render<FlareCollapse>(p => p
            .Add(x => x.Header, "More")
            .AddChildContent("<p class=\"inner\">Body</p>"));

        var btn = cut.Find($"button.{Css.Classes.Collapse.Header}");
        Assert.Equal("false", btn.GetAttribute("aria-expanded"));

        btn.Click();

        Assert.Contains(Css.Classes.Collapse.Expanded, cut.Find($".{Css.Classes.Collapse.Root}").ClassName);
        Assert.Equal("true", cut.Find($"button.{Css.Classes.Collapse.Header}").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Headerless_RendersNoToggleButton()
    {
        var cut = Render<FlareCollapse>(p => p
            .AddChildContent("<p class=\"inner\">Body</p>"));

        Assert.Empty(cut.FindAll($"button.{Css.Classes.Collapse.Header}"));
    }
}
