# Carousel (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-carousel`
(`useCarouselStyles`, `useCarouselNavButtonStyles`, `useCarouselButtonStyles`,
`useCarouselCardStyles`, `useCarouselNavStyles`). В колонке `Token Name` - реальный Fluent
alias-токен, в колонке `Value` - его разрешённое значение. `:active` в CSS соответствует Pressed;
непрозрачность точки навигации (`opacity`) задаётся литералами по состояниям.

# Carousel - Root (контекст: Default, Light/Dark)

Корень скрывает горизонтальное переполнение; вариант `elevated` резервирует место под тень.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Carousel elevated margin | `spacingVerticalL` | 16px | 16px |
| Carousel elevated padding | `spacingVerticalL` | 16px | 16px |
| Carousel overflow-x | (literal) | hidden | hidden |

# Carousel - Nav Dot (CarouselNavButton) - Default (контекст: Default, Light)

Точка рисуется псевдоэлементом `::after`; цвет заливки - `colorNeutralForeground1`,
видимость управляется `opacity`.

## Enabled (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot fill color | `colorNeutralForeground1` | #242424 |
| Nav dot opacity | (literal) | 0.6 |
| Nav dot track color | `colorTransparentBackground` | transparent |

## Hovered (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot hovered opacity | (literal) | 0.75 |

## Pressed (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot pressed opacity | (literal) | 1 |

## Selected (active dot)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot selected fill color | `colorNeutralForeground1` | #242424 |
| Nav dot selected opacity | (literal) | 1 |
| Nav dot selected hovered opacity | (literal) | 0.75 |
| Nav dot selected pressed opacity | (literal) | 0.65 |

# Carousel - Nav Dot (CarouselNavButton) - Default (контекст: Default, Dark)

## Enabled (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot fill color | `colorNeutralForeground1` | #ffffff |
| Nav dot opacity | (literal) | 0.6 |
| Nav dot track color | `colorTransparentBackground` | transparent |

## Hovered (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot hovered opacity | (literal) | 0.75 |

## Pressed (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot pressed opacity | (literal) | 1 |

## Selected (active dot)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot selected fill color | `colorNeutralForeground1` | #ffffff |
| Nav dot selected opacity | (literal) | 1 |
| Nav dot selected hovered opacity | (literal) | 0.75 |
| Nav dot selected pressed opacity | (literal) | 0.65 |

# Carousel - Nav Dot (CarouselNavButton) - Brand (контекст: Default, Light)

Вариант `appearance="brand"`: активная точка использует compound-brand заливку,
неактивная (`unselectedBrand`) - нейтральную с пониженной непрозрачностью.

## Enabled (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot brand unselected fill | `colorNeutralForeground1` | #242424 |
| Nav dot brand unselected opacity | (literal) | 0.6 |

## Hovered / Pressed (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot brand unselected hovered opacity | (literal) | 0.75 |
| Nav dot brand unselected pressed opacity | (literal) | 1 |

## Selected (active dot)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot brand selected fill | `colorCompoundBrandBackground` | #0f6cbd |
| Nav dot brand selected hovered fill | `colorCompoundBrandBackgroundHover` | #115ea3 |
| Nav dot brand selected pressed fill | `colorCompoundBrandBackgroundPressed` | #0f548c |
| Nav dot brand selected opacity | (literal) | 1 |

# Carousel - Nav Dot (CarouselNavButton) - Brand (контекст: Default, Dark)

## Enabled (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot brand unselected fill | `colorNeutralForeground1` | #ffffff |
| Nav dot brand unselected opacity | (literal) | 0.6 |

## Hovered / Pressed (unselected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot brand unselected hovered opacity | (literal) | 0.75 |
| Nav dot brand unselected pressed opacity | (literal) | 1 |

