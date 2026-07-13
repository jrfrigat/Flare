# Card (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-card`
(`useCardStyles.styles.ts`, плюс `useCardHeaderStyles`, `useCardFooterStyles`, `useCardPreviewStyles`).
В колонке `Token Name` - реальный Fluent alias-токен, в колонке `Value` - его разрешённое значение.
Hover/Pressed применяются только к интерактивной карточке (`interactive`/`selectable`); `:active` в CSS
соответствует Pressed. Граница карточки рисуется псевдоэлементом `::after` (`strokeWidthThin`).

# Card - Color - Filled (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled container color | `colorNeutralBackground1` | #ffffff |
| Card filled label color | `colorNeutralForeground1` | #242424 |
| Card filled border color (::after) | `colorTransparentStroke` | transparent |
| Card filled border width | `strokeWidthThin` | 1px |
| Card filled elevation | `shadow4` | 0 0 2px rgba(0,0,0,0.12), 0 2px 4px rgba(0,0,0,0.14) |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled hovered container color | `colorNeutralBackground1Hover` | #f5f5f5 |
| Card filled hovered label color | `colorNeutralForeground1Hover` | #242424 |
| Card filled hovered elevation | `shadow8` | 0 0 2px rgba(0,0,0,0.12), 0 4px 8px rgba(0,0,0,0.14) |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled pressed container color | `colorNeutralBackground1Pressed` | #e0e0e0 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled selected container color | `colorNeutralBackground1Selected` | #ebebeb |
| Card filled selected border color (::after) | `colorNeutralStroke1Selected` | #bdbdbd |
| Card filled selected label color | `colorNeutralForeground1Selected` | #242424 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Card filled disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Card filled disabled border color (::after) | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Card filled disabled elevation | `shadow2` | 0 0 2px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.14) |

# Card - Color - Filled (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled container color | `colorNeutralBackground1` | #292929 |
| Card filled label color | `colorNeutralForeground1` | #ffffff |
| Card filled border color (::after) | `colorTransparentStroke` | transparent |
| Card filled border width | `strokeWidthThin` | 1px |
| Card filled elevation | `shadow4` | 0 0 2px rgba(0,0,0,0.24), 0 2px 4px rgba(0,0,0,0.28) |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled hovered container color | `colorNeutralBackground1Hover` | #3d3d3d |
| Card filled hovered label color | `colorNeutralForeground1Hover` | #ffffff |
| Card filled hovered elevation | `shadow8` | 0 0 2px rgba(0,0,0,0.24), 0 4px 8px rgba(0,0,0,0.28) |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled pressed container color | `colorNeutralBackground1Pressed` | #1f1f1f |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled selected container color | `colorNeutralBackground1Selected` | #383838 |
| Card filled selected border color (::after) | `colorNeutralStroke1Selected` | #707070 |
| Card filled selected label color | `colorNeutralForeground1Selected` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Card filled disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Card filled disabled border color (::after) | `colorNeutralStrokeDisabled` | #424242 |
| Card filled disabled elevation | `shadow2` | 0 0 2px rgba(0,0,0,0.24), 0 1px 2px rgba(0,0,0,0.28) |

# Card - Color - Filled-Alternative (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt container color | `colorNeutralBackground2` | #fafafa |
| Card filled-alt label color | `colorNeutralForeground1` | #242424 |
| Card filled-alt border color (::after) | `colorTransparentStroke` | transparent |
| Card filled-alt elevation | `shadow4` | 0 0 2px rgba(0,0,0,0.12), 0 2px 4px rgba(0,0,0,0.14) |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt hovered container color | `colorNeutralBackground2Hover` | #f0f0f0 |
| Card filled-alt hovered label color | `colorNeutralForeground2Hover` | #242424 |
| Card filled-alt hovered elevation | `shadow8` | 0 0 2px rgba(0,0,0,0.12), 0 4px 8px rgba(0,0,0,0.14) |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt pressed container color | `colorNeutralBackground2Pressed` | #dbdbdb |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt selected container color | `colorNeutralBackground2Selected` | #e6e6e6 |
| Card filled-alt selected border color (::after) | `colorNeutralStroke1Selected` | #bdbdbd |
| Card filled-alt selected label color | `colorNeutralForeground2Selected` | #242424 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Card filled-alt disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Card filled-alt disabled border color (::after) | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Card filled-alt disabled elevation | `shadow2` | 0 0 2px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.14) |

