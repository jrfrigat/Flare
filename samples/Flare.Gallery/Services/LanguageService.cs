using Flare.Gallery.Resources;
using Microsoft.JSInterop;
using System.Globalization;

namespace Flare.Gallery.Services;

public sealed class LanguageService
{
    private readonly IJSRuntime _js;

    // Предоставляем прямой и безопасный доступ к ресурсам через сервис
    public GalleryStrings Strings => new();

    public LanguageService(IJSRuntime js)
    {
        _js = js;
    }

    public string CurrentCulture { get; private set; } = "en";

    public event Action? LanguageChanged;

    public static readonly IReadOnlyList<(string Code, string DisplayName)> SupportedCultures = [
        ("en", "English"),
        ("ru", "Русский"),
    ];

    // Метод сделан асинхронным для работы с localStorage (критично для PWA офлайн)
    public async Task SetCultureAsync(string cultureCode)
    {
        if (cultureCode == CurrentCulture) return;

        CurrentCulture = cultureCode;
        var culture = new CultureInfo(cultureCode);

        // Для Blazor WASM ОБЯЗАТЕЛЬНО менять DefaultThreadCurrent...
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        GalleryStrings.Culture = culture;

        // Сохраняем выбор пользователя, чтобы PWA помнил его офлайн
        await _js.InvokeVoidAsync("localStorage.setItem", "blazorCulture", cultureCode);

        LanguageChanged?.Invoke();
    }

    // Инициализация культуры при старте приложения.
    // Приоритет: 1) сохранённый выбор пользователя -> 2) язык браузера -> 3) "en".
    public async Task InitializeCultureAsync()
    {
        // 1. Явный выбор пользователя (сохранён в localStorage, важно для офлайн PWA).
        var storedCulture = await _js.InvokeAsync<string?>("localStorage.getItem", "blazorCulture");

        // 2. Иначе - язык браузера: рантайм Blazor WASM инициализирует CurrentUICulture из
        //    navigator.language, поэтому отдельный JS-вызов не нужен.
        // 3. Иначе - "en".
        var cultureCode = Resolve(storedCulture)
            ?? Resolve(CultureInfo.CurrentUICulture.Name)
            ?? "en";

        CurrentCulture = cultureCode;
        var culture = new CultureInfo(cultureCode);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        GalleryStrings.Culture = culture;
    }

    // Сопоставляет код культуры (например "ru-RU") с поддерживаемым кодом ("ru");
    // возвращает null, если язык не поддерживается.
    private static string? Resolve(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        return SupportedCultures
            .Select(c => c.Code)
            .FirstOrDefault(c => code.StartsWith(c, StringComparison.OrdinalIgnoreCase));
    }
}
