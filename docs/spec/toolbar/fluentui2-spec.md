# Toolbar (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходников `@fluentui/react-toolbar`
(`useToolbarStyles.styles.ts`, `useToolbarButtonStyles.styles.ts`, `useToolbarDividerStyles.styles.ts`).

**ВАЖНО: Toolbar в Fluent UI 2 - тонкий layout-контейнер без собственных цветовых токенов.**
Сама поверхность Toolbar задаёт только `display/align/padding` (никаких background/border/color).
`ToolbarButton` целиком переиспользует `useButtonStyles_unstable` из `@fluentui/react-button`
(appearance по умолчанию = `subtle`), добавляя лишь вертикальную раскладку и увеличенную иконку.
`ToolbarDivider` целиком переиспользует `useDividerStyles_unstable` из `@fluentui/react-divider`
(appearance по умолчанию = `default`), добавляя лишь toolbar-паддинги. Таблицы ниже отражают
эти переиспользованные токены.

# Toolbar - Surface (контекст: Default, Light/Dark)

Поверхность Toolbar не задаёт ни фон, ни рамку, ни цвет текста - только раскладку. Цвет фона
наследуется от родителя (обычно `colorNeutralBackground1`). Ниже - только геометрия.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar layout | (literal) | flex, align-items center |
| Toolbar padding (small) | (literal) | 0px 4px |
| Toolbar padding (medium, default) | (literal) | 4px 8px |
| Toolbar padding (large) | (literal) | 4px 20px |
| Toolbar vertical layout | (literal) | flex-direction column, width fit-content |

# Toolbar - Button (ToolbarButton = Button subtle) (контекст: Default, Light)

Переиспользует Button appearance `subtle`. См. `docs/spec/button/fluentui2-spec.md`.

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button container color | `colorSubtleBackground` | transparent |
| Toolbar button outline color | (transparent) | transparent |
| Toolbar button label color | `colorNeutralForeground2` | #424242 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button hovered container color | `colorSubtleBackgroundHover` | #f5f5f5 |
| Toolbar button hovered label color | `colorNeutralForeground2Hover` | #242424 |
| Toolbar button hovered icon color | `colorNeutralForeground2BrandHover` | #0f6cbd |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button pressed container color | `colorSubtleBackgroundPressed` | #e0e0e0 |
| Toolbar button pressed label color | `colorNeutralForeground2Pressed` | #242424 |
| Toolbar button pressed icon color | `colorNeutralForeground2BrandPressed` | #115ea3 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button disabled container color | `colorTransparentBackground` | transparent |
| Toolbar button disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Toolbar - Button (ToolbarButton = Button subtle) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button container color | `colorSubtleBackground` | transparent |
| Toolbar button outline color | (transparent) | transparent |
| Toolbar button label color | `colorNeutralForeground2` | #d6d6d6 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button hovered container color | `colorSubtleBackgroundHover` | #383838 |
| Toolbar button hovered label color | `colorNeutralForeground2Hover` | #ffffff |
| Toolbar button hovered icon color | `colorNeutralForeground2BrandHover` | #479ef5 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button pressed container color | `colorSubtleBackgroundPressed` | #2e2e2e |
| Toolbar button pressed label color | `colorNeutralForeground2Pressed` | #ffffff |
| Toolbar button pressed icon color | `colorNeutralForeground2BrandPressed` | #2886de |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button disabled container color | `colorTransparentBackground` | transparent |
| Toolbar button disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Toolbar - Divider (ToolbarDivider = Divider default) (контекст: Default, Light/Dark)

Переиспользует Divider appearance `default`. Линия - цвет border у псевдоэлементов `::before/::after`.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Toolbar divider line color | `colorNeutralStroke2` | #e0e0e0 | #525252 |
| Toolbar divider content color | `colorNeutralForeground2` | #424242 | #d6d6d6 |

# Toolbar - Divider geometry

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar divider padding (horizontal) | (literal) | 0 12px |
| Toolbar divider max width (horizontal) | (literal) | 1px |
| Toolbar divider max width (vertical) | (literal) | initial |

# Toolbar - Button vertical layout

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Toolbar button vertical direction | (literal) | flex-direction column |
| Toolbar button vertical icon size | (literal) | 24px |
| Toolbar button vertical icon margin | (literal) | 0 |
