# Flare.Components.IDE

Компоненты раскладки в стиле IDE для библиотеки компонентов [Flare](https://github.com/jrfrigat/Flare) на Blazor:
лента, вкладки документов, инструментальные панели, сплиттер и строка состояния. Дополнительный
пакет, расширяющий `Flare.Components`.

```sh
dotnet add package Flare.Components.IDE
```

Требует `Flare.Components` и пакет `Flare.Theme.*`. Регистрируется вызовом
`builder.Services.AddFlareIde()` (базовая настройка - в readme пакета `Flare.Components`).

## Динамический докинг

`FlareDockWorkspace` позволяет разместить произвольный Blazor-контент во вкладках.
Передайте `DockGroup` для начального набора вкладок или дерево из `DockSplit` и
`DockGroup`. Подключите `_content/Flare.Components.IDE/css/flare-ide.css` и выделите
место для рабочей области.

```razor
@using Flare.Components.IDE

<FlareDockWorkspace Layout="_layout" Items="_items" OnChangeRequested="Apply">
    <ContentTemplate Context="item">
        <FlareText>@item.Title</FlareText>
    </ContentTemplate>
</FlareDockWorkspace>

@code {
    private DockNode _layout = new DockGroup("main", ["data", "report"], "data");
    private readonly DockItem[] _items = [new("data", "Data"), new("report", "Report")];

    private void Apply(DockLayoutChange change)
    {
        if (ReferenceEquals(change.Previous, _layout))
            _layout = change.Proposed;
    }
}
```

Перетащите вкладку к краю группы, чтобы создать сплит. Перенос, перестановка и разделение
доступны также через меню действий с вкладкой: мышью, касанием или клавиатурой.
Каждая группа хранит свой выбор. Ключи элементов и узлов уникальны.
Корневая группа может быть пустой.

Чтобы вкладки подстраивались под ширину своей группы, установите `AdaptiveTabCompression="true"`.
По умолчанию режим выключен. При сужении группы подписи вкладок сокращаются, затем уступают место
иконкам из `DockItem.Icon`.
Вкладки без иконки сохраняют короткую подпись, а доступным именем каждой вкладки остается
`DockItem.Title`. Если компактные вкладки выходят за пределы полосы, появляются стрелки прокрутки.
При расширении группы подписи возвращаются.
При использовании `TabTemplate` передавайте `DockItem.Icon` и `DockItem.Title` для компактного вида.
Индикаторы, которые должны оставаться видимыми, выводите через `TabMarkerTemplate`, а кнопки
вкладки через `TabActionsTemplate`.

Приложение владеет принятой раскладкой. Если оно отклоняет операцию, сохраняется текущая
раскладка. После асинхронного подтверждения проверьте, что `Previous` еще актуальна.
Представление раскладки и прикладные данные приложение сохраняет в своем хранилище.

Активная вкладка сохраняет экземпляр компонента при переносе в пределах одного хоста и
рендерера. `DockItem.KeepAlive` удерживает уже открытый компонент после переключения на
другую вкладку. Закрытие освобождает экземпляр. При повторном открытии приложение может
восстановить свои данные.

Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
