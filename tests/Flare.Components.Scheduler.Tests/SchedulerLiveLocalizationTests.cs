using System.Globalization;
using Flare.Abstractions;
using Flare.Components.Scheduler;
using Flare.Components.Tests;
using Flare.Infrastructure;
using Flare.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Scheduler.Tests;

public sealed class SchedulerLiveLocalizationTests : FlareTestContext
{
    [Fact]
    public async Task OpenEditorChangesLanguageWithoutReplacingDraftOrFormattingCulture()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        var dialogs = new DialogService();
        Services.AddFlareScheduler();
        Services.AddSingleton<IFlareLocalization>(culture);
        Services.AddSingleton<IDialogService>(dialogs);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<FlareDialogProvider>(0); b.CloseComponent();
            b.OpenComponent<FlareScheduler<SchedulerEventData>>(1);
            b.AddAttribute(2, "Mapping", SchedulerMappings.EventData);
            b.AddAttribute(3, "Date", new DateOnly(2026, 11, 4));
            b.AddAttribute(4, "View", SchedulerView.Agenda);
            b.AddAttribute(5, "Culture", CultureInfo.GetCultureInfo("en-US"));
            b.AddAttribute(6, "Editable", true);
            b.AddAttribute(7, "OnChange", EventCallback.Factory.Create<SchedulerChangeEventArgs<SchedulerEventData>>(this, _ => { }));
            b.CloseComponent();
        }));
        var scheduler = cut.FindComponent<FlareScheduler<SchedulerEventData>>().Instance;
        Task pending = null!;
        await cut.InvokeAsync(() => { pending = scheduler.CreateEventAsync(new(new(2026, 11, 4, 9, 0, 0, TimeSpan.Zero), new(2026, 11, 4, 10, 0, 0, TimeSpan.Zero))); });
        cut.WaitForAssertion(() => Assert.Single(dialogs.OpenDialogs));
        var editor = cut.FindComponent<FlareSchedulerEditor<SchedulerEventData>>().Instance;
        cut.Find("input[type=text]").Change("Unsaved event");
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("de-DE"), CultureInfo.GetCultureInfo("ru")));
        var text = culture.GetLocalizer<SchedulerStrings>();
        cut.WaitForAssertion(() => Assert.Contains(text["Create"].Value, cut.Find("h2").TextContent));
        Assert.Same(editor, cut.FindComponent<FlareSchedulerEditor<SchedulerEventData>>().Instance);
        Assert.Equal("Unsaved event", editor.Context.Draft.Title);
        Assert.Equal("en-US", scheduler.EffectiveCulture.Name);
        Assert.False(editor.Context.IsClosed);
        Assert.Same(editor.Context, scheduler.ActiveEdit);
        cut.FindAll("button").Single(x => x.TextContent == text["Cancel"].Value).Click();
        await pending;
    }
}
