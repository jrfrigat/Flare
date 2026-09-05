# Flare.Theme.FluentUI2

Тема Fluent UI 2, светлая и темная, для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor.

```sh
dotnet add package Flare.Theme.FluentUI2
```

```csharp
// как тема по умолчанию...
builder.Services.AddFlare(opts => opts.DefaultTheme = new FluentUI2Theme());
// ...или зарегистрировать и переключать во время работы:
builder.Services.AddFlareTheme(new FluentUI2Theme());
// await ThemeService.SetThemeAsync("fluent2");
```

## Fluent UI 2 -> Flare

Fluent называет часть элементов иначе, чем Material, и в двух местах разделяет то, что Material
считает одним компонентом. В колонке Flare - сам компонент, в третьей - то, что заставляет его читаться
как Fluent, а не как Material.

| Fluent UI 2 | Flare | Чем выбирается |
| :-- | :-- | :-- |
| Button (primary, secondary, outline, subtle, transparent) | `FlareButton` | `Variant="ButtonVariant.Filled\|Tonal\|Outlined\|Text"` - «primary» это `Filled`, «subtle» это `Text` |
| Compound button | `FlareButton` | вторая строка содержимого внутри `ChildContent` |
| Toggle button | `FlareToggleButton` | `@bind-Toggled` - Fluent меняет цвет, а не форму |
| Split button | `FlareSplitButton` | |
| Menu button | `FlareButton` + `FlareMenu` | |
| Card | `FlareCard` | |
| Badge / Counter badge / Presence badge | `FlareBadge`, `FlareAvatar` | присутствие - это точка статуса у `FlareAvatar` |
| Avatar / Avatar group | `FlareAvatar`, `FlareAvatarGroup` | |
| Checkbox / Radio group / Switch | `FlareCheckbox`, `FlareRadioGroup`, `FlareSwitch` | |
| Slider | `FlareSlider` | |
| Input / Textarea | `FlareField`, `FlareTextArea` | |
| Combobox / Dropdown | `FlareCombobox`, `FlareSelect` | Combobox у Fluent редактируемый, Dropdown - нет |
| SpinButton | `FlareNumericField` | |
| Field (подпись, подсказка и валидация вокруг контрола) | `FlareField` | обвязка встроена, а не является отдельной оберткой |
| Menu / MenuItem | `FlareMenu`, `FlareMenuItem` | |
| Toolbar | `FlareToolbar` | |
| Dialog / Drawer | `FlareDialog`, `FlareLayoutDrawer` | |
| Popover / Tooltip | `FlarePopover`, `FlareTooltip` | |
| Toast | `ISnackbarService` | внедряется через DI, разметки нет |
| MessageBar | `FlareAlert` | |
| ProgressBar / Spinner | `FlareProgress` | `Variant="ProgressVariant.Linear\|Circular"` |
| TabList | `FlareTabs` | |
| Breadcrumb | `FlareBreadcrumb` | |
| Accordion | `FlareAccordion` | |
| DataGrid / Table | `FlareDataGrid`, `FlareTable` | |
| Tree | `FlareTreeView` | |
| Link | `FlareLink` | |
| Divider | `FlareDivider` | |
| Rating | `FlareRating` | |
| Persona | `FlareAvatar` + `FlareText` | отдельного компонента нет, собирается из этих |

## Что тема меняет помимо цвета

- **Дискретные заливки состояний, а не полупрозрачная пленка.** Там, где Material рисует наложение
  `currentColor` на прозрачности состояния, Fluent назначает плоскую заливку на каждое состояние через
  те же токены `--flare-state-*-layer`.
- **Disabled перекрашивается, а не гаснет.** У каждого компонента здесь `DisabledOpacity` равна `1`, а
  работу делает плоская disabled-палитра, - Material же гасит весь элемент до 38%.
- **Фокус - это обводка, сосуществующая с наведением**, поэтому тема ставит
  `--flare-state-focus-hover-layer` в свою hover-заливку, а не в focus-заливку.
- **Более прямые углы** и более плотная типографическая шкала, чем у обоих поколений Material.

Требуется `Flare.Components`. Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
