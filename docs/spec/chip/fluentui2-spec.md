# Chip (Fluent UI 2)

Fluent UI 2 не имеет компонента с именем "Chip"; его аналог - семейство **Tag** из
`@fluentui/react-tags`. Ниже - маппинг Flare Chip на два ближайших примитива: **Tag**
(статичный, с необязательной кнопкой закрытия) и **InteractionTagPrimary** (кликабельный chip-элемент).
Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки взяты из `useTagStyles.styles.ts` и `useInteractionTagPrimaryStyles.styles.ts`.
Рамка задаётся `strokeWidthThin solid colorTransparentStroke` (по умолчанию прозрачная);
`:active` в CSS соответствует Pressed.

# Chip (Tag) - Appearance - Filled (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag filled container color | `colorNeutralBackground3` | #f5f5f5 |
| Tag filled label color | `colorNeutralForeground2` | #424242 |
| Tag filled border color | `colorTransparentStroke` | transparent |
| Tag filled border width | `strokeWidthThin` | 1px |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag filled disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Tag filled disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Tag filled disabled border color | `colorTransparentStrokeDisabled` | transparent |

# Chip (Tag) - Appearance - Filled (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag filled container color | `colorNeutralBackground3` | #141414 |
| Tag filled label color | `colorNeutralForeground2` | #d6d6d6 |
| Tag filled border color | `colorTransparentStroke` | transparent |
| Tag filled border width | `strokeWidthThin` | 1px |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag filled disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Tag filled disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Tag filled disabled border color | `colorTransparentStrokeDisabled` | transparent |

