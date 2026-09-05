using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareSplitButtonTests : FlareTestContext
{
    [Fact]
    public void RendersMainLabelAndTrigger()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.SplitButton.Main}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.SplitButton.Trigger}"));
        Assert.Contains("Save", cut.Find($".{Css.Classes.SplitButton.Main}").TextContent);
    }

    [Fact]
    public void Menu_IsClosedInitially()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save"))
            .Add(x => x.MenuItems, b =>
            {
                b.OpenComponent<FlareMenuItem>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(2, "Save as...")));
                b.CloseComponent();
            }));

        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    [Fact]
    public void Menu_OpensOnTriggerClick_AndShowsItems()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save"))
            .Add(x => x.MenuItems, b =>
            {
                b.OpenComponent<FlareMenuItem>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(2, "Save as...")));
                b.CloseComponent();
            }));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
        Assert.Contains("Save as...", cut.Markup);
    }

    [Fact]
    public void Disabled_DisablesBothButtons()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save")));

        Assert.All(cut.FindAll($".{Css.Classes.SplitButton.Root} button"),
            btn => Assert.True(btn.HasAttribute("disabled")));
    }
}
