# Slider (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-slider`
(`useSliderStyles.styles.ts`). В колонке `Token Name` - реальный Fluent alias-токен
(см. `docs/spec/_pallete/fluentui2-spec.md`), в колонке `Value` - его разрешённое значение.
Состояние `Pressed` в CSS соответствует `:active`. Прогресс (заполненная часть) рисуется
градиентом по слоту `rail`: `progressColor` до значения, далее `railColor`.

# Slider - Color - Default (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider rail color | `colorNeutralStrokeAccessible` | #616161 |
| Slider progress (filled track) color | `colorCompoundBrandBackground` | #0f6cbd |
| Slider thumb color | `colorCompoundBrandBackground` | #0f6cbd |
| Slider thumb inner ring color | `colorNeutralBackground1` | #ffffff |
| Slider thumb outer border color | `colorNeutralStroke1` | #d1d1d1 |
| Slider rail outline color | `colorTransparentStroke` | transparent |
| Slider step marks color | `colorNeutralBackground1` | #ffffff |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider hovered progress color | `colorCompoundBrandBackgroundHover` | #115ea3 |
| Slider hovered thumb color | `colorCompoundBrandBackgroundHover` | #115ea3 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider pressed progress color | `colorCompoundBrandBackgroundPressed` | #0f548c |
| Slider pressed thumb color | `colorCompoundBrandBackgroundPressed` | #0f548c |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider disabled rail color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Slider disabled progress color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Slider disabled thumb color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Slider disabled thumb outer border color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Slider - Color - Default (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider rail color | `colorNeutralStrokeAccessible` | #adadad |
| Slider progress (filled track) color | `colorCompoundBrandBackground` | #479ef5 |
| Slider thumb color | `colorCompoundBrandBackground` | #479ef5 |
| Slider thumb inner ring color | `colorNeutralBackground1` | #292929 |
| Slider thumb outer border color | `colorNeutralStroke1` | #666666 |
| Slider rail outline color | `colorTransparentStroke` | transparent |
| Slider step marks color | `colorNeutralBackground1` | #292929 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider hovered progress color | `colorCompoundBrandBackgroundHover` | #62abf5 |
| Slider hovered thumb color | `colorCompoundBrandBackgroundHover` | #62abf5 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider pressed progress color | `colorCompoundBrandBackgroundPressed` | #2886de |
| Slider pressed thumb color | `colorCompoundBrandBackgroundPressed` | #2886de |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider disabled rail color | `colorNeutralBackgroundDisabled` | #141414 |
| Slider disabled progress color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Slider disabled thumb color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Slider disabled thumb outer border color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Slider - Focus (контекст: Default, Light/Dark)

Фокус применён к root по `:focus-within` через `createFocusOutlineStyle` (Fluent 2 -
двойное кольцо: внутреннее `colorStrokeFocus2`, внешнее `colorStrokeFocus1`). Смещение
`outlineOffset` подстроено под горизонтальную/вертикальную ориентацию.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Slider focus inner stroke color | `colorStrokeFocus2` | #000000 | #ffffff |
| Slider focus outer stroke color | `colorStrokeFocus1` | #ffffff | #000000 |
| Slider focus inner stroke width | `strokeWidthThin` | 1px | 1px |
| Slider focus outer outline width | `strokeWidthThick` | 2px | 2px |

# Slider - Size - Medium (default)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider medium thumb size | (literal `sliderThumbSizeVar`) | 20px |
| Slider medium inner thumb radius | (literal `sliderInnerThumbRadiusVar`) | 6px |
| Slider medium rail size (thickness) | (literal `sliderRailSizeVar`) | 4px |
| Slider medium min height | (literal) | 32px |
| Slider medium min width (horizontal) | (literal) | 120px |
| Slider rail corner radius | `borderRadiusXLarge` | 8px |
| Slider rail outline width | (literal) | 1px |
| Slider thumb corner radius | `borderRadiusCircular` | 10000px |
| Slider thumb inner ring width | (derived: thumb size * 0.2) | 4px |
| Slider thumb outer border width | (derived: thumb size * 0.05) | 1px |

# Slider - Size - Small

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Slider small thumb size | (literal `sliderThumbSizeVar`) | 16px |
| Slider small inner thumb radius | (literal `sliderInnerThumbRadiusVar`) | 5px |
| Slider small rail size (thickness) | (literal `sliderRailSizeVar`) | 2px |
| Slider small min height | (literal) | 24px |
| Slider small min width (horizontal) | (literal) | 120px |
| Slider rail corner radius | `borderRadiusXLarge` | 8px |
| Slider rail outline width | (literal) | 1px |
| Slider thumb corner radius | `borderRadiusCircular` | 10000px |
| Slider thumb inner ring width | (derived: thumb size * 0.2) | 3.2px |
| Slider thumb outer border width | (derived: thumb size * 0.05) | 0.8px |
