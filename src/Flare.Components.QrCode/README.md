# Flare.Components.QrCode

QR code component that renders a fully scannable SVG from a string value, for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library. Add-on package that extends
`Flare.Components`.

```sh
dotnet add package Flare.Components.QrCode
```

Requires `Flare.Components` and a `Flare.Theme.*` package. Use `<FlareQrCode Value="..." />` once
Flare is set up (see the `Flare.Components` readme).

Encodes in byte mode across the full ISO/IEC 18004 range, versions 1 to 40, picking the smallest
symbol that fits: up to 2953 bytes at error correction level L and 1273 at level H.

Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
