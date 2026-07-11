# Flare capability gaps found while building the "Weir" admin

**Source:** rebuilding the Weir admin PWA (a database-gateway control panel) to use Flare components
exclusively, deleting all bespoke app CSS. Flare `0.1.2`, Visual Studio theme.

**Follow-up:** a fifth gap was found later (during the dashboard live-metrics work, on Flare `0.1.4`):
`FlareChart` has no sparkline / chromeless line mode, so the dashboard's two mini-charts are still bespoke
inline SVG. Tracked separately in [flarechart-sparkline-mode.md](flarechart-sparkline-mode.md).

**Overall:** the component set covered almost the entire admin. The whole app shell, every form, every
list, every status indicator and the dashboard now compose from Flare primitives; `app.css` shrank
from ~650 lines to ~30 (only the pre-Blazor boot splash and the Blazor framework error bar, neither of
which is a Flare component). `FlareLayout` + `FlareLayoutDrawer` + `FlareNavMenu` gave the fixed
shell / scrolling-content behavior for free, and `FlareDataGrid`, `FlareField`/`FlareNumericField`/
`FlareSelect`, `FlareCard`, `FlareChip`, `FlareDivider`(labeled) and `FlareCodeBlock` did the heavy
lifting. The gaps below are the four places the app still had to fall back to composition-from-
primitives or a small inline style. None are blockers; each is a candidate for widening what an app can
express without touching CSS.

---

## Summary

| # | Gap | Severity | Fallback used | Status on `main` (0.1.3) |
|---|-----|----------|---------------|--------------------------|
| 1 | No description-list / property-grid component | medium | Composed from `FlareStack` + `FlareDivider` + `FlareText` rows | **Confirmed gap** -- build `FlareDescriptionList` (R1) |
| 2 | No inline-code component (`FlareCode`) | low | `<FlareText Element="span" Style="font-family:monospace">` | **Confirmed gap** -- `FlareText` has no mono option and cannot even render `<code>` (R2) |
| 3 | No file-upload / file-picker component | medium | Framework `<InputFile>` behind a label, with a visual-only `FlareButton` | **Already exists** -- `FlareFileUpload` in `Flare.Components.Media` (drop-zone); needs only a button variant (R3) |
| 4 | No "fill remaining space" / auto-fit layout primitive | low | Inline `flex:1 1 <basis>;min-width:0` on cards and chart columns | **Already exists** -- `FlareGrid.MinColumnWidth` + `FlareStack.StretchItems/StretchFirst`; minor residual (R4) |

---

## 1. No description-list / property-grid component

**Severity:** medium -- the most-repeated composition in the app.

**Need:** a read-only list of label -> value rows (a term/definition list) for detail panels. Weir shows
this for a stored procedure's discovered parameters: name + a direction chip on the left, the SQL type
on the right, with nested rows for table-valued-parameter columns.

**What Flare offers today:** nothing dedicated. `FlareDataGrid` is for tabular/sortable data, not a
two-column key/value panel, and carries header/filter chrome that is wrong here.

**Fallback:** a `FlareStack` with a `FlareDivider` before each row and a `FlareStack Row` of two
`FlareText`s per row (label group + value). Works and is theme-correct, but every detail view re-authors
the same row scaffolding.

**Proposed enhancement:** a `FlareDescriptionList` (or `FlareFields` read-only) that takes items and a
label/value template and renders consistent, aligned, optionally-striped rows -- the read-only analogue
of `FlareDataGrid` for key/value data.

**Answer (verified on `main` / 0.1.3):** Confirmed gap -- core Flare has no read-only key/value component.
The nearest thing is `FlarePropertyGrid` (+ `FlarePropertyGridItem`), but it lives in the
`Flare.Components.IDE` add-on and is a Visual-Studio-style *editable* property sheet (its value slot is
designed to host editors) wrapped in IDE panel chrome -- wrong for a general detail panel. The
`FlareDescriptionList` proposal stands, in core. **-> R1.**

---

## 2. No inline-code component

**Severity:** low.

**Need:** inline monospace for code-like tokens shown mid-flow -- HTTP routes (`POST /api/orders`),
SQL types, `schema.object` names, an API-key prefix. `FlareCodeBlock` is a block element, too heavy for
an inline token, and applies its own padding/background.

