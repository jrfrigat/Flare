# MD3 Expressive: size fidelity across all components

**Measured 2026-07-17 against Flare 0.7.0-dev.** Every number below was read from the RUNNING app with the
`md3-expressive` theme active and every `var()` chain resolved - not from the theme source, which shows the
MD3 baseline rather than this theme's result. Specs are `docs/spec/*/md3-expressive-spec.md` (29 files; the
`_pallete` one states no sizes). 1rem = 16px; 1dp == 1px at default scale.

Scope: SIZES only (heights, widths, icon sizes, paddings, radii, thicknesses). Colour, motion and elevation
are out of scope.

**Nothing in this report has been changed in code.** It is a decision document.

---

## The headline: MD3 Expressive is MD3 baseline + the button family

```csharp
// MaterialDesign3ExpressiveTheme.cs:20
public DesignTokens Design => MaterialDesignTokens.Design with { ButtonGroup = ... };
```

The theme overrides **exactly one** token record, and ships three scoped CSS files (button, button-group,
split-button) that add the press/hover morphs. For every other component it emits the MD3 baseline verbatim.

So most "deviations from MD3 Expressive" below are not oversights in a value - they are Expressive geometry
that was never adopted. Where a spec file carries two generations of the same component (a legacy
`md.comp.menu.*` family and a current `md.comp.menus.*` one), Flare consistently tracks the legacy numbers.
That is a coherent state to be in; it is just not the state the theme's name claims.

---

## Systemic causes (fix these and most individual rows follow)

### A. `FlareIcon`'s 22px default leaks into every component that does not pin an icon size

`icon.css:7` sets `font-size: var(--flare-typescale-title-large-size)` = 1.375rem = **22px** as the icon
baseline. Any component that renders a `<FlareIcon>` without setting a size inherits it. That single fact
produces at least six spec misses at once:

| Component | Spec | Flare | |
|---|---|---|---|
| Chip leading icon | 18dp | 22px | +4 |
| Chip trailing/close | 18dp | ~13.9px (`0.9em` of the 14px label) | -4 |
| Snackbar icon | 24dp | 22px | -2 |
| FAB icon | 24 / 28 / 36dp | 22px flat | -2 / -6 / -14 |
| Text-field leading/trailing icon | 24dp | 22px | -2 |
| Tab icon | 24dp | 22px | -2 |
| Card icon | 24dp | 22px | not expressed |

Note the shape of it: the two ends of a single chip currently render at 22px and ~14px against a spec that
pins both at 18dp. Components that DO pin a size (dialog 24dp, menu item 20dp, button ramp 20/20/24/32/40)
are all spec-exact - so the pattern to copy already exists in-house.

### B. Tokens that never reach the paint

The same class of defect as the MD2 `Gap = "0"` bug fixed in 0.7.0: the token is present, plausible, and
does nothing.

1. **`--flare-drawer-width: 360px` is dead.** It is spec-exact (MD3 nav drawer = 360dp) and consumed at
   exactly one site - `drawer.css:90`, `.flare-drawer--mini:hover`. The real width is a C# literal:
   `FlareDrawer.razor:43` `Width { get; set; } = "280px"`. The theme ships the right number; the component
   ignores it. (`--flare-layout-drawer-width` is a separate 260px.)
2. **The split-button's per-size seam ramp never paints.** `--flare-split-btn-main-radius-{size}-*` emits
   4/4/4/8/12px, which matches the spec's 4/4/4/8/12dp **exactly**. The theme's own
   `MaterialDesign3Expressive/wwwroot/css/components/split-button.css:38-42` then overrides every seam corner
   with `var(--flare-shape-small)` **`!important`** = 8px at every size. Five tokens dead; xs/sm/md +4, xl -4.
3. **Circular progress reads its geometry tokens as SVG user units, not px.** The SVG is
   `viewBox="0 0 44 44"` while the box is `--_circ-size`, so `stroke-width: 4px` resolves against the user
   coordinate system. **Verified by measurement**: at a 10x scale (440px box) a CSS `stroke-width: 4px` paints
   **40.5px**. At the real md size the theme's `4px` paints **3.64px** (x0.909). Every circular token is
   affected - stroke (xs 3px->1.64, md 4px->3.64, xl 5px->7.27), `circular-gap`, ring wave amplitude - and the
   ring itself paints 36.4px inside its own 40px box.
4. **`--flare-input-focus-border` / `--flare-input-focus-border-bottom` are never read.** Both variant
   classes set them; no rule consumes them. A theme tuning focus thickness through them gets silence.

### C. Geometry hardcoded in core CSS - correct value, unreachable by any theme

Not fidelity bugs (most of the values are right), but they violate the token mandate: no theme can retune
them. Nav link icon `1.25rem` (`nav.css:93`), submenu indicator `1.125rem` (`menu.css:80`), checkbox and
radio 40dp state layers, chip 1dp outline and 24dp avatar, list one/two-line heights, the whole time-picker
dial (256px / 8px / 2px), divider 1px, dialog divider 1px, card focus ring.

