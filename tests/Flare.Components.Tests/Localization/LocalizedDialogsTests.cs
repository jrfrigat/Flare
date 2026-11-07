using System.Globalization;
using Flare.Abstractions;
using Flare.Infrastructure;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Flare.Components.Resources;

namespace Flare.Components.Tests;

public sealed class LocalizedDialogsTests : FlareTestContext
{
    [Fact]
    public async Task OpenGuardRelocalizesTitleBodyAndActionsWithoutClosingTheOwner()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        var dialogs = new DialogService();
        Services.AddSingleton<IFlareLocalization>(culture);
        Services.AddSingleton<IDialogService>(dialogs);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<FlareDialogProvider>(0); b.CloseComponent();
            b.OpenComponent<FlareDialog>(1);
            b.AddAttribute(2, "Visible", true);
            b.AddAttribute(3, "Title", "Editor");
            b.AddAttribute(4, "ChildContent", (RenderFragment)(child =>
            {
                child.OpenComponent<FlareDialogCloseGuard>(0);
                child.AddAttribute(1, "HasUnsavedChanges", true);
                child.CloseComponent();
            }));
            b.CloseComponent();
        }));
        var owner = cut.FindComponents<FlareDialog>().Single().Instance;
        Task<bool> pending = null!;
        await cut.InvokeAsync(() => { pending = owner.CloseAsync(); });
        cut.WaitForAssertion(() => Assert.Single(dialogs.OpenDialogs));
        var request = dialogs.OpenDialogs[0];
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("ru")));
        var text = culture.GetLocalizer<FlareStrings>();
        cut.WaitForAssertion(() => Assert.Contains(text["DialogGuard_Title"].Value, cut.Find("h2").TextContent));
        Assert.Contains(text["DialogGuard_Message"].Value, cut.Markup);
        Assert.Same(request, Assert.Single(dialogs.OpenDialogs));
        Assert.False(pending.IsCompleted);
        cut.FindAll("button").Single(x => x.TextContent == text["DialogGuard_KeepEditing"].Value).Click();
        Assert.False(await pending);
        Assert.True(owner.Visible);
    }

    [Fact]
    public async Task PromptRetainsTypedValueAndExplicitEmptyCaptionOnSwitch()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        var messages = new MessageBoxService();
        Services.AddSingleton<IFlareLocalization>(culture);
        Services.AddSingleton<IMessageBoxService>(messages);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareMessageBoxProvider>());
        var result = messages.PromptAsync("Title", "Name", confirmLabel: "");
        cut.WaitForState(() => cut.FindAll("input").Count == 1);
        cut.Find("input").Input("Unsaved text");
        var request = messages.Current;
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("ru")));
        cut.WaitForAssertion(() => Assert.Contains(culture.GetLocalizer<FlareStrings>()["Dialog_Cancel"].Value, cut.Markup));
        Assert.Equal("Unsaved text", cut.Find("input").GetAttribute("value"));
        Assert.Equal("", cut.FindAll("button")[1].TextContent);
        Assert.Same(request, messages.Current);
        cut.FindAll("button")[1].Click();
        Assert.Equal("Unsaved text", await result);
    }

    [Fact]
    public async Task AnOpenConfirmationKeepsItsRequestAndTranslatesDefaultButtons()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        var dialogs = new DialogService();
        Services.AddSingleton<IFlareLocalization>(culture);
        Services.AddSingleton<IDialogService>(dialogs);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareDialogProvider>());
        var response = dialogs.ConfirmAsync("Explicit title", "Explicit message");
        cut.WaitForAssertion(() => Assert.Contains("Cancel", cut.Markup));
        var request = dialogs.Current;
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("ru")));
        cut.WaitForAssertion(() => Assert.Contains("\u041e\u0442\u043c\u0435\u043d\u0430", cut.Markup));
        Assert.Same(request, dialogs.Current);
        Assert.Contains("Explicit title", cut.Markup);
        Assert.False(response.IsCompleted);
        dialogs.Respond(true);
        Assert.True(await response);
    }
}
