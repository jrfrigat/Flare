# Flare.Theme.Aero

Тема Aero (глянцевая, полупрозрачная), светлая и тёмная, для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor.

```sh
dotnet add package Flare.Theme.Aero
```

```csharp
// как тема по умолчанию...
builder.Services.AddFlare(opts => opts.DefaultTheme = new AeroTheme());
// ...или зарегистрировать и переключать во время работы:
builder.Services.AddFlareTheme(new AeroTheme());
// await ThemeService.SetThemeAsync("aero");
```

## Windows Aero -> Flare

Aero - это визуальный стиль Windows, а не опубликованная библиотека компонентов, поэтому таблица ниже
сопоставляет общие элементы управления Win32, которые он одевал, с компонентами Flare, играющими их
роль. Если вы переносите «настольного» вида приложение - интерфейс в духе 1С или Office 2010 - читать
надо эту колонку.

| Aero / Win32 | Flare | Примечания |
| :-- | :-- | :-- |
| Командная кнопка | `FlareButton` | `Variant="ButtonVariant.Tonal"` - та самая серая командная кнопка |
| Кнопка по умолчанию (акцентная) | `FlareButton` | `Variant="ButtonVariant.Filled"` |
| Link label | `FlareLink` | |
| Поле ввода / поле с маской | `FlareField`, `FlareMaskedField` | |
| Счётчик (up-down) | `FlareNumericField` | |
| Combo box (редактируемый / список) | `FlareCombobox`, `FlareSelect` | редактируемый против только-список |
| Флажок / переключатель | `FlareCheckbox`, `FlareRadioGroup` | |
| Trackbar | `FlareSlider` | |
| Индикатор выполнения | `FlareProgress` | |
| Group box | `FlareCard` | `Variant="CardVariant.Outlined"` |
| Tab control | `FlareTabs` | |
| List view (таблица) | `FlareTable`, `FlareDataGrid` | |
| Tree view | `FlareTreeView` | |
| Панель инструментов / Rebar | `FlareToolbar` | |
| Строка состояния | `FlareStatusBar` | |
| Главное и контекстное меню | `FlareMenuBar`, `FlareMenu` | |
| Balloon tip | `FlareTooltip` | |
| Task dialog | `FlareDialog`, `IMessageBoxService` | |
| Выбор даты/времени, календарь | `FlareDatePicker`, `FlareCalendar` | |
| Разделитель панелей | `FlareSplitter` | |

## Что тема меняет помимо цвета

- **Глянцевые вертикальные градиенты** на каждой приподнятой поверхности, с внутренним бликом сверху
  и рамкой в 1px.
- **Смена градиента И ЕСТЬ наведение** - тема ставит свои токены слоя состояния в `transparent`, а не
  позволяет полупрозрачной плёнке лечь поверх глянца.
- **Голубоватое свечение при наведении** и утопленная вдавленность при нажатии - обе из хрома той эпохи.
- Всё это построено на токенах темы, разрешаемых через `color-mix`, поэтому глянец следует за палитрой
  и работает в тёмной теме, а не является фиксированным набором синих.

Требуется `Flare.Components`. Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
