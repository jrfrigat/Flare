using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareChipClosableTests : FlareTestContext
{
    [Fact]
    public void Closeable_False_Default_NoCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, false));

        Assert.Empty(cut.FindAll($"button.{Css.Classes.Chip.Close}"));
    }

    [Fact]
    public void Closeable_True_RendersCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Chip.Close}"));
    }

    [Fact]
    public void Closeable_True_CloseButtonHasAriaLabel()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true));

        var btn = cut.Find($"button.{Css.Classes.Chip.Close}");
        Assert.NotNull(btn.GetAttribute("aria-label"));
    }

    [Fact]
    public void Closeable_True_ClickClose_InvokesOnClose()
    {
        var invoked = false;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true)
            .Add(x => x.OnClose, EventCallback.Factory.Create(this, () => { invoked = true; })));

        cut.Find($"button.{Css.Classes.Chip.Close}").Click();

        Assert.True(invoked);
    }

    [Fact]
    public void Label_RendersLabelText()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "MyChip"));

        Assert.Contains("MyChip", cut.Find($".{Css.Classes.Chip.Label}").TextContent);
    }

    [Fact]
    public void Closeable_True_AndSelected_BothRenderCorrectly()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true)
            .Add(x => x.Selected, true));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Chip.Close}"));
        Assert.Contains(Css.Classes.Chip.Selected, cut.Find($".{Css.Classes.Chip.Root}").ClassName ?? "");
    }
}
