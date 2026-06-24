# Flare.Icons

Offline SVG icon set for the [Flare](https://github.com/jrfrigat/Flare) Blazor component library:
Material Symbols in five styles (Filled, Outlined, Rounded, Sharp, Two-Tone) plus brand and
file-format icons, exposed as strongly-typed `Icons.*` SVG paths.

Optional package - `FlareIcon` works without it (it falls back to the Material Symbols web font). Add
this when you want bundled SVG icons with no external font dependency.

```sh
dotnet add package Flare.Icons
```

```razor
<FlareIcon Icon="@Icons.Material.Rounded.Home" />
```

Repository & docs: https://github.com/jrfrigat/Flare  ·  MIT licensed.
