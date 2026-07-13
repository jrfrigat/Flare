# SplitButton (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходников `@fluentui/react-button`
(`useSplitButtonStyles.styles.ts` + `useMenuButtonStyles.styles.ts`). В колонке `Token Name` -
реальный Fluent alias-токен, в колонке `Value` - его разрешённое значение.
Состояние `Pressed` в CSS соответствует `:hover:active, :active:focus-visible`.

SplitButton состоит из двух кнопок: `primaryActionButton` (основное действие) и `menuButton`
(триггер меню, `MenuButton`), разделённых вертикальным делителем. Обе кнопки - это обычный
`Button`, поэтому цвета лица (фон/текст/иконка/обводка) каждого appearance берутся ЦЕЛИКОМ из
таблиц Button (см. `docs/spec/button/fluentui2-spec.md`, разделы Primary/Secondary/Outline/
Subtle/Transparent). Ниже задокументированы только СПЕЦИФИЧНЫЕ для SplitButton слоты: делитель
(правая граница `primaryActionButton`), раскрытое состояние триггера меню (`aria-expanded` =
selected), иконка-шеврон меню, геометрия стыка и фокус.

# SplitButton - Divider - Primary (контекст: Default, Light)

Делитель = `border-right-color` слота `primaryActionButton`. Для Primary он фиксирован во всех
интерактивных состояниях.

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary divider color | `colorNeutralStrokeOnBrand` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary hovered divider color | `colorNeutralStrokeOnBrand` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary pressed divider color | `colorNeutralStrokeOnBrand` | #ffffff |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary disabled divider color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# SplitButton - Divider - Primary (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary divider color | `colorNeutralStrokeOnBrand` | #292929 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary hovered divider color | `colorNeutralStrokeOnBrand` | #292929 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary pressed divider color | `colorNeutralStrokeOnBrand` | #292929 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary disabled divider color | `colorNeutralStrokeDisabled` | #424242 |

# SplitButton - Divider - Secondary / Outline (контекст: Default, Light)

Secondary и Outline не переопределяют делитель (в исходнике `secondary`/`outline` пусты),
поэтому делитель = обычная правая граница кнопки Button (`colorNeutralStroke1` и её
hover/pressed-варианты).

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary divider color | `colorNeutralStroke1` | #d1d1d1 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary hovered divider color | `colorNeutralStroke1Hover` | #c7c7c7 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary pressed divider color | `colorNeutralStroke1Pressed` | #b3b3b3 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary disabled divider color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# SplitButton - Divider - Secondary / Outline (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary divider color | `colorNeutralStroke1` | #666666 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary hovered divider color | `colorNeutralStroke1Hover` | #757575 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary pressed divider color | `colorNeutralStroke1Pressed` | #6b6b6b |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary disabled divider color | `colorNeutralStrokeDisabled` | #424242 |

# SplitButton - Divider - Subtle / Transparent (контекст: Default, Light)

У subtle и transparent делитель прозрачный во всех состояниях.

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle divider color | `colorTransparentBackground` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle hovered divider color | `colorTransparentBackgroundHover` | transparent |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle pressed divider color | `colorTransparentBackgroundPressed` | transparent |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle disabled divider color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# SplitButton - Divider - Subtle / Transparent (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle divider color | `colorTransparentBackground` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle hovered divider color | `colorTransparentBackgroundHover` | transparent |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle pressed divider color | `colorTransparentBackgroundPressed` | transparent |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle disabled divider color | `colorNeutralStrokeDisabled` | #424242 |

# SplitButton - Menu trigger (expanded/selected) - Primary (контекст: Default, Light)

Когда меню раскрыто (`aria-expanded`), слот `menuButton` (MenuButton) переходит в selected-вид.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary expanded container color | `colorBrandBackgroundSelected` | #0f548c |
| SplitButton primary expanded label color | `colorNeutralForegroundOnBrand` | #ffffff |
| SplitButton primary expanded menu-icon color | `colorNeutralForegroundOnBrand` | #ffffff |

# SplitButton - Menu trigger (expanded/selected) - Primary (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton primary expanded container color | `colorBrandBackgroundSelected` | #0f548c |
| SplitButton primary expanded label color | `colorNeutralForegroundOnBrand` | #ffffff |
| SplitButton primary expanded menu-icon color | `colorNeutralForegroundOnBrand` | #ffffff |

# SplitButton - Menu trigger (expanded/selected) - Secondary (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary expanded container color | `colorNeutralBackground1Selected` | #ebebeb |
| SplitButton secondary expanded outline color | `colorNeutralStroke1Selected` | #bdbdbd |
| SplitButton secondary expanded label color | `colorNeutralForeground1Selected` | #242424 |
| SplitButton secondary expanded menu-icon color | `colorNeutralForeground1Selected` | #242424 |

