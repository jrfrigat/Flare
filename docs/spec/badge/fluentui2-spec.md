# Badge (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/appearance/color -> alias-токен взяты из исходника `@fluentui/react-badge`
(`useBadgeStyles.styles.ts`, плюс `useCounterBadgeStyles`, `usePresenceBadgeStyles`).
Badge не имеет интерактивных состояний (hover/pressed/disabled) - только матрица
appearance (filled/ghost/outline/tint) x color (brand/danger/important/informative/severe/subtle/success/warning).
Граница рисуется псевдоэлементом `::after` (`strokeWidthThin`); у `ghost` `::after` скрыт.

# Badge - Appearance - Filled (контекст: Default, Light)

Контейнер `filled` использует прозрачную рамку (`colorTransparentStroke`) через `::after`.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge filled brand container | `colorBrandBackground` | #0f6cbd |
| Badge filled brand label | `colorNeutralForegroundOnBrand` | #ffffff |
| Badge filled danger container | `colorPaletteRedBackground3` | #d13438 |
| Badge filled danger label | `colorNeutralForegroundOnBrand` | #ffffff |
| Badge filled important container | `colorNeutralForeground1` | #242424 |
| Badge filled important label | `colorNeutralBackground1` | #ffffff |
| Badge filled informative container | `colorNeutralBackground5` | #ebebeb |
| Badge filled informative label | `colorNeutralForeground3` | #616161 |
| Badge filled severe container | `colorPaletteDarkOrangeBackground3` | #da3b01 |
| Badge filled severe label | `colorNeutralForegroundOnBrand` | #ffffff |
| Badge filled subtle container | `colorNeutralBackground1` | #ffffff |
| Badge filled subtle label | `colorNeutralForeground1` | #242424 |
| Badge filled success container | `colorPaletteGreenBackground3` | #107c10 |
| Badge filled success label | `colorNeutralForegroundOnBrand` | #ffffff |
| Badge filled warning container | `colorPaletteYellowBackground3` | #fde300 |
| Badge filled warning label | `colorNeutralForeground1Static` | #242424 |
| Badge filled border color | `colorTransparentStroke` | transparent |
| Badge filled border width | `strokeWidthThin` | 1px |

# Badge - Appearance - Filled (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge filled brand container | `colorBrandBackground` | #115ea3 |
| Badge filled brand label | `colorNeutralForegroundOnBrand` | #ffffff |
| Badge filled danger container | `colorPaletteRedBackground3` | #d13438 |
| Badge filled danger label | `colorNeutralForegroundOnBrand` | #ffffff |
| Badge filled important container | `colorNeutralForeground1` | #ffffff |
| Badge filled important label | `colorNeutralBackground1` | #292929 |
| Badge filled informative container | `colorNeutralBackground5` | #000000 |
| Badge filled informative label | `colorNeutralForeground3` | #adadad |
| Badge filled severe container | `colorPaletteDarkOrangeBackground3` | #da3b01 |
| Badge filled severe label | `colorNeutralForegroundOnBrand` | #ffffff |
| Badge filled subtle container | `colorNeutralBackground1` | #292929 |
| Badge filled subtle label | `colorNeutralForeground1` | #ffffff |
| Badge filled success container | `colorPaletteGreenBackground3` | #107c10 |
| Badge filled success label | `colorNeutralForegroundOnBrand` | #ffffff |
| Badge filled warning container | `colorPaletteYellowBackground3` | #fde300 |
| Badge filled warning label | `colorNeutralForeground1Static` | #242424 |
| Badge filled border color | `colorTransparentStroke` | transparent |
| Badge filled border width | `strokeWidthThin` | 1px |

# Badge - Appearance - Ghost (контекст: Default, Light)

`ghost` не имеет фона и рамки (`::after` скрыт); задаётся только цвет метки.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge ghost brand label | `colorBrandForeground1` | #0f6cbd |
| Badge ghost danger label | `colorPaletteRedForeground3` | #d13438 |
| Badge ghost important label | `colorNeutralForeground1` | #242424 |
| Badge ghost informative label | `colorNeutralForeground3` | #616161 |
| Badge ghost severe label | `colorPaletteDarkOrangeForeground3` | #da3b01 |
| Badge ghost subtle label | `colorNeutralForegroundStaticInverted` | #ffffff |
| Badge ghost success label | `colorPaletteGreenForeground3` | #107c10 |
| Badge ghost warning label | `colorPaletteYellowForeground2` | #817400 |

