# Migrating to 0.38

Everything here is mechanical: a rename, a token to add, or a parameter to set. Nothing needs a
redesign, and nothing changes how a component looks unless the entry says so.

**If you use an in-box theme and write no CSS against Flare's own classes, there is nothing to do.**
The work below is for two audiences: people who wrote their own `ITheme`, and people whose stylesheets
or code name Flare's CSS classes directly.

---

## 1. Custom themes: new required token members

`required` members do not have defaults, so each of these is a compile error until you supply a value.
The in-box themes state the numbers core used to hold, so copying them reproduces exactly what you had.

### `DesignTokens.Layer` - the stacking ladder (new record)

```csharp
Layer = new LayerTokens
{
    Chrome   = "100",   // pinned shell furniture: app bar, bottom nav, scroll-to-top
    Drawer   = "200",   // a drawer and its scrim
    Dropdown = "300",   // a panel anchored to its control: select, menu, popover
    Modal    = "400",   // dialog, message box, confirm
    Toast    = "500",   // snackbar
    Tooltip  = "600",
    Drag     = "700",   // what the pointer is holding
},
```

Keep them ascending and leave room between them: a component lifts one of its own parts with
`calc(var(--flare-z-drawer) + 1)`, and that offset must not reach the rung above. `LayerLadderTests`
checks both for every registered theme, so a mistake here fails the build rather than the page.

### `TypographyTokens.MonoFont`

```csharp
MonoFont = "monospace, monospace",   // or your own: "\"JetBrains Mono\", monospace"
```

Name a real family before the generic. A generic alone makes several engines use their own "monospace
default size", which paints text at a size nothing else measured - that is why the fallback repeats it.

### `NavTokens` - five colours

```csharp
ActiveColor    = "var(--flare-color-on-secondary-container)",  // see the note below
ItemColor      = "var(--flare-color-on-surface-variant)",
ItemHoverColor = "var(--flare-color-on-surface)",
GroupColor     = "var(--flare-color-on-surface-variant)",
MetaColor      = "var(--flare-color-on-surface-variant)",
```

**Pick `ActiveColor` to match your `ActiveIndicator`, not by copying the line above.** If your indicator
is a tinted pill (`secondary-container`), `on-secondary-container` is right. If your indicator is `none`
and you mark the active item with an accent bar, the label is read against the drawer plane and wants
`on-surface`. Core used to fix this to the container role for everyone, which is the whole reason the
token exists.

### `LayoutTokens` - seven members

```csharp
ShellBg   = "var(--flare-color-background)",
DrawerBg  = "var(--flare-color-surface-container-low)",
RailBg    = "var(--flare-layout-drawer-bg)",   // point at the drawer to keep them one surface
ContentBg = "transparent",
// No shadow: 0 draws none. Core owns the sign, so you never state which edge.
DrawerShadowOffset = "0px",
DrawerShadowBlur   = "0px",
DrawerShadowColor  = "transparent",
```

### `ColorScheme.SurfaceContainerLowest`

The sixth surface plane, furthest from the content.

```csharp
SurfaceContainerLowest = "#FFFFFF",   // light
SurfaceContainerLowest = "#0F0D13",   // dark (Material 3 N4)
```

It is distinct from `Background`, which stays the colour of the document rather than of a panel.

### `CheckboxTokens` / `RadioTokens` - the size ramp replaces `Size`

`Size` is gone. Its value becomes `SizeMd`, and the four steps core used to hardcode become yours:

```csharp
// Checkbox
SizeXs = "0.875rem", SizeSm = "1rem", SizeMd = <your old Size>, SizeLg = "1.375rem", SizeXl = "1.625rem",
StateLayerSize = "2.5rem",

// Radio
SizeXs = "1rem", SizeSm = "1.125rem", SizeMd = <your old Size>, SizeLg = "1.5rem", SizeXl = "1.75rem",
StateLayerSize = "2.5rem",
```

### `ChipTokens` - label typography

