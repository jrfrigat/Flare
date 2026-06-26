---
_layout: landing
---

# Flare

**Flare** is a MIT-licensed UI component library for **.NET 10 Blazor** (WebAssembly *and* Server),
with 100+ components and multiple swappable design systems built on a single token-based theming engine.

- **Components** - 100+ accessible Blazor components (forms, data grid, navigation, overlays, charts, media, and more).
- **Themes** - Material Design 3 Expressive, Fluent UI 2, Aero, Liquid Glass, Visual Studio - each a separate package, switchable at runtime.
- **Tokens** - one semantic token system (`--flare-color/shape/typescale/state/elevation/motion-*`) drives every theme.

## Start here

| | |
| :-- | :-- |
| [Getting Started](docs/en/getting-started.md) | Install, DI setup, styles, first component. |
| [For AI Agents](docs/en/ai-agents.md) | A compact, machine-friendly guide for LLM coding agents. |
| [Architecture](docs/en/architecture.md) | Modules, tokens, services, theming deep-dive. |
| [Theme Creation](docs/en/theme-creation-guide.md) | Design tokens, palettes, custom themes. |
| [API Reference](api/) | Full generated API for every public type and parameter. |

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
    opts.DefaultTheme   = new Md3Theme();
    opts.DefaultPalette = Md3Palettes.Violet;
    opts.DefaultMode    = ThemeMode.Auto;
});
```

See the [live Gallery](https://github.com/jrfrigat/Flare) for interactive examples of every component.
