using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareChipClosableInteractionTests : FlareTestContext
{
    [Fact]
    public void Closable_True_RendersCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closable, true));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Chip.Close}"));
    }

    [Fact]
    public void Closeable_True_AlsoRendersCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Chip.Close}"));
    }

    [Fact]
    public void CloseButton_HasAriaLabel()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closable, true));

        var btn = cut.Find($"button.{Css.Classes.Chip.Close}");
        Assert.NotNull(btn.GetAttribute("aria-label"));
    }

    [Fact]
    public void OnClose_FiredWhenCloseButtonClicked()
    {
        var invoked = false;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closable, true)
            .Add(x => x.OnClose, EventCallback.Factory.Create(this, () => { invoked = true; })));

        cut.Find($"button.{Css.Classes.Chip.Close}").Click();

        Assert.True(invoked);
    }

    [Fact]
    public void WithoutClosable_NoCloseButtonRendered()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closable, false)
            .Add(x => x.Closeable, false));

        Assert.Empty(cut.FindAll($"button.{Css.Classes.Chip.Close}"));
    }
}