Worse: **the date/time pickers hardcode geometry inline in markup** - `FlareDatePicker.razor:47`,
`FlareDateTimePicker.razor` and `FlareTimePicker.razor:46` all carry
`<FlareIcon Style="font-size:1.25rem" />`. That is a literal dimension in a component's markup, bypassing the
token system entirely, and it is 20px against the spec's 24dp.

### D. The carousel is entirely un-tokenized

No `CarouselTokens` record and no `--flare-carousel-*` variable exists anywhere. Arrows (2.5rem), dots
(0.5rem / 1.25rem active), insets and the focus offset are literals in
`Flare.Components.Carousel/wwwroot/css/carousel.css`. A theme cannot resize the carousel at all. Its item
radius is also `var(--flare-shape-medium)` = 12px vs the spec's 28dp, and it sits on the carousel root rather
than on the slides, so the items are never rounded.

---

## Spec-exact components

**Checkbox, radio, switch, badge, tooltip, divider** are spec-exact on every size row they express.
Switch in particular: 52x32 track, 2dp outline, 16/24/28dp handle states, 16dp icon - all correct.
**Dialog, card and snackbar** are exact except where cause A bites. The snackbar deserves a note: its
`padding-block: 0.875rem` is tuned so both the 48dp single-line and 68dp two-line spec heights fall out of the
box model without a hardcoded height.

---

## Real deviations, by component

### Button family (known + new)

| Property | Spec (xs/sm/md/lg/xl) | Flare | |
|---|---|---|---|
| Container height | 32/40/56/96/136 | 32/40/48/56/64 | KNOWN (see `md3e-button-expressive-sizes.md`) |
| Leading/trailing space | 12/16/24/48/64 | 12/16/24/32/40 | KNOWN |
| **Icon-label gap** | 8/8/8/**12/16** | 8/8/8/**8/12** | **NEW: lg -4, xl -4** |
| **Outline width** | 1/1/1/**2/3** | 1px hardcoded, no token | **NEW: lg -1, xl -2** |
| **Icon-button sm icon** | **24** | 20 | **NEW: -4** (composition can't express it: button sm = 20dp, icon-button sm = 24dp) |

Icon sizes, the full label typescale, round shape, the pressed-morph ramp (8/8/12/16/16), xs/sm heights and
xs/sm/md padding are all exact.

### Split button

| Property | Spec | Flare | |
|---|---|---|---|
| Seam corner (rest) | 4/4/4/8/12 | painted 8 at every size | ramp dead - see B2 |
| Seam corner (hover/press) | 8/12/12/20/20 | capsule `height/2` = 16/20/24/28/32 | +8..+12 |
| Trigger width | 48/48/56/96/136 | 48px fixed | md -8, lg -48, xl -88; renders non-square where spec says square |
| Caret icon | 22/22/26/38/50 | 20/20/24/32/40 (reuses the *button* icon ramp) | -2/-2/-2/-6/-10 |

### Button group

- Inner corner fixed 8px vs spec 8/8/8/**16/20** (lg -8, xl -12).
- **Pressed morph goes the wrong way**: the spec *shrinks* the inner corner on press (8->4, 16->12, 20->16) and
  reserves the 50% capsule for the *selected* state. Flare grows it to a capsule on hover/press, and has no
  selected geometry.

### FAB

- No icon-size knob -> 22px flat vs spec 24/28/36dp. With `height:auto`, that makes md **54px vs 56dp**;
  setting the glyph to 24dp lands md on 56 exactly.
- lg computes 78px and matches neither 80 nor 96dp.
- Extended-FAB icon-label space is one 12px token vs spec 8/12/16dp (small +4, large -4).

### Slider

The spec has a full xs/sm/md/lg/xl ramp (lines 1057-1186) - Flare matches it on 14 of 15 rows.

| Property | Spec | Flare | |
|---|---|---|---|
| Track height (all 5) | 16/24/40/56/96 | identical | MATCH |
| Handle height | 44/44/**44**/68/108 | 44/44/**52**/68/108 | **md +8** |
| Focus handle width | 2dp | stays 4px (`pressed-width` applies on `:active` only) | the token exists; only the selector is missing |
| Value bubble height | 28dp | 24px (emergent, no token) | -4 |

The default (`TrackSize.Xs` = 16dp track / 44dp handle) is canonical - the old
`md3e-slider-default-size` issue was deleted as factually wrong.

### Menu

The spec carries both generations. Flare tracks the legacy one.

| Property | Spec (Expressive `md.comp.menus.*`) | Flare | |
|---|---|---|---|
| Item height | **44dp** (line 1016) | 48px (= legacy `md.comp.menu.list-item.container.height`, line 114) | +4 |
| First/last item outer shape | 12dp (line 1019) | `--flare-menu-item-radius-end: 4px` | -8; the *structure* is already right, only the value is wrong |
| Cascading indicator | 20dp | 18px hardcoded (`menu.css:80`) | -2, untokenized |

Everything else in the menu is Expressive-exact: 16dp panel, 2dp gap, 8dp group shape and padding, 4dp item
shape, 8/16/12dp spaces, 20dp icons.

### Navigation

