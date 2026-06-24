# Flare - Component Code Conventions

> [Русская версия ->](../ru/component-conventions.md) · [README](../../README.md)

A single code style for **all** components. The canonical reference is **`FlareButton`**.
When creating or changing any component, bring its architecture in line with these rules.

## 1. CSS architecture - one global bundle

### CSS lives in `wwwroot/css/` (a global bundle), NOT in scoped `*.razor.css`

Flare is a published NuGet library with token-driven theming. All component styles live in the
**global bundle** `src/Flare.Components/wwwroot/css/*.css`, imported through `flare-components.css`.

**Why not scoped `*.razor.css`:**
- Scoped CSS adds a `[b-hash]` to every rule, raising specificity and getting in the way of a
  consumer overriding styles with ordinary `.flare-*` classes.
- A component's classes are often used by related components (`.flare-avatar` in `FlareAvatarGroup`,
  `.flare-icon` in `FlareClipboard`, etc.) - scoped CSS breaks those cases.
- `*.cs` components (`FlareText`) are not wired to scoped CSS at all.
- A global bundle is predictable, uniform, and matches the design-system standard
  (MudBlazor, Fluent UI Blazor, Radzen, and others).

**Rules:**
- One component (or a related group) - one CSS file in `wwwroot/css/`.
- The file is added via `@import` in `flare-components.css`.
- When you touch a component, make sure its CSS file exists and is in the bundle.
- Examples: `button.css` -> `FlareButton`, `menu.css`/`menuitem.css`/`menugroup.css` -> the Menu group.
- Theme-specific tweaks (MD3 vs Fluent) go in `src/Flare.Theme.*/wwwroot/css/components/*.css`.

## 2. Tokens - through the token system (no hardcoded colors/numbers)

### Full path for adding a component token

| What | Where |
| :-- | :-- |
| Token values (per-theme) | `src/Flare.Core/Tokens/Components/<Comp>Tokens.cs` |
| CSS variable names | `src/Flare.Core/Css/Tokens/<Comp>Tokens.cs` (namespace `Flare.Css.Tokens`, holder classes `Css.Tokens.<Comp>.*`; base/helper - `Css.Tokens.Vars`) |
| Variable emission | `src/Flare.Core/Services/CssVarMap.cs` |
| MD3 values | `src/Flare.Theme.MaterialDesign3Expressive/MaterialDesignTokens.cs` (+ dark theme) |
| Fluent values | `src/Flare.Theme.FluentUI2/FluentUI2Tokens.cs` (+ dark theme) |
| CSS classes | `src/Flare.Core/Css/Classes/<Comp>.cs` (namespace `Flare.Css.Classes`, holder classes `Css.Classes.<Comp>.*`) |

### Token rules
- Rely on semantic tokens: `--flare-color-*`, `--flare-shape-*`, `--flare-typescale-*`,
  `--flare-state-*`, `--flare-elevation-*`, `--flare-motion-*`.
- **No hardcoded values** in CSS - only `var(--flare-*)`.
- One set of CSS rules works in both MD3 and Fluent - the theme supplies the differences via token values.
- Component-specific tokens (geometry, typography, states) go in a separate `XxxTokens.cs`
  (see `ButtonTokens`, `MenuTokens` as references).
- When adding a new component, always create `XxxTokens.cs` and the `Css.Tokens.Xxx` holder.

### Already-implemented token records
The full set lives in `src/Flare.Core/Tokens/Components/`: Alert, Avatar, Badge, Button, Card,
Checkbox, Chip, DataGrid, Dialog, Drawer, Fab, Input, Menu, Popover, Progress, Radio, Select,
Slider, Snackbar, SplitButton, Switch, TableOfContents, Tabs, ToggleButton, Tooltip.

### 2.1 One color system - `FlareColor`
Any public color-choice parameter of a component is **only** `FlareColor` (one name everywhere:
`Color`). Do not introduce separate color enums (`LinkColor`, `TimelineColor`, ...) and do not
duplicate `Color` + `CustomColor` - all of it is unified in `FlareColor`.

`FlareColor` is a `readonly record struct` that holds **either** a semantic role
(`FlareColorRole`: Default/Primary/Secondary/Tertiary/Success/Warning/Error/Info/OnSurface/
OnSurfaceVariant) **or** an arbitrary CSS string:
- role -> a shared cached class `flare-color-{role}` (fast path, `Color.CssClass`);
- custom -> inline `--fc-*` variables on the element (`Color.IsCustom` / `Color.Value`,
  the value is sanitized via `CssValidator.SanitizeColor`);
- `FlareColor.Default` -> neither class nor variables (the component uses its own CSS fallback).

**Local role variables** (set by the `flare-color-*` class or inline):
`--fc-main` (accent), `--fc-on` (contrast on main), `--fc-container` (tonal background),
`--fc-on-container` (text on the tonal background). A component reads only the subset it needs
through `var(--fc-*, <fallback>)`. The names are constants in `Css.Tokens.LocalColor`:

| How many vars | Components | What they use |
| :-- | :-- | :-- |
| 1 (`--fc-main`) | Text, Icon, Rating, Progress, Input, Link, Timeline | accent / text / border |
| 2 (`--fc-main` + `--fc-on`) | Badge, Pagination, Chip | fill + contrast |
| 2 container (`--fc-container` + `--fc-on-container`) | Avatar, FAB, Calendar event | tonal background + text |
| 4 (all) | Button | filled/tonal/outlined/text variants |

Pattern in a component:
```csharp
private string  _colorClass => Color.CssClass ?? string.Empty;          // role -> class
private string? _colorStyle => Color.IsCustom                            // custom -> inline
    ? $"{Css.Tokens.LocalColor.Main}:{Color.Value};{Css.Tokens.LocalColor.On}:{FlareColorResolver.OnColor(Color.Value!)};"
    : null;
```
A component's CSS has **no per-color classes** (`flare-x--primary`, etc.); only
`var(--fc-*, <semantic fallback>)`. The role classes are defined once in
`wwwroot/css/color-roles.css`.

## 3. Minimal JS
- Effects (ripple, shape morphing, entrance animations) - done with CSS.
- **Do not use JS for animations.** If an effect is impossible without JS, document it as a
  limitation and ship the closest CSS approximation.

## 4. XML documentation (for API auto-generation)
- **Fully** document with XML comments all public types, `[Parameter]` properties, methods,
  enum members, etc. (used to auto-generate the API in the Gallery).
- Minimum: a `<summary>` on every public member; `<param>`/`<returns>` on methods.
- Style as in `FlareButton.razor` (every `[Parameter]` carries a `<summary>`).

## 5. Other
- Build and tests must pass; verify the component visually in `Flare.Gallery` (not in the legacy
  sample `Flare.Legacy`).
- Do not leave stale code (dead enums/classes) - remove it when you find it.
