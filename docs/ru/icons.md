# Иконки

Иконки Flare - это **полиморфный тип-значение**, а не один компонент. `FlareIcon` - абстрактный дескриптор;
каждый провайдер - это конкретный `FlareIcon`, который подставляется в любой параметр типа `FlareIcon`
(`Icon` у кнопки, элемент навигации, адорнмент поля, ...) или рисуется отдельно через `<FlareIconView>`.

По умолчанию всё - **inline-SVG**: без иконочного шрифта, без сетевого запроса, без FOUT, и независимо от темы
(иконки наследуют `currentColor`).

Все иконочные типы - в namespace **`Flare.Icons`**: `FlareIcon`, `FlareSvgIcon` и встроенный набор `FlareIcons`
поставляются в небольшом пакете `Flare.Icons`; пакеты провайдеров ниже добавляют свои каталоги в тот же
namespace. Добавьте `using Flare.Icons` (или `@using Flare.Icons`). Рендер-компонент `FlareIconView` - в
`Flare.Components`.

## Встроенный набор: `FlareIcons`

Пакет `Flare.Icons` поставляет собственный SVG-набор без внешних зависимостей (`FlareIcons`, ~90 иконок),
которым рисуется дефолтный хром компонентов (шевроны, close, сортировка, тогглы дерева, ...). `Flare.Components`
зависит от него, так что всё работает из коробки без дополнительных пакетов.

```razor
<FlareIconView Value="@FlareIcons.Home" />
<FlareIconButton Icon="@FlareIcons.Settings" AriaLabel="Settings" />
```

- Всегда ссылайтесь на иконку **типизированным членом** - поиска по имени-строке нет. Это намеренно: поиск по
  имени убил бы тримминг (пришлось бы держать весь каталог) и добавил бы рантайм-стоимость.
- `FlareIcons.All` и `FlareIcons.Find(id)` остаются как явный API каталога для встроенного набора (например,
  для страницы-браузера иконок); провайдерские (Material/Fluent) иконки они не резолвят.

## Пакеты провайдеров

Ядро не зависит ни от одного стороннего набора иконок. Подключайте только нужный пакет; каждый опционален.

| Пакет | Тип / каталог | Доставка |
| :-- | :-- | :-- |
| `Flare.Icons.MaterialDesign3.Svg` | `MaterialDesign3Icons.Regular.*` / `.Filled.*` (3894) | inline-SVG |
| `Flare.Icons.MaterialDesign2.Svg` | `MaterialDesign2Icons.*` (2122, filled) | inline-SVG |
| `Flare.Icons.FluentUI.Svg` | `FluentUIIcons.Regular.*` / `.Filled.*` (~5000) | inline-SVG |
| `Flare.Icons.MaterialDesign3.Symbols` | `FlareMaterialDesign3Icon` | вариативный шрифт Material Symbols |
| `Flare.Icons.MaterialDesign2.Symbols` | `FlareMaterialDesign2Icon` | шрифт Material Icons |
| `Flare.Icons.FontAwesome.Symbols` | `FlareFontAwesomeIcon` | шрифт Font Awesome |

- `.Svg`-пакеты самодостаточны (SVG-графика встроена) - грузить в рантайме нечего.
- `.Symbols`-пакеты рисуют `<span>`/`<i>` с классом шрифта провайдера; **шрифт подключает хост-приложение**
  (например, `<link>` на Google Fonts для Material Symbols или таблица стилей Font Awesome).

```razor
@* SVG-каталоги - самодостаточны *@
<FlareIconView Value="@MaterialDesign3Icons.Regular.Home" />
<FlareIconView Value="@MaterialDesign3Icons.Filled.Home" />   @* та же иконка, залитая *@
<FlareIconButton Icon="@FluentUIIcons.Regular.Settings" AriaLabel="Settings" />

@* Шрифтовые провайдеры - шрифт грузит хост; оси/стили - опции провайдера *@
<FlareIconView Value="@(new FlareMaterialDesign3Icon { Name = "home", Fill = true, Weight = 500 })" />
<FlareIconView Value="@(new FlareFontAwesomeIcon { Name = "house", Variant = FontAwesomeVariant.Solid })" />
```

`.Svg`-версия набора предпочтительна (самодостаточна, темизируема, без FOUT); `.Symbols` берите только когда вы
и так грузите этот шрифт или вам нужны именно оси вариативного шрифта.

Что такое "Fluent UI 2"? У Microsoft один набор иконок - **Fluent UI System Icons** - поставляется здесь как
`FluentUIIcons`. Это и есть набор иконок Fluent 2; отдельной библиотеки "FluentUI2" не существует.

## Своя SVG

Передайте любую SVG напрямую - path-данные или полную внутреннюю разметку - через `FlareSvgIcon`:

```razor
<FlareIconView Value="@(new FlareSvgIcon { Data = "M3 18h18v-2H3v2z" })" />
<FlareIconView Value="@(new FlareSvgIcon { Data = "<path .../><path .../>", ViewBox = "0 -960 960 960" })" />
```

> Безопасность: `FlareSvgIcon.Data` (и `Name` любого шрифтового провайдера) выводится дословно. Передавайте
> только доверенные значения, написанные разработчиком - никогда не пользовательский ввод.

## Размер и цвет

`FlareIconView` и каждый `FlareIcon` принимают `Size` (любая CSS-длина) или `SizePx`, и `Color` (роль
`FlareColor` или кастомный цвет). Иначе иконки наследуют `currentColor` и совпадают с окружающим текстом.

```razor
<FlareIconView Value="@FlareIcons.Star" SizePx="32" Color="FlareColor.Primary" />
<FlareIconView Value="@FlareIcons.Bolt" Size="3rem" Color="@FlareColor.Custom("#FFB300")" />
```

## Производительность: приезжают только используемые иконки

Каждая иконка каталога - отдельный static-член, а SVG-пакеты помечены `IsTrimmable`. Поэтому
**trimmed-публикация Blazor WebAssembly** (дефолт в Release) выкидывает все члены каталога, на которые вы не
ссылаетесь - вы платите только за реально используемые иконки.

- **Ссылайтесь на иконки типизированным членом** (`MaterialDesign3Icons.Regular.Home`), а не строкой.
  Static-член отслеживается IL-линкером; строковое имя - нет.
- Не "рутьте" весь каталог из всегда-загруженного кода (например, страница "показать все иконки", которая
  перечисляет тип рефлексией) - это удержит весь набор. Питайте такие страницы явным списком типизированных
  членов.

Замер на Flare Gallery: он ссылается на ~160 из 3894 Material Symbols, и trimmed-публикация ужимает
`Flare.Icons.MaterialDesign3.Svg` с **8.9 MB до ~180 KB**. Пакет, на который вы не ссылаетесь (например,
`FluentUI.Svg` в этой сборке), не приезжает вообще.
