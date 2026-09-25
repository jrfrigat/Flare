# Fluent UI v8 (Fluent 1) - палитра

Разрешенные цвета темы Fluent UI v8. Светлая схема - `createTheme()` из `@fluentui/theme` 2.7.2; темная - `DarkTheme` из `@fluentui/theme-samples` 8.7.226 (в ядре v8 темной темы нет). Компоненты v8 берут цвета из `semanticColors`, а те выводятся из `palette`.

## palette (light / dark)

### Тема (акцентная лестница)

| Токен | Light | Dark |
|---|---|---|
| `themeDarker` | #004578 | #82c7ff |
| `themeDark` | #005a9e | #6cb8f6 |
| `themeDarkAlt` | #106ebe | #3aa0f3 |
| `themePrimary` | #0078d4 | #2899f5 |
| `themeSecondary` | #2b88d8 | #0078d4 |
| `themeTertiary` | #71afe5 | #235a85 |
| `themeLight` | #c7e0f4 | #004c87 |
| `themeLighter` | #deecf9 | #043862 |
| `themeLighterAlt` | #eff6fc | #092c47 |

### Нейтральная лестница

| Токен | Light | Dark |
|---|---|---|
| `black` | #000000 | #ffffff |
| `blackTranslucent40` | rgba(0,0,0,.4) | rgba(0,0,0,.4) |
| `neutralDark` | #201f1e | #faf9f8 |
| `neutralPrimary` | #323130 | #f3f2f1 |
| `neutralPrimaryAlt` | #3b3a39 | #c8c6c4 |
| `neutralSecondary` | #605e5c | #a19f9d |
| `neutralSecondaryAlt` | #8a8886 | #979693 |
| `neutralTertiary` | #a19f9d | #797775 |
| `neutralTertiaryAlt` | #c8c6c4 | #484644 |
| `neutralQuaternary` | #d2d0ce | #3b3a39 |
| `neutralQuaternaryAlt` | #e1dfdd | #323130 |
| `neutralLight` | #edebe9 | #292827 |
| `neutralLighter` | #f3f2f1 | #252423 |
| `neutralLighterAlt` | #faf9f8 | #201f1e |
| `white` | #ffffff | #1b1a19 |
| `whiteTranslucent40` | rgba(255,255,255,.4) | rgba(255,255,255,.4) |

### Акцент

| Токен | Light | Dark |
|---|---|---|
| `accent` | #0078d4 | #2899f5 |

### Общие цвета

| Токен | Light | Dark |
|---|---|---|
| `yellowDark` | #d29200 | #d29200 |
| `yellow` | #ffb900 | #ffb900 |
| `yellowLight` | #fff100 | #fff100 |
| `orange` | #d83b01 | #d83b01 |
| `orangeLight` | #ea4300 | #ea4300 |
| `orangeLighter` | #ff8c00 | #ff8c00 |
| `redDark` | #a4262c | #F1707B |
| `red` | #e81123 | #e81123 |
| `magentaDark` | #5c005c | #5c005c |
| `magenta` | #b4009e | #b4009e |
| `magentaLight` | #e3008c | #e3008c |
| `purpleDark` | #32145a | #32145a |
| `purple` | #5c2d91 | #5c2d91 |
| `purpleLight` | #b4a0ff | #b4a0ff |
| `blueDark` | #002050 | #002050 |
| `blueMid` | #00188f | #00188f |
| `blue` | #0078d4 | #0078d4 |
| `blueLight` | #00bcf2 | #00bcf2 |
| `tealDark` | #004b50 | #004b50 |
| `teal` | #008272 | #008272 |
| `tealLight` | #00b294 | #00b294 |
| `greenDark` | #004b1c | #004b1c |
| `green` | #107c10 | #107c10 |
| `greenLight` | #bad80a | #bad80a |

## semanticColors (light / dark)

### Фон и текст страницы (body)

| Токен | Light | Dark |
|---|---|---|
| `bodyBackground` | #ffffff | #1b1a19 |
| `bodyFrameBackground` | #ffffff | #1b1a19 |
| `bodyTextChecked` | #000000 | #ffffff |
| `bodyBackgroundChecked` | #edebe9 | #292827 |
| `bodyFrameDivider` | #edebe9 | #292827 |
| `bodyDivider` | #edebe9 | #292827 |
| `variantBorder` | #edebe9 | #292827 |
| `bodyBackgroundHovered` | #f3f2f1 | #252423 |
| `variantBorderHovered` | #a19f9d | #797775 |
| `bodyText` | #323130 | #f3f2f1 |
| `bodyStandoutBackground` | #faf9f8 | #201f1e |
| `defaultStateBackground` | #faf9f8 | #201f1e |
| `bodySubtext` | #605e5c | #a19f9d |

### Карточки

