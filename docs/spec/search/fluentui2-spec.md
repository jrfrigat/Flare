# SearchBox (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-search`
(`useSearchBoxStyles.styles.ts`). SearchBox **построен поверх Input**: в конце хука вызывается
`useInputStyles_unstable(state)`, поэтому все цвета рамки/фона/текста наследуются от Input
(appearance по умолчанию = outline). SearchBox добавляет только геометрию, отступы и два
слота-иконки: `contentBefore` (иконка поиска) и `dismiss` (иконка очистки). Полную таблицу
chrome-цветов см. в `docs/spec/input/fluentui2-spec.md`; ниже - наследуемые значения
(appearance=outline) + собственные слоты-иконки.

# SearchBox - Color - Root (inherited from Input, appearance=outline) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox container color | `colorNeutralBackground1` | #ffffff |
| SearchBox stroke color (top/left/right) | `colorNeutralStroke1` | #d1d1d1 |
| SearchBox bottom stroke color | `colorNeutralStrokeAccessible` | #616161 |
| SearchBox corner radius | `borderRadiusMedium` | 4px |
| SearchBox input text color | `colorNeutralForeground1` | #242424 |
| SearchBox placeholder color | `colorNeutralForeground4` | #707070 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox hovered stroke color | `colorNeutralStroke1Hover` | #c7c7c7 |
| SearchBox hovered bottom stroke color | `colorNeutralStrokeAccessibleHover` | #575757 |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox focused stroke color | `colorNeutralStroke1Pressed` | #b3b3b3 |
| SearchBox focused bottom stroke color | `colorNeutralStrokeAccessiblePressed` | #4d4d4d |
| SearchBox focus underline color (`::after`) | `colorCompoundBrandStroke` | #0f6cbd |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox disabled container color | `colorTransparentBackground` | transparent |
| SearchBox disabled stroke color | `colorNeutralStrokeDisabled` | #e0e0e0 |
| SearchBox disabled text color | `colorNeutralForegroundDisabled` | #bdbdbd |

# SearchBox - Color - Root (inherited from Input, appearance=outline) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox container color | `colorNeutralBackground1` | #292929 |
| SearchBox stroke color (top/left/right) | `colorNeutralStroke1` | #666666 |
| SearchBox bottom stroke color | `colorNeutralStrokeAccessible` | #adadad |
| SearchBox corner radius | `borderRadiusMedium` | 4px |
| SearchBox input text color | `colorNeutralForeground1` | #ffffff |
| SearchBox placeholder color | `colorNeutralForeground4` | #999999 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox hovered stroke color | `colorNeutralStroke1Hover` | #757575 |
| SearchBox hovered bottom stroke color | `colorNeutralStrokeAccessibleHover` | #bdbdbd |

## Focused

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox focused stroke color | `colorNeutralStroke1Pressed` | #6b6b6b |
| SearchBox focused bottom stroke color | `colorNeutralStrokeAccessiblePressed` | #b3b3b3 |
| SearchBox focus underline color (`::after`) | `colorCompoundBrandStroke` | #479ef5 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox disabled container color | `colorTransparentBackground` | transparent |
| SearchBox disabled stroke color | `colorNeutralStrokeDisabled` | #424242 |
| SearchBox disabled text color | `colorNeutralForegroundDisabled` | #5c5c5c |

# SearchBox - Color - Icons (search + dismiss) (контекст: Default, Light)

`contentBefore` (иконка поиска) наследует content-цвет Input; `dismiss` (иконка очистки)
задаёт собственный reset с тем же токеном + `cursor: pointer`.

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox search icon color (`contentBefore`) | `colorNeutralForeground3` | #616161 |
| SearchBox dismiss icon color (`dismiss`) | `colorNeutralForeground3` | #616161 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox search icon disabled color | `colorNeutralForegroundDisabled` | #bdbdbd |
| SearchBox dismiss icon disabled color | `colorNeutralForegroundDisabled` | #bdbdbd |

# SearchBox - Color - Icons (search + dismiss) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox search icon color (`contentBefore`) | `colorNeutralForeground3` | #adadad |
| SearchBox dismiss icon color (`dismiss`) | `colorNeutralForeground3` | #adadad |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox search icon disabled color | `colorNeutralForegroundDisabled` | #5c5c5c |
| SearchBox dismiss icon disabled color | `colorNeutralForegroundDisabled` | #5c5c5c |

# SearchBox - Size - Small

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox small min height (from Input) | (literal `fieldHeights.small`) | 24px |
| SearchBox small root padding inline | `spacingHorizontalSNudge` | 6px |
| SearchBox small input padding-left | `spacingHorizontalSNudge` | 6px |
| SearchBox small input padding-right (unfocused) | `spacingHorizontalSNudge` | 6px |
| SearchBox small icon size (search + dismiss) | (literal `> svg`) | 16px |
| SearchBox small root column gap | (literal) | 0 |

# SearchBox - Size - Medium (default)

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox medium min height (from Input) | (literal `fieldHeights.medium`) | 32px |
| SearchBox medium root padding inline | `spacingHorizontalS` | 8px |
| SearchBox medium input padding-left | `spacingHorizontalSNudge` | 6px |
| SearchBox medium input padding-right (unfocused) | `spacingHorizontalS` | 8px |
| SearchBox medium icon size (search + dismiss) | (literal `> svg`) | 20px |
| SearchBox medium root column gap | (literal) | 0 |

# SearchBox - Size - Large

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox large min height (from Input) | (literal `fieldHeights.large`) | 40px |
| SearchBox large root padding inline | `spacingHorizontalMNudge` | 10px |
| SearchBox large input padding-left | `spacingHorizontalSNudge` | 6px |
| SearchBox large input padding-right (unfocused) | `spacingHorizontalMNudge` | 10px |
| SearchBox large icon size (search + dismiss) | (literal `> svg`) | 24px |
| SearchBox large root column gap | (literal) | 0 |

# SearchBox - Layout - contentAfter (dismiss reveal)

Слот `contentAfter` (обёртка dismiss) скрыт до фокуса и раскрывается при `focused`.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox contentAfter padding-left (focused) | `spacingHorizontalM` | 12px |
| SearchBox contentAfter column gap (focused) | `spacingHorizontalXS` | 4px |
| SearchBox root max width | (literal) | 468px |

# SearchBox - Motion

Наследует анимацию нижней brand-подсветки фокуса от Input (`::after` scaleX).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| SearchBox focus-in duration | `durationNormal` | 200ms |
| SearchBox focus-in delay | `curveDecelerateMid` | cubic-bezier(0,0,0,1) |
| SearchBox focus-out duration | `durationUltraFast` | 50ms |
| SearchBox focus-out delay | `curveAccelerateMid` | cubic-bezier(1,0,1,1) |
