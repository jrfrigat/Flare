# Flare - Конвенции кода компонентов

> [English version ->](../en/component-conventions.md) - [README](../../README.ru.md)

Единый стиль кода для **всех** компонентов. Канонический образец - **`FlareButton`**.
При создании или правке любого компонента приводи его архитектуру к этим правилам.

## 1. Архитектура CSS - глобальный бандл

### CSS живёт в `wwwroot/css/` (глобальный бандл), НЕ в scoped `*.razor.css`

Flare - это публикуемая NuGet-библиотека с токен-driven темизацией. Все стили компонентов
хранятся в **глобальном бандле** `src/Flare.Components/wwwroot/css/*.css`, подключаемом через
`flare-components.css`.

**Почему не scoped `*.razor.css`:**
- Scoped добавляет `[b-hash]` к каждому правилу, что повышает специфичность и мешает
  потребителю переопределять стили обычными `.flare-*` классами.
- Классы одного компонента часто используются в связанных компонентах
  (`.flare-avatar` в `FlareAvatarGroup`, `.flare-icon` в `FlareClipboard` и т.д.) - scoped сломает такие случаи.
- `*.cs`-компоненты (`FlareText`) к scoped CSS не подключаются.
- Глобальный бандл предсказуем, единообразен и соответствует стандарту дизайн-систем
  (MudBlazor, Fluent UI Blazor, Radzen и др.).

**Правила:**
- Один компонент (или связанная группа) - один CSS-файл в `wwwroot/css/`.
- Файл добавляется через `@import` в `flare-components.css`.
- Если правишь компонент - убедись, что его CSS-файл существует и добавлен в бандл.
- Примеры: `button.css` -> `FlareButton`, `menu.css`/`menuitem.css`/`menugroup.css` -> Menu-группа.
- Theme-специфичные доводки (MD3 vs Fluent) - в `src/Flare.Theme.*/wwwroot/css/components/*.css`.

## 2. Токены - через систему токенов (никаких "зашитых" цветов/чисел)

### Полный путь добавления токена компонента

| Что | Где |
| :-- | :-- |
| Запись токенов (per-theme) | `src/Flare.Abstractions/Tokens/Components/<Comp>Tokens.cs` |
| Имена CSS-переменных | `src/Flare.Abstractions/Css/Tokens/<Comp>Tokens.cs` (namespace `Flare.Css.Tokens`, holder-классы `Css.Tokens.<Comp>.*`; база/helper - `Css.Tokens.Vars`) |
| Эмиссия переменных | `src/Flare.Theming/Services/CssVarMap.cs` |
| Значения для MD3 | `src/Flare.Theme.MaterialDesign3Expressive/MaterialDesignTokens.cs` (+ dark theme) |
| Значения для Fluent | `src/Flare.Theme.FluentUI2/FluentUI2Tokens.cs` (+ dark theme) |
| CSS-классы | `src/Flare.Abstractions/Css/Classes/<Comp>.cs` (namespace `Flare.Css.Classes`, holder-классы `Css.Classes.<Comp>.*`) |

> **Две системы токенов, связанные через `[CssVar]`.** `Css/Tokens/*` хранит константы ИМЁН переменных;
> записи в `Tokens/*` хранят ЗНАЧЕНИЯ под каждую тему. Помечай каждое скалярное свойство-значение
> атрибутом `[CssVar(Css.Tokens.<Comp>.<Prop>)]` (образец - `ButtonTokens`), чтобы связь значение->имя
> была декларативной; drift-тест `CssVarAttributeTests` падает, если помеченное имя не эмитится
> `CssVarMap.FlattenDesign`. Составные токены (поугловые радиусы, типографика) остаются только во flatten.

### Правила токенов
- Опирайся на семантические токены: `--flare-color-*`, `--flare-shape-*`, `--flare-typescale-*`,
  `--flare-state-*`, `--flare-elevation-*`, `--flare-motion-*`.
- **Никаких хардкоднутых значений** в CSS - только `var(--flare-*)`.
- Один набор CSS-правил работает и в MD3, и в Fluent - различия задаёт тема через значения токенов.
- Компонент-специфичные токены (геометрия, типографика, состояния) выносятся в отдельный
  `XxxTokens.cs` (см. `ButtonTokens`, `MenuTokens` как образцы).
