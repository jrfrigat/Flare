# App bar (Fluent UI 2)

**Fluent UI 2 не имеет прямого аналога App bar (панели приложения); ниже - маппинг на
ближайший примитив: поверхность Toolbar (`@fluentui/react-toolbar`), несущая subtle-кнопки
(`@fluentui/react-button`) и текст заголовка стиля subtitle1/subtitle2. Значения
приблизительны и требуют сверки с Figma.**

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки взяты из исходников `@fluentui/react-toolbar` (`useToolbarStyles.styles.ts`) и
`@fluentui/react-button` (`useButtonStyles.styles.ts`, ветка `subtle`). В колонке `Token Name` -
реальный Fluent alias-токен, в колонке `Value` - разрешённое значение. Состояние `Pressed`
в CSS соответствует `:hover:active, :active:focus-visible`.

**Важно:** сам Fluent Toolbar - это только flex-контейнер с горизонтальным padding; у него
НЕТ собственных токенов фона/возвышения. Поверхность App bar ниже (`Surface`) - приближение
через нейтральный фон + тень и подлежит сверке с Figma.

# App bar - Surface (контекст: Default, Light/Dark)

Приближение: поверхность-контейнер (нейтральный фон) с тенью-возвышением над контентом.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| App bar surface container color | `colorNeutralBackground1` | #ffffff | #292929 |
| App bar surface elevation shadow | `shadow4` | 0 0 2px rgba(0,0,0,0.12), 0 2px 4px rgba(0,0,0,0.14) | 0 0 2px rgba(0,0,0,0.24), 0 2px 4px rgba(0,0,0,0.28) |
| App bar surface raised elevation shadow | `shadow8` | 0 0 2px rgba(0,0,0,0.12), 0 4px 8px rgba(0,0,0,0.14) | 0 0 2px rgba(0,0,0,0.24), 0 4px 8px rgba(0,0,0,0.28) |

Альтернативные уровни фона (по потребности темы): `colorNeutralBackground2` (#fafafa / #1f1f1f),
`colorNeutralBackground3` (#f5f5f5 / #141414).

# App bar - Toolbar padding (Size)

Единственные собственные стили Toolbar - горизонтальный padding по размеру (theme-independent).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar small padding | (literal) | 0px 4px |
| App bar medium padding (default) | (literal) | 4px 8px |
| App bar large padding | (literal) | 4px 20px |

# App bar - Action item (subtle Button) (контекст: Default, Light)

Элементы-кнопки на панели - Button appearance `subtle`.

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item container color | `colorSubtleBackground` | transparent |
| App bar item outline color | (transparent) | transparent |
| App bar item label color | `colorNeutralForeground2` | #424242 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item hovered container color | `colorSubtleBackgroundHover` | #f5f5f5 |
| App bar item hovered label color | `colorNeutralForeground2Hover` | #242424 |
| App bar item hovered icon color | `colorNeutralForeground2BrandHover` | #0f6cbd |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item pressed container color | `colorSubtleBackgroundPressed` | #e0e0e0 |
| App bar item pressed label color | `colorNeutralForeground2Pressed` | #242424 |
| App bar item pressed icon color | `colorNeutralForeground2BrandPressed` | #115ea3 |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item disabled container color | `colorTransparentBackground` | transparent |
| App bar item disabled label color | `colorNeutralForegroundDisabled` | #bdbdbd |

# App bar - Action item (subtle Button) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item container color | `colorSubtleBackground` | transparent |
| App bar item outline color | (transparent) | transparent |
| App bar item label color | `colorNeutralForeground2` | #d6d6d6 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item hovered container color | `colorSubtleBackgroundHover` | #383838 |
| App bar item hovered label color | `colorNeutralForeground2Hover` | #ffffff |
| App bar item hovered icon color | `colorNeutralForeground2BrandHover` | #479ef5 |

## Pressed

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item pressed container color | `colorSubtleBackgroundPressed` | #2e2e2e |
| App bar item pressed label color | `colorNeutralForeground2Pressed` | #ffffff |
| App bar item pressed icon color | `colorNeutralForeground2BrandPressed` | #2886de |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item disabled container color | `colorTransparentBackground` | transparent |
| App bar item disabled label color | `colorNeutralForegroundDisabled` | #5c5c5c |

# App bar - Title text

Заголовок панели - типографический стиль `subtitle1` (основной) либо `subtitle2`
(компактный). Значения theme-independent.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar title (subtitle1) font family | `fontFamilyBase` | 'Segoe UI', 'Segoe UI Web (West European)', -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', sans-serif |
| App bar title (subtitle1) size | `fontSizeBase500` | 20px |
| App bar title (subtitle1) weight | `fontWeightSemibold` | 600 |
| App bar title (subtitle1) line-height | `lineHeightBase500` | 28px |
| App bar title (subtitle2, компактный) size | `fontSizeBase400` | 16px |
| App bar title (subtitle2, компактный) weight | `fontWeightSemibold` | 600 |
| App bar title (subtitle2, компактный) line-height | `lineHeightBase400` | 22px |

# App bar - Motion

Наследуется от subtle-кнопок (Button).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| App bar item transition duration | `durationFaster` | 100ms |
| App bar item transition easing | `curveEasyEase` | cubic-bezier(0.33, 0, 0.67, 1) |
| App bar item transition properties | (literal) | background, border, color |
