# Tooltip (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот -> alias-токен взяты из исходника `@fluentui/react-tooltip`
(`useTooltipStyles.styles.ts`). Tooltip неинтерактивен (нет hover/pressed/disabled) - есть
только два appearance (`normal` по умолчанию и `inverted`) и переключение видимости
(`display`). Тень задана `filter: drop-shadow(...)` двумя shadow-color токенами
(`colorNeutralShadowAmbient` + `colorNeutralShadowKey`) - filter-аппроксимация elevation
(комментарий в исходнике ссылается на `shadow8`; ближайший box-shadow-токен - `shadow16`,
см. таблицу Shadow). Стрелка (`createArrowStyles`) наследует фон и рамку поверхности.

# Tooltip - Color - Surface (default) (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tooltip surface background color | `colorNeutralBackground1` | #ffffff |
| Tooltip surface text color | `colorNeutralForeground1` | #242424 |
| Tooltip surface border color | `colorTransparentStroke` | transparent |
| Tooltip surface border width | (literal) | 1px |
| Tooltip surface corner radius | `borderRadiusMedium` | 4px |

# Tooltip - Color - Surface (default) (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tooltip surface background color | `colorNeutralBackground1` | #292929 |
| Tooltip surface text color | `colorNeutralForeground1` | #ffffff |
| Tooltip surface border color | `colorTransparentStroke` | transparent |
| Tooltip surface border width | (literal) | 1px |
| Tooltip surface corner radius | `borderRadiusMedium` | 4px |

# Tooltip - Color - Surface (inverted) (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tooltip inverted background color | `colorNeutralBackgroundStatic` | #333333 |
| Tooltip inverted text color | `colorNeutralForegroundStaticInverted` | #ffffff |
| Tooltip inverted border color | `colorTransparentStroke` | transparent |
| Tooltip inverted corner radius | `borderRadiusMedium` | 4px |

# Tooltip - Color - Surface (inverted) (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tooltip inverted background color | `colorNeutralBackgroundStatic` | #3d3d3d |
| Tooltip inverted text color | `colorNeutralForegroundStaticInverted` | #ffffff |
| Tooltip inverted border color | `colorTransparentStroke` | transparent |
| Tooltip inverted corner radius | `borderRadiusMedium` | 4px |

# Tooltip - Shadow (контекст: Default, Light/Dark)

`filter: drop-shadow(0 0 2px <ambient>) drop-shadow(0 4px 8px <key>)`. Значения цвета -
разрешённые shadow-color токены. Столбец "shadow16 (box-shadow эквивалент)" приведён как
ближайший готовый elevation-токен для справки.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Tooltip shadow ambient color (`drop-shadow` #1) | `colorNeutralShadowAmbient` | rgba(0,0,0,0.12) | rgba(0,0,0,0.24) |
| Tooltip shadow key color (`drop-shadow` #2) | `colorNeutralShadowKey` | rgba(0,0,0,0.14) | rgba(0,0,0,0.28) |
| Tooltip shadow16 (box-shadow эквивалент) | `shadow16` | 0 0 2px rgba(0,0,0,0.12), 0 8px 16px rgba(0,0,0,0.14) | 0 0 2px rgba(0,0,0,0.24), 0 8px 16px rgba(0,0,0,0.28) |

# Tooltip - Size / Geometry

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tooltip max width | (literal) | 240px |
| Tooltip padding | (literal, `5px 12px 7px 12px` минус 1px рамки) | 4px 11px 6px 11px |
| Tooltip corner radius | `borderRadiusMedium` | 4px |
| Tooltip arrow height | (literal `arrowHeight`) | 6px |
| Tooltip arrow border radius | (literal `tooltipBorderRadius`) | 4px |

# Tooltip - Typography

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tooltip font family | `fontFamilyBase` | 'Segoe UI', 'Segoe UI Web (West European)', -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', sans-serif |
| Tooltip font size | `fontSizeBase200` | 12px |
| Tooltip line height | `lineHeightBase200` | 16px |
