# Radio (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-radio`
(`useRadioStyles.styles.ts`). В колонке `Token Name` - реальный Fluent alias-токен
(см. `docs/spec/_pallete/fluentui2-spec.md`), в колонке `Value` - его разрешённое значение.
Состояние `Pressed` в CSS соответствует `:hover:active`. Точка выбора (`::after`) окрашена
`currentColor` индикатора, поэтому её цвет совпадает с цветом индикатора.

# Radio - Color - Unchecked (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio unchecked label color | `colorNeutralForeground3` | #616161 |
| Radio unchecked indicator border color | `colorNeutralStrokeAccessible` | #616161 |
| Radio unchecked indicator border width | `strokeWidthThin` | 1px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio unchecked hovered label color | `colorNeutralForeground2` | #424242 |
| Radio unchecked hovered indicator border color | `colorNeutralStrokeAccessibleHover` | #575757 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio unchecked pressed label color | `colorNeutralForeground1` | #242424 |
| Radio unchecked pressed indicator border color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |

# Radio - Color - Unchecked (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio unchecked label color | `colorNeutralForeground3` | #adadad |
| Radio unchecked indicator border color | `colorNeutralStrokeAccessible` | #adadad |
| Radio unchecked indicator border width | `strokeWidthThin` | 1px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio unchecked hovered label color | `colorNeutralForeground2` | #d6d6d6 |
| Radio unchecked hovered indicator border color | `colorNeutralStrokeAccessibleHover` | #bdbdbd |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio unchecked pressed label color | `colorNeutralForeground1` | #ffffff |
| Radio unchecked pressed indicator border color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |

# Radio - Color - Checked (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio checked label color | `colorNeutralForeground1` | #242424 |
| Radio checked indicator border color | `colorCompoundBrandStroke` | #0f6cbd |
| Radio checked dot color | `colorCompoundBrandForeground1` | #0f6cbd |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio checked hovered indicator border color | `colorCompoundBrandStrokeHover` | #115ea3 |
| Radio checked hovered dot color | `colorCompoundBrandForeground1Hover` | #115ea3 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio checked pressed indicator border color | `colorCompoundBrandStrokePressed` | #0f548c |
| Radio checked pressed dot color | `colorCompoundBrandForeground1Pressed` | #0f548c |

# Radio - Color - Checked (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio checked label color | `colorNeutralForeground1` | #ffffff |
| Radio checked indicator border color | `colorCompoundBrandStroke` | #479ef5 |
| Radio checked dot color | `colorCompoundBrandForeground1` | #479ef5 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio checked hovered indicator border color | `colorCompoundBrandStrokeHover` | #62abf5 |
| Radio checked hovered dot color | `colorCompoundBrandForeground1Hover` | #62abf5 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio checked pressed indicator border color | `colorCompoundBrandStrokePressed` | #2886de |
| Radio checked pressed dot color | `colorCompoundBrandForeground1Pressed` | #2886de |

# Radio - Color - Disabled (контекст: Default, Light)

Одно состояние для checked / unchecked (Fluent задаёт единый блок `:disabled`).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Radio disabled indicator border color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Radio disabled dot color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Radio - Color - Disabled (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Radio disabled indicator border color | `colorNeutralStrokeDisabled` | #424242 |
| Radio disabled dot color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Radio - Focus (контекст: Default, Light/Dark)

Фокус применён к root по `:focus-within` через `createFocusOutlineStyle` (Fluent 2 -
двойное кольцо: внутреннее `colorStrokeFocus2`, внешнее `colorStrokeFocus1`).

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Radio focus inner stroke color | `colorStrokeFocus2` | #000000 | #ffffff |
| Radio focus outer stroke color | `colorStrokeFocus1` | #ffffff | #000000 |
| Radio focus inner stroke width | `strokeWidthThin` | 1px | 1px |
| Radio focus outer outline width | `strokeWidthThick` | 2px | 2px |

# Radio - Size - Medium (default)

Radio имеет единственный размер (индикатор 16px, точка = 62.5% индикатора через
`transform: scale(0.625)`).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Radio indicator size | (literal) | 16px |
| Radio glyph font size | (literal) | 12px |
| Radio indicator border width | `strokeWidthThin` | 1px |
| Radio indicator corner radius | `borderRadiusCircular` | 10000px |
| Radio dot scale | (literal) | 0.625 |
| Radio indicator margin block | `spacingVerticalS` | 8px |
| Radio indicator margin inline | `spacingHorizontalS` | 8px |
| Radio label size | `fontSizeBase300` | 14px |
| Radio label line-height | `lineHeightBase300` | 20px |
| Radio label padding block | `spacingVerticalS` | 8px |
| Radio label padding inline | `spacingHorizontalS` | 8px |
| Radio label-indicator gap (after) | `spacingHorizontalXS` | 4px |
| Radio label padding top (below) | `spacingVerticalXS` | 4px |
