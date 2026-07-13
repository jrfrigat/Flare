# Dialog (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходника `@fluentui/react-dialog`
(`useDialogSurfaceStyles.styles.ts`, `useDialogTitleStyles.styles.ts`,
`useDialogContentStyles.styles.ts`, `useDialogBodyStyles.styles.ts`,
`useDialogActionsStyles.styles.ts`). Поверхность (`DialogSurface`) - непрозрачный
`colorNeutralBackground1` + тень `shadow64` + прозрачная рамка `colorTransparentStroke`;
подложка (backdrop) - `colorBackgroundOverlay`. У поверхности нет hover/pressed-состояний.

# Dialog - Color - Surface (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Dialog surface container color | `colorNeutralBackground1` | #ffffff |
| Dialog surface text color | `colorNeutralForeground1` | #242424 |
| Dialog surface border color | `colorTransparentStroke` | transparent |
| Dialog surface border width | (literal `SURFACE_BORDER_WIDTH`) | 1px |
| Dialog surface corner radius | `borderRadiusXLarge` | 8px |
| Dialog surface elevation (shadow) | `shadow64` | 0 0 8px rgba(0,0,0,0.12), 0 32px 64px rgba(0,0,0,0.14) |

## Backdrop

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Dialog backdrop color | `colorBackgroundOverlay` | rgba(0, 0, 0, 0.4) |
| Dialog nested/transparent backdrop color | `colorTransparentBackground` | transparent |

# Dialog - Color - Surface (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Dialog surface container color | `colorNeutralBackground1` | #292929 |
| Dialog surface text color | `colorNeutralForeground1` | #ffffff |
| Dialog surface border color | `colorTransparentStroke` | transparent |
| Dialog surface border width | (literal `SURFACE_BORDER_WIDTH`) | 1px |
| Dialog surface corner radius | `borderRadiusXLarge` | 8px |
| Dialog surface elevation (shadow) | `shadow64` | 0 0 8px rgba(0,0,0,0.24), 0 32px 64px rgba(0,0,0,0.28) |

## Backdrop

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Dialog backdrop color | `colorBackgroundOverlay` | rgba(0, 0, 0, 0.5) |
| Dialog nested/transparent backdrop color | `colorTransparentBackground` | transparent |

# Dialog - Size - Surface (geometry)

Геометрия поверхности не зависит от темы (константы из `contexts/constants.ts` и reset-стилей).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Dialog surface padding | (literal `SURFACE_PADDING`) | 24px |
| Dialog body/actions gap | (literal `DIALOG_GAP`) | 8px |
| Dialog surface max width | (literal) | 600px |
| Dialog surface max height | (literal) | 100vh |
| Dialog content min height | (literal) | 32px |
| Dialog content padding (focus-offset) | `strokeWidthThick` | 2px |
| Dialog fullscreen scrollbar offset | (literal `DIALOG_FULLSCREEN_DIALOG_SCROLLBAR_OFFSET`) | 4px |
| Dialog breakpoint (mobile) selector | (literal) | max-width: 480px |
| Dialog short-screen selector | (literal) | max-height: 359px |

# Dialog - Typography - Title (subtitle1)

`DialogTitle` использует `typographyStyles.subtitle1` (тема-независимо).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Dialog title font size | `fontSizeBase500` | 20px |
| Dialog title font weight | `fontWeightSemibold` | 600 |
| Dialog title line height | `lineHeightBase500` | 28px |
| Dialog title font family | `fontFamilyBase` | 'Segoe UI', ... , sans-serif |

# Dialog - Typography - Content (body1)

`DialogContent` использует `typographyStyles.body1` (тема-независимо).

| Display Name | Token Name | Value |
|--------------|------------|-------|
| Dialog content font size | `fontSizeBase300` | 14px |
| Dialog content font weight | `fontWeightRegular` | 400 |
| Dialog content line height | `lineHeightBase300` | 20px |
| Dialog content font family | `fontFamilyBase` | 'Segoe UI', ... , sans-serif |
