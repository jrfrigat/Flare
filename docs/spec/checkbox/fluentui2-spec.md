# Checkbox (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-checkbox`
(`useCheckboxStyles.styles.ts`). В колонке `Token Name` - реальный Fluent alias-токен
(см. `docs/spec/_pallete/fluentui2-spec.md`), в колонке `Value` - его разрешённое значение.
Состояние `Pressed` в CSS соответствует `:active`; цвет текста задаётся на слоте root и
наследуется меткой, а границу/фон/галочку индикатора задают внутренние CSS-переменные
(`--fui-Checkbox__indicator--borderColor/backgroundColor/color`).

# Checkbox - Color - Unchecked (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox unchecked label color | `colorNeutralForeground3` | #616161 |
| Checkbox unchecked indicator border color | `colorNeutralStrokeAccessible` | #616161 |
| Checkbox unchecked indicator background color | (transparent) | transparent |
| Checkbox unchecked indicator border width | `strokeWidthThin` | 1px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox unchecked hovered label color | `colorNeutralForeground2` | #424242 |
| Checkbox unchecked hovered indicator border color | `colorNeutralStrokeAccessibleHover` | #575757 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox unchecked pressed label color | `colorNeutralForeground1` | #242424 |
| Checkbox unchecked pressed indicator border color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |

# Checkbox - Color - Unchecked (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox unchecked label color | `colorNeutralForeground3` | #adadad |
| Checkbox unchecked indicator border color | `colorNeutralStrokeAccessible` | #adadad |
| Checkbox unchecked indicator background color | (transparent) | transparent |
| Checkbox unchecked indicator border width | `strokeWidthThin` | 1px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox unchecked hovered label color | `colorNeutralForeground2` | #d6d6d6 |
| Checkbox unchecked hovered indicator border color | `colorNeutralStrokeAccessibleHover` | #bdbdbd |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox unchecked pressed label color | `colorNeutralForeground1` | #ffffff |
| Checkbox unchecked pressed indicator border color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |

# Checkbox - Color - Checked (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox checked label color | `colorNeutralForeground1` | #242424 |
| Checkbox checked indicator background color | `colorCompoundBrandBackground` | #0f6cbd |
| Checkbox checked indicator border color | `colorCompoundBrandBackground` | #0f6cbd |
| Checkbox checked glyph (checkmark) color | `colorNeutralForegroundInverted` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox checked hovered indicator background color | `colorCompoundBrandBackgroundHover` | #115ea3 |
| Checkbox checked hovered indicator border color | `colorCompoundBrandBackgroundHover` | #115ea3 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox checked pressed indicator background color | `colorCompoundBrandBackgroundPressed` | #0f548c |
| Checkbox checked pressed indicator border color | `colorCompoundBrandBackgroundPressed` | #0f548c |

# Checkbox - Color - Checked (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox checked label color | `colorNeutralForeground1` | #ffffff |
| Checkbox checked indicator background color | `colorCompoundBrandBackground` | #479ef5 |
| Checkbox checked indicator border color | `colorCompoundBrandBackground` | #479ef5 |
| Checkbox checked glyph (checkmark) color | `colorNeutralForegroundInverted` | #242424 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox checked hovered indicator background color | `colorCompoundBrandBackgroundHover` | #62abf5 |
| Checkbox checked hovered indicator border color | `colorCompoundBrandBackgroundHover` | #62abf5 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox checked pressed indicator background color | `colorCompoundBrandBackgroundPressed` | #2886de |
| Checkbox checked pressed indicator border color | `colorCompoundBrandBackgroundPressed` | #2886de |

# Checkbox - Color - Mixed (indeterminate) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox mixed label color | `colorNeutralForeground1` | #242424 |
| Checkbox mixed indicator border color | `colorCompoundBrandStroke` | #0f6cbd |
| Checkbox mixed indicator background color | (transparent) | transparent |
| Checkbox mixed glyph (dash) color | `colorCompoundBrandForeground1` | #0f6cbd |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox mixed hovered indicator border color | `colorCompoundBrandStrokeHover` | #115ea3 |
| Checkbox mixed hovered glyph (dash) color | `colorCompoundBrandForeground1Hover` | #115ea3 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox mixed pressed indicator border color | `colorCompoundBrandStrokePressed` | #0f548c |
| Checkbox mixed pressed glyph (dash) color | `colorCompoundBrandForeground1Pressed` | #0f548c |

