# Flare.Theme.VisualStudio

Visual Studio (2022/2026-inspired) light and dark theme for the
[Flare](https://github.com/jrfrigat/Flare) Blazor component library.

```sh
dotnet add package Flare.Theme.VisualStudio
```

```csharp
// default theme...
builder.Services.AddFlare(opts => opts.DefaultTheme = new VisualStudioTheme());
// ...or register and switch at runtime:
builder.Services.AddFlareTheme(new VisualStudioTheme());
// await ThemeService.SetThemeAsync("visualstudio");
```

Requires `Flare.Components`. Repository & docs: https://github.com/jrfrigat/Flare  ·  MIT licensed.
