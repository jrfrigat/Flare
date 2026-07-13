# DatePicker (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходников `@fluentui/react-datepicker-compat`
(`DatePicker/useDatePickerStyles.styles.ts`), поля ввода `@fluentui/react-input`
(`Input/useInputStyles.styles.ts`) и календарной сетки `@fluentui/react-calendar-compat`
(`CalendarDayGrid/useCalendarDayGridStyles.styles.ts`). В колонке `Token Name` - реальный
Fluent alias-токен, в колонке `Value` - его разрешённое значение.
Состояние `Pressed` соответствует `:active, :focus-within` у поля.

DatePicker построен на текстовом поле `Input` (appearance по умолчанию - `outline`) плюс
поповер-поверхность (`popupSurface`) с календарём. Ниже задокументированы: поле, поверхность
поповера и ячейки дней календаря.

# DatePicker - Field (Input outline) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field container color | `colorNeutralBackground1` | #ffffff |
| Field outline color | `colorNeutralStroke1` | #d1d1d1 |
| Field bottom outline color | `colorNeutralStrokeAccessible` | #616161 |
| Field outline width | (literal) | 1px |
| Field input text color | `colorNeutralForeground1` | #242424 |
| Field placeholder color | `colorNeutralForeground4` | #707070 |
| Field icon color | `colorNeutralForeground3` | #616161 |

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

Нижняя focus-полоса (::after), анимируется по ширине.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field focus underline color | `colorCompoundBrandStroke` | #0f6cbd |
| Field focus underline (re-press) color | `colorCompoundBrandStrokePressed` | #0f548c |
| Field focus underline width | (literal) | 2px |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field disabled container color | `colorTransparentBackground` | transparent |
| Field disabled outline color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| Field disabled text color | `colorNeutralForegroundDisabled` | #bdbdbd |

# DatePicker - Field (Input outline) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field container color | `colorNeutralBackground1` | #292929 |
| Field outline color | `colorNeutralStroke1` | #666666 |
| Field bottom outline color | `colorNeutralStrokeAccessible` | #adadad |
| Field outline width | (literal) | 1px |
| Field input text color | `colorNeutralForeground1` | #ffffff |
| Field placeholder color | `colorNeutralForeground4` | #999999 |
| Field icon color | `colorNeutralForeground3` | #adadad |

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
| Field focus underline color | `colorCompoundBrandStroke` | #479ef5 |
| Field focus underline (re-press) color | `colorCompoundBrandStrokePressed` | #2886de |
| Field focus underline width | (literal) | 2px |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field disabled container color | `colorTransparentBackground` | transparent |
| Field disabled outline color | `colorNeutralStrokeDisabled` | #424242 |
| Field disabled text color | `colorNeutralForegroundDisabled` | #5c5c5c |

# DatePicker - Popup surface (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Popup surface color | `colorNeutralBackground1` | #ffffff |
| Popup text color | `colorNeutralForeground1` | #242424 |
| Popup border color | `colorTransparentStroke` | transparent |
| Popup border width | (literal) | 1px |
| Popup corner radius | `borderRadiusMedium` | 4px |
| Popup elevation | `shadow16` | 0 0 2px rgba(0,0,0,0.12), 0 8px 16px rgba(0,0,0,0.14) |

# DatePicker - Popup surface (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Popup surface color | `colorNeutralBackground1` | #292929 |
| Popup text color | `colorNeutralForeground1` | #ffffff |
| Popup border color | `colorTransparentStroke` | transparent |
| Popup border width | (literal) | 1px |
| Popup corner radius | `borderRadiusMedium` | 4px |
| Popup elevation | `shadow16` | 0 0 2px rgba(0,0,0,0.24), 0 8px 16px rgba(0,0,0,0.28) |

# DatePicker - Calendar day cell (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day cell text color | `colorNeutralForeground1` | #242424 |
| Day cell font size | `fontSizeBase200` | 12px |
| Day cell font weight | `fontWeightRegular` | 400 |
| Day button container color | `colorTransparentBackground` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day cell hovered container color | `colorBrandBackgroundInvertedHover` | #ebf3fc |
| Day cell hovered text color | `colorNeutralForeground1Static` | #242424 |
| Day button hover corner radius | `borderRadiusMedium` | 4px |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day cell pressed container color | `colorBrandBackgroundInvertedPressed` | #b4d6fa |
| Day cell pressed text color | `colorNeutralForeground1Static` | #242424 |

