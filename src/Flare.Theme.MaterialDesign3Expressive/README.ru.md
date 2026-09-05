# Flare.Theme.MaterialDesign3Expressive

Тема Material Design 3 (Expressive), светлая и темная, для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor, вместе со встроенными палитрами MD3.

```sh
dotnet add package Flare.Theme.MaterialDesign3Expressive
```

```csharp
// как тема по умолчанию...
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3ExpressiveTheme();
    opts.DefaultPalette = Md3Palettes.Violet;
});
// ...или зарегистрировать рядом с другими и переключать во время работы:
builder.Services.AddFlareTheme(new MaterialDesign3ExpressiveTheme());
// await ThemeService.SetThemeAsync("md3-expressive");
```

## Material 3 Expressive -> Flare

Expressive - это не отдельный набор компонентов, а тот же M3 с более широкой осью размеров, формой,
реагирующей на взаимодействие, и более пружинистой анимацией. Поэтому таблица ниже - это таблица M3,
а раздел после нее говорит, что эта тема делает сверх базовой.

| Material 3 | Flare | Чем выбирается |
| :-- | :-- | :-- |
| Обычные кнопки (elevated, filled, tonal, outlined, text) | `FlareButton` | `Variant="ButtonVariant.Elevated\|Filled\|Tonal\|Outlined\|Text"` |
| Размеры кнопок (XS-XL) | `FlareButton` | `Size` - все пять ступеней принадлежат Expressive; в базовом M3 кнопка одна |
| Toggle-кнопка | `FlareToggleButton` | `@bind-Toggled`; выбор меняет форму круглая<->квадратная |
| Группы кнопок (standard, connected) | `FlareButtonGroup` | `Connected`, `Vertical` |
| Segmented buttons | `FlareButtonGroup` из `FlareToggleButton` | `Connected="true"` |
| FAB, малый и большой FAB | `FlareFloatingActionButton` | `Size="FabSize.Sm\|Md\|Lg"` |
| Extended FAB | `FlareFloatingActionButton` | FAB с подписью И ЕСТЬ расширенный: задайте `Label` |
| FAB-меню | `FlareFloatingActionMenu` + `FlareFloatingActionMenuItem` | |
| Иконочные кнопки | `FlareIconButton` | `Variant` повторяет варианты кнопки |
| Split button | `FlareSplitButton` | |
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
| Индикаторы прогресса (линейный, круговой) | `FlareProgress` | `Variant="ProgressVariant.Linear\|Circular"`; `Wavy` - от Expressive |
| Нижняя панель навигации | `FlareBottomNav` | |
| Навигационный рельс | `FlareNavMenu` | `Mode="NavMenuMode.Rail"` |
| Навигационная панель (standard, modal) | `FlareLayoutDrawer` | `Variant="DrawerVariant.Persistent\|Temporary\|Responsive"` |
| Верхняя панель приложения | `FlareLayoutAppBar` | |
| Вкладки (primary, secondary) | `FlareTabs` | `Variant="TabsVariant.Primary\|Underline"` |
| Поиск | `FlareCombobox` | |
| Выбор даты | `FlareDatePicker`, `FlareDateRangePicker` | |
| Выбор времени | `FlareTimePicker` | |
| Карусель | `FlareCarousel` | пакет `Flare.Components.Carousel` |
| Разделитель | `FlareDivider` | |
| Bottom sheet / side sheet | `FlareDialog` | `Size="DialogSize.FullWidth"` плюс размещение |
| Панель команд | `FlareToolbar` | |

Из перечисленного не пропущено ничего, но Flare добавляет многое, чего M3 не описывает (таблица
данных, дерево, kanban, ribbon, конструктор запросов). Они берут цвет и форму из тех же токенов -
просто сравнивать их с Material не с чем.

## Что тема меняет помимо цвета

- **Шкала размеров - как в спецификации**: 32/40/56/96/136dp для кнопок, а не пять мягких ступеней.
- **Форма реагирует на взаимодействие**: нажатая кнопка становится квадратнее, выбранная toggle-кнопка
  меняет форму на противоположную, и обе - на пружине, а не на линейном easing.
- **Группа кнопок обменивается шириной при нажатии**: нажатый сегмент растет, соседние отдают ровно
  столько же, поэтому ширина самой группы не меняется.
- **Волнистые индикаторы прогресса** и пружинные примитивы анимации (`MotionTokens.EasingSpring*`).

Требуется `Flare.Components`. Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