| Токен | Light | Dark |
|---|---|---|
| `cardStandoutBackground` | #ffffff | #1b1a19 |
| `cardShadow` | 0 1.6px 3.6px 0 rgba(0, 0, 0, 0.132), 0 0.3px 0.9px 0 rgba(0, 0, 0, 0.108) | 0 1.6px 3.6px 0 rgba(0, 0, 0, 0.132), 0 0.3px 0.9px 0 rgba(0, 0, 0, 0.108) |
| `cardShadowHovered` | 0 3.2px 7.2px 0 rgba(0, 0, 0, 0.132), 0 0.6px 1.8px 0 rgba(0, 0, 0, 0.108) | 0 0 1px #797775 |

### Кнопки

| Токен | Light | Dark |
|---|---|---|
| `accentButtonText` | #ffffff | #1b1a19 |
| `buttonBackground` | #ffffff | #1b1a19 |
| `primaryButtonText` | #ffffff | #1b1a19 |
| `primaryButtonTextHovered` | #ffffff | #1b1a19 |
| `primaryButtonTextPressed` | #ffffff | #1b1a19 |
| `buttonTextCheckedHovered` | #000000 | #ffffff |
| `primaryButtonBackground` | #0078d4 | #2899f5 |
| `accentButtonBackground` | #0078d4 | #2899f5 |
| `primaryButtonBackgroundPressed` | #005a9e | #6cb8f6 |
| `primaryButtonBackgroundHovered` | #106ebe | #3aa0f3 |
| `buttonBackgroundCheckedHovered` | #edebe9 | #292827 |
| `buttonBackgroundPressed` | #edebe9 | #292827 |
| `buttonBackgroundHovered` | #f3f2f1 | #252423 |
| `buttonBackgroundDisabled` | #f3f2f1 | #252423 |
| `buttonBorderDisabled` | #f3f2f1 | #252423 |
| `primaryButtonBackgroundDisabled` | #f3f2f1 | #252423 |
| `primaryButtonTextDisabled` | #d2d0ce | #3b3a39 |
| `buttonTextDisabled` | #a19f9d | #797775 |
| `buttonText` | #323130 | #ffffff |
| `buttonTextHovered` | #201f1e | #f3f2f1 |
| `buttonTextChecked` | #201f1e | #faf9f8 |
| `buttonTextPressed` | #201f1e | #faf9f8 |
| `buttonBorder` | #8a8886 | #979693 |
| `buttonBackgroundChecked` | #c8c6c4 | #484644 |
| `primaryButtonBorder` | transparent | transparent |

### Поля ввода

| Токен | Light | Dark |
|---|---|---|
| `inputBackground` | #ffffff | #1b1a19 |
| `inputForegroundChecked` | #ffffff | #1b1a19 |
| `inputBackgroundChecked` | #0078d4 | #2899f5 |
| `inputIcon` | #0078d4 | #2899f5 |
| `inputFocusBorderAlt` | #0078d4 | #2899f5 |
| `inputBackgroundCheckedHovered` | #005a9e | #6cb8f6 |
| `inputIconHovered` | #005a9e | #6cb8f6 |
| `inputPlaceholderBackgroundChecked` | #deecf9 | #043862 |
| `inputIconDisabled` | #a19f9d | #797775 |
| `inputBorderHovered` | #323130 | #f3f2f1 |
| `inputText` | #323130 | #f3f2f1 |
| `inputTextHovered` | #201f1e | #faf9f8 |
| `inputBorder` | #605e5c | #a19f9d |
| `smallInputBorder` | #605e5c | #a19f9d |
| `inputPlaceholderText` | #605e5c | #a19f9d |

### Списки

| Токен | Light | Dark |
|---|---|---|
| `listBackground` | #ffffff | #1b1a19 |
| `listItemBackgroundChecked` | #edebe9 | #292827 |
| `listHeaderBackgroundPressed` | #edebe9 | #292827 |
| `listItemBackgroundHovered` | #f3f2f1 | #252423 |
| `listHeaderBackgroundHovered` | #f3f2f1 | #252423 |
| `listItemBackgroundCheckedHovered` | #e1dfdd | #323130 |
| `listText` | #323130 | #f3f2f1 |
| `listTextColor` | #323130 | #323130 |

### Меню

| Токен | Light | Dark |
|---|---|---|
| `menuBackground` | #ffffff | #252423 |
| `menuIcon` | #0078d4 | #3aa0f3 |
| `menuHeader` | #0078d4 | #ffffff |
| `menuItemBackgroundPressed` | #edebe9 | #3b3a39 |
| `menuItemBackgroundChecked` | #edebe9 | #292827 |
| `menuItemBackgroundHovered` | #f3f2f1 | #323130 |
| `menuItemText` | #323130 | #f3f2f1 |
| `menuItemTextHovered` | #201f1e | #faf9f8 |
| `menuDivider` | #c8c6c4 | #484644 |

### Ссылки