**What Flare offers today:** `FlareCodeBlock` (block only). No inline `FlareCode`/`FlareKbd`, and no
monospace typography role on `FlareText` (the `TypographyScale` roles are all proportional).

**Fallback:** `<FlareText Element="span" Style="font-family:monospace">` -- an inline style, repeated at
every code token.

**Proposed enhancement:** a `FlareCode` inline component (rendered `<code>`, theme-styled), and/or a
`monospace` option on `FlareText` (a `Mono` bool or a `Family` param), so code tokens need no inline CSS.

**Answer (verified on `main` / 0.1.3):** Confirmed gap. `FlareText` exposes no monospace option (its
params are `Typo`/`Element`/`Color`/`Weight`/`Align`/`AnchorId`), and its `Element` allow-list is
`h1-h6, p, span, div, strong, em, small, b, i` -- `code` is not even permitted, so you cannot render a
semantic `<code>` through it either. `FlareCodeBlock`/`FlareHighlighter` are block-level only. The
`FlareCode` proposal stands. **-> R2.**

---

## 3. No file-upload / file-picker component

**Severity:** medium.

**Need:** a button that opens the OS file dialog and hands back the chosen file. Weir's endpoint "Import"
takes a `.json` file.

**What Flare offers today:** nothing; the app uses the framework `<InputFile>`. `FlareButton` cannot wrap
an `<InputFile>` and still trigger the dialog (it renders its own `<button>`), so there is no Flare-styled
way to present a file trigger.

**Fallback:** a `<label>` wrapping a visual-only `FlareButton` (`Style="pointer-events:none"`) and a
transparent full-size `<InputFile>` overlaid on top, so a click on the label reaches the input. Two small
inline styles plus a framework control.

**Proposed enhancement:** a `FlareFileUpload` (button and/or drop-zone variants, `Accept`, `Multiple`,
an `OnFiles` callback) -- or, minimally, a documented file-input mode on `FlareButton` that renders the
label/input pairing correctly.

**Answer (verified on `main` / 0.1.3):** Largely already solved -- the report missed it because the
component ships in the `Flare.Components.Media` add-on, not core. `FlareFileUpload` already exists with
`Accept`, `Multiple`, `MaxFiles`, `DropText`, `Disabled` and an
`OnFilesChanged(IReadOnlyList<IBrowserFile>)` callback, wrapping the framework `<InputFile>` in a themed
drag-and-drop label -- it would have replaced the Weir fallback outright. What is missing is only the
*compact button* form factor: `FlareFileUpload` is a drop zone with no button/inline variant. **-> R3**
(add a variant + a discoverability note that file upload lives in `Flare.Components.Media`).

---

## 4. No "fill remaining space" / auto-fit layout primitive

**Severity:** low -- already acknowledged in Flare's own Gallery `MainLayout`
("Flare has no 'fills remaining space' primitive, so the flex sizing ... is expressed inline").

**Need:** cards/panels that grow to share a row and wrap to the next line at a minimum width (dashboard
stat tiles, data-connection cards), and content columns that fill remaining space beside a fixed rail
(two side-by-side charts).

**What Flare offers today:** `FlareStack Row Wrap` and `FlareGrid Columns="N"`. `Wrap` reflows but has no
per-item grow/basis; `FlareGrid` is a fixed column count with no auto-fit (`repeat(auto-fit, minmax(...))`)
equivalent, so it does not reflow by available width.

**Fallback:** inline `flex:1 1 <basis>;min-width:0` on each card, and a plain `<div style="flex:1 1 22rem;
min-width:0">` around each chart -- the same pattern Flare's own layout uses.

**Proposed enhancement:** either an auto-fit mode on `FlareGrid` (a `MinColumnWidth`/`AutoFit` param that
emits `repeat(auto-fit, minmax(MinColumnWidth, 1fr))`), or grow/basis props on `FlareStack` items (e.g. a
`FlareStackItem` with `Grow`/`Basis`), so responsive card walls need no inline flex.

