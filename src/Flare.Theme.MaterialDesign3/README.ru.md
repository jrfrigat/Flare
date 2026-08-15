# Flare.Theme.MaterialDesign3

Тема Material Design 3 (базовая), светлая и тёмная, для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor, вместе со встроенными палитрами MD3.

```sh
dotnet add package Flare.Theme.MaterialDesign3
```

```csharp
// как тема по умолчанию...
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3Theme();
    opts.DefaultPalette = Md3Palettes.Violet;
});
// ...или зарегистрировать рядом с другими и переключать во время работы:
builder.Services.AddFlareTheme(new MaterialDesign3Theme());
// await ThemeService.SetThemeAsync("md3");
```

## Material 3 -> Flare

| Material 3 | Flare | Чем выбирается |
| :-- | :-- | :-- |
| Обычные кнопки (elevated, filled, tonal, outlined, text) | `FlareButton` | `Variant="ButtonVariant.Elevated\|Filled\|Tonal\|Outlined\|Text"` |
| Toggle-кнопка | `FlareToggleButton` | `@bind-Toggled` |
| Segmented buttons | `FlareButtonGroup` из `FlareToggleButton` | `Connected="true"` |
| FAB, малый и большой FAB | `FlareFloatingActionButton` | `Size="FabSize.Sm\|Md\|Lg"` |
| Extended FAB | `FlareFloatingActionButton` | FAB с подписью И ЕСТЬ расширенный: задайте `Label` |
| Иконочные кнопки | `FlareIconButton` | `Variant` повторяет варианты кнопки |
| Карточки (elevated, filled, outlined) | `FlareCard` | `Variant="CardVariant.Elevated\|Filled\|Outlined"` |
| Чипы (assist, filter, input, suggestion) | `FlareChip` | `Variant`, плюс `@bind-Selected` для filter и `Closable` для input |
| Checkbox / Radio / Switch | `FlareCheckbox`, `FlareRadioGroup`, `FlareSwitch` | |
| Слайдеры | `FlareSlider` | `Range="true"` для двух ползунков |
| Текстовые поля (filled, outlined) | `FlareField` и типизированные поля | `Variant` |
| Меню | `FlareMenu` + `FlareMenuItem` | |
| Списки | `FlareList` + `FlareListItem` | |
| Диалоги (обычный, полноэкранный) | `FlareDialog` | `Size="DialogSize.FullScreen"` |
| Snackbar | `ISnackbarService` | внедряется через DI, разметки нет |
| Подсказки (plain, rich) | `FlareTooltip` | |
| Значки | `FlareBadge` | `Standalone` для голой пилюли |
| Индикаторы прогресса (линейный, круговой) | `FlareProgress` | `Variant="ProgressVariant.Linear\|Circular"` |
| Нижняя панель навигации | `FlareBottomNav` | |
| Навигационный рельс | `FlareNavMenu` | `Mode="NavMenuMode.Rail"` |
| Навигационная панель (standard, modal) | `FlareLayoutDrawer` | `Variant="DrawerVariant.Persistent\|Temporary\|Responsive"` |
| Верхняя панель приложения | `FlareLayoutAppBar` | |
| Вкладки (primary, secondary) | `FlareTabs` | `Variant="TabsVariant.Primary\|Underline"` |
| Поиск | `FlareCombobox` | |
| Выбор даты и времени | `FlareDatePicker`, `FlareDateRangePicker`, `FlareTimePicker` | |
| Разделитель | `FlareDivider` | |
| Карусель | `FlareCarousel` | пакет `Flare.Components.Carousel` |

## Базовая, а не Expressive

Этот пакет - M3 в том виде, в каком он был описан до обновления Expressive, и разница намеренная:

- **Один размер кнопки.** В собственной таблице Material «M3 против Expressive» указана единственная
  кнопка 40dp; ступени XS/M/L/XL приходят вместе с Expressive. Параметр `Size` работает и здесь -
  просто шаги мягче.
- **Нет морфинга формы.** Нажатая кнопка не становится квадратнее, выбранная toggle-кнопка не меняет форму.
- **Нет волнистого прогресса** и пружинной анимации.

Если всё это нужно - берите `Flare.Theme.MaterialDesign3Expressive`; палитры и цветовые роли общие,
поэтому переключение между ними меняет геометрию и движение, но не цвет.

Требуется `Flare.Components`. Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
