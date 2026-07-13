# Tabs (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходников `@fluentui/react-tabs`
(`useTabStyles.styles.ts`, `useTabListStyles.styles.ts`). В колонке `Token Name` - реальный
Fluent alias-токен (см. `docs/spec/_pallete/fluentui2-spec.md`), в колонке `Value` - его
разрешённое значение. Состояние `Pressed` в CSS соответствует `:enabled:active`; `Hovered` =
`:enabled:hover`. Tab имеет две группы appearance: линейные (`transparent` по умолчанию,
`subtle`) с полосой-индикатором и круглые «pill» (`subtle-circular`, `filled-circular`).

# Tab - Color - Transparent (default) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab container color | `colorTransparentBackground` | transparent |
| Tab label color | `colorNeutralForeground2` | #424242 |
| Tab icon color | `colorNeutralForeground2` | #424242 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab hovered container color | `colorTransparentBackgroundHover` | transparent |
| Tab hovered label color | `colorNeutralForeground2Hover` | #242424 |
| Tab hovered icon color | `colorNeutralForeground2Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pressed container color | `colorTransparentBackgroundPressed` | transparent |
| Tab pressed label color | `colorNeutralForeground2Pressed` | #242424 |
| Tab pressed icon color | `colorNeutralForeground2Pressed` | #242424 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab selected label color | `colorNeutralForeground1` | #242424 |
| Tab selected icon color | `colorCompoundBrandForeground1` | #0f6cbd |
| Tab selected hovered label color | `colorNeutralForeground1Hover` | #242424 |
| Tab selected hovered icon color | `colorCompoundBrandForeground1Hover` | #115ea3 |
| Tab selected pressed label color | `colorNeutralForeground1Pressed` | #242424 |
| Tab selected pressed icon color | `colorCompoundBrandForeground1Pressed` | #0f548c |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab disabled container color | `colorTransparentBackground` | transparent |
| Tab disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Tab disabled icon color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Tab - Color - Transparent (default) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab container color | `colorTransparentBackground` | transparent |
| Tab label color | `colorNeutralForeground2` | #d6d6d6 |
| Tab icon color | `colorNeutralForeground2` | #d6d6d6 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab hovered container color | `colorTransparentBackgroundHover` | transparent |
| Tab hovered label color | `colorNeutralForeground2Hover` | #ffffff |
| Tab hovered icon color | `colorNeutralForeground2Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pressed container color | `colorTransparentBackgroundPressed` | transparent |
| Tab pressed label color | `colorNeutralForeground2Pressed` | #ffffff |
| Tab pressed icon color | `colorNeutralForeground2Pressed` | #ffffff |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab selected label color | `colorNeutralForeground1` | #ffffff |
| Tab selected icon color | `colorCompoundBrandForeground1` | #479ef5 |
| Tab selected hovered label color | `colorNeutralForeground1Hover` | #ffffff |
| Tab selected hovered icon color | `colorCompoundBrandForeground1Hover` | #62abf5 |
| Tab selected pressed label color | `colorNeutralForeground1Pressed` | #ffffff |
| Tab selected pressed icon color | `colorCompoundBrandForeground1Pressed` | #2886de |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab disabled container color | `colorTransparentBackground` | transparent |
| Tab disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Tab disabled icon color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Tab - Color - Subtle (контекст: Default, Light)

Отличается от Transparent только цветом контейнера; цвета label/icon идентичны.

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab container color | `colorSubtleBackground` | transparent |
| Tab label color | `colorNeutralForeground2` | #424242 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab hovered container color | `colorSubtleBackgroundHover` | #f5f5f5 |
| Tab hovered label color | `colorNeutralForeground2Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pressed container color | `colorSubtleBackgroundPressed` | #e0e0e0 |
| Tab pressed label color | `colorNeutralForeground2Pressed` | #242424 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab selected label color | `colorNeutralForeground1` | #242424 |
| Tab selected icon color | `colorCompoundBrandForeground1` | #0f6cbd |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Tab disabled icon color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Tab - Color - Subtle (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab container color | `colorSubtleBackground` | transparent |
| Tab label color | `colorNeutralForeground2` | #d6d6d6 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab hovered container color | `colorSubtleBackgroundHover` | #383838 |
| Tab hovered label color | `colorNeutralForeground2Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pressed container color | `colorSubtleBackgroundPressed` | #2e2e2e |
| Tab pressed label color | `colorNeutralForeground2Pressed` | #ffffff |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab selected label color | `colorNeutralForeground1` | #ffffff |
| Tab selected icon color | `colorCompoundBrandForeground1` | #479ef5 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Tab disabled icon color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Tab - Selection indicator (::after) (контекст: Default, Light)

