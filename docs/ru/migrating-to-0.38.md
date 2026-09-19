# Переход на 0.38

Все изменения механические: переименование, добавленный токен или выставленный параметр. Ничего не
требует переделки, и ничего не меняет вид компонента, кроме случаев, где это сказано прямо.

**Если вы используете встроенную тему и не пишете CSS по классам Flare - делать нечего.** Все ниже
адресовано двум группам: тем, кто написал свою `ITheme`, и тем, чьи стили или код называют классы Flare
напрямую.

---

## 1. Пользовательские темы: новые обязательные токены

У `required`-членов нет значений по умолчанию, поэтому каждый из них - ошибка компиляции, пока вы не
зададите значение. Встроенные темы задают те же числа, что держало ядро, так что копирование
воспроизводит ровно прежний вид.

### `DesignTokens.Layer` - лестница слоев (новая запись)

```csharp
Layer = new LayerTokens
{
    Chrome   = "100",   // закрепленный хром оболочки: шапка, нижняя навигация, кнопка наверх
    Drawer   = "200",   // дровер и его scrim
    Dropdown = "300",   // панель, привязанная к контролу: select, меню, popover
    Modal    = "400",   // диалог, message box, confirm
    Toast    = "500",   // snackbar
    Tooltip  = "600",
    Drag     = "700",   // то, что держит указатель
},
```

Порядок возрастающий, и между ступенями нужен запас: компонент поднимает свою часть через
`calc(var(--flare-z-drawer) + 1)`, и это смещение не должно дотянуться до следующей ступени. И то и
другое проверяет `LayerLadderTests` по каждой зарегистрированной теме, так что ошибка уронит сборку, а
не страницу.

### `TypographyTokens.MonoFont`

```csharp
MonoFont = "monospace, monospace",   // или свой: "\"JetBrains Mono\", monospace"
```

Настоящее семейство пишите перед родовым. Родовое в одиночку заставляет несколько движков взять свой
«monospace default size» - текст рисуется размером, который никто не измерял; поэтому запасное значение
и дублирует его.

### `NavTokens` - пять цветов

```csharp
ActiveColor    = "var(--flare-color-on-secondary-container)",  // см. примечание ниже
ItemColor      = "var(--flare-color-on-surface-variant)",
ItemHoverColor = "var(--flare-color-on-surface)",
GroupColor     = "var(--flare-color-on-surface-variant)",
MetaColor      = "var(--flare-color-on-surface-variant)",
```

**`ActiveColor` выбирайте под свой `ActiveIndicator`, а не копируйте строку выше.** Если индикатор -
тонированная пилюля (`secondary-container`), то `on-secondary-container` верен. Если индикатор `none` и
активный пункт отмечен боковой чертой, подпись читается на плоскости дровера и хочет `on-surface`.
Раньше ядро фиксировало здесь container-роль для всех - ровно поэтому токен и появился.

### `LayoutTokens` - семь членов

```csharp
ShellBg   = "var(--flare-color-background)",
DrawerBg  = "var(--flare-color-surface-container-low)",
RailBg    = "var(--flare-layout-drawer-bg)",   // укажите на дровер, чтобы это была одна поверхность
ContentBg = "transparent",
// Тени нет: 0 не рисует ничего. Знак задает ядро, поэтому край указывать не нужно.
DrawerShadowOffset = "0px",
DrawerShadowBlur   = "0px",
DrawerShadowColor  = "transparent",
```

### `ColorScheme.SurfaceContainerLowest`

Шестая поверхностная плоскость, самая дальняя от контента.

```csharp
SurfaceContainerLowest = "#FFFFFF",   // светлая
SurfaceContainerLowest = "#0F0D13",   // темная (Material 3 N4)
```

Она отличается от `Background`, который остается цветом документа, а не панели.

### `CheckboxTokens` / `RadioTokens` - шкала вместо `Size`

`Size` удален. Его значение становится `SizeMd`, а четыре шага, которые ядро держало литералами, теперь
ваши:

```csharp
// Checkbox
SizeXs = "0.875rem", SizeSm = "1rem", SizeMd = <прежний Size>, SizeLg = "1.375rem", SizeXl = "1.625rem",
StateLayerSize = "2.5rem",

// Radio
SizeXs = "1rem", SizeSm = "1.125rem", SizeMd = <прежний Size>, SizeLg = "1.5rem", SizeXl = "1.75rem",
StateLayerSize = "2.5rem",
```

### `ChipTokens` - типографика метки

