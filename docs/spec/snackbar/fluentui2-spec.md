# Snackbar (Fluent UI 2)

**У Fluent UI 2 нет компонента "Snackbar"; ближайший аналог - `Toast` (`@fluentui/react-toast`),
всплывающее временное уведомление.** Значения разрешены из `@fluentui/tokens`
(webLightTheme / webDarkTheme, v1.0.0-alpha.23); привязки слот/состояние -> alias-токен взяты
из `useToastStyles.styles.ts` (root), `useToastTitleStyles.styles.ts` (title + media/icon + action),
`useToastBodyStyles.styles.ts` (body + subtitle). Toast - нейтральная поверхность
`colorNeutralBackground1` + тень `shadow8`; интент (success/warning/error/info) выражается
ЦВЕТОМ ИКОНКИ в title-media, а не фоном. Для инлайн-уведомления в потоке (не всплывающего) в
Fluent используется `MessageBar` - см. секцию в конце (там интент задаёт фон + рамку).

# Snackbar - Color - Surface (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Snackbar container color | `colorNeutralBackground1` | #ffffff |
| Snackbar text color | `colorNeutralForeground1` | #242424 |
| Snackbar border color | `colorTransparentStroke` | transparent |
| Snackbar border width | (literal) | 1px |
| Snackbar corner radius | `borderRadiusMedium` | 4px |
| Snackbar elevation (shadow) | `shadow8` | 0 0 2px rgba(0,0,0,0.12), 0 4px 8px rgba(0,0,0,0.14) |
| Snackbar title color | `colorNeutralForeground1` | #242424 |
| Snackbar action (link) color | `colorBrandForeground1` | #0f6cbd |
| Snackbar body color | `colorNeutralForeground1` | #242424 |
| Snackbar subtitle color | `colorNeutralForeground2` | #424242 |

## Inverted (backgroundAppearance = inverted)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Snackbar inverted container color | `colorNeutralBackgroundInverted` | #292929 |
| Snackbar inverted text color | `colorNeutralForegroundInverted2` | #ffffff |
| Snackbar inverted action color | `colorBrandForegroundInverted` | #479ef5 |

## Intent (icon color in title-media)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Snackbar success icon color | `colorStatusSuccessForeground1` | #0e700e |
| Snackbar error icon color | `colorStatusDangerForeground1` | #b10e1c |
| Snackbar warning icon color | `colorStatusWarningForeground1` | #bc4b09 |
| Snackbar info icon color | `colorNeutralForeground2` | #424242 |

# Snackbar - Color - Surface (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Snackbar container color | `colorNeutralBackground1` | #292929 |
| Snackbar text color | `colorNeutralForeground1` | #ffffff |
| Snackbar border color | `colorTransparentStroke` | transparent |
| Snackbar border width | (literal) | 1px |
| Snackbar corner radius | `borderRadiusMedium` | 4px |
| Snackbar elevation (shadow) | `shadow8` | 0 0 2px rgba(0,0,0,0.24), 0 4px 8px rgba(0,0,0,0.28) |
| Snackbar title color | `colorNeutralForeground1` | #ffffff |
| Snackbar action (link) color | `colorBrandForeground1` | #479ef5 |
| Snackbar body color | `colorNeutralForeground1` | #ffffff |
| Snackbar subtitle color | `colorNeutralForeground2` | #d6d6d6 |

## Inverted (backgroundAppearance = inverted)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Snackbar inverted container color | `colorNeutralBackgroundInverted` | #ffffff |
| Snackbar inverted text color | `colorNeutralForegroundInverted2` | #242424 |
| Snackbar inverted action color | `colorBrandForegroundInverted` | #0f6cbd |

## Intent (icon color in title-media)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Snackbar success icon color | `colorStatusSuccessForeground1` | #54b054 |
| Snackbar error icon color | `colorStatusDangerForeground1` | #dc626d |
| Snackbar warning icon color | `colorStatusWarningForeground1` | #faa06b |
| Snackbar info icon color | `colorNeutralForeground2` | #d6d6d6 |

# Snackbar - Size (geometry)

Тема-независимые значения (reset-стили Toast).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Snackbar padding | (literal) | 12px |
| Snackbar corner radius | `borderRadiusMedium` | 4px |
| Snackbar media (icon) size | (literal) | 16px |
| Snackbar media right padding | (literal) | 8px |
| Snackbar action left padding | (literal) | 12px |
| Snackbar body top padding | (literal) | 6px |
| Snackbar subtitle top padding | (literal) | 4px |

# Snackbar - Typography

Тема-независимо.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Snackbar title font size | `fontSizeBase300` | 14px |
| Snackbar title font weight | `fontWeightSemibold` | 600 |
| Snackbar title line height | (literal) | 20px |
| Snackbar body font size | `fontSizeBase300` | 14px |
| Snackbar body font weight | `fontWeightRegular` | 400 |
| Snackbar body line height | `lineHeightBase300` | 20px |
| Snackbar subtitle font size | `fontSizeBase200` | 12px |
| Snackbar subtitle font weight | `fontWeightRegular` | 400 |

# Snackbar - Inline variant (MessageBar)

Инлайн-уведомление в потоке страницы - `MessageBar` (`@fluentui/react-message-bar`,
`useMessageBarStyles.styles.ts`). В отличие от Toast, интент задаёт ФОН + ЦВЕТ РАМКИ + иконку.
База: фон `colorNeutralBackground3`, рамка `strokeWidthThin`/`colorNeutralStroke1`,
радиус `borderRadiusMedium`, `min-height: 36px`, иконка `fontSizeBase500` (20px)
цвета `colorNeutralForeground3` (для info).

## Intent (Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| MessageBar info background | `colorNeutralBackground3` | #f5f5f5 |
| MessageBar info icon | `colorNeutralForeground3` | #616161 |
| MessageBar error background | `colorStatusDangerBackground1` | #fdf3f4 |
| MessageBar error border | `colorStatusDangerBorder1` | #eeacb2 |
| MessageBar error icon | `colorStatusDangerForeground1` | #b10e1c |
| MessageBar warning background | `colorStatusWarningBackground1` | #fff9f5 |
| MessageBar warning border | `colorStatusWarningBorder1` | #fdcfb4 |
| MessageBar warning icon | `colorStatusWarningForeground3` | #bc4b09 |
| MessageBar success background | `colorStatusSuccessBackground1` | #f1faf1 |
| MessageBar success border | `colorStatusSuccessBorder1` | #9fd89f |
| MessageBar success icon | `colorStatusSuccessForeground1` | #0e700e |

## Intent (Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| MessageBar info background | `colorNeutralBackground3` | #141414 |
| MessageBar info icon | `colorNeutralForeground3` | #adadad |
| MessageBar error background | `colorStatusDangerBackground1` | #3b0509 |
| MessageBar error border | `colorStatusDangerBorder1` | #c50f1f |
| MessageBar error icon | `colorStatusDangerForeground1` | #dc626d |
| MessageBar warning background | `colorStatusWarningBackground1` | #4a1e04 |
| MessageBar warning border | `colorStatusWarningBorder1` | #f7630c |
| MessageBar warning icon | `colorStatusWarningForeground3` | #f98845 |
| MessageBar success background | `colorStatusSuccessBackground1` | #052505 |
| MessageBar success border | `colorStatusSuccessBorder1` | #107c10 |
| MessageBar success icon | `colorStatusSuccessForeground1` | #54b054 |
