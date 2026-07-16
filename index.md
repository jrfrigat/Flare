---
_layout: landing
---

# Flare

**Flare** is a MIT-licensed, theme-agnostic UI component library for **.NET 10 Blazor** (WebAssembly *and* Server),
with 130+ components and zero baked-in styling, built on a single semantic token-based theming engine.
Build your own design system, or start instantly with one of seven production-ready preset design systems shipped as independent, optional packages.

- **Components** - 130+ accessible Blazor components (forms, data grid, navigation, overlays, charts, media, and more).
- **Themes** - Material Design 3 Expressive, Material Design 3, Material Design 2, Fluent UI 2, Aero, Liquid Glass, Visual Studio - each a separate package, switchable at runtime.
- **Dynamic Color** - opt-in palette derived from the OS/browser accent (Windows/macOS accent, Android Material You) via the active theme's generator.
- **Tokens** - one semantic token system (`--flare-color/shape/typescale/state/elevation/motion-*`) drives every theme.

## Start here

| | |
| :-- | :-- |
| [Getting Started](docs/en/getting-started.md) | Install, DI setup, styles, first component. |
| [For AI Agents](docs/en/ai-agents.md) | A compact, machine-friendly guide for LLM coding agents. |
| [Architecture](docs/en/architecture.md) | Modules, tokens, services, theming deep-dive. |
| [Theme Creation](docs/en/theme-creation-guide.md) | Design tokens, palettes, custom themes. |
| [API Reference](api/index.md) | Full generated API for every public type and parameter. |

## Install

```sh
dotnet add package Flare.Blazor
dotnet add package Flare.Theme.MaterialDesign3Expressive
```

```csharp
// Program.cs
using Flare.Extensions;
using Flare.Theme.MaterialDesign3Expressive;

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme   = new MaterialDesign3ExpressiveTheme();
    opts.DefaultPalette = Md3Palettes.Violet;
    opts.DefaultMode    = ThemeMode.Auto;
});
```

See the [live Gallery](https://github.com/jrfrigat/Flare) for interactive examples of every component.
