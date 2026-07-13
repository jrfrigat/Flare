# Button group (Fluent UI 2)

**Fluent UI 2 не имеет прямого аналога Button group (группы кнопок / сегментированного
контрола); ниже - маппинг на ближайший примитив: сгруппированные ToggleButton
(`@fluentui/react-button`, сегментация в духе Toolbar). Значения приблизительны и
требуют сверки с Figma.**

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки взяты из исходника `@fluentui/react-button` (`useToggleButtonStyles.styles.ts`;
невыбранные rest-состояния наследуются от `useButtonStyles.styles.ts`). В колонке
`Token Name` - реальный Fluent alias-токен, в колонке `Value` - разрешённое значение.
Состояние `Pressed` в CSS соответствует `:hover:active, :active:focus-visible`.
ToggleButton не имеет собственного токена «группировки»: секция `Grouping geometry`
ниже - приближение общей границы/радиуса соседних элементов и подлежит сверке с Figma.

# Button group - Item unchecked (secondary, default) (контекст: Default, Light)

Невыбранный элемент = базовая (secondary) кнопка.

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item container color | `colorNeutralBackground1` | #ffffff |
| Group item outline color | `colorNeutralStroke1` | #d1d1d1 |
| Group item outline width | `strokeWidthThin` | 1px |
| Group item label color | `colorNeutralForeground1` | #242424 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item hovered container color | `colorNeutralBackground1Hover` | #f5f5f5 |
| Group item hovered outline color | `colorNeutralStroke1Hover` | #c7c7c7 |
| Group item hovered label color | `colorNeutralForeground1Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item pressed container color | `colorNeutralBackground1Pressed` | #e0e0e0 |
| Group item pressed outline color | `colorNeutralStroke1Pressed` | #b3b3b3 |
| Group item pressed label color | `colorNeutralForeground1Pressed` | #242424 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Group item disabled outline color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Group item disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Button group - Item unchecked (secondary, default) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item container color | `colorNeutralBackground1` | #292929 |
| Group item outline color | `colorNeutralStroke1` | #666666 |
| Group item outline width | `strokeWidthThin` | 1px |
| Group item label color | `colorNeutralForeground1` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item hovered container color | `colorNeutralBackground1Hover` | #3d3d3d |
| Group item hovered outline color | `colorNeutralStroke1Hover` | #757575 |
| Group item hovered label color | `colorNeutralForeground1Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item pressed container color | `colorNeutralBackground1Pressed` | #1f1f1f |
| Group item pressed outline color | `colorNeutralStroke1Pressed` | #6b6b6b |
| Group item pressed label color | `colorNeutralForeground1Pressed` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Group item disabled outline color | `colorNeutralStrokeDisabled` | #424242 |
| Group item disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Button group - Item checked (secondary, default) (контекст: Default, Light)

Выбранный элемент = ToggleButton `checked` (secondary).

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item checked container color | `colorNeutralBackground1Selected` | #ebebeb |
| Group item checked outline color | `colorNeutralStroke1` | #d1d1d1 |
| Group item checked outline width | `strokeWidthThin` | 1px |
| Group item checked label color | `colorNeutralForeground1Selected` | #242424 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item checked hovered container color | `colorNeutralBackground1Hover` | #f5f5f5 |
| Group item checked hovered outline color | `colorNeutralStroke1Hover` | #c7c7c7 |
| Group item checked hovered label color | `colorNeutralForeground1Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item checked pressed container color | `colorNeutralBackground1Pressed` | #e0e0e0 |
| Group item checked pressed outline color | `colorNeutralStroke1Pressed` | #b3b3b3 |
| Group item checked pressed label color | `colorNeutralForeground1Pressed` | #242424 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item checked disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Group item checked disabled outline color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Group item checked disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Button group - Item checked (secondary, default) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item checked container color | `colorNeutralBackground1Selected` | #383838 |
| Group item checked outline color | `colorNeutralStroke1` | #666666 |
| Group item checked outline width | `strokeWidthThin` | 1px |
| Group item checked label color | `colorNeutralForeground1Selected` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item checked hovered container color | `colorNeutralBackground1Hover` | #3d3d3d |
| Group item checked hovered outline color | `colorNeutralStroke1Hover` | #757575 |
| Group item checked hovered label color | `colorNeutralForeground1Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item checked pressed container color | `colorNeutralBackground1Pressed` | #1f1f1f |
| Group item checked pressed outline color | `colorNeutralStroke1Pressed` | #6b6b6b |
| Group item checked pressed label color | `colorNeutralForeground1Pressed` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item checked disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Group item checked disabled outline color | `colorNeutralStrokeDisabled` | #424242 |
| Group item checked disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Button group - Item checked (primary/brand) (контекст: Default, Light)