Полоса выделения выбранной вкладки (`activeIndicatorStyles`, псевдоэлемент `::after`).

## Enabled (selected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab indicator base color | `colorTransparentStroke` | transparent |
| Tab indicator selected color | `colorCompoundBrandStroke` | #0f6cbd |
| Tab indicator selected corner radius | `borderRadiusCircular` | 10000px |

## Hovered (selected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab indicator selected hovered color | `colorCompoundBrandStrokeHover` | #115ea3 |

## Pressed (selected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab indicator selected pressed color | `colorCompoundBrandStrokePressed` | #0f548c |

## Disabled (selected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab indicator disabled color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Tab - Selection indicator (::after) (контекст: Default, Dark)

## Enabled (selected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab indicator base color | `colorTransparentStroke` | transparent |
| Tab indicator selected color | `colorCompoundBrandStroke` | #479ef5 |
| Tab indicator selected corner radius | `borderRadiusCircular` | 10000px |

## Hovered (selected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab indicator selected hovered color | `colorCompoundBrandStrokeHover` | #62abf5 |

## Pressed (selected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab indicator selected pressed color | `colorCompoundBrandStrokePressed` | #2886de |

## Disabled (selected)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab indicator disabled color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Tab - Pending indicator (::before, hover-preview) (контекст: Default, Light/Dark)

Полоса-подсказка при наведении на невыбранную вкладку (`pendingIndicatorStyles`, `::before`).

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Tab pending hovered color | `colorNeutralStroke1Hover` | #c7c7c7 | #757575 |
| Tab pending pressed color | `colorNeutralStroke1Pressed` | #b3b3b3 | #6b6b6b |
| Tab pending disabled color | `colorTransparentStroke` | transparent | transparent |
| Tab pending corner radius | `borderRadiusCircular` | 10000px | 10000px |

# Tab - Pill (filled-circular) (контекст: Default, Light)

Круглая «filled» pill-вкладка (`useCircularAppearanceStyles.filled*`).

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill container color | `colorNeutralBackground3` | #f5f5f5 |
| Tab pill label color | `colorNeutralForeground2` | #424242 |
| Tab pill border color | `colorTransparentStroke` | transparent |
| Tab pill border width | `strokeWidthThin` | 1px |
| Tab pill corner radius | `borderRadiusCircular` | 10000px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill hovered container color | `colorNeutralBackground3Hover` | #ebebeb |
| Tab pill hovered label color | `colorNeutralForeground2Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill pressed container color | `colorNeutralBackground3Pressed` | #d6d6d6 |
| Tab pill pressed label color | `colorNeutralForeground2Pressed` | #242424 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill selected container color | `colorBrandBackground` | #0f6cbd |
| Tab pill selected label color | `colorNeutralForegroundOnBrand` | #ffffff |
| Tab pill selected hovered container color | `colorBrandBackgroundHover` | #115ea3 |
| Tab pill selected pressed container color | `colorBrandBackgroundPressed` | #0c3b5e |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill disabled container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Tab pill disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Tab pill disabled+selected border color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Tab - Pill (filled-circular) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill container color | `colorNeutralBackground3` | #141414 |
| Tab pill label color | `colorNeutralForeground2` | #d6d6d6 |
| Tab pill border color | `colorTransparentStroke` | transparent |
| Tab pill border width | `strokeWidthThin` | 1px |
| Tab pill corner radius | `borderRadiusCircular` | 10000px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill hovered container color | `colorNeutralBackground3Hover` | #292929 |
| Tab pill hovered label color | `colorNeutralForeground2Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill pressed container color | `colorNeutralBackground3Pressed` | #0a0a0a |
| Tab pill pressed label color | `colorNeutralForeground2Pressed` | #ffffff |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill selected container color | `colorBrandBackground` | #115ea3 |
| Tab pill selected label color | `colorNeutralForegroundOnBrand` | #ffffff |
| Tab pill selected hovered container color | `colorBrandBackgroundHover` | #0f6cbd |
| Tab pill selected pressed container color | `colorBrandBackgroundPressed` | #0c3b5e |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill disabled container color | `colorNeutralBackgroundDisabled` | #141414 |
| Tab pill disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Tab pill disabled+selected border color | `colorNeutralStrokeDisabled` | #424242 |

