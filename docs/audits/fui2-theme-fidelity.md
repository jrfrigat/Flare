# FluentUI2 theme fidelity audit (Flare vs Fluent UI 2 spec)

Сверка реализации темы `Flare.Theme.FluentUI2` с `docs/spec/<comp>/fluentui2-spec.md`
по всем осям, с упором на состояния. Дата: 2026-07-13. Значения спеки = `@fluentui/tokens`
1.0.0-alpha.23; реализация = `FluentUI2Tokens.cs` + `Flare.Components/wwwroot/css/*` +
`Flare.Theme.FluentUI2/wwwroot/css/components/*`.

Шкала: **OK** (модель+значение совпадают) / **APPROX** (та же идея, другой механизм/значение) /
**DEV** (значение/модель расходятся) / **GAP** (состояния/оси нет) / **N/A**.

> **ВАЖНО (поправка метода, 2026-07-13).** Первая версия этого аудита читала типизированные
> записи `*Tokens` и переоценила расхождения. Эффективный рендер зависит ещё от (а) словаря
> `Extended`, который применяется ПОСЛЕДНИМ в `CssVarMap.FlattenDesign` и перекрывает
> одноимённые CSS-переменные, и (б) того, какой ВАРИАНТ/СОСТОЯНИЕ в базовом CSS реально
> потребляет токен. Проверка по CSS/Extended показала, что как минимум **Switch и Tabs уже
> Fluent-корректны** (см. ниже). Значения, помеченные [verified], сверены с CSS-использованием;
> остальные - кандидаты, требующие проверки в рантайме (Gallery) до правок.

## Резюме (главное)

1. **rest-палитра верна.** Роли `--flare-color-*` в `LightColors/DarkColors` замаплены на точные
   значения Fluent: `Primary #0F6CBD` = colorBrandBackground, `Outline #D1D1D1` = colorNeutralStroke1,
   `Surface #FFFFFF`, `OnSurface #242424`, `SurfaceContainer* = #EBEBEB/#E0E0E0/#D6D6D6`,
   `SecondaryContainer #EBEBEB` = colorNeutralBackground1Selected. Rest-цвета почти везде = OK.

2. **Движок состояний - MD3-нативный, не Fluent (системно).** Из тем-оверрайдов CSS существует
   ТОЛЬКО `button.css`. Все остальные 27 компонентов применяют hover/pressed/focus через общий
   `::before` state-layer (opacity Hover 0.10 / Focus 0.10 / Pressed 0.12 / Selected 0.12), а disabled
   - через opacity всего элемента (0.40). Fluent 2 использует **дискретный цвет на состояние**
   (`colorNeutralBackground1Hover` = конкретный серый, `...Disabled` = плоская палитра). => для любого
   интерактивного компонента hover/pressed/disabled - **APPROX**, а не токен-в-токен.

