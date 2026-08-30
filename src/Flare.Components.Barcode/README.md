# Flare.Components.Barcode

Linear (1D) barcode component that renders a scannable SVG from a string value, for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library. Add-on package that extends
`Flare.Components`.

```sh
dotnet add package Flare.Components.Barcode
```

Requires `Flare.Components` and a `Flare.Theme.*` package. Use
`<FlareBarcode Value="..." Symbology="BarcodeSymbology.Code128" />` once Flare is set up (see the
`Flare.Components` readme).

Seven symbologies, encoded in managed code with no JavaScript and no external encoder: Code 128 (with
automatic A/B/C subset switching), EAN-13, EAN-8, UPC-A, Code 39, ITF-14 and Codabar. Check digits are
computed when the symbology defines one, and an input the symbology cannot represent renders nothing
rather than an unscannable symbol.

Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
