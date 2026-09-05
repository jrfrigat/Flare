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
}
