# Menu (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходников `@fluentui/react-menu`
(`useMenuPopoverStyles.styles.ts`, `useMenuListStyles.styles.ts`, `useMenuItemStyles.styles.ts`).
В колонке `Token Name` - реальный Fluent alias-токен (см. `docs/spec/_pallete/fluentui2-spec.md`),
в колонке `Value` - его разрешённое значение. Состояние `Pressed` в CSS соответствует `:hover:active`.

# Menu - Surface (MenuPopover) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu popover container color | `colorNeutralBackground1` | #ffffff |
| Menu popover text color | `colorNeutralForeground1` | #242424 |
| Menu popover border color | `colorTransparentStroke` | transparent |
| Menu popover border width | (literal) | 1px |
| Menu popover corner radius | `borderRadiusMedium` | 4px |
| Menu popover elevation shadow | `shadow16` | 0 0 2px rgba(0,0,0,0.12), 0 8px 16px rgba(0,0,0,0.14) |

# Menu - Surface (MenuPopover) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu popover container color | `colorNeutralBackground1` | #292929 |
| Menu popover text color | `colorNeutralForeground1` | #ffffff |
| Menu popover border color | `colorTransparentStroke` | transparent |
| Menu popover border width | (literal) | 1px |
| Menu popover corner radius | `borderRadiusMedium` | 4px |
| Menu popover elevation shadow | `shadow16` | 0 0 2px rgba(0,0,0,0.24), 0 8px 16px rgba(0,0,0,0.28) |

# Menu - Item (MenuItem) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item container color | `colorNeutralBackground1` | #ffffff |
| Menu item label color | `colorNeutralForeground2` | #424242 |
| Menu item icon color (inherits label) | `colorNeutralForeground2` | #424242 |
| Menu item secondary content color | `colorNeutralForeground3` | #616161 |
| Menu item subtext color | `colorNeutralForeground3` | #616161 |
| Menu item corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item hovered container color | `colorNeutralBackground1Hover` | #f5f5f5 |
| Menu item hovered label color | `colorNeutralForeground2Hover` | #242424 |
| Menu item hovered icon color | `colorNeutralForeground2BrandSelected` | #0f6cbd |
| Menu item hovered secondary content color | `colorNeutralForeground3Hover` | #424242 |
| Menu item hovered subtext color | `colorNeutralForeground3Hover` | #424242 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item pressed container color | `colorNeutralBackground1Pressed` | #e0e0e0 |
| Menu item pressed label color | `colorNeutralForeground2Pressed` | #242424 |
| Menu item pressed subtext color | `colorNeutralForeground3Pressed` | #424242 |

## Submenu open (submenuOpen; визуально = Hovered)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item submenu-open container color | `colorNeutralBackground1Hover` | #f5f5f5 |
| Menu item submenu-open label color | `colorNeutralForeground2Hover` | #242424 |
| Menu item submenu-open icon color | `colorNeutralForeground2BrandSelected` | #0f6cbd |
| Menu item submenu-open subtext color | `colorNeutralForeground3Hover` | #424242 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item disabled container color (unchanged) | `colorNeutralBackground1` | #ffffff |
| Menu item disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Menu item disabled icon color | `colorNeutralForegroundDisabled` | #bdbdbd |
| Menu item disabled subtext color | `colorNeutralForegroundDisabled` | #bdbdbd |

