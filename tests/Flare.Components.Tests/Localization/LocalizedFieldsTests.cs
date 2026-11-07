using System.Globalization;
using Flare.Abstractions;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests;

public sealed class LocalizedFieldsTests : FlareTestContext
{
    [Fact]
    public async Task NumberFormattingFollowsFormatCultureWhileExplicitCultureWins()
    {
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo("en-US"));
        Services.AddSingleton<IFlareLocalization>(context);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<FlareNumericField<decimal>>(0);
            b.AddAttribute(1, "Value", 1234.5m);
            b.AddAttribute(2, "Format", "N2");
            b.CloseComponent();
            b.OpenComponent<FlareNumericField<decimal>>(3);
            b.AddAttribute(4, "Value", 1234.5m);
            b.AddAttribute(5, "Format", "N2");
            b.AddAttribute(6, "Culture", CultureInfo.GetCultureInfo("en-US"));
            b.CloseComponent();
        }));
        await cut.InvokeAsync(() => context.SetCulture(CultureInfo.GetCultureInfo("de-DE"), CultureInfo.GetCultureInfo("ru")));
        Assert.Equal("1.234,50", cut.FindAll("input")[0].GetAttribute("value"));
        Assert.Equal("1,234.50", cut.FindAll("input")[1].GetAttribute("value"));
    }

    [Fact]
    public async Task PickerKeepsUncommittedTextAndRelocalizesItsError()
    {
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo("en-US"));
        Services.AddSingleton<IFlareLocalization>(context);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareDatePicker>(c => c
            .Add(x => x.AllowInput, true).Add(x => x.DateFormat, "dd.MM.yyyy")));
        var input = cut.Find("input");
        input.Focus();
        input.Input("12.0");
        input.Change("12.0");
        var typed = cut.Find("input").GetAttribute("value");
        await cut.InvokeAsync(() => context.SetCulture(CultureInfo.GetCultureInfo("en-US"), CultureInfo.GetCultureInfo("ru")));
        var expected = context.GetLocalizer<FlareStrings>()["DatePicker_Error_Incomplete"].Value;
        cut.WaitForAssertion(() => Assert.Contains(expected, cut.Markup));
        Assert.Equal(typed, cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public async Task UploadRejectionChangesLanguageWithoutReplacingTheInput()
    {
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(context);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareFileUploadZone>(c => c.Add(x => x.MaxFileSize, 1)));
        var input = cut.FindComponent<InputFile>();
        input.UploadFiles(InputFileContent.CreateFromText("too large", "oversize.txt"));
        Assert.Contains("was not added", cut.Markup);
        await cut.InvokeAsync(() => context.SetCulture(CultureInfo.GetCultureInfo("ru")));
        var expected = context.GetLocalizer<FlareStrings>()["FileUpload_FileTooLarge", "oversize.txt", "1 B"].Value;
        cut.WaitForAssertion(() => Assert.Contains(expected, cut.Find("[role=alert]").TextContent));
        Assert.Same(input.Instance, cut.FindComponent<InputFile>().Instance);
    }

    [Fact]
    public async Task ExplicitPickerCaptionsIncludingEmptyStayIndependent()
    {
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(context);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<FlareDatePicker>(0);
            b.AddAttribute(1, "Inline", true);
            b.AddAttribute(2, "ClearText", "Custom clear");
            b.CloseComponent();
            b.OpenComponent<FlareDatePicker>(3);
            b.AddAttribute(4, "Inline", true);
            b.AddAttribute(5, "ClearText", "");
            b.CloseComponent();
        }));
        await cut.InvokeAsync(() => context.SetCulture(CultureInfo.GetCultureInfo("ru")));
        Assert.Equal("Custom clear", cut.FindComponents<FlareDatePicker>()[0].Instance.ClearText);
        Assert.Equal("", cut.FindComponents<FlareDatePicker>()[1].Instance.ClearText);
        Assert.Contains("Custom clear", cut.Markup);
    }

    [Fact]
    public async Task CalendarSwitchKeepsItsInstanceViewAndExplicitFormattingCulture()
    {
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo("en-US"));
        Services.AddSingleton<IFlareLocalization>(context);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareCalendar>(c => c
            .Add(x => x.ViewDate, new DateOnly(2026, 5, 1)).Add(x => x.Culture, CultureInfo.GetCultureInfo("en-US"))));
        var calendar = cut.FindComponent<FlareCalendar>().Instance;
        cut.FindAll("button")[1].Click();
        Assert.Contains("June", cut.Markup);
        await cut.InvokeAsync(() => context.SetCulture(CultureInfo.GetCultureInfo("de-DE"), CultureInfo.GetCultureInfo("ru")));
        var expected = context.GetLocalizer<FlareStrings>()["Calendar_NextMonth"].Value;
        cut.WaitForAssertion(() => Assert.Equal(expected, cut.FindAll("button")[1].GetAttribute("aria-label")));
        Assert.Contains("June", cut.Markup);
        Assert.Same(calendar, cut.FindComponent<FlareCalendar>().Instance);
    }
}
