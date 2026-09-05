# Flare.Theme.MaterialDesign2

Тема Material Design 2, светлая и темная, для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor, вместе со встроенными палитрами MD2.

```sh
dotnet add package Flare.Theme.MaterialDesign2
```

```csharp
// как тема по умолчанию...
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign2Theme();
    opts.DefaultPalette = Md2Palettes.Purple;
});
// ...или зарегистрировать рядом с другими и переключать во время работы:
builder.Services.AddFlareTheme(new MaterialDesign2Theme());
// await ThemeService.SetThemeAsync("md2");
```

## Material 2 -> Flare

Словарь M2 меньше, чем у M3, поэтому в нескольких строках один компонент M2 ложится на компонент
Flare, который обслуживает и понятие из M3. Там, где M2 и M3 расходятся в названии, указано название M2.

| Material 2 | Flare | Чем выбирается |
| :-- | :-- | :-- |
| Кнопки (contained, outlined, text) | `FlareButton` | `Variant="ButtonVariant.Filled\|Outlined\|Text"` - «contained» это `Filled` |
| Toggle-кнопки | `FlareToggleButton`, `FlareButtonGroup` | |
| Floating action button (обычный, mini, extended) | `FlareFloatingActionButton` | `Size`; для расширенного задайте `Label` |
| Иконочные кнопки | `FlareIconButton` | |
| Карточки | `FlareCard` | M2 рисует одну карточку; `Variant` все равно предлагает три из M3 |
| Чипы (input, choice, filter, action) | `FlareChip` | `@bind-Selected`, `Closable` |
| Checkbox / Radio / Switch | `FlareCheckbox`, `FlareRadioGroup`, `FlareSwitch` | |
| Слайдеры (непрерывный, дискретный, диапазон) | `FlareSlider` | `Step` для дискретного, `Range="true"` для двух ползунков |
| Текстовые поля (filled, outlined) | `FlareField` и типизированные поля | `Variant` |
| Меню | `FlareMenu` + `FlareMenuItem` | |
| Списки | `FlareList` + `FlareListItem` | |
| Диалоги | `FlareDialog` | |
| Snackbars | `ISnackbarService` | внедряется через DI, разметки нет |
| Подсказки | `FlareTooltip` | |
| Значки | `FlareBadge` | |
| Индикаторы прогресса (линейный, круговой) | `FlareProgress` | `Variant` |
| Нижняя навигация | `FlareBottomNav` | |
| Навигационная панель (standard, modal, bottom) | `FlareLayoutDrawer` | `Variant` |
| Верхняя панель приложения | `FlareLayoutAppBar` | |
| Вкладки (фиксированные, прокручиваемые) | `FlareTabs` | прокрутка включается сама, когда полоса не помещается |
| Banners | `FlareAlert` | `Variant="AlertVariant.Filled\|Outlined\|Text"` |
| Таблицы данных | `FlareTable`, `FlareDataGrid` | таблица данных добавляет сортировку, страницы, группировку и правку |
| Backdrop | `FlareOverlay` | |
| Image lists | `FlareGrid` из `FlareImage` | |
| Выбор даты и времени | `FlareDatePicker`, `FlareTimePicker` | |
| Разделители | `FlareDivider` | |

## Что тема меняет помимо цвета

- **Площе и прямоугольнее.** Шкала формы M2 заканчивается заметно ниже, чем у M3, и кнопка-пилюля не
  является значением по умолчанию: кнопки - прямоугольники со скруглением 4dp.
- **Иерархию несет тень**, а не тональный цвет поверхности: M2 опирается на тени там, где M3 опирается
  на тональные подмешивания в surface.
- **Подписи кнопок в верхнем регистре**, от чего M3 отказался.

Требуется `Flare.Components`. Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