3. **Дискретные state-цвета, что всё же заданы, выражены MD3-моделью.** `SelectedBg = secondary-container`,
   `ActiveColor = primary`, `DisabledBg = on-surface 4%`, `Disabled* = on-surface 12%/38% mix`. Значение
   иногда совпадает (secondary-container = #EBEBEB = Fluent selected), но модель - MD3.

4. **Остаточный MD3-Expressive след:**
   - **Switch [verified: ЛОЖНАЯ ТРЕВОГА].** Типизированный `SwitchTokens` держит MD3-значения
     (thumb 1.75rem на press, halo, focus 3px secondary), НО словарь `Extended` перекрывает те же
     CSS-переменные Fluent-значениями (pressed=normal, hover-shadow=none, focus=нейтральный
     double-ring) и применяется последним. Рантайм = **Fluent-корректный статичный свитч. OK.**
     Остаётся лишь code smell: типизированная запись содержит мёртвые MD3-значения.
   - **Tabs [verified: ЛОЖНАЯ ТРЕВОГА].** Дефолтные/underline табы в `tabs.css` рисуют активный
     таб как brand-индикатор (`--flare-tabs-active-color` = primary, border-bottom 3px). `SelectedBg`
     = secondary-container применяется ТОЛЬКО вариантом-пилюлей (segmented), где залитый фон уместен.
     Рантайм = **brand-индикатор. OK.**
   - **Slider [verified]** - MD3 state-halo 28px @ 6%/8% вокруг thumb реально рендерится
     (`slider.css` :hover/:active ::thumb). Fluent thumb halo не имеет. **APPROX** (мелко).

5. **Focus-ring непоследователен.** `Extended` задаёт корректный нейтральный double-ring Fluent
   (`FocusStrokeColor #000` / `FocusStrokeOuter #FFF`), и Button/Checkbox его используют. Но **Switch**
   (`FocusOutline 3px secondary`), **Menu** (`ItemFocusRingColor secondary 3px`), **Slider** используют
   3px `secondary`-кольцо (MD3-стиль), а не нейтральный 2px+внешний Fluent. **DEV** у этих.

6. **disabled везде = opacity 0.40 всего элемента** (или on-surface-mix), а Fluent = дискретная плоская
   палитра (`colorNeutralBackgroundDisabled #f0f0f0` / `Foreground #bdbdbd` / `Stroke #e0e0e0`).
   Поблёкший rest != плоский серый. **APPROX/DEV системно.**

## Матрица (по осям; акцент - состояния)

| Компонент | rest | hover | pressed | selected/checked | focus | disabled | geom | typo | motion |
|---|---|---|---|---|---|---|---|---|---|
| button | OK | APPROX(overlay) | APPROX(color-mix) | APPROX | DEV(non-filled=blue) | DEV(opacity) | OK | OK | OK |
| split-button | OK | APPROX | APPROX | APPROX | DEV | DEV | OK | OK | OK |
| toggle/button-group | OK | APPROX | APPROX | DEV(secondary-container) | DEV | APPROX | OK | OK | OK |
| fab | OK | APPROX | APPROX(color-mix) | N/A | DEV | DEV | OK(circular) | N/A | OK |
| checkbox | OK | GAP(halo killed, no discrete hover) | APPROX | OK(brand check) | OK(double-ring) | APPROX | OK | OK | N/A |
| radio | OK | GAP(halo killed) | APPROX | OK | OK | APPROX | OK | OK | N/A |
| switch [verified] | OK | OK(no halo) | OK(no grow) | OK(brand track) | OK(neutral ring) | APPROX(on-surface mix) | OK | N/A | OK |
| slider | OK | APPROX(28px halo) | APPROX | N/A | DEV(secondary) | APPROX | OK(thin rail) | N/A | OK |
| input | OK | APPROX | N/A | N/A | OK(2px brand underline) | APPROX(on-surface mix) | OK | OK | OK |
| search | OK | APPROX | N/A | N/A | OK | APPROX | OK | OK | OK |
| date-picker | OK | APPROX | APPROX | APPROX(day-cell) | OK | APPROX | OK | OK | OK |
| time-picker | OK | APPROX | APPROX | APPROX(option) | APPROX | APPROX | OK | OK | OK |
| menu | OK | APPROX(overlay) | APPROX | APPROX | DEV(3px secondary) | APPROX | OK | OK | OK |
| tabs [verified] | OK | APPROX | APPROX | OK(brand indicator; pill only on segmented) | APPROX | APPROX | OK | OK | OK |
| toolbar | OK | APPROX | APPROX | N/A | DEV | APPROX | OK | N/A | OK |
| nav | OK | APPROX | APPROX | OK(3px brand bar) | APPROX | APPROX | OK | OK | OK |
| list | OK | APPROX(overlay) | APPROX | APPROX | APPROX | APPROX | OK | OK | N/A |
| card | OK | APPROX | APPROX | APPROX | APPROX | APPROX | OK | OK | OK |
| chip | OK | APPROX | APPROX | OK(#EBEBEB=selected) | APPROX | APPROX | OK | OK | OK |
| carousel | OK | APPROX | APPROX | APPROX(nav dot) | APPROX | N/A | OK | N/A | APPROX |
| badge | OK | N/A | N/A | N/A | N/A | N/A | OK | OK | N/A |
| tooltip | OK(inverse) | N/A | N/A | N/A | N/A | N/A | OK | OK | OK |
| dialog | OK | N/A | N/A | N/A | N/A | N/A | OK | OK | OK |
| sheet(drawer) | OK | N/A | N/A | N/A | N/A | N/A | OK | N/A | OK |
| snackbar(toast) | OK | N/A | N/A | N/A | N/A | N/A | OK | OK | OK |
| progress | OK | N/A | N/A | N/A | N/A | N/A | OK(2px rail) | N/A | OK |
| divider | OK | N/A | N/A | N/A | N/A | N/A | OK | OK | N/A |
| app-bar | OK | APPROX(items) | APPROX | N/A | DEV | APPROX | OK | OK | OK |

## Топ расхождений (после CSS-проверки; приоритет фикса)

Отсортировано по достоверности. Switch и Tabs УБРАНЫ - проверка показала, что они уже Fluent-корректны.

1. **[verified] disabled = opacity вместо дискретной disabled-палитры** (системно, база). Fluent:
   bg #f0f0f0 / text #bdbdbd / border #e0e0e0. Сейчас: поблёкший rest @ 0.40. Крупнейший реальный отход.
2. **[verified] Дискретная state-модель Fluent не реализована** (системно, база): hover/pressed =
   полупрозрачный overlay, а не `colorNeutralBackground1Hover/Pressed`. Заметно на secondary/subtle
   кнопках, menu/list item, input. (Это большая архитектурная правка - трогает общий state-движок.)
3. **[verified] Button non-filled focus = синий (primary)** вместо нейтрального `colorStrokeFocus2`
   (`button.css` outline primary для outlined/tonal/text). Мелкий, локальный фикс.
4. **[verified] Slider thumb halo** (MD3 28px @ 6%/8%) - у Fluent нет. Мелкий.
5. **[verified] checkbox/radio hover** - halo убран (верно), но потемнения обводки на hover нет ->
   слабый hover-фидбек. Мелкий.
6. **Кандидаты (не проверены в рантайме):** Menu item focus 3px secondary; scrim 0.32 vs 0.40;
   menu/dialog тени `elevation-N` vs Fluent shadow16/64. Требуют проверки перед правкой.

## Что НЕ является дефектом Flare

- fab / app-bar / button-group: у Fluent 2 нет таких компонентов - спека сама помечена как приближение,
  так что «отход от спеки» тут ожидаем.
- rest-палитра, геометрия (радиусы, тонкие рельсы, компактные высоты), типографика (Segoe, размеры) и
  motion (длительности) - в основном соответствуют Fluent.