# Tab - Pill (subtle-circular) (контекст: Default, Light)

Круглая «subtle» pill-вкладка (`useCircularAppearanceStyles.subtle*`).

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill container color | `colorSubtleBackground` | transparent |
| Tab pill label color | `colorNeutralForeground2` | #424242 |
| Tab pill border color | `colorTransparentStroke` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill hovered container color | `colorSubtleBackgroundHover` | #f5f5f5 |
| Tab pill hovered border color | `colorNeutralStroke1Hover` | #c7c7c7 |
| Tab pill hovered label color | `colorNeutralForeground2Hover` | #242424 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill pressed container color | `colorSubtleBackgroundPressed` | #e0e0e0 |
| Tab pill pressed border color | `colorNeutralStroke1Pressed` | #b3b3b3 |
| Tab pill pressed label color | `colorNeutralForeground2Pressed` | #242424 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill selected container color | `colorBrandBackground2` | #ebf3fc |
| Tab pill selected border color | `colorCompoundBrandStroke` | #0f6cbd |
| Tab pill selected label color | `colorBrandForeground2` | #115ea3 |
| Tab pill selected hovered container color | `colorBrandBackground2Hover` | #cfe4fa |
| Tab pill selected hovered border color | `colorCompoundBrandStrokeHover` | #115ea3 |
| Tab pill selected hovered label color | `colorBrandForeground2Hover` | #0f548c |
| Tab pill selected pressed container color | `colorBrandBackground2Pressed` | #96c6fa |
| Tab pill selected pressed border color | `colorCompoundBrandStrokePressed` | #0f548c |
| Tab pill selected pressed label color | `colorBrandForeground2Pressed` | #0a2e4a |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill disabled container color | `colorSubtleBackground` | transparent |
| Tab pill disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Tab pill disabled+selected container color | `colorNeutralBackgroundDisabled` | #f0f0f0 |
| Tab pill disabled+selected border color | `colorNeutralStrokeDisabled` | #e0e0e0 |

# Tab - Pill (subtle-circular) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill container color | `colorSubtleBackground` | transparent |
| Tab pill label color | `colorNeutralForeground2` | #d6d6d6 |
| Tab pill border color | `colorTransparentStroke` | transparent |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill hovered container color | `colorSubtleBackgroundHover` | #383838 |
| Tab pill hovered border color | `colorNeutralStroke1Hover` | #757575 |
| Tab pill hovered label color | `colorNeutralForeground2Hover` | #ffffff |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill pressed container color | `colorSubtleBackgroundPressed` | #2e2e2e |
| Tab pill pressed border color | `colorNeutralStroke1Pressed` | #6b6b6b |
| Tab pill pressed label color | `colorNeutralForeground2Pressed` | #ffffff |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill selected container color | `colorBrandBackground2` | #082338 |
| Tab pill selected border color | `colorCompoundBrandStroke` | #479ef5 |
| Tab pill selected label color | `colorBrandForeground2` | #62abf5 |
| Tab pill selected hovered container color | `colorBrandBackground2Hover` | #0c3b5e |
| Tab pill selected hovered border color | `colorCompoundBrandStrokeHover` | #62abf5 |
| Tab pill selected hovered label color | `colorBrandForeground2Hover` | #96c6fa |
| Tab pill selected pressed container color | `colorBrandBackground2Pressed` | #061724 |
| Tab pill selected pressed border color | `colorCompoundBrandStrokePressed` | #2886de |
| Tab pill selected pressed label color | `colorBrandForeground2Pressed` | #ebf3fc |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab pill disabled container color | `colorSubtleBackground` | transparent |
| Tab pill disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Tab pill disabled+selected container color | `colorNeutralBackgroundDisabled` | #141414 |
| Tab pill disabled+selected border color | `colorNeutralStrokeDisabled` | #424242 |

