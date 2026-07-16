# Flare.Theme.MaterialDesign3Expressive

Material Design 3 (Expressive) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library, including the built-in MD3
palettes.

```sh
dotnet add package Flare.Theme.MaterialDesign3Expressive
```

```csharp
// as the default theme...
builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3ExpressiveTheme();
    opts.DefaultPalette = Md3Palettes.Violet;
});
// ...or register alongside others, then switch at runtime:
builder.Services.AddFlareTheme(new MaterialDesign3ExpressiveTheme());
// await ThemeService.SetThemeAsync("md3-expressive");
```

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
