using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareClipboardTests : FlareTestContext
{
    [Fact]
    public void RendersRootButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello"));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Clipboard.Root}"));
    }

    [Fact]
    public void RendersDefaultCopyIcon_WhenNoChildContent()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello"));

        // The default copy icon is now the built-in SVG (no Material Symbols font dependency).
        Assert.NotEmpty(cut.FindAll("svg path"));
    }

    [Fact]
    public void RendersChildContent_WhenProvided()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "copy me")
            .AddChildContent("<span id=\"copy-label\">Copy</span>"));

        Assert.NotEmpty(cut.FindAll("#copy-label"));
    }

    [Fact]
    public void InitialState_NotCopied_NoCheckedClass()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello"));

        Assert.Empty(cut.FindAll($".{Css.Classes.Clipboard.Copied}"));
    }

    [Fact]
    public void TextParam_IsRequired_RendersWithValue()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "some content"));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Clipboard.Root}"));
    }

    [Fact]
    public void ButtonHasTypeButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "value"));

        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void AdditionalAttributes_AppliedToButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "data")
            .AddUnmatched("data-testid", "clipboard-btn"));

        Assert.Equal("clipboard-btn", cut.Find("button").GetAttribute("data-testid"));
    }

    [Fact]
    public void FeedbackContent_NotRenderedInitially()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello")
            .Add(x => x.FeedbackContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "id", "feedback-content");
                b.CloseElement();
            })));

        Assert.Empty(cut.FindAll("#feedback-content"));
    }

    [Fact]
    public async Task OnCopied_IsNotHeldBackByTheFeedbackAnimation()
    {
        // It used to be raised AFTER the confirmation delay, so a caller learned the copy had succeeded a
        // full two seconds late. A long delay here means the test only passes if OnCopied runs before it.
        var copied = false;
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello")
            .Add(x => x.FeedbackDurationMs, 30_000)
            .Add(x => x.OnCopied, EventCallback.Factory.Create(this, () => copied = true)));

        _ = cut.Find($"button.{Css.Classes.Clipboard.Root}").ClickAsync(new MouseEventArgs());

        // Let the copy + callback run, but nowhere near the 30s confirmation.
        await Task.Delay(200, Xunit.TestContext.Current.CancellationToken);
        Assert.True(copied);
    }

    [Fact]
    public void DisabledAndLoading_ReachTheInnerButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello")
            .Add(x => x.Disabled, true)
            .Add(x => x.Loading, true));

        var button = cut.Find($"button.{Css.Classes.Clipboard.Root}");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Contains(Css.Classes.Button.Loading, button.ClassList);
    }
}

// ------------------------------------------------------------------------------
// FlareShortcuts  (3 tests from Wave6)
// ------------------------------------------------------------------------------