# Chip (Tag) - Appearance - Outline (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag outline container color | `colorSubtleBackground` | transparent |
| Tag outline label color | `colorNeutralForeground2` | #424242 |
| Tag outline border color | `colorNeutralStroke1` | #d1d1d1 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag outline disabled container color | `colorSubtleBackground` | transparent |
| Tag outline disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Tag outline disabled border color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Chip (Tag) - Appearance - Outline (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag outline container color | `colorSubtleBackground` | transparent |
| Tag outline label color | `colorNeutralForeground2` | #d6d6d6 |
| Tag outline border color | `colorNeutralStroke1` | #666666 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag outline disabled container color | `colorSubtleBackground` | transparent |
| Tag outline disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Tag outline disabled border color | `colorNeutralStrokeDisabled` | #424242 |

# Chip (Tag) - Appearance - Brand (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag brand container color | `colorBrandBackground2` | #ebf3fc |
| Tag brand label color | `colorBrandForeground2` | #115ea3 |
| Tag brand border color | `colorTransparentStroke` | transparent |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag brand disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Tag brand disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Tag brand disabled border color | `colorTransparentStrokeDisabled` | transparent |

# Chip (Tag) - Appearance - Brand (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag brand container color | `colorBrandBackground2` | #082338 |
| Tag brand label color | `colorBrandForeground2` | #62abf5 |
| Tag brand border color | `colorTransparentStroke` | transparent |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tag brand disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Tag brand disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Tag brand disabled border color | `colorTransparentStrokeDisabled` | transparent |

# Chip (Tag) - Selected (контекст: Default, Light/Dark)

Состояние `selected` перекрывает appearance (единое для filled/outline/brand).

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Tag selected container color | `colorBrandBackground` | #0f6cbd | #115ea3 |
| Tag selected label color | `colorNeutralForegroundOnBrand` | #ffffff | #ffffff |
| Tag selected border color | `colorBrandStroke1` | #0f6cbd | #479ef5 |

# Chip (Tag) - Dismiss Icon (контекст: Default, Light/Dark)

Кнопка закрытия имеет hover/active состояния; для `selected` цвет метки остаётся `colorNeutralForegroundOnBrand`.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Tag dismiss hovered color (filled/outline/brand) | `colorCompoundBrandForeground1Hover` | #115ea3 | #62abf5 |
| Tag dismiss pressed color (filled/outline/brand) | `colorCompoundBrandForeground1Pressed` | #0f548c | #2886de |
| Tag dismiss hovered/pressed color (selected) | `colorNeutralForegroundOnBrand` | #ffffff | #ffffff |

# Chip (InteractionTagPrimary) - Appearance - Filled (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag filled container color | `colorNeutralBackground3` | #f5f5f5 |
| InteractionTag filled label color | `colorNeutralForeground2` | #424242 |
| InteractionTag filled border color | `colorTransparentStroke` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag filled hovered container color | `colorNeutralBackground3Hover` | #ebebeb |
| InteractionTag filled hovered label color | `colorNeutralForeground2Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag filled pressed container color | `colorNeutralBackground3Pressed` | #d6d6d6 |
| InteractionTag filled pressed label color | `colorNeutralForeground2Pressed` | #242424 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag filled disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| InteractionTag filled disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| InteractionTag filled disabled border color | `colorTransparentStrokeDisabled` | transparent |

# Chip (InteractionTagPrimary) - Appearance - Filled (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag filled container color | `colorNeutralBackground3` | #141414 |
| InteractionTag filled label color | `colorNeutralForeground2` | #d6d6d6 |
| InteractionTag filled border color | `colorTransparentStroke` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag filled hovered container color | `colorNeutralBackground3Hover` | #292929 |
| InteractionTag filled hovered label color | `colorNeutralForeground2Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag filled pressed container color | `colorNeutralBackground3Pressed` | #0a0a0a |
| InteractionTag filled pressed label color | `colorNeutralForeground2Pressed` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag filled disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| InteractionTag filled disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| InteractionTag filled disabled border color | `colorTransparentStrokeDisabled` | transparent |

# Chip (InteractionTagPrimary) - Appearance - Outline (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag outline container color | `colorSubtleBackground` | transparent |
| InteractionTag outline label color | `colorNeutralForeground2` | #424242 |
| InteractionTag outline border color | `colorNeutralStroke1` | #d1d1d1 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag outline hovered container color | `colorSubtleBackgroundHover` | #f5f5f5 |
| InteractionTag outline hovered label color | `colorNeutralForeground2Hover` | #242424 |
| InteractionTag outline hovered filled-icon color | `colorNeutralForeground2BrandHover` | #0f6cbd |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag outline pressed container color | `colorSubtleBackgroundPressed` | #e0e0e0 |
| InteractionTag outline pressed label color | `colorNeutralForeground2Pressed` | #242424 |
| InteractionTag outline pressed filled-icon color | `colorNeutralForeground2BrandPressed` | #115ea3 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag outline disabled container color | `colorSubtleBackground` | transparent |
| InteractionTag outline disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| InteractionTag outline disabled border color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Chip (InteractionTagPrimary) - Appearance - Outline (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag outline container color | `colorSubtleBackground` | transparent |
| InteractionTag outline label color | `colorNeutralForeground2` | #d6d6d6 |
| InteractionTag outline border color | `colorNeutralStroke1` | #666666 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag outline hovered container color | `colorSubtleBackgroundHover` | #383838 |
| InteractionTag outline hovered label color | `colorNeutralForeground2Hover` | #ffffff |
| InteractionTag outline hovered filled-icon color | `colorNeutralForeground2BrandHover` | #479ef5 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag outline pressed container color | `colorSubtleBackgroundPressed` | #2e2e2e |
| InteractionTag outline pressed label color | `colorNeutralForeground2Pressed` | #ffffff |
| InteractionTag outline pressed filled-icon color | `colorNeutralForeground2BrandPressed` | #2886de |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag outline disabled container color | `colorSubtleBackground` | transparent |
| InteractionTag outline disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| InteractionTag outline disabled border color | `colorNeutralStrokeDisabled` | #424242 |

# Chip (InteractionTagPrimary) - Appearance - Brand (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag brand container color | `colorBrandBackground2` | #ebf3fc |
| InteractionTag brand label color | `colorBrandForeground2` | #115ea3 |
| InteractionTag brand border color | `colorTransparentStroke` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag brand hovered container color | `colorBrandBackground2Hover` | #cfe4fa |
| InteractionTag brand hovered label color | `colorCompoundBrandForeground1Hover` | #115ea3 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag brand pressed container color | `colorBrandBackground2Pressed` | #96c6fa |
| InteractionTag brand pressed label color | `colorCompoundBrandForeground1Pressed` | #0f548c |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag brand disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| InteractionTag brand disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| InteractionTag brand disabled border color | `colorTransparentStrokeDisabled` | transparent |

# Chip (InteractionTagPrimary) - Appearance - Brand (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag brand container color | `colorBrandBackground2` | #082338 |
| InteractionTag brand label color | `colorBrandForeground2` | #62abf5 |
| InteractionTag brand border color | `colorTransparentStroke` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag brand hovered container color | `colorBrandBackground2Hover` | #0c3b5e |
| InteractionTag brand hovered label color | `colorCompoundBrandForeground1Hover` | #62abf5 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag brand pressed container color | `colorBrandBackground2Pressed` | #061724 |
| InteractionTag brand pressed label color | `colorCompoundBrandForeground1Pressed` | #2886de |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag brand disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| InteractionTag brand disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| InteractionTag brand disabled border color | `colorTransparentStrokeDisabled` | transparent |

# Chip (InteractionTagPrimary) - Selected (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag selected container color | `colorBrandBackground` | #0f6cbd |
| InteractionTag selected label color | `colorNeutralForegroundOnBrand` | #ffffff |
| InteractionTag selected border color | `colorBrandStroke1` | #0f6cbd |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag selected hovered container color | `colorBrandBackgroundHover` | #115ea3 |
| InteractionTag selected hovered label color | `colorNeutralForegroundOnBrand` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag selected pressed container color | `colorBrandBackgroundPressed` | #0c3b5e |
| InteractionTag selected pressed label color | `colorNeutralForegroundOnBrand` | #ffffff |

# Chip (InteractionTagPrimary) - Selected (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag selected container color | `colorBrandBackground` | #115ea3 |
| InteractionTag selected label color | `colorNeutralForegroundOnBrand` | #ffffff |
| InteractionTag selected border color | `colorBrandStroke1` | #479ef5 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag selected hovered container color | `colorBrandBackgroundHover` | #0f6cbd |
| InteractionTag selected hovered label color | `colorNeutralForegroundOnBrand` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| InteractionTag selected pressed container color | `colorBrandBackgroundPressed` | #0c3b5e |
| InteractionTag selected pressed label color | `colorNeutralForegroundOnBrand` | #ffffff |

# Chip - Focus (контекст: Default, Light/Dark)

Кастомный индикатор фокуса (`createCustomFocusIndicatorStyle`): внешняя обводка
`strokeWidthThick solid colorStrokeFocus2`, радиус равен форме chip.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Chip focus outline color | `colorStrokeFocus2` | #000000 | #ffffff |
| Chip focus outline width | `strokeWidthThick` | 2px | 2px |
| Chip focus radius (rounded) | `borderRadiusMedium` | 4px | 4px |
| Chip focus radius (circular) | `borderRadiusCircular` | 10000px | 10000px |

# Chip - Size (theme-independent)

Высота задаётся контейнером (Tag / InteractionTag); внутренние отступы - литералы (7px/5px/5px);
размер иконки/dismiss - литералы.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Chip medium height (default) | (literal) | 32px |
| Chip medium inline padding | (literal `tagSpacingMedium`) | 7px |
| Chip medium icon size | (literal `mediumIconSize`) | 20px |
| Chip small height | (literal) | 24px |
| Chip small inline padding | (literal `tagSpacingSmall`) | 5px |
| Chip small icon size | (literal `smallIconSize`) | 16px |
| Chip extra-small height | (literal) | 20px |
| Chip extra-small inline padding | (literal `tagSpacingExtraSmall`) | 5px |
| Chip extra-small icon size | (literal `extraSmallIconSize`) | 12px |

# Chip - Shape (theme-independent)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Chip shape rounded (default) | `borderRadiusMedium` | 4px |
| Chip shape circular | `borderRadiusCircular` | 10000px |
| Chip border width | `strokeWidthThin` | 1px |

# Chip - Typography (theme-independent)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Chip primary text (medium) | `body1` | 14px / 20px / 400 |
| Chip primary text (small / extra-small) | `caption1` | 12px / 16px / 400 |
| Chip primary text (with secondary) | `caption1` | 12px / 16px / 400 |
| Chip secondary text | `caption2` | 10px / 14px / 400 |

# Chip - Motion

Исходник `@fluentui/react-tags` не задаёт токены перехода (`transition`); смена состояний
опирается на переключение цветовых токенов без явной анимации.