## Selected (active dot)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot brand selected fill | `colorCompoundBrandBackground` | #479ef5 |
| Nav dot brand selected hovered fill | `colorCompoundBrandBackgroundHover` | #62abf5 |
| Nav dot brand selected pressed fill | `colorCompoundBrandBackgroundPressed` | #2886de |
| Nav dot brand selected opacity | (literal) | 1 |

# Carousel - Nav Dot - Focus (контекст: Default, Light/Dark)

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Nav dot focus border color | `colorStrokeFocus2` | #000000 | #ffffff |
| Nav dot focus border width | `strokeWidthThick` | 2px | 2px |
| Nav dot focus radius | `borderRadiusMedium` | 4px | 4px |
| Nav dot high-contrast outline | `strokeWidthThin` | 1px | 1px |

# Carousel - Nav Dot - Size (theme-independent)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav dot width (unselected) | `spacingHorizontalS` | 8px |
| Nav dot height | `spacingVerticalS` | 8px |
| Nav dot hit-area padding | `spacingVerticalS` / `spacingHorizontalS` | 8px 8px |
| Nav dot width (selected) | `spacingHorizontalL` | 16px |
| Nav dot hit-area padding (selected) | `spacingVerticalS` / `spacingHorizontalXS` | 8px 4px |
| Nav dot corner radius (unselected) | (literal) | 50% |
| Nav dot corner radius (selected) | (literal) | 4px |

# Carousel - Nav Button (CarouselButton) (контекст: Default, Light)

Стрелки prev/next: композиция `@fluentui/react-button` (базовые токены Button) с оверрайдом
цвета/фона поверх полупрозрачной нейтральной подложки.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav button icon/label color | `colorNeutralForeground2` | #424242 |
| Nav button container color | `colorNeutralBackgroundAlpha` | rgba(255, 255, 255, 0.5) |

# Carousel - Nav Button (CarouselButton) (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav button icon/label color | `colorNeutralForeground2` | #d6d6d6 |
| Nav button container color | `colorNeutralBackgroundAlpha` | rgba(26, 26, 26, 0.5) |

# Carousel - Nav Container (CarouselNav) (контекст: Default, Light)

Контейнер точек навигации: полупрозрачная нейтральная подложка со скруглением.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav container background | `colorNeutralBackgroundAlpha` | rgba(255, 255, 255, 0.5) |
| Nav container corner radius | `borderRadiusXLarge` | 8px |
| Nav container inline margin | `spacingHorizontalS` | 8px |
| Nav container focus outline color | `colorStrokeFocus2` | #000000 |
| Nav container focus outline width | `strokeWidthThick` | 2px |
| Nav container focus radius | `borderRadiusMedium` | 4px |

# Carousel - Nav Container (CarouselNav) (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Nav container background | `colorNeutralBackgroundAlpha` | rgba(26, 26, 26, 0.5) |
| Nav container corner radius | `borderRadiusXLarge` | 8px |
| Nav container inline margin | `spacingHorizontalS` | 8px |
| Nav container focus outline color | `colorStrokeFocus2` | #ffffff |
| Nav container focus outline width | `strokeWidthThick` | 2px |
| Nav container focus radius | `borderRadiusMedium` | 4px |

# Carousel - Card (CarouselCard) (контекст: Default, Light/Dark)

Слайд занимает 100% ширины вьюпорта; вариант `elevated` добавляет скругление и тень.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Card flex basis | (literal) | 100% | 100% |
| Card elevated corner radius | `borderRadiusXLarge` | 8px | 8px |
| Card elevated elevation | `shadow16` | 0 0 2px rgba(0,0,0,0.12), 0 8px 16px rgba(0,0,0,0.14) | 0 0 2px rgba(0,0,0,0.24), 0 8px 16px rgba(0,0,0,0.28) |

# Carousel - Motion

Исходник `@fluentui/react-carousel` не задаёт токены темы для перехода слайдов
(прокрутка реализована библиотекой Embla, а не CSS-`transition` на токенах);
изменения точек навигации анимируются только через `opacity` без явных motion-токенов.