| Property | Spec | Flare | |
|---|---|---|---|
| Nav bar height | **64dp** (Expressive) / 80dp (baseline) | 80px | **+16** - the largest visual miss, on the most Expressive-differentiated component |
| Rail width (collapsed) | 96dp (narrow 80dp) | 56px | **-40** |
| Drawer mini width | 96 / 80dp narrow | 72px | -24 |
| Nav drawer width | 360dp | 280px (C# literal; the 360px token is dead - B1) | -80 |
| Nav drawer icon | 24dp | 20px hardcoded (`nav.css:93`) | -4 |
| Nav drawer item height | 56dp (indicator) | ~40px derived | -16 |
| Rail item height | 64dp (short 56dp) | ~44px derived | ~-20 |
| Icon-label gap (bar + rail) | 4dp | 2px | -2 |
| Focus offset | **-3dp** | +2px (`a11y.css:18`) | wrong sign - the ring should be inset, it draws outset (global; also hits tabs and list) |

App bar: small (64dp) is exact; **Medium/Large/Medium-Flexible/Large-Flexible (112/152/112-136/120-152dp) have
no knob at all** - `FlareAppBar` exposes only `Dense`.

### Tabs

- Secondary indicator: 3px vs 2dp (one token serves both primary and secondary).
- Indicator radius 4px vs 3dp.
- Container height (48dp label-only / 64dp icon+label) has no knob - it is padding-derived at ~43px, and
  icon+label render side-by-side rather than stacked.

### List

- Supporting text 12px vs 14pt: `.flare-list-item__secondary` is glued to a shared helper rule
  (`input.css:223-227`) that pins `body-small`.
- Divider is full-bleed: 0px inset vs 16dp leading + 16dp trailing.
- Container radius 12px vs 16dp - and it borrows `--flare-card-radius`; a list has no radius token.
- Item padding-block 12px vs 10dp; between-space 16px vs 12dp.
- Item shape 0 vs 4dp (Expressive), and the whole state-morph ladder (hover 12dp, focus/pressed/selected
  16dp) is absent. Three-line items (88dp) absent.

### Input / pickers

| Property | Spec | Flare | |
|---|---|---|---|
| Text-field height (filled + outlined) | 56dp | **52px measured** | **-4**; there is no height token, and `input.css:112`'s comment claims "~56dp via 1rem + 24px line" while the CSS never sets a `line-height`, so the input computes `normal` (~18.75px) |
| Outlined focus outline | 3dp | 2px | -1 |
| Date-picker panel | 360dp wide, 16dp shape | `min-width: 18rem` (288px), 12px | -72 / -4 |
| Date-picker day grid | 48x48dp cells, 40dp state layer | fluid `1fr` columns, no state layer | not expressed (ironically `--flare-calendar-cell-min-height: 3rem` exists - but on `FlareCalendar`, a different component) |
| Today's date | 1dp outline ring | `font-weight:700` only | absent - a visible MD3 signature, not just a number |
| Clock-dial handle | 48dp | 36px | -12, and below the 48dp touch-target minimum |
| Time-selector box | 96x80dp | ~80x52px | -16 / ~-28 (root cause: display size 44px vs MD3's display-large 57px) |

**Search is N-A**: `FlareSearch.cs` is a static relevance-scoring helper, not a component. MD3's search bar
(56dp pill) and search view have no Flare counterpart - 36 spec rows unmapped.

### Progress

Linear is exact on every current row (4dp height, 10dp wavy, 4dp stop, 4dp gap, 3dp/40dp wave). The circular
variant is entirely governed by cause B3. Beyond that: no `with-wave` circular size (spec 48dp; Flare reuses
40px), and the ring wave is modelled as a **count** (`ring-waves: 8`) rather than a wavelength, giving 12.85px
vs the spec's 15dp - so the wave redistributes as the ring resizes instead of holding its wavelength.

---

## Not expressed - breadth gaps, not fidelity bugs

Full-screen dialog (4 rows), MD3 side sheet (256dp - `FlareDrawer` is faithfully a 360dp *nav* drawer, a
different component), date-picker Modal / Modal-input variants, time-picker Input variant, search bar/view,
app-bar Medium/Large/Flexible, horizontal menu, list leading media slots, button-group standard/separated
model, icon-button narrow/wide widths.

---

## Suggested order, if any of this is to be acted on

1. **Cause A** (icon default) - one root cause, six spec rows, and the in-house pattern to copy already
   exists. Cheapest fidelity win in the report.
2. **Cause B** (tokens that do nothing) - these are correctness bugs, not taste: a theme sets a value and
   gets something else. B1 (drawer width) and B3 (circular user units) are the worst because both ship a
   spec-exact number that never renders.
3. **The Expressive-vs-baseline question** - nav bar 64 vs 80, menu item 44 vs 48, list item shape. This is a
   product decision, not a bug: adopting them changes the look of every app. It is the same question as
   `md3e-button-expressive-sizes.md`, and should probably be answered once for the whole theme rather than
   per component.
4. **Cause C/D** (hardcoded and inline geometry) - mandate cleanup; no visual change.
