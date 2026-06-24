# Flare.Theme.LiquidGlass

Liquid Glass (frosted, depth-layered) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library.

```sh
dotnet add package Flare.Theme.LiquidGlass
```

```csharp
// default theme...
builder.Services.AddFlare(opts => opts.DefaultTheme = new LiquidGlassTheme());
// ...or register and switch at runtime:
builder.Services.AddFlareTheme(new LiquidGlassTheme());
// await ThemeService.SetThemeAsync("liquid-glass");
```

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