**Answer (verified on `main` / 0.1.3):** Mostly already solved -- both pieces exist and predate 0.1.2, so
the report missed them. (a) `FlareGrid` already has `MinColumnWidth` (any CSS length; shipped since the
initial release) which emits `repeat(auto-fill, minmax(min, 1fr))` -- a media-query-free responsive card
wall, exactly the dashboard-tile case. (b) `FlareStack` already has `StretchItems` (every child
`flex:1 1 0`) and `StretchFirst` (only the first child `flex:1 1 auto`, since 0.0.8) for the
fill-remaining-space cases. Residual (minor): `FlareGrid` uses `auto-fill`, not `auto-fit`, with no toggle
(so 2 tiles in a wide row stay narrow instead of stretching to fill), and there is no *per-item* grow/basis
for asymmetric fills (`StretchItems` is all-or-nothing with a `0` basis). Also stale: Flare's own
`samples/Flare.Gallery/Layout/MainLayout.razor:46` still comments "Flare has no 'fills remaining space'
primitive" and uses inline flex -- `StretchFirst` now covers that. **-> R4** (small polish).

---

## Refinements to implement (доработки)

**All implemented on `main` (2026-07-11)** - see the commit against each item. Registry regenerated
(158 components / 1870 params / 100 enums).

- **R1 -- `FlareDescriptionList` (core, medium). DONE (68c6278).** Read-only label -> value panel:
  `FlareDescriptionList` + `FlareDescriptionItem`, aligned columns (a `<dl>` grid with `display:contents`
  rows), `Striped`/`Bordered`/`LabelWidth`, plain `Label` or rich `LabelContent` (name + direction chip),
  and nested rows by composing a list inside a value (the TVP-columns case). Gallery page + EN/RU.
- **R2 -- `FlareCode` + `FlareText.Mono` (core). DONE (30f4b47).** New inline `FlareCode` (themed `<code>`
  chip, same recipe as markdown inline code); `FlareText.Mono` swaps just the font; `code`/`kbd`/`samp`/`pre`
  added to the `FlareText` element allow-list (so `Element="kbd" Mono` renders keystrokes - covers the
  `FlareKbd` idea without a new type). Typography-page demo + EN/RU.
- **R3 -- `FlareFileUpload` button variant (Flare.Components.Media). DONE (88ac895).** New `Variant`
  (`DropZone` | `Button`); `Button` renders a compact pill that opens the file dialog, reusing the same
  input + file list. `ButtonText` (localized). Gallery demo + EN/RU. (Discoverability note: file upload
  lives in `Flare.Components.Media`.)
- **R4 -- Layout polish (core). DONE (e6782f9).** (a) `FlareGrid.AutoFit` -> `repeat(auto-fit, ...)` so a
  partially-filled card wall stretches to fill the row. (b) `FlareStack.StretchLast` (mirror of
  `StretchFirst`) for a fixed leading rail + a pane that fills the rest. (c) Dogfooded `StretchFirst` in the
  Gallery `MainLayout` and deleted the stale "no 'fills remaining space' primitive" comment. A full
  per-item `FlareStackItem(Grow/Basis)` was judged lower value than `StretchLast` + `MinColumnWidth` and
  deferred.

**Not required:** gaps 3 and 4 need no new components -- the capability already ships
(`FlareFileUpload`, `FlareGrid.MinColumnWidth`, `FlareStack.StretchItems`/`StretchFirst`); they need only
the small polish above plus better discoverability. The recurring meta-finding is discoverability: two of
the four "gaps" were existing features the app author never found.

---

## Not gaps (composition is the intended path)

- **Stat / metric tiles** -- `FlareCard` + two `FlareText`s (label + value) is exactly the pattern in
  Flare's own `LayoutShellAdminDemo`; no dedicated component needed.
- **Status pills** -- `FlareChip` (`Size="Sm"`, `Variant="Filled"`, semantic `Color`) covers every
  status indicator (healthy/error, enabled/disabled, endpoint/no-endpoint, active/revoked).
- **Section headings with a trailing hint** -- a `FlareStack Row` with `Justify="SpaceBetween"` and two
  `FlareText`s; labeled `FlareDivider` (`Text=...`) covers in-form section separators.