# SplitButton - Menu trigger (expanded/selected) - Secondary (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton secondary expanded container color | `colorNeutralBackground1Selected` | #383838 |
| SplitButton secondary expanded outline color | `colorNeutralStroke1Selected` | #707070 |
| SplitButton secondary expanded label color | `colorNeutralForeground1Selected` | #ffffff |
| SplitButton secondary expanded menu-icon color | `colorNeutralForeground1Selected` | #ffffff |

# SplitButton - Menu trigger (expanded/selected) - Outline (контекст: Default, Light)

Outline в раскрытом состоянии усиливает обводку до `strokeWidthThicker`.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton outline expanded outline color | `colorNeutralStroke1Selected` | #bdbdbd |
| SplitButton outline expanded outline width | `strokeWidthThicker` | 3px |
| SplitButton outline expanded label color | `colorNeutralForeground1Selected` | #242424 |
| SplitButton outline expanded menu-icon color | `colorNeutralForeground1Selected` | #242424 |

# SplitButton - Menu trigger (expanded/selected) - Outline (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton outline expanded outline color | `colorNeutralStroke1Selected` | #707070 |
| SplitButton outline expanded outline width | `strokeWidthThicker` | 3px |
| SplitButton outline expanded label color | `colorNeutralForeground1Selected` | #ffffff |
| SplitButton outline expanded menu-icon color | `colorNeutralForeground1Selected` | #ffffff |

# SplitButton - Menu trigger (expanded/selected) - Subtle (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle expanded container color | `colorSubtleBackgroundSelected` | #ebebeb |
| SplitButton subtle expanded label color | `colorNeutralForeground2Selected` | #242424 |
| SplitButton subtle expanded menu-icon color | `colorNeutralForeground2BrandSelected` | #0f6cbd |

# SplitButton - Menu trigger (expanded/selected) - Subtle (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton subtle expanded container color | `colorSubtleBackgroundSelected` | #333333 |
| SplitButton subtle expanded label color | `colorNeutralForeground2Selected` | #ffffff |
| SplitButton subtle expanded menu-icon color | `colorNeutralForeground2BrandSelected` | #479ef5 |

# SplitButton - Menu trigger (expanded/selected) - Transparent (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton transparent expanded container color | `colorTransparentBackgroundSelected` | transparent |
| SplitButton transparent expanded label color | `colorNeutralForeground2BrandSelected` | #0f6cbd |
| SplitButton transparent expanded menu-icon color | `colorNeutralForeground2BrandSelected` | #0f6cbd |

# SplitButton - Menu trigger (expanded/selected) - Transparent (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SplitButton transparent expanded container color | `colorTransparentBackgroundSelected` | transparent |
| SplitButton transparent expanded label color | `colorNeutralForeground2BrandSelected` | #479ef5 |
| SplitButton transparent expanded menu-icon color | `colorNeutralForeground2BrandSelected` | #479ef5 |

# SplitButton - Menu icon (chevron) - Size

Слот `menuIcon` (шеврон вниз) в MenuButton. Размер зависит от size кнопки.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu icon size (small) | (literal) | 12px |
| Menu icon size (medium) | (literal) | 12px |
| Menu icon size (large) | (literal) | 16px |
| Menu icon line-height (small/medium) | `lineHeightBase200` | 16px |
| Menu icon line-height (large) | `lineHeightBase400` | 22px |
| Menu icon left gap (not icon-only) | `spacingHorizontalXS` | 4px |

# SplitButton - Geometry (стык двух кнопок)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu-button min target size | (literal `MIN_TARGET_SIZE`) | 24px |
| Primary action button right radius | (literal) | 0 |
| Menu button left radius | (literal) | 0 |
| Menu button left border width | (literal) | 0 |
| Container display | (literal) | inline-flex |

Обе половины наследуют геометрию Button (высота/паддинги/скругление внешних углов =
`borderRadiusMedium` 4px). Внутренние (стыковые) углы обнуляются: у `primaryActionButton`
скругление снимается справа, у `menuButton` - слева, и у `menuButton` убирается левая граница,
чтобы делитель был одинарным.

# SplitButton - Focus (контекст: Default, Light/Dark)

Фокус - двойное кольцо Fluent (как у Button), но индикатор обрезается по стыковой грани:
у `primaryActionButton` без правых радиусов, у `menuButton` без левого края/радиусов.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Focus inner stroke color | `colorStrokeFocus2` | #000000 | #ffffff |
| Focus outer stroke color | `colorStrokeFocus1` | #ffffff | #000000 |
| Focus inner stroke width | `strokeWidthThin` | 1px | 1px |
| Focus outer outline width | `strokeWidthThick` | 2px | 2px |

# SplitButton - Motion

Наследует переходы Button.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Transition duration | `durationFaster` | 100ms |
| Transition easing | `curveEasyEase` | cubic-bezier(0.33, 0, 0.67, 1) |
| Transition properties | (literal) | background, border, color |
