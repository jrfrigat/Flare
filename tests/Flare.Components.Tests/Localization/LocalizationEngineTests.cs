using System.Globalization;
using Flare.Abstractions;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;

namespace Flare.Components.Tests;

public sealed class LocalizationEngineTests : BunitContext
{
    [Theory]
    [InlineData("en", "Cancel")]
    [InlineData("ru-RU", "\u041e\u0442\u043c\u0435\u043d\u0430")]
    [InlineData("de-DE", "Cancel")]
    public void BuiltInsFollowCultureFallback(string culture, string expected)
    {
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo(culture));
        Assert.Equal(expected, context.GetLocalizer<FlareStrings>()["Picker_Cancel"].Value);
        Assert.True(context.GetLocalizer<FlareStrings>()["MissingKey"].ResourceNotFound);
    }

    [Fact]
    public void PartialAndEmptyOverridesKeepBuiltInsAndFormatInTheSeparateCulture()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IStringLocalizer<FlareStrings>>(new ApplicationText());
        using var provider = services.BuildServiceProvider();
        var context = new FlareLocalization(provider);
        context.SetCulture(CultureInfo.GetCultureInfo("de-DE"), CultureInfo.GetCultureInfo("fr-FR"));
        var text = context.GetLocalizer<FlareStrings>();
        Assert.Equal("Suivant", text["Picker_Next"].Value);
        Assert.Equal("", text["Picker_Ok"].Value);
        Assert.False(text["Picker_Ok"].ResourceNotFound);
        Assert.Equal("Cancel", text["Picker_Cancel"].Value);
        Assert.Equal("1.234,50", text["Number", 1234.5m].Value);
        Assert.Equal("two custom items", text["Count", 2].Value);
        Assert.Contains(text.GetAllStrings(true), x => x.Name == "Picker_Next" && x.Value == "Suivant");
        context.SetCulture(CultureInfo.GetCultureInfo("ru-RU"));
        Assert.Equal("\u0421\u043b\u0435\u0434\u0443\u044e\u0449\u0438\u0439", text["Picker_Next"].Value);
        Assert.Same(text, context.GetLocalizer<FlareStrings>());
    }

    [Fact]
    public void MutableCultureIsSnapshottedAndCanBeAppliedAgainAfterEditing()
    {
        var source = (CultureInfo)CultureInfo.GetCultureInfo("de-DE").Clone();
        var context = new FlareLocalization();
        context.SetCulture(source);
        source.NumberFormat.NumberDecimalSeparator = "!";
        Assert.Equal(",", context.Culture.NumberFormat.NumberDecimalSeparator);
        context.SetCulture(source);
        Assert.Equal("!", context.Culture.NumberFormat.NumberDecimalSeparator);
        Assert.True(context.Culture.IsReadOnly);
    }

    [Fact]
    public async Task IndependentScopesDoNotChangeAmbientOrDefaultCultures()
    {
        var before = CultureInfo.CurrentCulture;
        var beforeUI = CultureInfo.CurrentUICulture;
        var defaults = CultureInfo.DefaultThreadCurrentUICulture;
        var russian = new FlareLocalization();
        var english = new FlareLocalization();
        russian.SetCulture(CultureInfo.GetCultureInfo("ru"));
        english.SetCulture(CultureInfo.GetCultureInfo("en"));
        await Task.WhenAll(Task.Run(() => Check(russian, "\u041e\u0442\u043c\u0435\u043d\u0430"), Xunit.TestContext.Current.CancellationToken),
            Task.Run(() => Check(english, "Cancel"), Xunit.TestContext.Current.CancellationToken));
        Assert.Same(before, CultureInfo.CurrentCulture);
        Assert.Same(beforeUI, CultureInfo.CurrentUICulture);
        Assert.Same(defaults, CultureInfo.DefaultThreadCurrentUICulture);
        static void Check(FlareLocalization context, string expected)
        {
            for (var i = 0; i < 1000; i++)
                Assert.Equal(expected, context.GetLocalizer<FlareStrings>()["Picker_Cancel"].Value);
        }
    }

    [Fact]
    public async Task ServerScopesAndPrerenderWorkWithoutBrowserServices()
    {
        var registrations = new ServiceCollection();
        registrations.AddLogging();
        registrations.AddScoped<IFlareLocalization>(sp => new FlareLocalization(sp));
        using var root = registrations.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using var first = root.CreateScope();
        using var second = root.CreateScope();
        first.ServiceProvider.GetRequiredService<IFlareLocalization>().SetCulture(CultureInfo.GetCultureInfo("ru"));
        second.ServiceProvider.GetRequiredService<IFlareLocalization>().SetCulture(CultureInfo.GetCultureInfo("en"));
        Assert.NotSame(first.ServiceProvider.GetRequiredService<IFlareLocalization>(), second.ServiceProvider.GetRequiredService<IFlareLocalization>());
        foreach (var (scope, expected) in new[] { (first, "\u041e\u0442\u043c\u0435\u043d\u0430"), (second, "Cancel") })
        {
            await using var renderer = new HtmlRenderer(scope.ServiceProvider, NullLoggerFactory.Instance);
            var html = await renderer.Dispatcher.InvokeAsync(async () =>
            {
                RenderFragment child = b => { b.OpenComponent<Probe>(0); b.CloseComponent(); };
                var output = await renderer.RenderComponentAsync<FlareLocalizationProvider>(
                    ParameterView.FromDictionary(new Dictionary<string, object?> { ["ChildContent"] = child }));
                return output.ToHtmlString();
            });
            Assert.Contains(expected, System.Net.WebUtility.HtmlDecode(html));
        }
    }

    [Fact]
    public void FailedFormattingRestoresAmbientCulture()
    {
        var before = CultureInfo.CurrentCulture;
        var beforeUI = CultureInfo.CurrentUICulture;
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo("ru"));
        Assert.Throws<FormatException>(() => context.GetLocalizer<FlareStrings>()["FileUpload_FileTooLarge", "one"]);
        Assert.Same(before, CultureInfo.CurrentCulture);
        Assert.Same(beforeUI, CultureInfo.CurrentUICulture);
    }

    [Fact]
    public async Task ProviderRefreshesMountedConsumerWithoutReinitializingAndUnsubscribes()
    {
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(context);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<Probe>());
        var instance = cut.FindComponent<Probe>().Instance;
        Assert.Equal("Cancel", cut.Find("span").TextContent);
        await cut.InvokeAsync(() => context.SetCulture(CultureInfo.GetCultureInfo("ru")));
        cut.WaitForAssertion(() => Assert.Equal("\u041e\u0442\u043c\u0435\u043d\u0430", cut.Find("span").TextContent));
        Assert.Same(instance, cut.FindComponent<Probe>().Instance);
        Assert.Equal(1, instance.Initializations);
        cut.Dispose();
        context.SetCulture(CultureInfo.GetCultureInfo("en"));
        Assert.Equal(1, instance.Initializations);
    }

    [Fact]
    public async Task CultureRefreshSkipsPlainTextButParentUpdatesStillRenderCachedContent()
    {
        var context = new FlareLocalization();
        context.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(context);
        var value = "First";
        RenderFragment fragment = b =>
        {
            b.OpenComponent<FlareText>(0);
            b.AddAttribute(1, nameof(FlareText.ChildContent), (RenderFragment)(x => x.AddContent(0, value)));
            b.CloseComponent();
        };
        var cut = Render<FlareLocalizationProvider>(p => p.Add(x => x.ChildContent, fragment));
        var plain = cut.FindComponent<FlareText>();
        var initialRenders = plain.RenderCount;
        await cut.InvokeAsync(() => context.SetCulture(CultureInfo.GetCultureInfo("ru")));
        Assert.Equal(initialRenders, plain.RenderCount);
        value = "Second";
        cut.Render(p => p.Add(x => x.ChildContent, fragment));
        Assert.Equal("Second", plain.Find("p").TextContent);
    }

    private sealed class ApplicationText : IStringLocalizer<FlareStrings>
    {
        public LocalizedString this[string name] => name switch
        {
            "Picker_Next" when CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr" => new(name, "Suivant"),
            "Picker_Ok" => new(name, ""),
            "Number" => new(name, "{0:N2}"),
            "Count" => new(name, "raw template"),
            _ => new(name, name, true)
        };
        public LocalizedString this[string name, params object[] arguments] => name == "Count"
            ? new(name, arguments[0] is 1 ? "one custom item" : "two custom items")
            : new(name, string.Format(this[name], arguments), this[name].ResourceNotFound);
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => new[] { this["Picker_Next"], this["Picker_Ok"] };
    }

    public sealed class Probe : FlareLocalizedComponentBase
    {
        public int Initializations { get; private set; }
        protected override string ComponentCssClass => "probe";
        protected override void OnInitialized() => Initializations++;
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddContent(1, CoreText["Picker_Cancel"].Value);
            builder.CloseElement();
        }
    }
}