## Selected

`daySelected` - фон диапазона; `daySingleSelected` - обведённая кнопка выбранного дня.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day selected container color | `colorBrandBackgroundInvertedSelected` | #cfe4fa |
| Day selected text color | `colorNeutralForeground1Static` | #242424 |
| Single-selected button color | `colorBrandBackgroundInvertedSelected` | #cfe4fa |
| Single-selected button border | `colorBrandStroke1` | #0f6cbd |
| Single-selected button radius | `borderRadiusMedium` | 4px |

## Today

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Today text color | `colorNeutralForegroundOnBrand` | #ffffff |
| Today text weight | `fontWeightSemibold` | 600 |
| Today marker color | `colorBrandBackground` | #0f6cbd |

## Disabled (outside bounds)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day disabled container color | `colorTransparentBackground` | transparent |
| Day disabled text color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Day outside navigated month color | `colorNeutralForeground4` | #707070 |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day focus outer stroke color | `colorStrokeFocus1` | #ffffff |
| Day focus inner stroke color | `colorStrokeFocus2` | #000000 |
| Day focus stroke width | `strokeWidthThick` | 2px |

# DatePicker - Calendar day cell (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day cell text color | `colorNeutralForeground1` | #ffffff |
| Day cell font size | `fontSizeBase200` | 12px |
| Day cell font weight | `fontWeightRegular` | 400 |
| Day button container color | `colorTransparentBackground` | transparent |

## Hovered

Инвертированные brand-токены статичны между темами (значения одинаковы).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day cell hovered container color | `colorBrandBackgroundInvertedHover` | #ebf3fc |
| Day cell hovered text color | `colorNeutralForeground1Static` | #242424 |
| Day button hover corner radius | `borderRadiusMedium` | 4px |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day cell pressed container color | `colorBrandBackgroundInvertedPressed` | #b4d6fa |
| Day cell pressed text color | `colorNeutralForeground1Static` | #242424 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day selected container color | `colorBrandBackgroundInvertedSelected` | #cfe4fa |
| Day selected text color | `colorNeutralForeground1Static` | #242424 |
| Single-selected button color | `colorBrandBackgroundInvertedSelected` | #cfe4fa |
| Single-selected button border | `colorBrandStroke1` | #479ef5 |
| Single-selected button radius | `borderRadiusMedium` | 4px |

## Today

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Today text color | `colorNeutralForegroundOnBrand` | #ffffff |
| Today text weight | `fontWeightSemibold` | 600 |
| Today marker color | `colorBrandBackground` | #115ea3 |

## Disabled (outside bounds)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day disabled container color | `colorTransparentBackground` | transparent |
| Day disabled text color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Day outside navigated month color | `colorNeutralForeground4` | #999999 |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Day focus outer stroke color | `colorStrokeFocus1` | #000000 |
| Day focus inner stroke color | `colorStrokeFocus2` | #ffffff |
| Day focus stroke width | `strokeWidthThick` | 2px |

# DatePicker - Calendar supplementary (контекст: Light/Dark)

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Event dot (dayMarker) color | `colorBrandForeground2` | #115ea3 | #62abf5 |
| Week-number text color | `colorNeutralForeground4` | #707070 | #999999 |
| Week-number divider color | `colorNeutralStroke2` | #e0e0e0 | #525252 |

# DatePicker - Size / Geometry

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Field height (small) | (literal) | 24px |
| Field height (medium, default) | (literal) | 32px |
| Field height (large) | (literal) | 40px |
| Field corner radius | `borderRadiusMedium` | 4px |
| Day-grid table width | (literal) | 196px |
| Day-grid table width (with week numbers) | (literal) | 226px |
| Day button size | (literal) | 24x24px |
| Today marker circle size | (literal) | 20x20px |
| Event dot size | (literal) | 4x4px |

# DatePicker - Motion

Focus-полоса поля анимируется по ширине (scaleX).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Focus-in duration | `durationNormal` | 200ms |
| Focus-out duration | `durationUltraFast` | 50ms |
| Transition property | (literal) | transform |