# Tab - Focus (контекст: Default, Light/Dark)

Кастомный focus-индикатор (`createCustomFocusIndicatorStyle`): тень `shadow4` + внешнее
кольцо `0 0 0 strokeWidthThick colorStrokeFocus2`. У круглых вкладок добавляется внутреннее
кольцо `strokeWidthThin colorNeutralStrokeOnBrand inset`.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Tab focus ring color | `colorStrokeFocus2` | #000000 | #ffffff |
| Tab focus ring width | `strokeWidthThick` | 2px | 2px |
| Tab focus base shadow | `shadow4` | 0 0 2px rgba(0,0,0,0.12), 0 2px 4px rgba(0,0,0,0.14) | 0 0 2px rgba(0,0,0,0.24), 0 2px 4px rgba(0,0,0,0.28) |
| Tab focus (circular) inner ring color | `colorNeutralStrokeOnBrand` | #ffffff | #292929 |
| Tab focus (circular) inner ring width | `strokeWidthThin` | 1px | 1px |
| Tab focus corner radius (linear) | `borderRadiusMedium` | 4px | 4px |

# Tab - Size - Small

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab small padding block (horizontal) | `spacingVerticalSNudge` | 6px |
| Tab small padding block (vertical orient.) | `spacingVerticalXXS` | 2px |
| Tab small padding inline | `spacingHorizontalSNudge` | 6px |
| Tab small icon-content gap | `spacingHorizontalXXS` | 2px |
| Tab small icon size | (literal) | 20px |
| Tab small label typography | (body1 / selected body1Strong) | 14px / 20px |
| Tab small corner radius (linear) | `borderRadiusMedium` | 4px |
| Tab small indicator thickness (horizontal) | `strokeWidthThick` | 2px |
| Tab small indicator thickness (vertical) | `strokeWidthThicker` | 3px |
| Tab small indicator inset (horizontal) | `spacingHorizontalSNudge` | 6px |
| Tab small indicator inset (vertical) | `spacingVerticalXS` | 4px |
| TabList small pill gap | `spacingHorizontalSNudge` | 6px |

# Tab - Size - Medium (default)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab medium padding block (horizontal) | `spacingVerticalM` | 12px |
| Tab medium padding block (vertical orient.) | `spacingVerticalSNudge` | 6px |
| Tab medium padding inline | `spacingHorizontalMNudge` | 10px |
| Tab medium icon-content gap | `spacingHorizontalSNudge` | 6px |
| Tab medium icon size | (literal) | 20px |
| Tab medium label typography | (body1 / selected body1Strong) | 14px / 20px |
| Tab medium corner radius (linear) | `borderRadiusMedium` | 4px |
| Tab medium indicator thickness | `strokeWidthThicker` | 3px |
| Tab medium indicator inset (horizontal) | `spacingHorizontalM` | 12px |
| Tab medium indicator inset (vertical) | `spacingVerticalS` | 8px |
| TabList pill gap | `spacingHorizontalS` | 8px |

# Tab - Size - Large

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab large padding block (horizontal) | `spacingVerticalL` | 16px |
| Tab large padding block (vertical orient.) | `spacingVerticalS` | 8px |
| Tab large padding inline | `spacingHorizontalMNudge` | 10px |
| Tab large icon-content gap | `spacingHorizontalSNudge` | 6px |
| Tab large icon size | (literal) | 24px |
| Tab large label typography | (body2 / selected subtitle2) | 16px / 22px |
| Tab large corner radius (linear) | `borderRadiusMedium` | 4px |
| Tab large indicator thickness | `strokeWidthThicker` | 3px |
| Tab large indicator inset (horizontal) | `spacingHorizontalM` | 12px |
| Tab large indicator inset (vertical) | `spacingVerticalMNudge` | 10px |
| TabList pill gap | `spacingHorizontalS` | 8px |

# Tab - Content typography

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Tab content padding block | `spacingVerticalNone` | 0 |
| Tab content padding inline | `spacingHorizontalXXS` | 2px |
| Tab content base (rest) | (body1) | 14px / 20px / 400 |
| Tab content selected (small/medium) | (body1Strong) | 14px / 20px / 600 |
| Tab content base (large) | (body2) | 16px / 22px / 400 |
| Tab content selected (large) | (subtitle2) | 16px / 22px / 600 |
