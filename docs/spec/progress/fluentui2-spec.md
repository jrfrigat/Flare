# Progress (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из `@fluentui/react-progress`
(`useProgressBarStyles.styles.ts`): слоты `root` (track) и `bar` (fill). Линейный
`ProgressBar` - трек `colorNeutralBackground6` + заливка `colorCompoundBrandBackground` (brand по
умолчанию), с интент-цветами (success/warning/error) в determinate-режиме.
Круговой прогресс в Fluent 2 - это `Spinner` (`@fluentui/react-spinner`); см. секцию в конце.

# Progress - Color - Bar (контекст: Default, Light)

## Track (root)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Progress track color | `colorNeutralBackground6` | #e6e6e6 |
| Progress track color (forced-colors) | (literal) | CanvasText |

## Bar (fill)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Progress bar color (brand, default) | `colorCompoundBrandBackground` | #0f6cbd |
| Progress bar color (success) | `colorPaletteGreenBackground3` | #107c10 |
| Progress bar color (warning) | `colorPaletteDarkOrangeBackground3` | #da3b01 |
| Progress bar color (error) | `colorPaletteRedBackground3` | #d13438 |
| Progress bar color (forced-colors) | (literal) | Highlight |
| Progress indeterminate gradient edge | `colorNeutralBackground6` | #e6e6e6 |
| Progress indeterminate gradient center | `colorTransparentBackground` | transparent |

# Progress - Color - Bar (контекст: Default, Dark)

## Track (root)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Progress track color | `colorNeutralBackground6` | #333333 |
| Progress track color (forced-colors) | (literal) | CanvasText |

## Bar (fill)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Progress bar color (brand, default) | `colorCompoundBrandBackground` | #479ef5 |
| Progress bar color (success) | `colorPaletteGreenBackground3` | #107c10 |
| Progress bar color (warning) | `colorPaletteDarkOrangeBackground3` | #da3b01 |
| Progress bar color (error) | `colorPaletteRedBackground3` | #d13438 |
| Progress bar color (forced-colors) | (literal) | Highlight |
| Progress indeterminate gradient edge | `colorNeutralBackground6` | #333333 |
| Progress indeterminate gradient center | `colorTransparentBackground` | transparent |

# Progress - Size (thickness / shape)

Тема-независимо.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Progress thickness (medium, default) | (literal) | 2px |
| Progress thickness (large) | (literal) | 4px |
| Progress corner radius (rounded, default) | `borderRadiusMedium` | 4px |
| Progress corner radius (square) | `borderRadiusNone` | 0 |
| Progress indeterminate max width | (literal) | 33% |

# Progress - Motion

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Progress determinate transition property | (literal) | width |
| Progress determinate transition duration | (literal) | 0.3s |
| Progress determinate transition easing | (literal) | ease |

# Spinner (круговой прогресс) - Color (контекст: Default, Light)

`Spinner` (`@fluentui/react-spinner`, `useSpinnerStyles.styles.ts`) - кольцо-маска: фон-трек
`colorBrandStroke2Contrast`, дуга `colorBrandStroke1`; inverted-вариант для тёмных подложек.

## Default

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Spinner track color | `colorBrandStroke2Contrast` | #b4d6fa |
| Spinner arc color | `colorBrandStroke1` | #0f6cbd |
| Spinner label color | (наследует текст) | inherit |

## Inverted (appearance = inverted)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Spinner track color | `colorNeutralStrokeAlpha2` | rgba(255, 255, 255, 0.2) |
| Spinner arc color | `colorNeutralStrokeOnBrand2` | #ffffff |
| Spinner label color | `colorNeutralForegroundStaticInverted` | #ffffff |

## Forced-colors

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Spinner track color | (literal) | HighlightText |
| Spinner arc color | (literal) | Highlight |

# Spinner - Color (контекст: Default, Dark)

## Default

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Spinner track color | `colorBrandStroke2Contrast` | #0e4775 |
| Spinner arc color | `colorBrandStroke1` | #479ef5 |
| Spinner label color | (наследует текст) | inherit |

## Inverted (appearance = inverted)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Spinner track color | `colorNeutralStrokeAlpha2` | rgba(255, 255, 255, 0.2) |
| Spinner arc color | `colorNeutralStrokeOnBrand2` | #ffffff |
| Spinner label color | `colorNeutralForegroundStaticInverted` | #ffffff |

# Spinner - Size (diameter / stroke)

Тема-независимо.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Spinner size (extra-tiny) | (literal) | 16px |
| Spinner size (tiny) | (literal) | 20px |
| Spinner size (extra-small) | (literal) | 24px |
| Spinner size (small) | (literal) | 28px |
| Spinner size (medium, default) | (literal) | 32px |
| Spinner size (large) | (literal) | 36px |
| Spinner size (extra-large) | (literal) | 40px |
| Spinner size (huge) | (literal) | 44px |
| Spinner stroke width (extra-tiny..small) | `strokeWidthThick` | 2px |
| Spinner stroke width (medium..extra-large) | `strokeWidthThicker` | 3px |
| Spinner stroke width (huge) | `strokeWidthThickest` | 4px |

# Spinner - Motion

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Spinner rotation duration | (literal) | 1.5s |
| Spinner rotation iteration | (literal) | infinite |
| Spinner rotation easing (ring) | (literal) | linear |
| Spinner tail easing | `curveEasyEase` | cubic-bezier(0.33, 0, 0.67, 1) |
| Spinner reduced-motion duration | (literal) | 1.8s |
