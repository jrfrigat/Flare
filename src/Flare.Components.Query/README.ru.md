# Flare.Components.Query

Визуальный конструктор запросов для библиотеки компонентов [Flare](https://github.com/jrfrigat/Flare) на Blazor,
построенный на модели запросов Querio. Дополнительный пакет, расширяющий `Flare.Components`.

```sh
dotnet add package Flare.Components.Query
```

Требует `Flare.Components` и пакет `Flare.Theme.*`. Используйте `<FlareQueryBuilder ... />` или
`<FlareQueryEditor ... />`, когда Flare уже настроен (см. readme пакета `Flare.Components`).

Собирает соединения, агрегаты, группировку, условия и постраничность поверх схемы, которую дает
вызывающий, и выдает сериализуемую спецификацию запроса. К базе данных он не подключается и ничего
не выполняет: во что превратится спецификация - в SQL, в HTTP-запрос, в LINQ-выражение - решает
потребитель.

Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
