# Flare.Components.Barcode

Компонент линейного (1D) штрихкода: рисует сканируемый SVG из строки. Для библиотеки компонентов
[Flare](https://github.com/jrfrigat/Flare) на Blazor. Дополнительный пакет, расширяющий `Flare.Components`.

```sh
dotnet add package Flare.Components.Barcode
```

Требует `Flare.Components` и пакет `Flare.Theme.*`. Используйте
`<FlareBarcode Value="..." Symbology="BarcodeSymbology.Code128" />`, когда Flare уже настроен
(см. readme пакета `Flare.Components`).

Семь символик, закодированных управляемым кодом - без JavaScript и без внешнего энкодера: Code 128
(с автоматическим переключением подмножеств A/B/C), EAN-13, EAN-8, UPC-A, Code 39, ITF-14 и Codabar.
Контрольная цифра считается там, где символика ее определяет, а ввод, который символика представить
не может, не рисует ничего - вместо несканируемого символа.

Репозиторий и документация: https://github.com/jrfrigat/Flare  -  лицензия MIT.