# Badge - Appearance - Ghost (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge ghost brand label | `colorBrandForeground1` | #479ef5 |
| Badge ghost danger label | `colorPaletteRedForeground3` | #e37d80 |
| Badge ghost important label | `colorNeutralForeground1` | #ffffff |
| Badge ghost informative label | `colorNeutralForeground3` | #adadad |
| Badge ghost severe label | `colorPaletteDarkOrangeForeground3` | #e9835e |
| Badge ghost subtle label | `colorNeutralForegroundStaticInverted` | #ffffff |
| Badge ghost success label | `colorPaletteGreenForeground3` | #9fd89f |
| Badge ghost warning label | `colorPaletteYellowForeground2` | #fef7b2 |

# Badge - Appearance - Outline (контекст: Default, Light)

Фон прозрачный; рамка по умолчанию `currentColor` (равна цвету метки), если не переопределена.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge outline brand label | `colorBrandForeground1` | #0f6cbd |
| Badge outline brand border | (currentColor) | #0f6cbd |
| Badge outline danger label | `colorPaletteRedForeground3` | #d13438 |
| Badge outline danger border | `colorPaletteRedBorder2` | #d13438 |
| Badge outline important label | `colorNeutralForeground3` | #616161 |
| Badge outline important border | `colorNeutralStrokeAccessible` | #616161 |
| Badge outline informative label | `colorNeutralForeground3` | #616161 |
| Badge outline informative border | `colorNeutralStroke2` | #e0e0e0 |
| Badge outline severe label | `colorPaletteDarkOrangeForeground3` | #da3b01 |
| Badge outline severe border | (currentColor) | #da3b01 |
| Badge outline subtle label | `colorNeutralForegroundStaticInverted` | #ffffff |
| Badge outline subtle border | (currentColor) | #ffffff |
| Badge outline success label | `colorPaletteGreenForeground3` | #107c10 |
| Badge outline success border | `colorPaletteGreenBorder2` | #107c10 |
| Badge outline warning label | `colorPaletteYellowForeground2` | #817400 |
| Badge outline warning border | (currentColor) | #817400 |

# Badge - Appearance - Outline (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge outline brand label | `colorBrandForeground1` | #479ef5 |
| Badge outline brand border | (currentColor) | #479ef5 |
| Badge outline danger label | `colorPaletteRedForeground3` | #e37d80 |
| Badge outline danger border | `colorPaletteRedBorder2` | #e37d80 |
| Badge outline important label | `colorNeutralForeground3` | #adadad |
| Badge outline important border | `colorNeutralStrokeAccessible` | #adadad |
| Badge outline informative label | `colorNeutralForeground3` | #adadad |
| Badge outline informative border | `colorNeutralStroke2` | #525252 |
| Badge outline severe label | `colorPaletteDarkOrangeForeground3` | #e9835e |
| Badge outline severe border | (currentColor) | #e9835e |
| Badge outline subtle label | `colorNeutralForegroundStaticInverted` | #ffffff |
| Badge outline subtle border | (currentColor) | #ffffff |
| Badge outline success label | `colorPaletteGreenForeground3` | #9fd89f |
| Badge outline success border | `colorPaletteGreenBorder2` | #9fd89f |
| Badge outline warning label | `colorPaletteYellowForeground2` | #fef7b2 |
| Badge outline warning border | (currentColor) | #fef7b2 |