# Checkbox - Color - Mixed (indeterminate) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox mixed label color | `colorNeutralForeground1` | #ffffff |
| Checkbox mixed indicator border color | `colorCompoundBrandStroke` | #479ef5 |
| Checkbox mixed indicator background color | (transparent) | transparent |
| Checkbox mixed glyph (dash) color | `colorCompoundBrandForeground1` | #479ef5 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox mixed hovered indicator border color | `colorCompoundBrandStrokeHover` | #62abf5 |
| Checkbox mixed hovered glyph (dash) color | `colorCompoundBrandForeground1Hover` | #62abf5 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox mixed pressed indicator border color | `colorCompoundBrandStrokePressed` | #2886de |
| Checkbox mixed pressed glyph (dash) color | `colorCompoundBrandForeground1Pressed` | #2886de |

# Checkbox - Color - Disabled (контекст: Default, Light)

Одно состояние для checked / unchecked / mixed (Fluent задаёт единый блок `disabled`).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Checkbox disabled indicator border color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Checkbox disabled glyph color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Checkbox - Color - Disabled (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Checkbox disabled indicator border color | `colorNeutralStrokeDisabled` | #424242 |
| Checkbox disabled glyph color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Checkbox - Focus (контекст: Default, Light/Dark)

Фокус применён к root по `:focus-within` через `createFocusOutlineStyle` (Fluent 2 -
двойное кольцо: внутреннее `colorStrokeFocus2`, внешнее `colorStrokeFocus1`).

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Checkbox focus inner stroke color | `colorStrokeFocus2` | #000000 | #ffffff |
| Checkbox focus outer stroke color | `colorStrokeFocus1` | #ffffff | #000000 |
| Checkbox focus inner stroke width | `strokeWidthThin` | 1px | 1px |
| Checkbox focus outer outline width | `strokeWidthThick` | 2px | 2px |

# Checkbox - Size - Medium (default)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox medium indicator size | (literal) | 16px |
| Checkbox medium glyph size | (literal) | 12px |
| Checkbox medium indicator border width | `strokeWidthThin` | 1px |
| Checkbox medium indicator corner radius | `borderRadiusSmall` | 2px |
| Checkbox medium indicator margin block | `spacingVerticalS` | 8px |
| Checkbox medium indicator margin inline | `spacingHorizontalS` | 8px |
| Checkbox medium label size | `fontSizeBase300` | 14px |
| Checkbox medium label line-height | `lineHeightBase300` | 20px |
| Checkbox medium label padding block | `spacingVerticalS` | 8px |
| Checkbox medium label padding inline | `spacingHorizontalS` | 8px |
| Checkbox medium label-indicator gap | `spacingHorizontalXS` | 4px |

# Checkbox - Size - Large

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox large indicator size | (literal) | 20px |
| Checkbox large glyph size | (literal) | 16px |
| Checkbox large indicator border width | `strokeWidthThin` | 1px |
| Checkbox large indicator corner radius | `borderRadiusSmall` | 2px |
| Checkbox large indicator margin block | `spacingVerticalS` | 8px |
| Checkbox large indicator margin inline | `spacingHorizontalS` | 8px |
| Checkbox large label size | `fontSizeBase300` | 14px |
| Checkbox large label line-height | `lineHeightBase300` | 20px |
| Checkbox large label padding block | `spacingVerticalS` | 8px |
| Checkbox large label padding inline | `spacingHorizontalS` | 8px |
| Checkbox large label-indicator gap | `spacingHorizontalXS` | 4px |

# Checkbox - Shape

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Checkbox shape square (default) | `borderRadiusSmall` | 2px |
| Checkbox shape circular | `borderRadiusCircular` | 10000px |
