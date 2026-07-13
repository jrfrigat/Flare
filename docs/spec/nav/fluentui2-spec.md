# Nav (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходников `@fluentui/react-nav`
(`sharedNavStyles.styles.ts`, `NavItem/useNavItemStyles.styles.ts`,
`NavCategoryItem/useNavCategoryItem.styles.ts`, `NavSubItem/useNavSubItemStyles.styles.ts`,
`NavDrawer/useNavDrawerStyles.styles.ts`, `NavDrawerBody/useNavDrawerBodyStyles.styles.ts`).
В колонке `Token Name` - реальный Fluent alias-токен, в колонке `Value` - его разрешённое значение.

Корневой класс всех пунктов (NavItem, NavCategoryItem, NavSubItem, AppItem) один и тот же -
`useRootDefaultClassName`. У пунктов нет собственного `:active`/pressed-фона в текущем
исходнике: определён токен `navItemTokens.backgroundColorPressed` (= `colorNeutralBackground4Pressed`),
но он НЕ подключён к CSS. Значение приведено в разделе Pressed как намеренная привязка (флаг ниже).

# NavItem - Color (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem container color | `colorNeutralBackground4` | #f0f0f0 |
| NavItem label color | `colorNeutralForeground2` | #424242 |
| NavItem corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem hovered container color | `colorNeutralBackground4Hover` | #fafafa |

## Pressed

Токен определён в `navItemTokens.backgroundColorPressed`, но в текущем исходнике не привязан к `:active`.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem pressed container color | `colorNeutralBackground4Pressed` | #f5f5f5 |

## Selected

Выбранный пункт не меняет фон; он получает "french fry"-индикатор (::after), жирный текст
(`body1Strong`) и заполненную (filled) brand-иконку. Фон остаётся `colorNeutralBackground4`.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem selected indicator color | `colorCompoundBrandForeground1` | #0f6cbd |
| NavItem selected label weight | `fontWeightSemibold` (body1Strong) | 600 |
| NavItem selected icon color (filled) | `colorNeutralForeground2BrandSelected` | #0f6cbd |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem focus outline color | `colorStrokeFocus2` | #000000 |
| NavItem focus outline width | `strokeWidthThick` | 2px |

# NavItem - Color (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem container color | `colorNeutralBackground4` | #0a0a0a |
| NavItem label color | `colorNeutralForeground2` | #d6d6d6 |
| NavItem corner radius | `borderRadiusMedium` | 4px |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem hovered container color | `colorNeutralBackground4Hover` | #1f1f1f |

## Pressed

Токен определён в `navItemTokens.backgroundColorPressed`, но в текущем исходнике не привязан к `:active`.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem pressed container color | `colorNeutralBackground4Pressed` | #000000 |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem selected indicator color | `colorCompoundBrandForeground1` | #479ef5 |
| NavItem selected label weight | `fontWeightSemibold` (body1Strong) | 600 |
| NavItem selected icon color (filled) | `colorNeutralForeground2BrandSelected` | #479ef5 |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavItem focus outline color | `colorStrokeFocus2` | #ffffff |
| NavItem focus outline width | `strokeWidthThick` | 2px |

# NavItem - Selected indicator ("french fry")

Общий индикатор выбора (NavItem / NavCategoryItem / NavSubItem) - вертикальная пилюля
слева через `::after`; при выборе анимируется от transparent к brand-цвету.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Indicator color | `colorCompoundBrandForeground1` | #0f6cbd (L) / #479ef5 (D) |
| Indicator width | (literal `indicatorWidth`) | 4px |
| Indicator height | (literal `indicatorHeight`) | 20px |
| Indicator radius | `borderRadiusCircular` | 10000px |
| Indicator inline offset | (literal `indicatorOffset`) | 16px |

# NavCategoryItem (заголовок группы)

Тот же корневой класс, что и у NavItem, плюс слот `expandIcon` (шеврон, `margin-inline-start:auto`).
Индикатор выбора и жирный текст показываются ТОЛЬКО когда категория выбрана И свёрнута
(`selected && open === false`). Цвета/фон/hover идентичны NavItem (см. таблицы выше).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Expand icon height | (literal) | 20px |
| Expand icon selected weight | `fontWeightSemibold` (body1Strong) | 600 |
| Selected (collapsed) indicator color | `colorCompoundBrandForeground1` | #0f6cbd (L) / #479ef5 (D) |
| Selected (collapsed) icon color (filled) | `colorNeutralForeground2BrandSelected` | #0f6cbd (L) / #479ef5 (D) |

# NavSubItem (вложенный пункт)

Тот же корневой класс + отступ и сдвинутый индикатор выбора. Цвета/hover/фокус идентичны
NavItem (см. таблицы выше); отличается только геометрия отступа.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Sub-item inline padding (default) | (literal) | 46px |
| Sub-item inline padding (small density) | (literal) | 40px |
| Selected indicator inline offset | (derived: `indicatorOffset` + 36) | -52px |
| Selected indicator color | `colorCompoundBrandForeground1` | #0f6cbd (L) / #479ef5 (D) |

# NavDrawer - Surface (контекст: Default, Light)

Поверхность drawer (inline-drawer из `@fluentui/react-drawer`) с nav-фоном.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavDrawer surface color | `colorNeutralBackground4` | #f0f0f0 |
| NavDrawer default width | (literal `defaultDrawerWidth`) | 260px |

# NavDrawer - Surface (контекст: Default, Dark)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| NavDrawer surface color | `colorNeutralBackground4` | #0a0a0a |
| NavDrawer default width | (literal `defaultDrawerWidth`) | 260px |

# NavDrawerBody

Прокручиваемое тело drawer; своего фона не задаёт (наследует поверхность NavDrawer).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Body padding block | (literal) | 0 |
| Body padding inline-start | `spacingHorizontalMNudge` | 10px |
| Body padding inline-end | `spacingHorizontalXS` | 4px |
| Row gap between items | `spacingVerticalXXS` | 2px |

# Nav - Size / Geometry

Плотность (density) default против small; общая для NavItem/NavCategoryItem/NavSubItem.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Item padding block (default) | `spacingVerticalMNudge` | 10px |
| Item padding inline-start | `spacingHorizontalMNudge` | 10px |
| Item padding inline-end | `spacingHorizontalS` | 8px |
| Item padding block (small density) | `spacingVerticalXS` | 4px |
| Icon-to-label gap | `spacingVerticalL` | 16px |
| Item corner radius | `borderRadiusMedium` | 4px |
| Icon min size | (literal) | 20x20px |
| Label typography | (literal `body1`) | 14px / 20px / 400 |
| Selected label typography | (literal `body1Strong`) | 14px / 20px / 600 |

# Nav - Motion

Фон меняется плавно; индикатор выбора анимируется появлением.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Background transition duration | `durationFaster` | 100ms |
| Background transition easing | `curveLinear` | cubic-bezier(0, 0, 1, 1) |
| Transition property | (literal) | background |
| Indicator animation duration | `durationFaster` | 100ms |
| Indicator animation fill-mode | (literal) | both |
| Indicator animation easing | `curveLinear` | cubic-bezier(0, 0, 1, 1) |
