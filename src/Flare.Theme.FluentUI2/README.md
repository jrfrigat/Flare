# Flare.Theme.FluentUI2

Fluent UI 2 light and dark theme for the [Flare](https://github.com/jrfrigat/Flare) Blazor component
library.

```sh
dotnet add package Flare.Theme.FluentUI2
```

```csharp
// default theme...
builder.Services.AddFlare(opts => opts.DefaultTheme = new FluentUI2Theme());
// ...or register and switch at runtime:
builder.Services.AddFlareTheme(new FluentUI2Theme());
// await ThemeService.SetThemeAsync("fluent2");
```

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.
