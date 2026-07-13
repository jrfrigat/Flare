# Input (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-input`
(`useInputStyles.styles.ts`). В колонке `Token Name` - реальный Fluent alias-токен, в колонке
`Value` - его разрешённое значение для темы. Состояние `Focused` в CSS соответствует
`:active, :focus-within` (изменение рамки), плюс нижняя brand-подсветка (`::after`) появляется
на `:focus-within`. Слоты input-текста и content-иконок общие для всех appearance.

# Input - Color - Outline (default) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input outline container color | `colorNeutralBackground1` | #ffffff |
| Input outline stroke color (top/left/right) | `colorNeutralStroke1` | #d1d1d1 |
| Input outline bottom stroke color | `colorNeutralStrokeAccessible` | #616161 |
| Input outline stroke width | (literal, = `strokeWidthThin`) | 1px |
| Input outline corner radius | `borderRadiusMedium` | 4px |
| Input outline text color | `colorNeutralForeground1` | #242424 |
| Input outline placeholder color | `colorNeutralForeground4` | #707070 |
| Input outline content/icon color | `colorNeutralForeground3` | #616161 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input outline hovered stroke color | `colorNeutralStroke1Hover` | #c7c7c7 |
| Input outline hovered bottom stroke color | `colorNeutralStrokeAccessibleHover` | #575757 |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input outline focused stroke color | `colorNeutralStroke1Pressed` | #b3b3b3 |
| Input outline focused bottom stroke color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |
| Input outline focus underline color (`::after`) | `colorCompoundBrandStroke` | #0f6cbd |
| Input outline focus underline width | (literal) | 2px |
| Input outline focus underline pressed color | `colorCompoundBrandStrokePressed` | #0f548c |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input outline disabled container color | `colorTransparentBackground` | transparent |
| Input outline disabled stroke color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Input outline disabled text color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Input outline disabled placeholder color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Input outline disabled content/icon color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Input - Color - Outline (default) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input outline container color | `colorNeutralBackground1` | #292929 |
| Input outline stroke color (top/left/right) | `colorNeutralStroke1` | #666666 |
| Input outline bottom stroke color | `colorNeutralStrokeAccessible` | #adadad |
| Input outline stroke width | (literal, = `strokeWidthThin`) | 1px |
| Input outline corner radius | `borderRadiusMedium` | 4px |
| Input outline text color | `colorNeutralForeground1` | #ffffff |
| Input outline placeholder color | `colorNeutralForeground4` | #999999 |
| Input outline content/icon color | `colorNeutralForeground3` | #adadad |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input outline hovered stroke color | `colorNeutralStroke1Hover` | #757575 |
| Input outline hovered bottom stroke color | `colorNeutralStrokeAccessibleHover` | #bdbdbd |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input outline focused stroke color | `colorNeutralStroke1Pressed` | #6b6b6b |
| Input outline focused bottom stroke color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |
| Input outline focus underline color (`::after`) | `colorCompoundBrandStroke` | #479ef5 |
| Input outline focus underline width | (literal) | 2px |
| Input outline focus underline pressed color | `colorCompoundBrandStrokePressed` | #2886de |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input outline disabled container color | `colorTransparentBackground` | transparent |
| Input outline disabled stroke color | `colorNeutralStrokeDisabled` | #424242 |
| Input outline disabled text color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Input outline disabled placeholder color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Input outline disabled content/icon color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Input - Color - Underline (контекст: Default, Light)