# Card - Color - Filled-Alternative (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt container color | `colorNeutralBackground2` | #1f1f1f |
| Card filled-alt label color | `colorNeutralForeground1` | #ffffff |
| Card filled-alt border color (::after) | `colorTransparentStroke` | transparent |
| Card filled-alt elevation | `shadow4` | 0 0 2px rgba(0,0,0,0.24), 0 2px 4px rgba(0,0,0,0.28) |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt hovered container color | `colorNeutralBackground2Hover` | #333333 |
| Card filled-alt hovered label color | `colorNeutralForeground2Hover` | #ffffff |
| Card filled-alt hovered elevation | `shadow8` | 0 0 2px rgba(0,0,0,0.24), 0 4px 8px rgba(0,0,0,0.28) |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt pressed container color | `colorNeutralBackground2Pressed` | #141414 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt selected container color | `colorNeutralBackground2Selected` | #2e2e2e |
| Card filled-alt selected border color (::after) | `colorNeutralStroke1Selected` | #707070 |
| Card filled-alt selected label color | `colorNeutralForeground2Selected` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card filled-alt disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Card filled-alt disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Card filled-alt disabled border color (::after) | `colorNeutralStrokeDisabled` | #424242 |
| Card filled-alt disabled elevation | `shadow2` | 0 0 2px rgba(0,0,0,0.24), 0 1px 2px rgba(0,0,0,0.28) |

