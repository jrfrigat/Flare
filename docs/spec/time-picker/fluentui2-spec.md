# TimePicker (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходников `@fluentui/react-timepicker-compat`
(`TimePicker/useTimePickerStyles.styles.ts`) и его основы `@fluentui/react-combobox`
(`Combobox/useComboboxStyles.styles.ts`, `Option/useOptionStyles.styles.ts`). В колонке
`Token Name` - реальный Fluent alias-токен, в колонке `Value` - его разрешённое значение.
Состояние `Pressed` соответствует `:active` (поле) / `:active` (опция).

**TimePicker построен на Combobox** (`useComboboxStyles_unstable`): TimePicker добавляет только
`max-height` списка, всё оформление поля/списка/опций - это Combobox. Ниже задокументированы:
поле (Combobox root, appearance `outline`), опции списка и поверхность listbox.

# TimePicker - Field (Combobox outline) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field container color | `colorNeutralBackground1` | #ffffff |
| Field outline color | `colorNeutralStroke1` | #d1d1d1 |
| Field bottom outline color | `colorNeutralStrokeAccessible` | #616161 |
| Field outline width | `strokeWidthThin` | 1px |
| Field input text color | `colorNeutralForeground1` | #242424 |
| Field placeholder color | `colorNeutralForeground4` | #707070 |
| Field expand/clear icon color | `colorNeutralStrokeAccessible` | #616161 |
| Field corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field hovered outline color | `colorNeutralStroke1Hover` | #c7c7c7 |
| Field hovered bottom outline color | `colorNeutralStrokeAccessibleHover` | #575757 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field pressed outline color | `colorNeutralStroke1Pressed` | #b3b3b3 |
| Field pressed bottom outline color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |

## Focused

`:focus-within` красит обводку в pressed-цвета; нижняя focus-полоса (::after) анимируется.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field focus outline color | `colorNeutralStroke1Pressed` | #b3b3b3 |
| Field focus bottom outline color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |
| Field focus underline color | `colorCompoundBrandStroke` | #0f6cbd |
| Field focus underline (re-press) color | `colorCompoundBrandStrokePressed` | #0f548c |
| Field focus underline width | `strokeWidthThick` | 2px |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field disabled container color | `colorTransparentBackground` | transparent |
| Field disabled outline color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Field disabled text color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Field disabled icon color | `colorNeutralForegroundDisabled` | #bdbdbd |

# TimePicker - Field (Combobox outline) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field container color | `colorNeutralBackground1` | #292929 |
| Field outline color | `colorNeutralStroke1` | #666666 |
| Field bottom outline color | `colorNeutralStrokeAccessible` | #adadad |
| Field outline width | `strokeWidthThin` | 1px |
| Field input text color | `colorNeutralForeground1` | #ffffff |
| Field placeholder color | `colorNeutralForeground4` | #999999 |
| Field expand/clear icon color | `colorNeutralStrokeAccessible` | #adadad |
| Field corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field hovered outline color | `colorNeutralStroke1Hover` | #757575 |
| Field hovered bottom outline color | `colorNeutralStrokeAccessibleHover` | #bdbdbd |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field pressed outline color | `colorNeutralStroke1Pressed` | #6b6b6b |
| Field pressed bottom outline color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field focus outline color | `colorNeutralStroke1Pressed` | #6b6b6b |
| Field focus bottom outline color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |
| Field focus underline color | `colorCompoundBrandStroke` | #479ef5 |
| Field focus underline (re-press) color | `colorCompoundBrandStrokePressed` | #2886de |
| Field focus underline width | `strokeWidthThick` | 2px |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field disabled container color | `colorTransparentBackground` | transparent |
| Field disabled outline color | `colorNeutralStrokeDisabled` | #424242 |
| Field disabled text color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Field disabled icon color | `colorNeutralForegroundDisabled` | #5c5c5c |

# TimePicker - Option (listbox item) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option text color | `colorNeutralForeground1` | #242424 |
| Option corner radius | `borderRadiusMedium` | 4px |
| Option font size | `fontSizeBase300` | 14px |
| Option line-height | `lineHeightBase300` | 20px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option hovered container color | `colorNeutralBackground1Hover` | #f5f5f5 |
| Option hovered text color | `colorNeutralForeground1Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option pressed container color | `colorNeutralBackground1Pressed` | #e0e0e0 |
| Option pressed text color | `colorNeutralForeground1Pressed` | #242424 |

## Selected

Одиночный выбор: фон не меняется, показывается галочка (`checkIcon`).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option selected check icon size | `fontSizeBase400` | 16px |
| Option selected text color | `colorNeutralForeground1` | #242424 |

## Focused

Активный (active-descendant) элемент получает рамку-индикатор через ::after.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option focus stroke color | `colorStrokeFocus2` | #000000 |
| Option focus stroke width | (literal) | 2px |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option disabled text color | `colorNeutralForegroundDisabled` | #bdbdbd |

# TimePicker - Option (listbox item) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option text color | `colorNeutralForeground1` | #ffffff |
| Option corner radius | `borderRadiusMedium` | 4px |
| Option font size | `fontSizeBase300` | 14px |
| Option line-height | `lineHeightBase300` | 20px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option hovered container color | `colorNeutralBackground1Hover` | #3d3d3d |
| Option hovered text color | `colorNeutralForeground1Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option pressed container color | `colorNeutralBackground1Pressed` | #1f1f1f |
| Option pressed text color | `colorNeutralForeground1Pressed` | #ffffff |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option selected check icon size | `fontSizeBase400` | 16px |
| Option selected text color | `colorNeutralForeground1` | #ffffff |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option focus stroke color | `colorStrokeFocus2` | #ffffff |
| Option focus stroke width | (literal) | 2px |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Option disabled text color | `colorNeutralForegroundDisabled` | #5c5c5c |

# TimePicker - Listbox surface (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Listbox corner radius | `borderRadiusMedium` | 4px |
| Listbox elevation | `shadow16` | 0 0 2px rgba(0,0,0,0.12), 0 8px 16px rgba(0,0,0,0.14) |
| Listbox max height | (literal) | min(80vh, 416px) |

# TimePicker - Listbox surface (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Listbox corner radius | `borderRadiusMedium` | 4px |
| Listbox elevation | `shadow16` | 0 0 2px rgba(0,0,0,0.24), 0 8px 16px rgba(0,0,0,0.28) |
| Listbox max height | (literal) | min(80vh, 416px) |

# TimePicker - Size / Geometry

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field height (small) | (literal) | 24px |
| Field height (medium, default) | (literal) | 32px |
| Field height (large) | (literal) | 40px |
| Field min width | (literal) | 250px |
| Field corner radius | `borderRadiusMedium` | 4px |
| Expand/clear icon size (small) | (literal `iconSizes.small`) | 16px |
| Expand/clear icon size (medium) | (literal `iconSizes.medium`) | 20px |
| Expand/clear icon size (large) | (literal `iconSizes.large`) | 24px |
| Option padding block | `spacingVerticalSNudge` | 6px |
| Option padding inline | `spacingHorizontalS` | 8px |

# TimePicker - Motion

Focus-полоса поля анимируется по ширине (scaleX), как у Input/Combobox.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Focus-in duration | `durationNormal` | 200ms |
| Focus-out duration | `durationUltraFast` | 50ms |
| Transition property | (literal) | transform |