- При добавлении нового компонента - обязательно создай `XxxTokens.cs` и holder `Css.Tokens.Xxx`.

### Уже реализованные токен-записи
Полный набор живёт в `src/Flare.Abstractions/Tokens/Components/`: Alert, Avatar, Badge, Button, Card,
Checkbox, Chip, DataGrid, Dialog, Drawer, Fab, Input, Menu, Popover, Progress, Radio, Select,
Slider, Snackbar, SplitButton, Switch, TableOfContents, Tabs, ToggleButton, Tooltip.

### 2.1 Единая система цвета - `FlareColor`
Любой публичный параметр выбора цвета компонента - **только** `FlareColor` (одно имя везде:
`Color`). Не вводи отдельные enum-цвета (`LinkColor`, `TimelineColor`, ...) и не дублируй
`Color` + `CustomColor` - всё это объединено в `FlareColor`.

`FlareColor` - это `readonly record struct`, который держит **либо** семантическую роль
(`FlareColorRole`: Default/Primary/Secondary/Tertiary/Success/Warning/Error/Info/OnSurface/
OnSurfaceVariant), **либо** произвольную CSS-строку:
- роль -> общий кэшируемый класс `flare-color-{role}` (быстрый путь, `Color.CssClass`);
- кастом -> инлайн-токены `--fc-*` на элементе (`Color.IsCustom` / `Color.Value`,
  значение санитизируется через `CssValidator.SanitizeColor`);
- `FlareColor.Default` -> ни класса, ни токенов (компонент берёт свой fallback из CSS).

**Локальные токены роли** (выставляются классом `flare-color-*` или инлайном):
`--fc-main` (акцент), `--fc-on` (контраст на main), `--fc-container` (тональный фон),
`--fc-on-container` (текст на тональном). Компонент читает только нужное подмножество
через `var(--fc-*, <fallback>)`:

| Сколько токенов | Компоненты | Что используют |
| :-- | :-- | :-- |
| 1 (`--fc-main`) | Text, Icon, Rating, Progress, Input, Link, Timeline | акцент/текст/бордер |
| 2 (`--fc-main` + `--fc-on`) | Badge, Pagination, Chip | заливка + контраст |
| 2 контейнер (`--fc-container` + `--fc-on-container`) | Avatar, FAB, Calendar event | тональный фон + текст |
| 4 (все) | Button | filled/tonal/outlined/text варианты |

Паттерн в компоненте:
```csharp
private string  _colorClass => Color.CssClass ?? string.Empty;          // role -> class
private string? _colorStyle => Color.IsCustom                            // custom -> inline
    ? $"{Css.Tokens.LocalColor.Main}:{Color.Value};{Css.Tokens.LocalColor.On}:{FlareColorResolver.OnColor(Color.Value!)};"
    : null;
```
CSS компонента - **никаких per-color классов** (`flare-x--primary` и т.п.); только
`var(--fc-*, <семантический fallback>)`. Роль-классы определены один раз в
`wwwroot/css/color-roles.css`.

## 3. Минимум JS
- Эффекты (ripple, морфинг формы, анимации появления) - средствами CSS.
- **JS для анимаций не применять.** Если эффект невозможен без JS - задокументировать как ограничение,
  сделать максимально близкое CSS-приближение.

## 4. XML-документация (для авто-генерации API)
- **Полностью** документируй XML-комментариями все public типы, `[Parameter]`-свойства, методы,
  члены enum и т.д. (используется для автогенерации API в Gallery).
- Минимум: `<summary>` на каждом public-члене; `<param>`/`<returns>` у методов.
- Стиль - как в `FlareButton.razor` (каждый `[Parameter]` снабжён `<summary>`).

## 5. Прочее
- Сборка и тесты должны проходить; проверяй компонент визуально в `Flare.Gallery` (не в legacy-примере `Flare.Legacy`).
- Не оставляй устаревший код (мёртвые enum/классы) - удаляй при обнаружении.
