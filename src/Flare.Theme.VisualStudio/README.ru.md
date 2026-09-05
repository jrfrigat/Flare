# Flare.Theme.VisualStudio

Тема в духе Visual Studio (2022/2026), светлая и темная, для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor.

```sh
dotnet add package Flare.Theme.VisualStudio
```

```csharp
// как тема по умолчанию...
builder.Services.AddFlare(opts => opts.DefaultTheme = new VisualStudioTheme());
// ...или зарегистрировать и переключать во время работы:
builder.Services.AddFlareTheme(new VisualStudioTheme());
// await ThemeService.SetThemeAsync("visualstudio");
```

## Оболочка Visual Studio -> Flare

Visual Studio - это продукт, а не опубликованная дизайн-система, поэтому таблица сопоставляет части
ее ОБОЛОЧКИ - то, на что вы показали бы пальцем в IDE, - с компонентами, которые их воспроизводят.
IDE-семейство Flare существует именно для приложений такой формы.

| Visual Studio | Flare | Примечания |
| :-- | :-- | :-- |
| Главное меню (File, Edit, View...) | `FlareMenuBar` | |
| Панель инструментов / командная панель | `FlareToolbar`, `FlareQuickAccessToolbar` | |
| Лента (в Office-подобных хостах) | `FlareRibbon` | |
| Вкладки документов | `FlareDocumentTabs` | |
| Окна инструментов (Solution Explorer, Properties) | `FlareToolPanel` | пристыкованные панели |
| Дерево Solution Explorer | `FlareTreeView`, `FlareDataTree` | |
| Окно Properties | `FlarePropertyGrid` | |
| Строка состояния | `FlareStatusBar` | |
| Разделитель панелей | `FlareSplitter` | |
| Output / список ошибок | `FlareDataGrid` | |
| Диалог параметров | `FlareDialog` + `FlareNavMenu` | |
| Палитра команд (Ctrl+Q) | `FlareCombobox`; `FlareShortcuts` документирует сочетания | |
| Backstage (меню «Файл») | `FlareBackstage` | |
| Оболочка целиком | `FlareIdeLayout` | собирает все перечисленное в один каркас |

Все остальное - кнопки, поля, флажки, таблицы - это обычный набор компонентов Flare в токенах этой
темы; VS их не переименовывает.

## Что тема меняет помимо цвета

- **Плотность по умолчанию.** Типографическая шкала и высоты элементов настроены под IDE, а не под палец.
- **Прямые углы и штрихи в 1px.** Шкала формы почти везде близка к нулю.
- **Выделение - плоская заливка, а не тональная пленка**: та же модель дискретных состояний, что и у
  Fluent, поэтому тема задает свои токены слоя состояния, а не полагается на значение из Material.

Требуется `Flare.Components`. Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
