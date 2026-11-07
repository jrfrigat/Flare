# Локализация

> [English version](../en/localization.md)

Flare содержит английские и русские ресурсы. Приложение может добавлять языки и частично заменять
переводы через стандартный `IStringLocalizer<T>`, включая независимые пакеты компонентов.
Язык интерфейса и форматирование дат/чисел задаются отдельно.

## Подключение обновляемого контекста

Сохраните существующие регистрацию `AddFlare` и тему. Она регистрирует scoped `IFlareLocalization`.
Оберните приложение вместе с провайдером диалогов:

```razor
<FlareLocalizationProvider>
    <FlareThemeProvider>
        <Routes />
        <FlareDialogProvider />
    </FlareThemeProvider>
</FlareLocalizationProvider>
```

Внедрите `Flare.Abstractions.IFlareLocalization` в переключатель языка:

```csharp
locale.SetCulture(CultureInfo.GetCultureInfo("de-DE"),
                  CultureInfo.GetCultureInfo("ru-RU"));
```

Подписи станут русскими, форматирование дат и чисел - немецким. Без второго аргумента первая культура
используется для обеих целей. Явный `Culture` компонента имеет приоритет для его форматирования.
До первого `SetCulture` контекст следует окружающим `CurrentCulture`/`CurrentUICulture`.
Без провайдера сохраняется ambient fallback ресурсов; для обновлений и переводов приложения нужен
провайдер. Не управляйте смонтированными компонентами через статический `FlareStrings.Culture`.

Смена культуры обновляет подписи, validation и ARIA-сообщения, сохраняя экземпляры, черновики и
открытые диалоги/popup. `@key` с культурой и перезагрузка страницы не нужны. Текст самого приложения
также нужно заново разрешать при смене культуры: готовая строка сама не переводится. Для уведомлений
компонент приложения может принимать каскадный `FlareLocalizationState`.

## Частичное переопределение ресурсов

Зарегистрируйте закрытый `IStringLocalizer<FlareStrings>` для ядра. Отсутствующие ключи берутся из
пакета. Для независимых пакетов регистрируются соответствующие типы ресурсов:

| Пакет | Тип ресурса |
| :-- | :-- |
| Ядро | `Flare.Components.Resources.FlareStrings` |
| Gantt | `Flare.Components.Gantt.GanttStrings` |
| Scheduler | `Flare.Components.Scheduler.SchedulerStrings` |
| IDE | `Flare.Components.IDE.IdeStrings` |
| Media | `Flare.Components.Resources.VideoPlayerStrings` |
| Query | `Flare.Components.Resources.QueryStrings` |

Например, создайте `MyApp.Translations.CoreOverrides` и соответствующие embedded-файлы
`Translations/CoreOverrides.resx` (пустой нейтральный), `CoreOverrides.fr.resx` с `Picker_Clear=Effacer`.
При стандартном пути ресурсов зарегистрируйте адаптер:

```csharp
services.AddSingleton<IStringLocalizer<FlareStrings>, CoreTranslations>();

sealed class CoreTranslations(IStringLocalizerFactory factory) : IStringLocalizer<FlareStrings>
{
    private readonly IStringLocalizer text = factory.Create(typeof(MyApp.Translations.CoreOverrides));
    public LocalizedString this[string name] => text[name];
    public LocalizedString this[string name, params object[] arguments] => text[name, arguments];
    public IEnumerable<LocalizedString> GetAllStrings(bool parents) => text.GetAllStrings(parents);
}
```

Подключите `Microsoft.Extensions.Localization` и namespace ресурса ядра. Сохраняйте флаг
`ResourceNotFound`: отсутствующий ключ включает fallback, намеренно пустой перевод - нет.
Индексатор с аргументами вызывается у вашего localizer, позволяя собственное склонение/форматирование.
Не внедряйте `IFlareLocalization` обратно в адаптер: это создаст цикл зависимостей.
Адаптер без пользовательского состояния может быть singleton; пользовательский должен быть scoped.

Приоритет: явная подпись экземпляра, localizer приложения, встроенный culture fallback (`ru-RU` в
`ru`), нейтральный английский ресурс. Localizer приложения сначала применяет собственный стандартный
fallback. Оставляйте нейтральный override-файл пустым, если заменяете только отдельные языки:
нейтральный текст приложения иначе перекроет встроенные переводы во всех языках.
Список ключей: `locale.GetLocalizer<FlareStrings>().GetAllStrings(true)`.

## Политика хоста

Определение языка браузера, сохранение выбора, переключатели и хранилище принадлежат хосту.
Движок Flare не обращается к JS, не устанавливает process defaults и не делит культуру между scope.

В WASM восстановите UI/format-культуры до `RunAsync`. Подключите нужные ICU-данные
(`BlazorWebAssemblyLoadAllGlobalizationData=true` поддерживает все культуры). Чтобы переключаться
на незагруженный satellite-язык без reload, загрузите ресурсы при старте. Gallery на .NET 10 использует:

```javascript
Blazor.start({
    configureRuntime: runtime => runtime.withConfig({ loadAllSatelliteResources: true })
});
```

Это однократно загружает все поставляемые satellite-ресурсы; учитывайте объем при большом числе языков.
См. [runtime loader](https://github.com/dotnet/runtime/blob/v10.0.0/src/mono/browser/runtime/loader/assets.ts)
и [руководство Blazor](https://learn.microsoft.com/aspnet/core/blazor/globalization-localization).
Gallery на `/localization` показывает восстановление browser/saved выбора и отдельное форматирование.
Французские переводы намеренно частичные, принадлежат приложению; отсутствующие подписи остаются английскими.

В Server/SSR настройте request localization middleware/cookies перед рендерингом и инициализируйте
scoped-контекст каждого circuit из запроса или предпочтений пользователя. Prerender не требует JS.
Сохраняйте язык в cookie хоста, если он должен действовать в следующих SSR-запросах. Не используйте
`DefaultThreadCurrentCulture`, static resource culture или изменяемые singleton-настройки пользователя
на Server. Для двух независимых деревьев локализации на одной странице нужен собственный DI scope
каждому провайдеру; вложенные провайдеры одного scope используют общий контекст.

## Миграция в 0.52

- Даты и числа по умолчанию используют `CurrentCulture` (или scoped-эквивалент), а не UI-культуру.
  Явный `Culture` сохраняет приоритет. Машинные форматы, например ISO, остаются invariant.
- Типы ресурсов Gantt/Scheduler и прежние static-accessors доступны. Для overrides и живых подписей
  используйте localizer контекста. `GanttStrings.Describe` также принимает localizer.
- Стандартные подписи dialog/message-box теперь nullable: `null` разрешает текущий перевод при рендеринге;
  явная строка, включая пустую, сохраняется. Для обновляемого заголовка есть `DialogOptions.TitleProvider`.
- Подписи IDE и `Labels` QueryBuilder по умолчанию `null`, что включает живые ресурсы.
  Явный `QueryBuilderLabels` остается полным снимком подписей, которым управляет приложение.
- Подписи экземпляра сохраняются; если они зависят от языка, пересчитывайте их при смене культуры.
- Для наследников FileUpload: `Rejections` теперь read-only и переводится при чтении;
  `FormatSize` стал методом экземпляра с культурой форматирования контекста.

`FlareText` и `FlareComponentBase` не получают локализационных сервисов и глобальных подписок.
Каскад используют только локализуемые компоненты; единственной подпиской на изменения владеет провайдер.
