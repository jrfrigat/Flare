# Sheet (Fluent UI 2)

**У Fluent UI 2 нет компонента "Sheet"; ближайший аналог - `Drawer` (`@fluentui/react-drawer`),
который покрывает и bottom-sheet, и side-sheet.** Значения разрешены из `@fluentui/tokens`
(webLightTheme / webDarkTheme, v1.0.0-alpha.23); привязки слот/состояние -> alias-токен взяты
из `useDrawerBaseStyles.styles.ts` (общая база), `useOverlayDrawerStyles.styles.ts`
(overlay + backdrop), `useInlineDrawerStyles.styles.ts` (inline + separator),
`useOverlayDrawerSurfaceStyles.styles.ts` (backdrop). Drawer поддерживает позиции
`start` / `end` / `bottom` и режимы overlay (модальный, с backdrop) / inline (в потоке).

**Флаг:** в исходнике `react-drawer` поверхность НЕ задаёт токен тени (в отличие от
`DialogSurface`/`shadow64`) - глубина читается через backdrop + рамку. Для sheet-подобной
тени (bottom-sheet) сверьтесь с Figma.

# Sheet - Color - Surface (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Sheet surface container color | `colorNeutralBackground1` | #ffffff |
| Sheet surface text color | `colorNeutralForeground1` | #242424 |
| Sheet surface edge border color (start/end) | `colorTransparentStroke` | transparent |
| Sheet surface edge border width (start/end) | `strokeWidthThin` | 1px |
| Sheet inline separator color | `colorNeutralBackground3` | #f5f5f5 |
| Sheet inline separator width | (literal) | 1px |

## Backdrop (overlay mode)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Sheet backdrop color | `colorBackgroundOverlay` | rgba(0, 0, 0, 0.4) |
| Sheet nested/transparent backdrop color | `colorTransparentBackground` | transparent |

# Sheet - Color - Surface (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Sheet surface container color | `colorNeutralBackground1` | #292929 |
| Sheet surface text color | `colorNeutralForeground1` | #ffffff |
| Sheet surface edge border color (start/end) | `colorTransparentStroke` | transparent |
| Sheet surface edge border width (start/end) | `strokeWidthThin` | 1px |
| Sheet inline separator color | `colorNeutralBackground3` | #141414 |
| Sheet inline separator width | (literal) | 1px |

## Backdrop (overlay mode)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Sheet backdrop color | `colorBackgroundOverlay` | rgba(0, 0, 0, 0.5) |
| Sheet nested/transparent backdrop color | `colorTransparentBackground` | transparent |

# Sheet - Size - Side (position start / end)

Размеры не зависят от темы; ширина задаётся через CSS-var `--fui-Drawer--size`.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Sheet side width (small) | (literal) | 320px |
| Sheet side width (medium, default) | (literal) | 592px |
| Sheet side width (large) | (literal) | 940px |
| Sheet side width (full) | (literal) | 100vw |
| Sheet side max width | (literal) | 100vw |
| Sheet side max height | (literal) | 100vh |

# Sheet - Size - Bottom (position bottom)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Sheet bottom height (small) | (literal) | 320px |
| Sheet bottom height (medium, default) | (literal) | 592px |
| Sheet bottom height (large) | (literal) | 940px |
| Sheet bottom height (full) | (literal) | 100% |
| Sheet bottom width | (literal) | 100vw |

# Sheet - Typography - Header title (subtitle1)

`DrawerHeaderTitle` heading переиспользует `DialogTitle` -> `typographyStyles.subtitle1`
(тема-независимо). Отступ между заголовком и action - `spacingHorizontalS`.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Sheet header title font size | `fontSizeBase500` | 20px |
| Sheet header title font weight | `fontWeightSemibold` | 600 |
| Sheet header title line height | `lineHeightBase500` | 28px |
| Sheet header title column gap | `spacingHorizontalS` | 8px |
