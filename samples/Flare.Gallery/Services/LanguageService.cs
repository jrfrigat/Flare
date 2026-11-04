using Flare.Abstractions;
using Flare.Gallery.Resources;
using Microsoft.JSInterop;
using System.Globalization;

namespace Flare.Gallery.Services;

/// <summary>WASM host policy for browser language, saved preferences and application resource captions.</summary>
public sealed class LanguageService(IJSRuntime js, IFlareLocalization localization)
{
    public string CurrentCulture => localization.UICulture.TwoLetterISOLanguageName;
    public CultureInfo FormattingCulture => localization.Culture;

    public static readonly IReadOnlyList<(string Code, string DisplayName)> SupportedCultures = [
        ("en", "English"), ("ru", "\u0420\u0443\u0441\u0441\u043a\u0438\u0439"), ("fr", "Fran\u00e7ais")
    ];

    public async Task SetCultureAsync(string cultureCode)
    {
        var code = Resolve(cultureCode) ?? "en";
        if (code == CurrentCulture) return;
        Apply(FormattingCulture, CultureInfo.GetCultureInfo(code));
        await PersistAsync("blazorCulture", code);
        await js.InvokeVoidAsync("document.documentElement.setAttribute", "lang", code);
    }

    public async Task SetFormattingCultureAsync(string code)
    {
        var culture = Parse(code) ?? CultureInfo.GetCultureInfo("en-US");
        Apply(culture, localization.UICulture);
        await PersistAsync("blazorFormattingCulture", culture.Name);
    }

    public async Task InitializeCultureAsync()
    {
        var browserLanguage = await js.InvokeAsync<string>("flareGalleryBrowserLanguage");
        var ui = Resolve(await ReadAsync("blazorCulture")) ?? Resolve(browserLanguage) ?? "en";
        var format = Parse(await ReadAsync("blazorFormattingCulture")) ?? CultureInfo.CurrentCulture;
        Apply(format, CultureInfo.GetCultureInfo(ui));
        await js.InvokeVoidAsync("document.documentElement.setAttribute", "lang", ui);
    }

    private void Apply(CultureInfo format, CultureInfo ui)
    {
        // This application is a single-user WASM host. Server applications use request/circuit culture
        // instead; the reusable localization engine never changes thread defaults or static resources.
        CultureInfo.DefaultThreadCurrentCulture = format;
        CultureInfo.DefaultThreadCurrentUICulture = ui;
        GalleryStrings.Culture = ui;
        localization.SetCulture(format, ui);
    }

    private async Task<string?> ReadAsync(string key)
    {
        try { return await js.InvokeAsync<string?>("localStorage.getItem", key); }
        catch (JSException) { return null; }
    }

    private async Task PersistAsync(string key, string value)
    {
        try { await js.InvokeVoidAsync("localStorage.setItem", key, value); }
        catch (JSException) { }
    }

    private static CultureInfo? Parse(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        try { return CultureInfo.GetCultureInfo(code); }
        catch (CultureNotFoundException) { return null; }
    }

    private static string? Resolve(string? code)
    {
        for (var culture = Parse(code); culture is not null && culture.Name.Length > 0; culture = culture.Parent)
            if (SupportedCultures.Any(x => x.Code == culture.Name)) return culture.Name;
        return null;
    }
}
