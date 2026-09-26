using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareDialogProviderComponentTests : FlareTestContext
{
    public FlareDialogProviderComponentTests()
    {
        Services.AddSingleton<IDialogService, DialogService>();
    }

    [Fact]
    public void NoComponentDialog_NoScrim()
    {
        var cut = Render<FlareDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Dialog.Scrim}"));
    }

    [Fact]
    public void Show_RendersBodyAndTitle()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<FlareDialogProvider>();

        service.Show<TestDialogBody>("Edit profile",
            new DialogParameters().Add(nameof(TestDialogBody.Payload), "hello"));
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Dialog.Scrim}").Count > 0);

        Assert.NotEmpty(cut.FindAll(".test-ok"));
        Assert.Contains("Edit profile", cut.Find($".{Css.Classes.Dialog.Title}").TextContent);
    }

    [Fact]
    public async Task ClickingBodyButton_ClosesDialog_AndResolvesResult()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<FlareDialogProvider>();

        var reference = service.Show<TestDialogBody>("Edit",
            new DialogParameters().Add(nameof(TestDialogBody.Payload), "hello"));
        cut.WaitForState(() => cut.FindAll(".test-ok").Count > 0);

        cut.Find(".test-ok").Click();
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Dialog.Scrim}").Count == 0);

        var result = await reference.Result;
        Assert.False(result.Cancelled);
        Assert.Equal("hello", result.GetData<string>());
        Assert.Empty(service.OpenDialogs);
    }

    [Fact]
    public async Task Confirm_ScrimClick_DismissesWithNull()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<FlareDialogProvider>();

        var pending = service.ConfirmAsync("Delete?", "It cannot be undone.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Dialog.Scrim}").Count > 0);

        cut.Find($".{Css.Classes.Dialog.Scrim}").Click();

        Assert.True(pending.IsCompleted);
        Assert.Null(await pending);
        Assert.Null(service.Current);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Dialog.Scrim}").Count == 0);
    }

    [Fact]
    public async Task Confirm_Escape_DismissesWithNull()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<FlareDialogProvider>();

        var pending = service.ConfirmAsync("Delete?", "It cannot be undone.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Dialog.Scrim}").Count > 0);

        await cut.InvokeAsync(() => cut.FindComponent<FlareDialog>().Instance.CloseFromEsc());

        Assert.True(pending.IsCompleted);
        Assert.Null(await pending);
        Assert.Null(service.Current);
    }

    [Fact]
    public async Task Alert_ScrimClick_Completes()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<FlareDialogProvider>();

        var pending = service.AlertAsync("Saved", "Your changes are saved.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Dialog.Scrim}").Count > 0);

        cut.Find($".{Css.Classes.Dialog.Scrim}").Click();

        // AlertAsync is an async wrapper whose continuation may be posted, so wait with a bound.
        await pending.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Null(service.Current);
    }

    [Fact]
    public async Task SecondConfirm_ShowsAfterTheFirstIsAnswered()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<FlareDialogProvider>();

        var first = service.ConfirmAsync("Delete draft?", "a", "Delete", "Keep");
        var second = service.ConfirmAsync("Leave page?", "b", "Leave", "Stay");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Dialog.Title}").Count > 0);
        Assert.Contains("Delete draft?", cut.Find($".{Css.Classes.Dialog.Title}").TextContent);

        cut.FindAll("button").First(b => b.TextContent.Contains("Delete")).Click();

        Assert.True(first.IsCompleted);
        Assert.True(await first);
        Assert.False(second.IsCompleted);
        cut.WaitForState(() => cut.Find($".{Css.Classes.Dialog.Title}").TextContent.Contains("Leave page?"));
        Assert.Single(cut.FindAll($".{Css.Classes.Dialog.Scrim}"));

        cut.FindAll("button").First(b => b.TextContent.Contains("Stay")).Click();

        Assert.True(second.IsCompleted);
        Assert.False(await second);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Dialog.Scrim}").Count == 0);
    }
}