Только нижняя рамка; фон прозрачный; corner radius = 0.

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input underline container color | `colorTransparentBackground` | transparent |
| Input underline bottom stroke color | `colorNeutralStrokeAccessible` | #616161 |
| Input underline bottom stroke width | (literal, = `strokeWidthThin`) | 1px |
| Input underline corner radius | (literal) | 0 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input underline hovered bottom stroke color | `colorNeutralStrokeAccessibleHover` | #575757 |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input underline focused bottom stroke color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |
| Input underline focus underline color (`::after`) | `colorCompoundBrandStroke` | #0f6cbd |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input underline disabled container color | `colorTransparentBackground` | transparent |
| Input underline disabled stroke color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Input - Color - Underline (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input underline container color | `colorTransparentBackground` | transparent |
| Input underline bottom stroke color | `colorNeutralStrokeAccessible` | #adadad |
| Input underline bottom stroke width | (literal, = `strokeWidthThin`) | 1px |
| Input underline corner radius | (literal) | 0 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input underline hovered bottom stroke color | `colorNeutralStrokeAccessibleHover` | #bdbdbd |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input underline focused bottom stroke color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |
| Input underline focus underline color (`::after`) | `colorCompoundBrandStroke` | #479ef5 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input underline disabled container color | `colorTransparentBackground` | transparent |
| Input underline disabled stroke color | `colorNeutralStrokeDisabled` | #424242 |

# Input - Color - Filled Lighter (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledLighter container color | `colorNeutralBackground1` | #ffffff |
| Input filledLighter stroke color | `colorTransparentStroke` | transparent |
| Input filledLighter corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledLighter hovered stroke color | `colorTransparentStrokeInteractive` | transparent |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledLighter focused stroke color | `colorTransparentStrokeInteractive` | transparent |
| Input filledLighter focus underline color (`::after`) | `colorCompoundBrandStroke` | #0f6cbd |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledLighter disabled container color | `colorTransparentBackground` | transparent |
| Input filledLighter disabled stroke color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Input - Color - Filled Lighter (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledLighter container color | `colorNeutralBackground1` | #292929 |
| Input filledLighter stroke color | `colorTransparentStroke` | transparent |
| Input filledLighter corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledLighter hovered stroke color | `colorTransparentStrokeInteractive` | transparent |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledLighter focused stroke color | `colorTransparentStrokeInteractive` | transparent |
| Input filledLighter focus underline color (`::after`) | `colorCompoundBrandStroke` | #479ef5 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledLighter disabled container color | `colorTransparentBackground` | transparent |
| Input filledLighter disabled stroke color | `colorNeutralStrokeDisabled` | #424242 |

# Input - Color - Filled Darker (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledDarker container color | `colorNeutralBackground3` | #f5f5f5 |
| Input filledDarker stroke color | `colorTransparentStroke` | transparent |
| Input filledDarker corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledDarker hovered stroke color | `colorTransparentStrokeInteractive` | transparent |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledDarker focused stroke color | `colorTransparentStrokeInteractive` | transparent |
| Input filledDarker focus underline color (`::after`) | `colorCompoundBrandStroke` | #0f6cbd |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledDarker disabled container color | `colorTransparentBackground` | transparent |
| Input filledDarker disabled stroke color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Input - Color - Filled Darker (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledDarker container color | `colorNeutralBackground3` | #141414 |
| Input filledDarker stroke color | `colorTransparentStroke` | transparent |
| Input filledDarker corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledDarker hovered stroke color | `colorTransparentStrokeInteractive` | transparent |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledDarker focused stroke color | `colorTransparentStrokeInteractive` | transparent |
| Input filledDarker focus underline color (`::after`) | `colorCompoundBrandStroke` | #479ef5 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input filledDarker disabled container color | `colorTransparentBackground` | transparent |
| Input filledDarker disabled stroke color | `colorNeutralStrokeDisabled` | #424242 |

# Input - Invalid (контекст: Default, Light/Dark)

Применяется на `:not(:focus-within), :hover:not(:focus-within)` (перекрывает рамку красным).

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Input invalid stroke color | `colorPaletteRedBorder2` | #d13438 | #e37d80 |

# Input - Size - Small

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input small min height | (literal `fieldHeights.small`) | 24px |
| Input small text size | `fontSizeBase200` (caption1) | 12px |
| Input small text line-height | `lineHeightBase200` | 16px |
| Input small text weight | `fontWeightRegular` | 400 |
| Input small corner radius | `borderRadiusMedium` | 4px |
| Input small input padding (no content, combined) | `spacingHorizontalS` | 8px |
| Input small root gap | `spacingHorizontalXXS` | 2px |
| Input small icon size | (literal `> svg`) | 16px |

# Input - Size - Medium (default)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input medium min height | (literal `fieldHeights.medium`) | 32px |
| Input medium text size | `fontSizeBase300` (body1) | 14px |
| Input medium text line-height | `lineHeightBase300` | 20px |
| Input medium text weight | `fontWeightRegular` | 400 |
| Input medium corner radius | `borderRadiusMedium` | 4px |
| Input medium input padding (no content, combined) | `spacingHorizontalM` | 12px |
| Input medium root gap | `spacingHorizontalXXS` | 2px |
| Input medium icon size | (literal `> svg`) | 20px |

# Input - Size - Large

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input large min height | (literal `fieldHeights.large`) | 40px |
| Input large text size | `fontSizeBase400` (body2) | 16px |
| Input large text line-height | `lineHeightBase400` | 22px |
| Input large text weight | `fontWeightRegular` | 400 |
| Input large corner radius | `borderRadiusMedium` | 4px |
| Input large input padding (no content, combined) | (derived: `spacingHorizontalM` + `spacingHorizontalSNudge`) | 18px |
| Input large root gap | `spacingHorizontalSNudge` | 6px |
| Input large icon size | (literal `> svg`) | 24px |

# Input - Motion

Анимация нижней brand-подсветки фокуса (`::after` scaleX). Примечание: в исходнике
`transitionDelay` задан curve-токеном (Fluent-квирк) - копируется как есть.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Input focus-in transition property | (literal) | transform |
| Input focus-in duration | `durationNormal` | 200ms |
| Input focus-in delay | `curveDecelerateMid` | cubic-bezier(0,0,0,1) |
| Input focus-out duration | `durationUltraFast` | 50ms |
| Input focus-out delay | `curveAccelerateMid` | cubic-bezier(1,0,1,1) |
