# Flare - Component Code Conventions

> [Русская версия ->](../ru/component-conventions.md) - [README](https://github.com/jrfrigat/Flare/blob/main/README.md)

A single code style for **all** components. The canonical reference is **`FlareButton`**.
When creating or changing any component, bring its architecture in line with these rules.

## 0. One responsibility - no "super components"

A type's description is its contract. If the XML doc says "renders text", the component cannot hold a
clipboard or a navigation manager. Three checks, any one of which means "not here":

- **A new injected service.** If the feature makes the component need something it otherwise would
  not, it belongs to another type. EVERY instance pays for it, not the ones using the feature.
- **The name stopped describing it.** If the name now wants "and also ...", that is two types.
- **It is application chrome, not library.** A heading anchor, a demo toolbar, a "show code" button
  serve the documentation site rather than the package consumer: they live in `samples/` and are
  assembled OUT of Flare components.

Compose instead of parameterising: the caller puts a small component beside it. A parameter is for
what the component itself is, not for the scenario around it. Removing one costs more than never
adding it - it is a breaking release.

**Example (0.32.0).** `FlareText` had `AnchorId`: it set an `id` and drew a "#" deep link, which made
a text component depend on `NavigationManager` and `IFlareClipboard` - hundreds of headings per page
resolving two scoped services for a feature a handful of them used. The parameter is gone: an id is
what the `id` attribute is for, and the "#" is the Gallery's `SectionAnchor`, built out of
`FlareText`, `IFlareClipboard` and `NavigationManager`.
## 1. CSS architecture - one global bundle

### CSS lives in `wwwroot/css/` (a global bundle), NOT in scoped `*.razor.css`

Flare is a published NuGet library with token-driven theming. All component styles live in the
**global bundle** `src/Flare.Components/wwwroot/css/*.css`, concatenated at build time into the single `flare-components.css` the package ships.

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
- The file is named in `flare-components.imports.css`, which is the load order the build concatenates in. A stylesheet nobody names fails the build rather than silently not shipping.
- When you touch a component, make sure its CSS file exists and is in the bundle.
- Examples: `button.css` -> `FlareButton`, `menu.css`/`menuitem.css`/`menugroup.css` -> the Menu group.
- Theme-specific tweaks (MD3 vs Fluent) go in `src/Flare.Theme.*/wwwroot/css/components/*.css`.

### A settled state never holds a transform

A state an element **rests** in (`--open`, `--visible`, `--shown`, `--expanded`) sets
`transform: none`, not an identity transform - `translate(0, 0)`, `translateX(0)`, `scale(1)`.
They paint identically and behave differently: any transform other than `none` makes the element a
**containing block for `position: fixed` descendants**. An overlay inside it is then positioned
against that element's corner rather than the viewport, and lands off screen. An open side panel
carried this for several releases: a select inside it drew 896px to the right of the window and read
as "the list opened empty".

It does not break the animation: interpolating a transform against `none` uses the identity matrix, so
`translateX(100%) -> none` is the same animation as `translateX(100%) -> translate(0, 0)`. During the
transition itself the transform is real and the containing block comes back - true for a panel whose
overlay is opened mid-animation, and unreachable in practice.

Inside `@keyframes` an identity transform is fine: an animation with no fill-mode falls back to the
base style when it finishes and holds nothing. Held by `SettledTransformTests`.

## 2. Tokens - through the token system (no hardcoded colors/numbers)

### Full path for adding a component token

| What | Where |
| :-- | :-- |
| Token values (per-theme) | `src/Flare.Abstractions/Tokens/Components/<Comp>Tokens.cs` |
| CSS variable names | `src/Flare.Abstractions/Css/Tokens/<Comp>Tokens.cs` (namespace `Flare.Css.Tokens`, holder classes `Css.Tokens.<Comp>.*`; base/helper - `Css.Tokens.Vars`) |
| Variable emission | `src/Flare.Theming/Services/CssVarMap.cs` |
| MD3 values | `src/Flare.Theme.MaterialDesign3Expressive/MaterialDesignTokens.cs` (+ dark theme) |
| Fluent values | `src/Flare.Theme.FluentUI2/FluentUI2Tokens.cs` (+ dark theme) |
| CSS classes | `src/Flare.Abstractions/Css/Classes/<Comp>.cs` (namespace `Flare.Css.Classes`, holder classes `Css.Classes.<Comp>.*`) |

> **Two token systems, linked by `[CssVar]`.** `Css/Tokens/*` holds the variable NAME constants; the
> records under `Tokens/*` hold the per-theme VALUES. Annotate each scalar value property with
> `[CssVar(Css.Tokens.<Comp>.<Prop>)]` (see `ButtonTokens` for the exemplar) so the value->name link is
> declarative; the `CssVarAttributeTests` drift test fails if an annotated name is not emitted by
> `CssVarMap.FlattenDesign`. Compound tokens that expand to several variables (per-corner radii,
> typography) stay mapped only in the flatten.

### Token rules
- Rely on semantic tokens: `--flare-color-*`, `--flare-shape-*`, `--flare-typescale-*`,
  `--flare-state-*`, `--flare-elevation-*`, `--flare-motion-*`.
- **No hardcoded values** in CSS - only `var(--flare-*)`.
- One set of CSS rules works in both MD3 and Fluent - the theme supplies the differences via token values.
- Component-specific tokens (geometry, typography, states) go in a separate `XxxTokens.cs`
  (see `ButtonTokens`, `MenuTokens` as references).
- When adding a new component, always create `XxxTokens.cs` and the `Css.Tokens.Xxx` holder.

### Two rules about the state layer and fallbacks

**A fallback in the core may not name a colour.** Inside the core's `wwwroot/css`,
`var(--flare-x, <identity>)` is fine - `1`, `auto`, `0deg`, `minmax(0, 1fr)`, a chain of other
variables: that is a per-instance channel a theme does not emit, and it carries no design decision. A
fallback that names a COLOUR decides for every theme at once how the component looks. The rule is
worded that way so that no exception list is needed: 44 reads across 19 tokens pass without being
enumerated, and a new one will pass too as long as it is an identity. Held by `CoreCssFallbackTests`.

**`color-mix(<semantic role> X%, <base>)` in a stylesheet is accepted, not unfinished work.** Some 23
files paint their states this way. The mandate allows semantic roles, so the coupling here is far
weaker than the `currentColor` overlay the whole state-layer work was started for, and pushing every
paint in the library through one channel would cost more than it returns. A separate layer is
introduced where the state is DIFFERENT: `--flare-state-selected-layer` for selection,
`--flare-datagrid-range-layer` for an Excel-shaped range - because calling a range and a selected row
one state would be untrue.

### Already-implemented token records
The full set lives in `src/Flare.Abstractions/Tokens/Components/`: Alert, Avatar, Badge, Button, Card,
Checkbox, Chip, DataGrid, Dialog, Drawer, Fab, Input, Menu, Popover, Progress, Radio, Select,
Slider, Snackbar, SplitButton, Switch, TableOfContents, Tabs, ToggleButton, Tooltip - plus ButtonGroup,
which carries both group models.

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

## 3a. Two-way parameter contract (controlled / uncontrolled)

A component with bindable state (`Expanded`/`ExpandedChanged`, `Value`/`ValueChanged`,
`Toggled`/`ToggledChanged`, ...) keeps its local state to one shape:

```csharp
protected override void OnParametersSet()
{
    if (ValueChanged.HasDelegate)      // CONTROLLED: the parameter is authoritative, always follow it
        _local = Value;
    else if (Value != _lastValue)      // UNCONTROLLED: overwrite local state only on a real change
        _local = Value;
    _lastValue = Value;                // mirror of the PARAMETER - assigned here and nowhere else
}
```

The rules it is made of:

- **`_last*` mirrors the parameter, not the state.** It answers exactly one question: did the parameter
  change since last time. Assigning it in an event handler (from the local state) is a defect: the
  mirror drifts from the parameter, and the next re-render of the parent *for any other reason* reads
  as an external change and reverts the component. That is what `FlareCollapse` did - it worked
  unreliably unless the caller drove its state.
- **Controlled means controlled.** If the parent listens to `XChanged` and decides not to change the
  parameter (a veto, a guard, an async confirmation), the component returns to the parameter's value.
- **Uncontrolled means local state survives the parent's re-renders.**
- The event handler changes local state only and raises `XChanged` - optimistically.

Held by `ControlledStateContractTests`; a new component with a two-way parameter adds its case there.

## 4. XML documentation (for API auto-generation)
- **Fully** document with XML comments all public types, `[Parameter]` properties, methods,
  enum members, etc. (used to auto-generate the API in the Gallery).
- Minimum: a `<summary>` on every public member; `<param>`/`<returns>` on methods.
- Style as in `FlareButton.razor` (every `[Parameter]` carries a `<summary>`).

## 4a. Content slot names

Razor decides that a child element is a named slot by matching its **tag name** against the component's
`RenderFragment` parameters; a component name is only checked when no parameter matches. So an
application component called `Icon`, written inside a Flare component that has an `Icon` slot, binds to
the slot and **never renders at all** - with no diagnostic, because both spellings are legal.

The rule:

- When the caller's text or markup is the **whole** of what the component renders, the slot is
  `ChildContent` (`FlareChip`, `FlareCheckbox`, `FlareDivider`).
- When it is **one named part** among several, the slot is `XxxContent`: `TitleContent`, `LabelContent`,
  `IconContent`, `HeaderContent`, `ActionsContent`.
- A slot is **never** a bare noun an application would plausibly name a component: `Icon`, `Header`,
  `Footer`, `Avatar`, `Badge`, `Actions`, `Placeholder`, `Empty`, `Counter`.

Guard: `SlotNameTests`, which also records the exceptions - `Columns` and `Grouping` on the data grid
stay, because they are collection slots every grid library spells this way and renaming them would cost
consumers more than the collision risk is worth.

Separately: an `Icon` parameter of type `FlareIcon` is a **value**, not a slot, and needs no rename. That
distinction is the reason the fragments moved to `IconContent`: `Icon` now means an icon everywhere.

## 5. Other
- Build and tests must pass; verify the component visually in `Flare.Gallery`.
- Do not leave stale code (dead enums/classes) - remove it when you find it.