Опциональный вариант: активный сегмент с brand-заливкой = ToggleButton `checked`
appearance `primary` (внутренний штрих `colorNeutralForegroundOnBrand` в accessible-режиме).

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item (brand) checked container color | `colorBrandBackgroundSelected` | #0f548c |
| Group item (brand) checked outline color | (transparent) | transparent |
| Group item (brand) checked label color | `colorNeutralForegroundOnBrand` | #ffffff |
| Group item (brand) checked inner stroke color | `colorNeutralForegroundOnBrand` | #ffffff |
| Group item (brand) checked inner stroke width | `strokeWidthThin` | 1px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item (brand) checked hovered container color | `colorBrandBackgroundHover` | #115ea3 |
| Group item (brand) checked hovered label color | `colorNeutralForegroundOnBrand` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item (brand) checked pressed container color | `colorBrandBackgroundPressed` | #0c3b5e |
| Group item (brand) checked pressed label color | `colorNeutralForegroundOnBrand` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item (brand) checked disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Group item (brand) checked disabled outline color | (transparent) | transparent |
| Group item (brand) checked disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Button group - Item checked (primary/brand) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item (brand) checked container color | `colorBrandBackgroundSelected` | #0f548c |
| Group item (brand) checked outline color | (transparent) | transparent |
| Group item (brand) checked label color | `colorNeutralForegroundOnBrand` | #ffffff |
| Group item (brand) checked inner stroke color | `colorNeutralForegroundOnBrand` | #ffffff |
| Group item (brand) checked inner stroke width | `strokeWidthThin` | 1px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item (brand) checked hovered container color | `colorBrandBackgroundHover` | #0f6cbd |
| Group item (brand) checked hovered label color | `colorNeutralForegroundOnBrand` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item (brand) checked pressed container color | `colorBrandBackgroundPressed` | #0c3b5e |
| Group item (brand) checked pressed label color | `colorNeutralForegroundOnBrand` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item (brand) checked disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Group item (brand) checked disabled outline color | (transparent) | transparent |
| Group item (brand) checked disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Button group - Grouping geometry

Приближение (у Fluent нет токена группы): внешние углы группы скруглены, внутренние
(смежные) углы прямые, общая граница соседних элементов схлопывается на толщину штриха.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group outer corner radius | `borderRadiusMedium` | 4px |
| Group inner (shared-edge) corner radius | `borderRadiusNone` | 0 |
| Shared / collapsed border width | `strokeWidthThin` | 1px |
| Selected item border (outline appearance) width | `strokeWidthThicker` | 3px |

# Button group - Size

Наследуется от Button (высоты по размеру, radius medium вне углов группы).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item small height (effective) | (derived) | 24px |
| Group item medium height (effective, default) | (derived) | 32px |
| Group item large height (effective) | (derived) | 40px |
| Group item corner radius (base) | `borderRadiusMedium` | 4px |

# Button group - Motion

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Group item transition duration | `durationFaster` | 100ms |
| Group item transition easing | `curveEasyEase` | cubic-bezier(0.33, 0, 0.67, 1) |
| Group item transition properties | (literal) | background, border, color |
