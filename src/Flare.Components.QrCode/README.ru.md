# Flare.Components.QrCode

Компонент QR-кода: рисует полностью сканируемый SVG из строки. Для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor. Дополнительный пакет, расширяющий `Flare.Components`.

```sh
dotnet add package Flare.Components.QrCode
```

Требует `Flare.Components` и пакет `Flare.Theme.*`. Используйте `<FlareQrCode Value="..." />`, когда Flare уже
настроен (см. readme пакета `Flare.Components`).

Кодирует в байтовом режиме по всему диапазону ISO/IEC 18004, версии с 1 по 40, выбирая наименьший
подходящий символ: до 2953 байт на уровне коррекции ошибок L и до 1273 на уровне H.

Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