# Badge - Appearance - Tint (контекст: Default, Light)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge tint brand container | `colorBrandBackground2` | #ebf3fc |
| Badge tint brand label | `colorBrandForeground2` | #115ea3 |
| Badge tint brand border | `colorBrandStroke2` | #b4d6fa |
| Badge tint danger container | `colorPaletteRedBackground1` | #fdf6f6 |
| Badge tint danger label | `colorPaletteRedForeground1` | #bc2f32 |
| Badge tint danger border | `colorPaletteRedBorder1` | #f1bbbc |
| Badge tint important container | `colorNeutralForeground3` | #616161 |
| Badge tint important label | `colorNeutralBackground1` | #ffffff |
| Badge tint important border | `colorTransparentStroke` | transparent |
| Badge tint informative container | `colorNeutralBackground4` | #f0f0f0 |
| Badge tint informative label | `colorNeutralForeground3` | #616161 |
| Badge tint informative border | `colorNeutralStroke2` | #e0e0e0 |
| Badge tint severe container | `colorPaletteDarkOrangeBackground1` | #fdf6f3 |
| Badge tint severe label | `colorPaletteDarkOrangeForeground1` | #c43501 |
| Badge tint severe border | `colorPaletteDarkOrangeBorder1` | #f4bfab |
| Badge tint subtle container | `colorNeutralBackground1` | #ffffff |
| Badge tint subtle label | `colorNeutralForeground3` | #616161 |
| Badge tint subtle border | `colorNeutralStroke2` | #e0e0e0 |
| Badge tint success container | `colorPaletteGreenBackground1` | #f1faf1 |
| Badge tint success label | `colorPaletteGreenForeground1` | #0e700e |
| Badge tint success border | `colorPaletteGreenBorder1` | #9fd89f |
| Badge tint warning container | `colorPaletteYellowBackground1` | #fffef5 |
| Badge tint warning label | `colorPaletteYellowForeground1` | #817400 |
| Badge tint warning border | `colorPaletteYellowBorder1` | #fef7b2 |

# Badge - Appearance - Tint (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge tint brand container | `colorBrandBackground2` | #082338 |
| Badge tint brand label | `colorBrandForeground2` | #62abf5 |
| Badge tint brand border | `colorBrandStroke2` | #0e4775 |
| Badge tint danger container | `colorPaletteRedBackground1` | #3f1011 |
| Badge tint danger label | `colorPaletteRedForeground1` | #e37d80 |
| Badge tint danger border | `colorPaletteRedBorder1` | #d13438 |
| Badge tint important container | `colorNeutralForeground3` | #adadad |
| Badge tint important label | `colorNeutralBackground1` | #292929 |
| Badge tint important border | `colorTransparentStroke` | transparent |
| Badge tint informative container | `colorNeutralBackground4` | #0a0a0a |
| Badge tint informative label | `colorNeutralForeground3` | #adadad |
| Badge tint informative border | `colorNeutralStroke2` | #525252 |
| Badge tint severe container | `colorPaletteDarkOrangeBackground1` | #411200 |
| Badge tint severe label | `colorPaletteDarkOrangeForeground1` | #e9835e |
| Badge tint severe border | `colorPaletteDarkOrangeBorder1` | #da3b01 |
| Badge tint subtle container | `colorNeutralBackground1` | #292929 |
| Badge tint subtle label | `colorNeutralForeground3` | #adadad |
| Badge tint subtle border | `colorNeutralStroke2` | #525252 |
| Badge tint success container | `colorPaletteGreenBackground1` | #052505 |
| Badge tint success label | `colorPaletteGreenForeground1` | #54b054 |
| Badge tint success border | `colorPaletteGreenBorder1` | #107c10 |
| Badge tint warning container | `colorPaletteYellowBackground1` | #4c4400 |
| Badge tint warning label | `colorPaletteYellowForeground1` | #feee66 |
| Badge tint warning border | `colorPaletteYellowBorder1` | #fde300 |

# Badge - Size (theme-independent)

Высота = ширина (min-width); горизонтальный padding у контентных размеров = spacing + `spacingHorizontalXXS`
(текстовый padding). Токены spacing: XXS=2px, XS=4px, SNudge=6px.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge tiny size | (literal) | 6px |
| Badge tiny font size | (literal) | 4px |
| Badge extra-small size | (literal) | 10px |
| Badge extra-small font size | (literal) | 6px |
| Badge small height / min-width | (literal) | 16px |
| Badge small padding inline | (derived: `spacingHorizontalXXS` + 2px) | 0 4px |
| Badge medium height / min-width (default) | (literal) | 20px |
| Badge medium padding inline | (derived: `spacingHorizontalXS` + 2px) | 0 6px |
| Badge large height / min-width | (literal) | 24px |
| Badge large padding inline | (derived: `spacingHorizontalXS` + 2px) | 0 6px |
| Badge extra-large height / min-width | (literal) | 32px |
| Badge extra-large padding inline | (derived: `spacingHorizontalSNudge` + 2px) | 0 8px |

