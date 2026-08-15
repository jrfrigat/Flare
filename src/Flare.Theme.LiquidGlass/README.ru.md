# Flare.Theme.LiquidGlass

Тема Liquid Glass (матовое стекло, слои глубины), светлая и тёмная, для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor.

```sh
dotnet add package Flare.Theme.LiquidGlass
```

```csharp
// как тема по умолчанию...
builder.Services.AddFlare(opts => opts.DefaultTheme = new LiquidGlassTheme());
// ...или зарегистрировать и переключать во время работы:
builder.Services.AddFlareTheme(new LiquidGlassTheme());
// await ThemeService.SetThemeAsync("liquid-glass");
```

## Liquid Glass -> Flare

Liquid Glass - это эстетика, а не каталог компонентов: полупрозрачные поверхности со слоями глубины и
бликом, в традиции платформ Apple. Отдельного словаря компонентов у неё нет, поэтому таблица ниже -
словарь элементов управления той платформы, которой принадлежит этот вид.

| Элемент платформы Apple | Flare | Примечания |
| :-- | :-- | :-- |
| Button (prominent, bordered, plain) | `FlareButton` | `Variant="ButtonVariant.Filled\|Outlined\|Text"` |
| Toggle (переключатель) | `FlareSwitch` | |
| Segmented control | `FlareButtonGroup` из `FlareToggleButton` | `Connected="true"` |
| Stepper | `FlareNumericField` | |
| Slider | `FlareSlider` | |
| Picker / Menu | `FlareSelect`, `FlareMenu` | |
| Search field | `FlareCombobox` | |
| Text field / Text editor | `FlareField`, `FlareTextArea` | |
| Sheet / Alert | `FlareDialog`, `IMessageBoxService` | |
| Popover | `FlarePopover` | |
| Toolbar | `FlareToolbar` | |
| Tab bar | `FlareBottomNav` | |
| Sidebar (NavigationSplitView) | `FlareLayoutDrawer` + `FlareNavMenu` | |
| List / Section | `FlareList`, `FlareCard` | |
| Disclosure group | `FlareAccordion`, `FlareCollapse` | |
| Progress view / Gauge | `FlareProgress`, `FlareMeter` | |
| Badge | `FlareBadge` | |
| Date picker | `FlareDatePicker` | |

## Что тема меняет помимо цвета

- **Полупрозрачные капсулы с преломляющей кромкой** - световой градиентный блик поверх
  полупрозрачной заливки, плюс внутренний блик и мягкая цветная тень.
- **Никакого backdrop blur.** Это осознанно и ради производительности: глубина читается из слоёв и
  блика, а не из `backdrop-filter`, который стоит одного прохода композитора на каждую поверхность.
- **Блик И ЕСТЬ обратная связь** - тема ставит токены слоя состояния в `transparent` и выражает
  наведение и нажатие насыщенностью и яркостью, а не наложением.
- **Жидкое сжатие при нажатии** (`scale(0.96)`) у всех вариантов кнопки.

Требуется `Flare.Components`. Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
