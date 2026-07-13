# FAB (Fluent UI 2)

**Fluent UI 2 не имеет прямого аналога FAB (плавающая кнопка действия - это чисто
Material-компонент); ниже - маппинг на ближайший примитив: круглая primary-кнопка
(`@fluentui/react-button`, круглая форма `circular`). Значения приблизительны и
требуют сверки с Figma.**

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-button`
(`useButtonStyles.styles.ts`, ветка `primary` + shape `circular` + icon-only sizes).
В колонке `Token Name` - реальный Fluent alias-токен, в колонке `Value` - его разрешённое
значение. Состояние `Pressed` в CSS соответствует `:hover:active, :active:focus-visible`.
Fluent-кнопка НЕ имеет собственного токена возвышения (elevation) - секция `Elevation`
ниже добавлена как приближение FAB через `shadow8`/`shadow16` и подлежит сверке с Figma.

# FAB - Color - Primary (круглая) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB container color | `colorBrandBackground` | #0f6cbd |
| FAB outline color | (transparent) | transparent |
| FAB label color | `colorNeutralForegroundOnBrand` | #ffffff |
| FAB icon color | `colorNeutralForegroundOnBrand` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB hovered container color | `colorBrandBackgroundHover` | #115ea3 |
| FAB hovered label color | `colorNeutralForegroundOnBrand` | #ffffff |
| FAB hovered icon color | `colorNeutralForegroundOnBrand` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB pressed container color | `colorBrandBackgroundPressed` | #0c3b5e |
| FAB pressed label color | `colorNeutralForegroundOnBrand` | #ffffff |
| FAB pressed icon color | `colorNeutralForegroundOnBrand` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| FAB disabled outline color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| FAB disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| FAB disabled icon color | `colorNeutralForegroundDisabled` | #bdbdbd |

# FAB - Color - Primary (круглая) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB container color | `colorBrandBackground` | #115ea3 |
| FAB outline color | (transparent) | transparent |
| FAB label color | `colorNeutralForegroundOnBrand` | #ffffff |
| FAB icon color | `colorNeutralForegroundOnBrand` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB hovered container color | `colorBrandBackgroundHover` | #0f6cbd |
| FAB hovered label color | `colorNeutralForegroundOnBrand` | #ffffff |
| FAB hovered icon color | `colorNeutralForegroundOnBrand` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB pressed container color | `colorBrandBackgroundPressed` | #0c3b5e |
| FAB pressed label color | `colorNeutralForegroundOnBrand` | #ffffff |
| FAB pressed icon color | `colorNeutralForegroundOnBrand` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| FAB disabled outline color | `colorNeutralStrokeDisabled` | #424242 |
| FAB disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| FAB disabled icon color | `colorNeutralForegroundDisabled` | #5c5c5c |

# FAB - Elevation (контекст: Default, Light/Dark)

Приближение: у Fluent-кнопки нет токена возвышения. FAB как поднятый над контентом
элемент отображается через box-shadow `shadow8` в покое и `shadow16` при наведении
(нужна сверка с Figma).

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| FAB resting elevation | `shadow8` | 0 0 2px rgba(0,0,0,0.12), 0 4px 8px rgba(0,0,0,0.14) | 0 0 2px rgba(0,0,0,0.24), 0 4px 8px rgba(0,0,0,0.28) |
| FAB hovered/raised elevation | `shadow16` | 0 0 2px rgba(0,0,0,0.12), 0 8px 16px rgba(0,0,0,0.14) | 0 0 2px rgba(0,0,0,0.24), 0 8px 16px rgba(0,0,0,0.28) |

# FAB - Focus (контекст: Default, Light/Dark)

Fluent primary-фокус - двойное кольцо через inset box-shadow: внутренний штрих цвета
`colorStrokeFocus2`, внутренний офсет-штрих цвета `colorNeutralForegroundOnBrand`, плюс
`shadow2`. Радиус фокус-индикатора круглый (`borderRadiusCircular`).

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| FAB focus outer stroke color | `colorStrokeFocus2` | #000000 | #ffffff |
| FAB focus inner stroke color | `colorNeutralForegroundOnBrand` | #ffffff | #ffffff |
| FAB focus inner stroke width | `strokeWidthThin` | 1px | 1px |
| FAB focus offset stroke width | `strokeWidthThick` | 2px | 2px |
| FAB focus outer ambient shadow | `shadow2` | 0 0 2px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.14) | 0 0 2px rgba(0,0,0,0.24), 0 1px 2px rgba(0,0,0,0.28) |
| FAB focus indicator radius | `borderRadiusCircular` | 10000px | 10000px |

# FAB - Shape

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB corner radius (круглая) | `borderRadiusCircular` | 10000px |

# FAB - Size - Small

Маппинг на icon-only small кнопку.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB small container size (min/max width) | (literal) | 24px |
| FAB small padding | (literal `buttonSpacingSmallWithIcon`) | 1px |
| FAB small icon size | (literal) | 20px |
| FAB small corner radius | `borderRadiusCircular` | 10000px |

# FAB - Size - Medium (default)

Маппинг на icon-only medium кнопку.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB medium container size (min/max width) | (literal) | 32px |
| FAB medium padding | (literal `buttonSpacingMedium`) | 5px |
| FAB medium icon size | (literal) | 20px |
| FAB medium corner radius | `borderRadiusCircular` | 10000px |

# FAB - Size - Large

Маппинг на icon-only large кнопку.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB large container size (min/max width) | (literal) | 40px |
| FAB large padding | (literal `buttonSpacingLargeWithIcon`) | 7px |
| FAB large icon size | (literal) | 24px |
| FAB large corner radius | `borderRadiusCircular` | 10000px |

# FAB - Motion

| Display Name | Token Name | Value |
|--------------|------------|-------|
| FAB transition duration | `durationFaster` | 100ms |
| FAB transition easing | `curveEasyEase` | cubic-bezier(0.33, 0, 0.67, 1) |
| FAB transition properties | (literal) | background, border, color |