# Badge - Icon Size (theme-independent)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge icon tiny | (literal) | 6px |
| Badge icon extra-small | (literal) | 10px |
| Badge icon small | (literal) | 12px |
| Badge icon medium (default) | (literal) | 12px |
| Badge icon large | (literal) | 16px |
| Badge icon extra-large | (literal) | 20px |

# Badge - Shape (theme-independent)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge shape circular (default) | `borderRadiusCircular` | 10000px |
| Badge shape rounded | `borderRadiusMedium` | 4px |
| Badge shape rounded (small-to-tiny) | `borderRadiusSmall` | 2px |
| Badge shape square | `borderRadiusNone` | 0 |

# Badge - Typography (theme-independent)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Badge label font (default) | `caption1Strong` | 12px / 16px / 600 |
| Badge label font (small-to-tiny) | `caption2Strong` | 10px / 14px / 600 |
| Badge font family | `fontFamilyBase` | 'Segoe UI', ... , sans-serif |

# CounterBadge (контекст: Default, Light/Dark)

`useCounterBadgeStyles.styles.ts` наследует всю палитру `Badge` (appearance/color выше);
добавляет режим `dot` (точка-индикатор без числа) и скрытие пустого значения.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| CounterBadge default appearance | `filled` (color=`danger`) | #d13438 / #ffffff | #d13438 / #ffffff |
| CounterBadge dot width | (literal) | 6px | 6px |
| CounterBadge dot height | (literal) | 6px | 6px |
| CounterBadge dot padding | (literal) | 0 | 0 |
| CounterBadge empty state | (literal `display:none`) | скрыт | скрыт |

# PresenceBadge (контекст: Default, Light)

`usePresenceBadgeStyles.styles.ts` - контейнер круглый, фон `colorNeutralBackground1`,
цвет иконки задаётся статусом присутствия.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| PresenceBadge container background | `colorNeutralBackground1` | #ffffff |
| PresenceBadge shape | `borderRadiusCircular` | 10000px |
| PresenceBadge busy / do-not-disturb / blocked | `colorPaletteRedBackground3` | #d13438 |
| PresenceBadge away | `colorPaletteMarigoldBackground3` | #eaa300 |
| PresenceBadge available | `colorPaletteLightGreenForeground3` | #13a10e |
| PresenceBadge offline | `colorNeutralForeground3` | #616161 |
| PresenceBadge out-of-office | `colorPaletteBerryForeground3` | #c239b3 |
| PresenceBadge unknown | `colorNeutralForeground3` | #616161 |
| PresenceBadge out-of-office icon inset | `colorNeutralBackground1` | #ffffff |

# PresenceBadge (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| PresenceBadge container background | `colorNeutralBackground1` | #292929 |
| PresenceBadge shape | `borderRadiusCircular` | 10000px |
| PresenceBadge busy / do-not-disturb / blocked | `colorPaletteRedBackground3` | #d13438 |
| PresenceBadge away | `colorPaletteMarigoldBackground3` | #eaa300 |
| PresenceBadge available | `colorPaletteLightGreenForeground3` | #3db838 |
| PresenceBadge offline | `colorNeutralForeground3` | #adadad |
| PresenceBadge out-of-office | `colorPaletteBerryForeground3` | #d161c4 |
| PresenceBadge unknown | `colorNeutralForeground3` | #adadad |
| PresenceBadge out-of-office icon inset | `colorNeutralBackground1` | #292929 |

# PresenceBadge - Size (theme-independent)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| PresenceBadge tiny | (literal) | 6px |
| PresenceBadge medium (default) | (literal, icon-driven) | 12px |
| PresenceBadge large | (literal) | 20px |
| PresenceBadge extra-large | (literal) | 28px |
| PresenceBadge icon inset padding | (literal) | 1px |