# Card - Color - Outline (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline container color | `colorTransparentBackground` | transparent |
| Card outline label color | `colorNeutralForeground1` | #242424 |
| Card outline border color (::after) | `colorNeutralStroke1` | #d1d1d1 |
| Card outline elevation | (literal) | none |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline hovered container color | `colorTransparentBackgroundHover` | transparent |
| Card outline hovered label color | `colorNeutralForeground1Hover` | #242424 |
| Card outline hovered border color (::after) | `colorNeutralStroke1Hover` | #c7c7c7 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline pressed container color | `colorTransparentBackgroundPressed` | transparent |
| Card outline pressed border color (::after) | `colorNeutralStroke1Pressed` | #b3b3b3 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline selected container color | `colorTransparentBackgroundSelected` | transparent |
| Card outline selected border color (::after) | `colorNeutralStroke1Selected` | #bdbdbd |
| Card outline selected label color | `colorNeutralForeground1Selected` | #242424 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline disabled container color | `colorTransparentBackground` | transparent |
| Card outline disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Card outline disabled border color (::after) | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Card - Color - Outline (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline container color | `colorTransparentBackground` | transparent |
| Card outline label color | `colorNeutralForeground1` | #ffffff |
| Card outline border color (::after) | `colorNeutralStroke1` | #666666 |
| Card outline elevation | (literal) | none |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline hovered container color | `colorTransparentBackgroundHover` | transparent |
| Card outline hovered label color | `colorNeutralForeground1Hover` | #ffffff |
| Card outline hovered border color (::after) | `colorNeutralStroke1Hover` | #757575 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline pressed container color | `colorTransparentBackgroundPressed` | transparent |
| Card outline pressed border color (::after) | `colorNeutralStroke1Pressed` | #6b6b6b |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline selected container color | `colorTransparentBackgroundSelected` | transparent |
| Card outline selected border color (::after) | `colorNeutralStroke1Selected` | #707070 |
| Card outline selected label color | `colorNeutralForeground1Selected` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card outline disabled container color | `colorTransparentBackground` | transparent |
| Card outline disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Card outline disabled border color (::after) | `colorNeutralStrokeDisabled` | #424242 |

# Card - Color - Subtle (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle container color | `colorSubtleBackground` | transparent |
| Card subtle label color | `colorNeutralForeground1` | #242424 |
| Card subtle border color (::after) | `colorTransparentStroke` | transparent |
| Card subtle elevation | (literal) | none |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle hovered container color | `colorSubtleBackgroundHover` | #f5f5f5 |
| Card subtle hovered label color | `colorNeutralForeground1Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle pressed container color | `colorSubtleBackgroundPressed` | #e0e0e0 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle selected container color | `colorSubtleBackgroundSelected` | #ebebeb |
| Card subtle selected border color (::after) | `colorNeutralStroke1Selected` | #bdbdbd |
| Card subtle selected label color | `colorNeutralForeground1Selected` | #242424 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Card subtle disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Card subtle disabled border color (::after) | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Card - Color - Subtle (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle container color | `colorSubtleBackground` | transparent |
| Card subtle label color | `colorNeutralForeground1` | #ffffff |
| Card subtle border color (::after) | `colorTransparentStroke` | transparent |
| Card subtle elevation | (literal) | none |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle hovered container color | `colorSubtleBackgroundHover` | #383838 |
| Card subtle hovered label color | `colorNeutralForeground1Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle pressed container color | `colorSubtleBackgroundPressed` | #2e2e2e |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle selected container color | `colorSubtleBackgroundSelected` | #333333 |
| Card subtle selected border color (::after) | `colorNeutralStroke1Selected` | #707070 |
| Card subtle selected label color | `colorNeutralForeground1Selected` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card subtle disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Card subtle disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Card subtle disabled border color (::after) | `colorNeutralStrokeDisabled` | #424242 |

# Card - Focus (контекст: Default, Light/Dark)

Фокус карточки - двойное кольцо (`createFocusOutlineStyle`): внутреннее цвета `colorStrokeFocus2`,
внешнее - `colorStrokeFocus1`; радиус равен радиусу карточки, ширина `strokeWidthThick`, смещение -2px.
Для `selectable`-карточки используется `:focus-within`, для интерактивной - `:focus`.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Card focus inner stroke color | `colorStrokeFocus2` | #000000 | #ffffff |
| Card focus outer stroke color | `colorStrokeFocus1` | #ffffff | #000000 |
| Card focus outline width | `strokeWidthThick` | 2px | 2px |
| Card focus outline offset | (literal) | -2px | -2px |

# Card - Size - Small

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card small padding (--fui-Card--size) | (literal) | 8px |
| Card small gap | (literal, `= --fui-Card--size`) | 8px |
| Card small corner radius | `borderRadiusSmall` | 2px |

# Card - Size - Medium (default)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card medium padding (--fui-Card--size) | (literal) | 12px |
| Card medium gap | (literal, `= --fui-Card--size`) | 12px |
| Card medium corner radius | `borderRadiusMedium` | 4px |

# Card - Size - Large

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Card large padding (--fui-Card--size) | (literal) | 16px |
| Card large gap | (literal, `= --fui-Card--size`) | 16px |
| Card large corner radius | `borderRadiusLarge` | 6px |

# CardHeader

`useCardHeaderStyles.styles.ts` - размерных токенов темы не использует; отступы/зазоры заданы литералами.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| CardHeader gap (--fui-CardHeader--gap) | (literal) | 12px |
| CardHeader image margin-right | (literal, `= gap`) | 12px |
| CardHeader action margin-left | (literal, `= gap`) | 12px |
| CardHeader layout (with description) | (literal) | grid (min-content 1fr min-content) |
| CardHeader layout (title only) | (literal) | flex |

# CardFooter

`useCardFooterStyles.styles.ts` - только раскладка, без токенов темы.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| CardFooter row gap | (literal) | 12px |
| CardFooter action margin-left | (literal) | auto |
| CardFooter direction | (literal) | row |

# CardPreview

`useCardPreviewStyles.styles.ts` - логотип позиционируется абсолютно; собственных токенов темы нет.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| CardPreview logo width | (literal) | 32px |
| CardPreview logo height | (literal) | 32px |
| CardPreview logo bottom offset | (literal) | 12px |
| CardPreview logo left offset | (literal) | 12px |
| CardPreview content sizing | (literal) | 100% x 100% (block) |

# Card - Motion

Исходник `@fluentui/react-card` не задаёт токены перехода (`transition`) для смены состояний
(hover/pressed опираются на изменение фоновых/теневых токенов без явной анимации).