```csharp
LabelFont    = "var(--flare-typescale-label-large-font)",
LabelWeight  = "inherit",   // the chip declared none and took it from around it
LabelSpacing = "normal",    // likewise
LabelSizeXs  = "var(--flare-typescale-label-small-size)",
LabelSizeSm  = "var(--flare-typescale-label-small-size)",
LabelSizeMd  = "var(--flare-typescale-label-large-size)",
LabelSizeLg  = "var(--flare-typescale-title-small-size)",
LabelSizeXl  = "var(--flare-typescale-title-medium-size)",
```

`inherit`/`normal` rather than the label-large values on purpose: using the theme's weight here makes
every chip bolder than it was.

### Removed token members

| Removed | Use instead |
| :-- | :-- |
| `BottomNavTokens.ZIndex` | the `Chrome` rung of `LayerTokens` |
| `CheckboxTokens.Size` | `CheckboxTokens.SizeMd` |
| `RadioTokens.Size` | `RadioTokens.SizeMd` |

---

## 2. CSS variables

| Was | Now |
| :-- | :-- |
| `--flare-z-appbar` | `--flare-z-chrome` |
| `--flare-bottom-nav-z-index` | `--flare-z-chrome` |
| `--flare-checkbox-size` | `--flare-checkbox-size-md` |
| `--flare-radio-size` | `--flare-radio-size-md` |

If you overrode any of these in application CSS, repoint them. The new names are on the ladder or the
ramp, so setting one step no longer silently leaves the others where core put them.

---

## 3. `Css.Classes`: size constants are all `Size*` now

Three spellings were in use; one survives. Rename in your own code and stylesheets:

```
Css.Classes.Button.Md        ->  Css.Classes.Button.SizeMd
Css.Classes.Switch.Sm        ->  Css.Classes.Switch.SizeSm
Css.Classes.Avatar.Lg        ->  Css.Classes.Avatar.SizeLg
...and the same for SplitButton, Chip, Fab, Meter, Progress, Slider, Pagination,
   Badge, Checkbox, Radio, Rating, Input
```

`Container` is deliberately unchanged: its `Xs`..`Xl` are breakpoint max-widths, not a control size.
`Utility` likewise keeps its spacing steps.

The **CSS class names themselves did not change** - `flare-btn--md` is still `flare-btn--md`. Only the
C# constants were renamed, so a stylesheet that spells the class out needs no edit.

### The medium size now emits a class

Every size scale renders its step, including the default one. A selector that assumed the default was
the absence of a class needs updating:

```css
/* before: "the default size" meant "no size class" */
.flare-checkbox:not(.flare-checkbox--xs):not(.flare-checkbox--sm) { … }

/* now */
.flare-checkbox--md { … }
```

---

## 4. Behaviour changes worth knowing

### `FlareChip` is a tag unless something is wired to it

A chip with no `OnClick`, no `SelectedChanged` and no `FlareChipGroup` no longer renders
`role="button"`, a tab stop or the hover/focus/pressed layers. If your handler arrives by attribute
splatting rather than through a parameter, say so explicitly:

```razor
<FlareChip Interaction="ChipInteraction.Button" @onclick="Handle">…</FlareChip>
```

The root element is a `<span>` rather than a `<div>`. It always painted as `inline-flex`, so nothing
moves - but a stylesheet selecting `div.flare-chip` will stop matching.

### `FlareRichTextEditor` filters its value

`Value` and pasted markup are filtered against an allowlist before they reach the page. Formatting,
lists, headings, quotes, links and images survive; scripts, event handlers and unsafe URL schemes do
not. If your application produces the markup itself and has not let a user near it:

```razor
<FlareRichTextEditor @bind-Value="_html" Sanitize="false" />
```

Do that only when you can state that the markup is trusted. An editor whose value survives a round trip
through storage is exactly where a script smuggled into it runs again on every later view.

### Command buttons no longer submit the surrounding form

Thirty-seven `<button>` elements inside Flare components had no `type`, which in HTML means `submit`.
If you were relying on a calendar arrow or a pagination page to post the form around it - deliberately
or not - that no longer happens. `FlareButton Type="ButtonType.Submit"` still submits.

### The shell drawer's divider is logical

`border-inline-end` instead of `border-right`. Under RTL it now lands on the edge facing the content,
which is where it belonged.
