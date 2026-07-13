# Divider (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот -> alias-токен взяты из исходника `@fluentui/react-divider`
(`useDividerStyles.styles.ts`). Divider неинтерактивен (нет hover/pressed/disabled) - есть
четыре appearance (`default`, `subtle`, `brand`, `strong`) и две ориентации
(horizontal / vertical). Линия рисуется border на псевдоэлементах `::before`/`::after`
(`border-top` для horizontal, `border-right` для vertical) - цвет линии одинаков для обеих
ориентаций. Каждый appearance задаёт цвет контента (текста) и цвет линии.

# Divider - Color - Default (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider default content color | `colorNeutralForeground2` | #424242 |
| Divider default line color | `colorNeutralStroke2` | #e0e0e0 |
| Divider default line width | `strokeWidthThin` | 1px |

# Divider - Color - Default (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider default content color | `colorNeutralForeground2` | #d6d6d6 |
| Divider default line color | `colorNeutralStroke2` | #525252 |
| Divider default line width | `strokeWidthThin` | 1px |

# Divider - Color - Subtle (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider subtle content color | `colorNeutralForeground3` | #616161 |
| Divider subtle line color | `colorNeutralStroke3` | #f0f0f0 |
| Divider subtle line width | `strokeWidthThin` | 1px |

# Divider - Color - Subtle (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider subtle content color | `colorNeutralForeground3` | #adadad |
| Divider subtle line color | `colorNeutralStroke3` | #3d3d3d |
| Divider subtle line width | `strokeWidthThin` | 1px |

# Divider - Color - Brand (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider brand content color | `colorBrandForeground1` | #0f6cbd |
| Divider brand line color | `colorBrandStroke1` | #0f6cbd |
| Divider brand line width | `strokeWidthThin` | 1px |

# Divider - Color - Brand (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider brand content color | `colorBrandForeground1` | #479ef5 |
| Divider brand line color | `colorBrandStroke1` | #479ef5 |
| Divider brand line width | `strokeWidthThin` | 1px |

# Divider - Color - Strong (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider strong content color | `colorNeutralForeground1` | #242424 |
| Divider strong line color | `colorNeutralStroke1` | #d1d1d1 |
| Divider strong line width | `strokeWidthThin` | 1px |

# Divider - Color - Strong (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider strong content color | `colorNeutralForeground1` | #ffffff |
| Divider strong line color | `colorNeutralStroke1` | #666666 |
| Divider strong line width | `strokeWidthThin` | 1px |

# Divider - Size / Geometry

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider line width | `strokeWidthThin` | 1px |
| Divider content spacing (gap to line) | (literal `contentSpacing`) | 12px |
| Divider inset spacing | (literal `insetSpacing`) | 12px |
| Divider start/end min length | (literal `minStartEndLength`) | 8px |
| Divider start/end max length | (literal `maxStartEndLength`) | 8px |
| Divider horizontal width | (literal) | 100% |
| Divider vertical min height | (literal) | 20px |
| Divider vertical min height (with children) | (literal `withChildren`) | 84px |

# Divider - Typography

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Divider font family | `fontFamilyBase` | 'Segoe UI', 'Segoe UI Web (West European)', -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', sans-serif |
| Divider font size | `fontSizeBase200` | 12px |
| Divider font weight | `fontWeightRegular` | 400 |
| Divider line height | `lineHeightBase200` | 16px |