# Menu - Item (MenuItem) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item container color | `colorNeutralBackground1` | #292929 |
| Menu item label color | `colorNeutralForeground2` | #d6d6d6 |
| Menu item icon color (inherits label) | `colorNeutralForeground2` | #d6d6d6 |
| Menu item secondary content color | `colorNeutralForeground3` | #adadad |
| Menu item subtext color | `colorNeutralForeground3` | #adadad |
| Menu item corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item hovered container color | `colorNeutralBackground1Hover` | #3d3d3d |
| Menu item hovered label color | `colorNeutralForeground2Hover` | #ffffff |
| Menu item hovered icon color | `colorNeutralForeground2BrandSelected` | #479ef5 |
| Menu item hovered secondary content color | `colorNeutralForeground3Hover` | #d6d6d6 |
| Menu item hovered subtext color | `colorNeutralForeground3Hover` | #d6d6d6 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item pressed container color | `colorNeutralBackground1Pressed` | #1f1f1f |
| Menu item pressed label color | `colorNeutralForeground2Pressed` | #ffffff |
| Menu item pressed subtext color | `colorNeutralForeground3Pressed` | #d6d6d6 |

## Submenu open (submenuOpen; визуально = Hovered)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item submenu-open container color | `colorNeutralBackground1Hover` | #3d3d3d |
| Menu item submenu-open label color | `colorNeutralForeground2Hover` | #ffffff |
| Menu item submenu-open icon color | `colorNeutralForeground2BrandSelected` | #479ef5 |
| Menu item submenu-open subtext color | `colorNeutralForeground3Hover` | #d6d6d6 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item disabled container color (unchanged) | `colorNeutralBackground1` | #292929 |
| Menu item disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Menu item disabled icon color | `colorNeutralForegroundDisabled` | #5c5c5c |
| Menu item disabled subtext color | `colorNeutralForegroundDisabled` | #5c5c5c |

# Menu - Item split-trigger separator (контекст: Default, Light/Dark)

Разделитель `::before` у split-триггера MenuItemSplitGroup.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Menu split separator color | `colorNeutralStroke1` | #d1d1d1 | #666666 |
| Menu split separator width | `strokeWidthThin` | 1px | 1px |
| Menu split separator height | (literal) | 24px | 24px |

# Menu - Focus (контекст: Default, Light/Dark)

MenuItem использует стандартный focus-outline Fluent (`createFocusOutlineStyle`) - обводка
цвета `colorStrokeFocus2` толщиной `strokeWidthThick`. Значения инвертированы между темами.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| Menu item focus stroke color | `colorStrokeFocus2` | #000000 | #ffffff |
| Menu item focus stroke width | `strokeWidthThick` | 2px | 2px |
| Menu item focus corner radius | `borderRadiusMedium` | 4px | 4px |

# Menu - Size - Item geometry

Геометрия/типографика единственного размера MenuItem (тема-независимо).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu item min height | (literal) | 32px |
| Menu item max width | (literal) | 290px |
| Menu item padding (все стороны) | `spacingVerticalSNudge` | 6px |
| Menu item icon-content gap | (literal) | 4px |
| Menu item label size | `fontSizeBase300` | 14px |
| Menu item label line-height | `lineHeightBase300` | 20px |
| Menu item label weight | `fontWeightRegular` | 400 |
| Menu item corner radius | `borderRadiusMedium` | 4px |
| Menu item icon slot size | (literal) | 20px x 20px |
| Menu item icon glyph size | (literal) | 20px |
| Menu item submenu-indicator slot size | (literal) | 20px x 20px |
| Menu item content inline padding | (literal) | 2px |
| Menu item secondary content size | `fontSizeBase200` (caption1) | 12px |
| Menu item secondary content line-height | `lineHeightBase300` (override) | 20px |
| Menu item subtext size | `fontSizeBase100` (caption2) | 10px |
| Menu item subtext line-height | `lineHeightBase100` (caption2) | 14px |

# Menu - Size - List geometry (MenuList)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu list layout | (literal) | flex column |
| Menu list item gap | (literal) | 2px |

# Menu - Size - Popover geometry (MenuPopover)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Menu popover min width | (literal) | 138px |
| Menu popover max width | (literal) | 300px |
| Menu popover padding | (literal) | 4px |
| Menu popover border width | `strokeWidthThin` (1px solid) | 1px |
| Menu popover corner radius | `borderRadiusMedium` | 4px |
| Menu popover base typography | (body1) | 14px / 20px / 400 |