| Токен | Light | Dark |
|---|---|---|
| `link` | #0078d4 | #2899f5 |
| `linkHovered` | #004578 | #82c7ff |
| `actionLink` | #323130 | #f3f2f1 |
| `actionLinkHovered` | #201f1e | #faf9f8 |

### Недоступное состояние

| Токен | Light | Dark |
|---|---|---|
| `disabledBackground` | #f3f2f1 | #323130 |
| `disabledSubtext` | #d2d0ce | #3b3a39 |
| `disabledBodyText` | #a19f9d | #797775 |
| `disabledText` | #a19f9d | #797775 |
| `disabledBodySubtext` | #c8c6c4 | #484644 |
| `disabledBorder` | #c8c6c4 | #484644 |

### Фокус

| Токен | Light | Dark |
|---|---|---|
| `focusBorder` | #605e5c | #a19f9d |

### Сообщения и статусы

| Токен | Light | Dark |
|---|---|---|
| `errorText` | #a4262c | #F1707B |
| `messageText` | #323130 | #F3F2F1 |
| `messageLink` | #005A9E | #6CB8F6 |
| `messageLinkHovered` | #004578 | #82C7FF |
| `infoIcon` | #605e5c | #C8C6C4 |
| `errorIcon` | #A80000 | #F1707B |
| `blockingIcon` | #FDE7E9 | #442726 |
| `warningIcon` | #797775 | #C8C6C4 |
| `severeWarningIcon` | #D83B01 | #FCE100 |
| `successIcon` | #107C10 | #92C353 |
| `infoBackground` | #f3f2f1 | #323130 |
| `errorBackground` | #FDE7E9 | #442726 |
| `blockingBackground` | #FDE7E9 | #442726 |
| `warningBackground` | #FFF4CE | #433519 |
| `severeWarningBackground` | #FED9CC | #4F2A0F |
| `successBackground` | #DFF6DD | #393D1B |
| `warningHighlight` | #ffb900 | #fff100 |
| `successText` | #107C10 | #92c353 |
| `warningText` | #323130 | #F3F2F1 |

## Справочные рампы (одинаковы для обеих схем)

### NeutralColors

| Токен | Hex |
|---|---|
| `black` | #000000 |
| `gray220` | #11100f |
| `gray210` | #161514 |
| `gray200` | #1b1a19 |
| `gray190` | #201f1e |
| `gray180` | #252423 |
| `gray170` | #292827 |
| `gray160` | #323130 |
| `gray150` | #3b3a39 |
| `gray140` | #484644 |
| `gray130` | #605e5c |
| `gray120` | #797775 |
| `gray110` | #8a8886 |
| `gray100` | #979593 |
| `gray90` | #a19f9d |
| `gray80` | #b3b0ad |
| `gray70` | #bebbb8 |
| `gray60` | #c8c6c4 |
| `gray50` | #d2d0ce |
| `gray40` | #e1dfdd |
| `gray30` | #edebe9 |
| `gray20` | #f3f2f1 |
| `gray10` | #faf9f8 |
| `white` | #ffffff |

### SharedColors

| Токен | Hex |
|---|---|
| `pinkRed10` | #750b1c |
| `red20` | #a4262c |
| `red10` | #d13438 |
| `redOrange20` | #603d30 |
| `redOrange10` | #da3b01 |
| `orange30` | #8e562e |
| `orange20` | #ca5010 |
| `orange10` | #ffaa44 |
| `yellow10` | #fce100 |
| `orangeYellow20` | #986f0b |
| `orangeYellow10` | #c19c00 |
| `yellowGreen10` | #8cbd18 |
| `green20` | #0b6a0b |
| `green10` | #498205 |
| `greenCyan10` | #00ad56 |
| `cyan40` | #005e50 |
| `cyan30` | #005b70 |
| `cyan20` | #038387 |
| `cyan10` | #00b7c3 |
| `cyanBlue20` | #004e8c |
| `cyanBlue10` | #0078d4 |
| `blue10` | #4f6bed |
| `blueMagenta40` | #373277 |
| `blueMagenta30` | #5c2e91 |
| `blueMagenta20` | #8764b8 |
| `blueMagenta10` | #8378de |
| `magenta20` | #881798 |
| `magenta10` | #c239b3 |
| `magentaPink20` | #9b0062 |
| `magentaPink10` | #e3008c |
| `gray40` | #393939 |
| `gray30` | #7a7574 |
| `gray20` | #69797e |
| `gray10` | #a0aeb2 |

### CommunicationColors

| Токен | Hex |
|---|---|
| `shade30` | #004578 |
| `shade20` | #005a9e |
| `shade10` | #106ebe |
| `primary` | #0078d4 |
| `tint10` | #2b88d8 |
| `tint20` | #c7e0f4 |
| `tint30` | #deecf9 |
| `tint40` | #eff6fc |

