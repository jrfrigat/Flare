# Flare.Theme.Aero

Aero (glassy, translucent) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library.

```sh
dotnet add package Flare.Theme.Aero
```

```csharp
// default theme...
builder.Services.AddFlare(opts => opts.DefaultTheme = new AeroTheme());
// ...or register and switch at runtime:
builder.Services.AddFlareTheme(new AeroTheme());
// await ThemeService.SetThemeAsync("aero");
```

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