```csharp
LabelFont    = "var(--flare-typescale-label-large-font)",
LabelWeight  = "inherit",   // чип не задавал начертание и брал его у окружения
LabelSpacing = "normal",    // то же про трекинг
LabelSizeXs  = "var(--flare-typescale-label-small-size)",
LabelSizeSm  = "var(--flare-typescale-label-small-size)",
LabelSizeMd  = "var(--flare-typescale-label-large-size)",
LabelSizeLg  = "var(--flare-typescale-title-small-size)",
LabelSizeXl  = "var(--flare-typescale-title-medium-size)",
```

`inherit`/`normal`, а не значения label-large, намеренно: подставив сюда начертание темы, вы сделаете
все чипы жирнее, чем они были.

### Удаленные токены

| Удалено | Чем заменено |
| :-- | :-- |
| `BottomNavTokens.ZIndex` | ступень `Chrome` у `LayerTokens` |
| `CheckboxTokens.Size` | `CheckboxTokens.SizeMd` |
| `RadioTokens.Size` | `RadioTokens.SizeMd` |

---

## 2. CSS-переменные

| Было | Стало |
| :-- | :-- |
| `--flare-z-appbar` | `--flare-z-chrome` |
| `--flare-bottom-nav-z-index` | `--flare-z-chrome` |
| `--flare-checkbox-size` | `--flare-checkbox-size-md` |
| `--flare-radio-size` | `--flare-radio-size-md` |

Если вы переопределяли их в CSS приложения - перенацельте. Новые имена лежат на лестнице или на шкале,
поэтому заданная одна ступень больше не оставляет молча остальные там, где их положило ядро.

---

## 3. `Css.Classes`: все размерные константы теперь `Size*`

Схем было три, осталась одна. Переименуйте в своем коде и стилях:

```
Css.Classes.Button.Md        ->  Css.Classes.Button.SizeMd
Css.Classes.Switch.Sm        ->  Css.Classes.Switch.SizeSm
Css.Classes.Avatar.Lg        ->  Css.Classes.Avatar.SizeLg
...и так же для SplitButton, Chip, Fab, Meter, Progress, Slider, Pagination,
   Badge, Checkbox, Radio, Rating, Input
```

`Container` намеренно не тронут: его `Xs`..`Xl` - максимальные ширины брейкпоинтов, а не размер
контрола. `Utility` так же сохраняет свои шаги отступов.

**Сами имена CSS-классов не менялись** - `flare-btn--md` остался `flare-btn--md`. Переименованы только
константы C#, поэтому таблица стилей, где класс написан строкой, правки не требует.

### У среднего размера теперь есть класс

Каждая шкала выдает свою ступень, включая ступень по умолчанию. Селектор, который считал, что
«по умолчанию» - это отсутствие класса, нужно поправить:

```css
/* было: «размер по умолчанию» означало «нет класса размера» */
.flare-checkbox:not(.flare-checkbox--xs):not(.flare-checkbox--sm) { … }

/* стало */
.flare-checkbox--md { … }
```

---

## 4. Изменения поведения, о которых стоит знать

### `FlareChip` - тег, если к нему ничего не подключено

Чип без `OnClick`, без `SelectedChanged` и вне `FlareChipGroup` больше не рендерит `role="button"`,
точку табуляции и слои hover/focus/pressed. Если обработчик приходит через splatting атрибутов, а не
параметром, скажите это явно:

```razor
<FlareChip Interaction="ChipInteraction.Button" @onclick="Handle">…</FlareChip>
```

Корневой элемент теперь `<span>`, а не `<div>`. Рисовался он `inline-flex` и раньше, так что визуально
ничего не сдвинулось, - но селектор `div.flare-chip` перестанет совпадать.

### `FlareRichTextEditor` фильтрует свое значение

`Value` и вставленная разметка проходят через список разрешенного прежде, чем попасть на страницу.
Форматирование, списки, заголовки, цитаты, ссылки и картинки остаются; скрипты, обработчики событий и
небезопасные схемы URL - нет. Если разметку делает само приложение и пользователь к ней не подходил:

```razor
<FlareRichTextEditor @bind-Value="_html" Sanitize="false" />
```

Делайте это, только если можете утверждать, что разметке можно доверять. Редактор, значение которого
проходит круг через хранилище, - именно то место, где протащенный внутрь скрипт исполнится снова при
каждом следующем показе.

### Командные кнопки больше не отправляют форму

У 37 элементов `<button>` внутри компонентов Flare не было `type`, а это в HTML означает `submit`. Если
вы полагались на то, что стрелка календаря или страница пагинации отправит форму вокруг - намеренно или
нет, - этого больше не происходит. `FlareButton Type="ButtonType.Submit"` отправляет по-прежнему.

### Разделитель дровера оболочки стал логическим

`border-inline-end` вместо `border-right`. В RTL он теперь оказывается на краю, обращенном к контенту,
то есть там, где ему и место.
