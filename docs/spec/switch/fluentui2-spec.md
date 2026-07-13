# Switch (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-switch`
(`useSwitchStyles.styles.ts`). В колонке `Token Name` - реальный Fluent alias-токен
(см. `docs/spec/_pallete/fluentui2-spec.md`), в колонке `Value` - его разрешённое значение.
Состояние `Pressed` в CSS соответствует `:hover:active`. Слот `indicator` = дорожка (track:
`background` + `border`), бегунок (thumb, внутренний `<i>`) окрашен `color`/`fill` индикатора.

# Switch - Color - Unchecked (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch unchecked track background color | (transparent) | transparent |
| Switch unchecked track border color | `colorNeutralStrokeAccessible` | #616161 |
| Switch unchecked thumb color | `colorNeutralStrokeAccessible` | #616161 |
| Switch unchecked track border width | (literal) | 1px |
| Switch unchecked label color | `colorNeutralForeground1` | #242424 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch unchecked hovered track border color | `colorNeutralStrokeAccessibleHover` | #575757 |
| Switch unchecked hovered thumb color | `colorNeutralStrokeAccessibleHover` | #575757 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch unchecked pressed track border color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |
| Switch unchecked pressed thumb color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |

# Switch - Color - Unchecked (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch unchecked track background color | (transparent) | transparent |
| Switch unchecked track border color | `colorNeutralStrokeAccessible` | #adadad |
| Switch unchecked thumb color | `colorNeutralStrokeAccessible` | #adadad |
| Switch unchecked track border width | (literal) | 1px |
| Switch unchecked label color | `colorNeutralForeground1` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch unchecked hovered track border color | `colorNeutralStrokeAccessibleHover` | #bdbdbd |
| Switch unchecked hovered thumb color | `colorNeutralStrokeAccessibleHover` | #bdbdbd |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch unchecked pressed track border color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |
| Switch unchecked pressed thumb color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |

# Switch - Color - Checked (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch checked track background color | `colorCompoundBrandBackground` | #0f6cbd |
| Switch checked track border color | `colorTransparentStroke` | transparent |
| Switch checked thumb color | `colorNeutralForegroundInverted` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch checked hovered track background color | `colorCompoundBrandBackgroundHover` | #115ea3 |
| Switch checked hovered track border color | `colorTransparentStrokeInteractive` | transparent |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch checked pressed track background color | `colorCompoundBrandBackgroundPressed` | #0f548c |
| Switch checked pressed track border color | `colorTransparentStrokeInteractive` | transparent |

# Switch - Color - Checked (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch checked track background color | `colorCompoundBrandBackground` | #479ef5 |
| Switch checked track border color | `colorTransparentStroke` | transparent |
| Switch checked thumb color | `colorNeutralForegroundInverted` | #242424 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch checked hovered track background color | `colorCompoundBrandBackgroundHover` | #62abf5 |
| Switch checked hovered track border color | `colorTransparentStrokeInteractive` | transparent |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch checked pressed track background color | `colorCompoundBrandBackgroundPressed` | #2886de |
| Switch checked pressed track border color | `colorTransparentStrokeInteractive` | transparent |

# Switch - Color - Disabled (контекст: Default, Light)

## Unchecked

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch disabled unchecked track border color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Switch disabled unchecked thumb color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Switch disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |

## Checked

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch disabled checked track background color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Switch disabled checked track border color | `colorTransparentStrokeDisabled` | transparent |
| Switch disabled checked thumb color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Switch - Color - Disabled (контекст: Default, Dark)

## Unchecked

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch disabled unchecked track border color | `colorNeutralStrokeDisabled` | #424242 |
| Switch disabled unchecked thumb color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Switch disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |

## Checked

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch disabled checked track background color | `colorNeutralBackgroundDisabled` | #141414 |
| Switch disabled checked track border color | `colorTransparentStrokeDisabled` | transparent |
| Switch disabled checked thumb color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Switch - Focus (контекст: Default, Light/Dark)

Фокус применён к root по `:focus-within` через `createFocusOutlineStyle` (Fluent 2 -
двойное кольцо: внутреннее `colorStrokeFocus2`, внешнее `colorStrokeFocus1`).

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Switch focus inner stroke color | `colorStrokeFocus2` | #000000 | #ffffff |
| Switch focus outer stroke color | `colorStrokeFocus1` | #ffffff | #000000 |
| Switch focus inner stroke width | `strokeWidthThin` | 1px | 1px |
| Switch focus outer outline width | `strokeWidthThick` | 2px | 2px |

# Switch - Size - Medium (default)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch medium track width | (literal) | 40px |
| Switch medium track height | (literal) | 20px |
| Switch medium thumb size | (derived: track height - 2) | 18px |
| Switch medium thumb-track gap | (literal) | 2px |
| Switch medium track border width | (literal) | 1px |
| Switch medium track corner radius | `borderRadiusCircular` | 10000px |
| Switch medium indicator margin block | `spacingVerticalS` | 8px |
| Switch medium indicator margin inline | `spacingHorizontalS` | 8px |
| Switch medium label size | `fontSizeBase300` | 14px |
| Switch medium label line-height | `lineHeightBase300` | 20px |
| Switch medium label padding block | `spacingVerticalS` | 8px |
| Switch medium label padding inline | `spacingHorizontalS` | 8px |
| Switch medium label gap (after/before) | `spacingHorizontalXS` | 4px |

# Switch - Size - Small

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch small track width | (literal) | 32px |
| Switch small track height | (literal) | 16px |
| Switch small thumb size | (derived: track height - 2) | 14px |
| Switch small thumb-track gap | (literal) | 2px |
| Switch small track border width | (literal) | 1px |
| Switch small track corner radius | `borderRadiusCircular` | 10000px |
| Switch small label size | `fontSizeBase200` | 12px |
| Switch small label line-height | `lineHeightBase200` | 16px |
| Switch small label gap (after/before) | `spacingHorizontalXS` | 4px |

# Switch - Motion

Переход задан на слоте `indicator` (дорожка) и на бегунке (`> *`, свойство `transform`).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Switch track transition duration | `durationNormal` | 200ms |
| Switch track transition easing | `curveEasyEase` | cubic-bezier(0.33, 0, 0.67, 1) |
| Switch track transition properties | (literal) | background, border, color |
| Switch thumb transition duration | `durationNormal` | 200ms |
| Switch thumb transition easing | `curveEasyEase` | cubic-bezier(0.33, 0, 0.67, 1) |
| Switch thumb transition property | (literal) | transform |
| Switch reduced-motion duration | (literal) | 0.01ms |
