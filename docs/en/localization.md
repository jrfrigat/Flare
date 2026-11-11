# Localization

> [Русская версия](../ru/localization.md)

Flare includes English and Russian resources. Applications can add languages and partially replace
translations through standard `IStringLocalizer<T>`, including the independent component packages.
UI language and date/number formatting are separate choices.

## Connect the live context

Keep your existing `AddFlare` and theme registration. It registers the scoped `IFlareLocalization`.
Wrap the application, including its dialog provider, once:

```razor
<FlareLocalizationProvider>
    <FlareThemeProvider>
        <Routes />
        <FlareDialogProvider />
    </FlareThemeProvider>
</FlareLocalizationProvider>
```

Inject `Flare.Abstractions.IFlareLocalization` into the language selector:

```csharp
locale.SetCulture(CultureInfo.GetCultureInfo("de-DE"),
                  CultureInfo.GetCultureInfo("ru-RU"));
```

This displays Russian captions with German date/number formatting. Omitting the second argument uses
the first culture for both. An explicit component `Culture` wins over the context's formatting culture.
Before the first `SetCulture`, the context follows ambient `CurrentCulture`/`CurrentUICulture`.
Without a provider, components retain ambient resource fallback; use the provider for live updates
and application overrides. Do not change static `FlareStrings.Culture` to control mounted components.

Changing culture refreshes localized captions, validation and ARIA announcements. It preserves mounted
instances, draft values and open dialogs/popups. No `@key` on culture and no page reload are needed.
Application-owned text must also be resolved again when culture changes; a previously computed string
cannot translate itself. A component can consume cascading `FlareLocalizationState` to receive updates.

## Partially replace package resources

Register a closed `IStringLocalizer<FlareStrings>` for core resources. Its missing keys fall through to
the package. Register the corresponding resource type for each independent package:

| Package | Resource type |
| :-- | :-- |
| Core | `Flare.Components.Resources.FlareStrings` |
| Gantt | `Flare.Components.Gantt.GanttStrings` |
| Scheduler | `Flare.Components.Scheduler.SchedulerStrings` |
| IDE | `Flare.Components.IDE.IdeStrings` |
| Media | `Flare.Components.Resources.VideoPlayerStrings` |
| Query | `Flare.Components.Resources.QueryStrings` |

For example, create `MyApp.Translations.CoreOverrides` and matching embedded
`Translations/CoreOverrides.resx` (empty neutral file), `CoreOverrides.fr.resx` with `Picker_Clear=Effacer`.
With the default resource path, register a thin adapter:

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

Use `Microsoft.Extensions.Localization` and the core resource namespace. Preserve the
`ResourceNotFound` flag: a missing key falls through, an intentionally empty translation does not.
The argument indexer is delegated to your localizer, allowing custom pluralization/formatting.
Do not inject `IFlareLocalization` back into this adapter; that creates a dependency cycle.
A stateless resource adapter can be singleton; user-specific adapters must be scoped.

Priority is explicit instance caption, application localizer, built-in culture fallback (`ru-RU` to
`ru`), then neutral English. The application localizer applies its own ordinary resource fallback
before the package is consulted. Keep the neutral override file empty when only replacing selected
languages; neutral application text otherwise shadows package translations in every language.
Get available keys with `locale.GetLocalizer<FlareStrings>().GetAllStrings(true)`.

## Host culture policy

Browser detection, saved preferences, UI selectors and storage belong to the host. Flare's engine
does not access JavaScript, set process defaults, or share a user's culture with another scope.

For WASM, initialize saved UI/format cultures before `RunAsync`. Include the ICU data needed by your
formats (`BlazorWebAssemblyLoadAllGlobalizationData=true` supports all cultures). To switch into an
unloaded satellite language without reloading, preload resources during startup. Gallery's .NET 11 host uses:

```javascript
Blazor.start({
    configureRuntime: runtime => runtime.withConfig({ loadAllSatelliteResources: true })
});
```

This downloads all shipped satellite resources once; consider that payload when shipping many languages.
See the [runtime loader](https://github.com/dotnet/runtime/blob/v10.0.0/src/mono/browser/runtime/loader/assets.ts)
and [Blazor globalization guidance](https://learn.microsoft.com/aspnet/core/blazor/globalization-localization).
Gallery demonstrates browser/saved preference restoration and independent formatting at `/localization`.
Its French translations are deliberately partial consumer overrides; missing captions remain English.

For Server/SSR, use request localization middleware/cookies before rendering, and initialize each circuit's
scoped context from that request or its user preferences. Prerender needs no JavaScript. Persist a language
choice in a host cookie if subsequent SSR requests must use it. Do not use `DefaultThreadCurrentCulture`,
static resource culture, or singleton mutable user preferences on Server. For two independent locale trees
in one page, give each provider its own DI scope; nested providers in the same scope share the context.

## Migration in 0.52

- Defaults for dates/numbers now use `CurrentCulture` (or the scoped equivalent), rather than UI culture.
  Explicit `Culture` still wins. Machine formats such as ISO values remain invariant.
- Gantt/Scheduler resource marker types and legacy static accessors remain available. Use the context
  localizer for overrides and live captions. `GanttStrings.Describe` also accepts a localizer.
- Default dialog/message-box captions are nullable: `null` resolves the current translation when rendered;
  an explicit string, including empty, is preserved. Use `DialogOptions.TitleProvider` for a live custom title.
- IDE caption parameters and QueryBuilder `Labels` default to `null`, which selects live resources.
  An explicit `QueryBuilderLabels` is a complete caption snapshot owned by the application.
- Explicit instance captions are preserved; if they depend on language, recompute them on culture changes.
- For derived FileUpload components, `Rejections` is read-only and translated when read;
  `FormatSize` is now an instance helper using the formatting context.

`FlareText` and `FlareComponentBase` gain no localization service or global event subscription. Only
localized components consume the cascade; the provider owns and releases the single change subscription.
