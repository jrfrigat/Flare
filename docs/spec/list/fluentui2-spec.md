# List (Fluent UI 2)

Значения разрешены из `@fluentui/tokens` (webLightTheme / webDarkTheme, v1.0.0-alpha.23);
привязки слот/состояние -> alias-токен взяты из исходников `@fluentui/react-list`
(`useListStyles.styles.ts`, `useListItemStyles.styles.ts`). В колонке `Token Name` - реальный
Fluent alias-токен (см. `docs/spec/_pallete/fluentui2-spec.md`), в колонке `Value` - его
разрешённое значение.

**ВАЖНО (flag): `@fluentui/react-list` - НЕстилизованный примитив.** `List` и `ListItem`
задают только сброс списка (`padding/margin: 0`, `list-style: none`), курсор для
кликабельных/выбираемых элементов и focus-обводку. У `ListItem` **НЕТ собственных токенов
фона для состояний rest / hover / active / selected** - фон и подсветку выделения должен
задать потребитель (или обёртка `Menu`/селект-паттерн). Ниже приведено всё, что реально
токенизировано компонентом; для состояний фона нужна сверка с Figma / целевым паттерном.

# List - Surface (List root) (контекст: Default, Light/Dark)

`List` - семантический `<ul>`-сброс без цветов; фон наследуется от родителя.

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List padding | (literal) | 0 |
| List margin | (literal) | 0 |
| List text-indent | (literal) | 0 |
| List marker | (literal) | list-style-type: none |

# List - Item (ListItem) (контекст: Default, Light)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item container color | (не задан компонентом; наследуется) | (inherited) |
| List item text color | (не задан компонентом; наследуется) | (inherited) |
| List item padding | (literal) | 0 |
| List item margin | (literal) | 0 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item hovered container color | (не токенизирован; задаёт потребитель) | (consumer-defined) |

## Pressed / Active

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item pressed container color | (не токенизирован; задаёт потребитель) | (consumer-defined) |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item selected container color | (не токенизирован; задаёт потребитель) | (consumer-defined) |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item disabled cursor | (literal) | default |

# List - Item (ListItem) (контекст: Default, Dark)

## Enabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item container color | (не задан компонентом; наследуется) | (inherited) |
| List item text color | (не задан компонентом; наследуется) | (inherited) |
| List item padding | (literal) | 0 |
| List item margin | (literal) | 0 |

## Hovered

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item hovered container color | (не токенизирован; задаёт потребитель) | (consumer-defined) |

## Pressed / Active

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item pressed container color | (не токенизирован; задаёт потребитель) | (consumer-defined) |

## Selected

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item selected container color | (не токенизирован; задаёт потребитель) | (consumer-defined) |

## Disabled

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item disabled cursor | (literal) | default |

# List - Item Focus (контекст: Default, Light/Dark)

Единственная реально токенизированная визуальная отделка `ListItem` - focus-обводка
(`createCustomFocusIndicatorStyle` с селектором `:focus`). Значения инвертированы между темами.

| Display Name | Token Name | Value (Light) | Value (Dark) |
|--------------|------------|---------------|--------------|
| List item focus outline color | `colorStrokeFocus2` | #000000 | #ffffff |
| List item focus outline width | `strokeWidthThick` | 2px | 2px |
| List item focus corner radius | `borderRadiusMedium` | 4px | 4px |

# List - Item geometry

| Display Name | Token Name | Value |
|--------------|------------|-------|
| List item clickable/selectable layout | (literal) | display flex, cursor pointer |
| List item checkmark align | (literal) | align-self center |
| List item checkmark indicator margin | (literal) | 4px |
