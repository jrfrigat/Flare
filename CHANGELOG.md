# Changelog

All notable changes to Flare are documented here. This project adheres to
[Semantic Versioning](https://semver.org/).

## [0.3.0] - 2026-07-16

A consolidation release around one idea: **a colored band on a track is one mechanism**. The slider's zones
grow into a model shared by the slider, the progress bar and a new `FlareMeter`, so the three no longer each
reinvent the same band - while the two kinds of band stay distinct types, because a range on a host's scale
and a weight that defines the scale are not the same thing. Plus a new part-to-whole meter, and hardening
that keeps a theme's token mismatch from breaking a control.

### Added
- **`FlareMeter`**: a part-to-whole bar - how a whole divides into proportional colored parts (a
  request-timing breakdown, a storage quota, a pass/fail/skip test ribbon). Non-interactive. Parts are
  `<FlareMeterSegment Value="..." Color="..." Label="..." />` children, sized in proportion to their sum,
  with an optional legend (`ShowLegend`, `ShowValues`, `Format`). Its track reuses the `FlareProgress`
  linear-track tokens (height, rounded ends, resting background), so a meter and a progress bar line up
  visually.
- **`FlareZone` now works on `FlareProgress` too**: the slider-only `FlareSliderZone` is generalized into
  `FlareZone`, an absolute `[Start, End]` range usable in both `FlareSlider` and `FlareProgress` - the two
  hosts that own their scale.
- **One shared mechanism behind both kinds**: `FlareZone` and `FlareMeterSegment` derive from
  `FlareZoneBase` and register through the same `IFlareZoneHost` contract, so all three hosts reuse one
  registration path and one band model. They stay **separate types on purpose**: a zone is a range on a
  scale the host owns, whereas a meter part is a weight that helps *define* the scale - so each type carries
  only the parameters that mean something for it, and putting the wrong one inside a host now raises a clear
  error instead of silently rendering nothing.
- **`FlareProgress` colored zones**: a declarative `<Zones>` slot for static bands on the 0-100 track
  (threshold / danger ranges, a loaded-so-far band), drawn under the active bar - the same layering the
  slider uses. Because zones need an uninterrupted track, using them renders a continuous track instead of
  the split (gap + trailing stop dot) one.
- **`FlareClipboard.Color`**: the copy control forwards a semantic color to its inner button, so it can be
  an emphasized call-to-action (e.g. a filled Primary "copy your new secret"). It previously forwarded only
  `Variant` and `Size` and silently dropped the color.
- **`IFlareButton.Color`**: the shared button contract now carries the semantic color alongside
  `Variant`/`Size`, so button wrappers forward the full appearance surface instead of dropping part of it.

### Changed
- **`FlareSliderZone` is deprecated** in favour of `FlareZone`. It still works - it is now a thin subclass
  of `FlareZone` - so existing `<FlareSliderZone Start End Color />` markup keeps compiling; it just warns.
  A media "buffered" band is expressed as a plain zone from the track start to the loaded point, so no
  dedicated buffer parameter is needed on the slider.

### Fixed
- **`FlareSwitch` thumb can no longer overflow its rail in any theme**: the handle is now clamped to the
  track height in the core CSS, so a theme that pairs a compact rail with a larger grow-on-check thumb
  degrades gracefully instead of rendering a ball bulging out of the track. This is a no-op for every
  built-in size (their thumbs already fit) and complements the Visual Studio theme's own token fix in 0.2.0
  by protecting any future theme from the same class of mismatch.

## [0.2.1] - 2026-07-13

A correctness patch for the optional `Flare.Components.QrCode` package: it generated QR codes that looked
valid but did not scan on any conforming reader, for every input. Three encoder defects are fixed and the
generator is now covered by a scannability regression suite.

### Fixed
- **`FlareQrCode` produced unscannable codes**: three independent bugs in the pure-C# encoder, each on its
  own enough to break decoding on a standard reader:
  - Reed-Solomon error-correction codewords were computed with an off-by-one in the systematic division
    (the generator's leading coefficient was not skipped), corrupting the EC of every code.
  - The error-correction block structure was wrong for two version/level combinations (version 3 at level
    M, version 4 at level Q), so a reader de-interleaving by the standard structure failed.
  - The level-H format-information constants for masks 5, 6 and 7 were wrong, so the recorded mask did not
    match the applied mask and the reader could not un-mask the data.
  Codes now decode correctly across all error-correction levels (L/M/Q/H) and supported versions (1-4),
  verified by an independent Reed-Solomon round-trip decoder.

## [0.2.0] - 2026-07-13

A fields, slider and theme-fidelity release: gaps found while building real apps on Flare (the Weir admin
and the PlaylistShared / Deka player) - colored slider zones, keyboard events across the field family, and
focus/visibility fixes - plus a pass over the in-box FluentUI2 and Material Design 3 Expressive themes so
they render faithfully, and a first step to decouple the core engine from any one theme's model.

### Added
- **`FlareSlider` colored zones**: a declarative `<Zones>` slot with `<FlareSliderZone Start End Color />`
  children - static colored regions on the track (safe/warning/danger gauges, a media-buffer band, or
  per-step coloring), each in its own `FlareColor`. Zones are drawn under the active fill (which always
  shows the current value on top) and work in single, range and vertical modes.
- **Keyboard events on the field family**: `OnKeyDown` / `OnKeyUp` on `FlarePasswordField`,
  `FlareMaskedField`, `FlareTextArea` and `FlareNumericField` (forwarded to the inner input), so patterns
  like "press Enter in the password field to submit the form" work without a wrapper handler.
  `FlareNumericField` raises them after its built-in ArrowUp/ArrowDown stepping.

### Changed
- **Theme authoring**: `InputTokens` gains required `FocusRing`, `FocusOutline` and `FocusOutlineOffset`
  fields - the field focus indicator, either a box-shadow ring or a real CSS outline. Custom themes that
  construct `InputTokens` directly must supply them; themes derived from the in-box themes via `with` inherit them.
- **Theme-agnostic state layer**: `StateTokens` gains required `HoverLayer`, `FocusLayer`, `PressedLayer`
  and `DraggedLayer` fields - the paint (colour + alpha) of the hover / focus / pressed / dragged overlay.
  The core no longer bakes a translucent-currentColor state layer; each theme now chooses its state model
  (a Material wash, or a discrete Fluent fill). Custom themes that construct `StateTokens` directly must
  supply them; themes derived via `with` inherit them. Component CSS also no longer carries baked literal
  fallbacks - every visual value now comes from the theme's tokens, so the core carries no theme opinion.

### Fixed
- **`FlareSwitch` in the Visual Studio 2026 theme** rendered with the "on" thumb overflowing the rail:
  the theme carried Material Design 3 thumb sizes (a 24px thumb) on a compact 40x20 rail. It now uses the
  Fluent v9 geometry - a 14px thumb, the same size in both states, that fits the rail.
- **Field focus indicator restored**: every `FlareField`-based control (`FlareField`, `FlarePasswordField`,
  `FlareTextArea`, `FlareNumericField`, `FlareSelect`, the pickers) had no focus affordance on mouse or
  keyboard. A layout-neutral, token-driven indicator is now drawn on `:focus-within`, configurable per theme
  and per variant - a box-shadow ring (a bottom active indicator or a full ring) or a real CSS outline. MD3
  and Fluent use the ring; Visual Studio uses an outline; the filled/outlined variants pick their own.
  Invalid fields get an error-colored ring.
- **FluentUI2 theme fidelity**: disabled controls now use Fluent's flat disabled palette (a discrete grey
  fill / foreground / stroke) instead of a 40%-faded copy of the enabled look; hover and pressed use
  Fluent's discrete fills (the neutral subtle greys, a darkened brand on filled buttons, a darkened stroke
  on outlined) instead of a translucent Material state layer; non-filled buttons get Fluent's neutral
  double focus ring. In the gallery the palette follows the active theme's own default, so Fluent shows its blue.
- **Material Design 3 Expressive theme fidelity**: outlined cards use the outline-variant role; the field
  focus indicator is the Expressive 3dp active indicator (was 2dp); the navigation active label uses weight
  700; the checkbox rest outline uses on-surface-variant (matching radio); list items take the one/two-line
  heights (56 / 72dp) and the selected item uses the secondary-container role; menu item height is 48dp.
- **Rich tooltips were unclickable** (every theme): the tooltip surface suppressed pointer events and never
  re-enabled them for the rich variant, so its actions could not be clicked. Rich tooltips are now
  interactive and use the medium shape.

## [0.1.9] - 2026-07-12

A polish release: gap follow-ups surfaced while building real apps on Flare (the Weir dashboard and the
PlaylistShared / Deka design), turning Style-only escape hatches into first-class parameters.

### Added
- **`FlareLayoutAppBar`**: `Height` (any CSS length) and `Dense` (a slimmer 48px bar for tool-window /
  IDE-style shells) - both drive the `--flare-layout-appbar-height` token; plus a dedicated
  `--flare-layout-appbar-bg` token so an app or theme can lift the nav surface above the canvas without
  inline CSS.
- **Date/time pickers**: `Autofocus` on `FlareDatePicker` / `FlareTimePicker` / `FlareDateTimePicker`
  (focuses the input on first render), matching the editable field family.

### Changed
- **`FlareChart` sparkline height is now fixed pixels**: in `Sparkline` mode `Height` pins the SVG's CSS
  pixel height (full-width, non-scaling) instead of scaling with the container width - the real sparkline
  contract. Non-sparkline charts keep the width-driven aspect ratio.

### Fixed
- **Icon-only buttons** (`FlareIconButton` and friends) rendered the glyph ~2px off-center: the
  leading/trailing optical tuck (meant for an icon next to a label) was not reset for the label-less
  icon-only case. The lone glyph is now centered; icon+label buttons keep the intentional tuck.

## [0.1.8] - 2026-07-11

An overlay/dialog release: cross-framework audit follow-ups (vs MudBlazor / Blazorise / DevExpress /
Fluent UI Blazor) across the whole overlay family - tooltip, menu, snackbar, popover and dialog.

### Added
- **`FlareTooltip`**: `Delay` (hover-intent show delay), independent `ShowOnHover` / `ShowOnFocus` /
  `ShowOnClick` triggers, an `Arrow`, and `Disabled` (the rich variant is now wired when `TooltipContent`
  is set).
- **`FlareMenu` context menu**: `Activation="RightClick"` turns it into a context menu (suppressing the
  browser menu), `PositionAtCursor` pins the panel to the pointer, `MaxHeight` scrolls a long list, and
  `FlareMenuItem.AutoClose="false"` keeps the menu open for toggle-style / multi-action items.
- **Snackbar**: `SnackbarOptions.PreventDuplicate` de-dupes repeats, `ISnackbarService.Remove(id)` and
  `Clear()` dismiss one or all programmatically, and `Show(RenderFragment, ...)` renders a custom,
  component-based body instead of plain text.
- **`FlarePopover`**: `Trigger="Hover"` (with `Delay` / `HideDelay`), `MaxHeight` scrolling, and
  `MatchAnchorWidth` (dropdown-style width that tracks the anchor); `MinWidth` / `MaxWidth` are now applied.
- **`FlareDialog`**: `ShowCloseButton` (built-in header X), a cancelable `BeforeClose` guard (veto a
  scrim / Escape / close-button dismissal, e.g. for unsaved changes), and `Draggable` + `Resizable` (drag
  the header, resize from a bottom-right gripper).

### Changed
- Dialogs now dismiss automatically on navigation by default (`CloseOnNavigation`, opt-out) - matching
  MudBlazor and Fluent UI Blazor.

## [0.1.7] - 2026-07-11

A date/time + charts release: cross-framework audit follow-ups for the pickers, and a ground-up
expansion of `FlareChart` (still native SVG, zero-JS, token-themed throughout).

### Added
- **Date/time pickers - public imperative API** on `FlareDatePicker` / `FlareTimePicker` /
  `FlareDateTimePicker`: `OpenAsync()`, `CloseAsync()`, `ToggleAsync()`, `ClearAsync()`, `FocusAsync()`,
  plus `Opened` / `Closed` events.
- **`FlareDatePicker`**: `OpenTo` (Day/Month/Year - jump straight to the year grid for far-back dates),
  `AutoClose`, `Inline` (an always-open calendar in normal flow), `ShowWeekNumbers`, an explicit
  `FirstDayOfWeek` override, and `DayClassFunc` for per-day custom CSS (holidays, highlights).
- **`FlareTimePicker`**: `ShowSeconds`, `Min` / `Max` time (out-of-range cells disabled), and `HourStep`.
- **`FlareChart` grew from 4 to 13 chart types** - added `Area`, `StackedBar`, `Scatter`, `Bubble`,
  `Radar`, `HeatMap`, `Rose`, `PolarArea` and `Combo` (per-series `ChartSeriesKind` bar/line/area).
- **`FlareChart` sparkline & fills**: `Sparkline` (chromeless, edge-to-edge with a crisp stroke), `Area`
  gradient fill, `Smooth` curves, `ShowMarkers`, and granular `ShowGrid` / `ShowLegend` /
  `ShowXAxisLabels` / `ShowYAxisLabels` / `LegendPosition` / `Padding` toggles.
- **`FlareChart` config & interactivity**: `YMin` / `YMax`, `YAxisFormat`, `XAxisTitle` / `YAxisTitle`,
  `Horizontal` bars, `ShowValues`, `OnPointClick`, `DonutRingRatio`, `BarWidthRatio`, an interactive
  legend (click a label to toggle a series), `TrendLine` (least-squares overlay) and `Annotations`
  (threshold / target / band overlays).
- **`FlareChart` polish**: `Animate` (token-driven, `prefers-reduced-motion`-aware enter animation - a
  differentiator vs Chart.js's JS animation and MudBlazor's none) and `DataTable` (a visually-hidden data
  table so screen readers can read the values).

## [0.1.6] - 2026-07-11

A field-family release: follow-ups from a cross-framework audit (vs MudBlazor / Blazorise / Fluent UI
Blazor) applied across the whole text/input field family.

### Added
- **`FocusAsync()` across the editable field family** - a new `FlareEditableFieldBase` gives `FlareField` /
  `FlareTextField`, `FlarePasswordField`, `FlareNumericField`, `FlareMaskedField` and `FlareTextArea` a
  programmatic `FocusAsync()`; `FlareOtpField` focuses its first cell. Plus `SelectAsync()` / `BlurAsync()`
  (and `SelectRangeAsync()` on `FlareField`), backed by three new `IElementJsService` helpers.
- **`Autofocus`** (focus on first render) and **`OnFocus` / `OnBlur`** events on the text-entry fields.
- **`Pattern`** (regex) and **`InputMode`** on `FlareField` / `FlareTextField`; **`Autocomplete`** and
  **`Spellcheck`** on the text fields; **`DataList`** (native `<datalist>` suggestions).
- **`Clearable` on every editable field** (was `FlareField`-only) plus **`OnClearButtonClick`**.
- **`FlareNumericField`**: public **`Increment()` / `Decrement()`** and **`SelectAllOnFocus`**.
- **`FlareTextArea`**: **`Resize`** (None/Vertical/Horizontal/Both) and **`Spellcheck`**.
- **`HelperTextOnFocus`** - shows the helper text only while the field is focused.

### Changed
- **`FlareOtpField` now composes the shared field chrome** (`FlareFieldChrome`) like the rest of the
  family: it gains `Label`, `HelperText` / `ErrorText` (a real message row, not just the red-cell `Error`
  bool), `Required`, `ReadOnly` and `EditContext` / `For` validation - an error message now also reddens
  the cells. The cell row itself is unchanged.

## [0.1.5] - 2026-07-11

A button-family release: follow-ups from a cross-framework audit (vs MudBlazor / Blazorise / Fluent UI
Blazor) applied across the whole button family.

### Added
- **`FlareButton`**: `FocusAsync()` (programmatic focus via a captured `ElementReference`),
  `LoadingTemplate` (custom loading content replacing the default spinner), and an explicit `Rel`.
- **`FocusAsync()` across the button family** - `FlareIconButton`, `FlareToggleButton`, `FlareSplitButton`
  (focuses the primary action) and `FlareFloatingActionButton`.
- **`ButtonEdge`** (`None`/`Start`/`End`) on `FlareIconButton` and `FlareToggleButton` - optical edge
  alignment (a negative inline margin) for app bars, toolbars and list-item leading/trailing slots.
- **`FlareToggleButton.Toggle()` / `SetToggledAsync(bool)`** - programmatic toggle control.
- **`FlareToggleGroup.Mandatory`** (single-select cannot be cleared) and **`CheckMark`** (a leading check
  on the selected item).
- **`FlareSplitButton`**: `Loading`, `FullWidth`, `Href`/`Target`/`Rel` (the primary action as a link),
  `Placement` (`MenuAnchor`), and public `Open()` / `Close()`. `FlareMenu` gains public
  `OpenAsync()` / `CloseAsync()`.
- **`FlareIconButton`**: `OnColor` and a `Rel` override.

### Changed
- **Link buttons default `rel="noopener noreferrer"` when `Target="_blank"`** (`FlareButton`,
  `FlareIconButton`, `FlareSplitButton`) - prevents reverse tabnabbing via `window.opener`. Override with
  the new `Rel` parameter.
- **`FlareFloatingActionButton` and `FlareFloatingActionMenuItem` `OnClick` are now
  `EventCallback<MouseEventArgs>`** (were the argument-less `EventCallback`), for consistency with the rest
  of the button family.

## [0.1.4] - 2026-07-11

### Added
- **`FlareDescriptionList` / `FlareDescriptionItem`** - a read-only key/value detail panel (the
  read-only analogue of `FlareDataGrid`), rendered as a semantic `<dl>` two-column grid so labels and
  values stay aligned across rows regardless of content width. `Striped`, `Bordered` and `LabelWidth`
  options; each item takes a plain `Label` or rich `LabelContent`, and lists nest by placing a
  `FlareDescriptionList` inside a value.
- **`FlareCode`** - a themed inline `<code>` chip (monospace on a subtle surface-container tonal chip,
  extra-small radius) for a code token mid-prose, matching the inline-code recipe used by the Markdown
  renderer so prose and standalone tokens read identically.
- **`FlareText.Mono`** - swaps `FlareText` to a monospace font while keeping the type-scale metrics;
  `code`, `kbd`, `samp` and `pre` are added to the element allow-list, so `<FlareText Element="kbd" Mono>`
  renders real keystrokes.
- **`FlareFileUpload.Variant`** (`FileUploadVariant.DropZone | Button`) - a compact button form factor
  that opens the OS file dialog with no drop area, reusing the same selected-file list, plus a localized
  `ButtonText`.
- **`FlareGrid.AutoFit`** - with `MinColumnWidth` set, emits `repeat(auto-fit, ...)` instead of
  `auto-fill` so the present cards stretch to fill the row with no empty trailing tracks.
- **`FlareStack.StretchLast`** - the mirror of `StretchFirst`: only the last child grows, for a fixed
  leading rail beside a pane that fills the rest.

## [0.1.3] - 2026-07-10

### Added
- **`IBrowserViewportService`** - one dependency-injected entry point for everything responsive: the
  current viewport size and breakpoint (`GetViewportSizeAsync`/`GetBreakpointAsync`), arbitrary CSS
  media-query matching (`MatchesAsync`), throttled window-resize and breakpoint-tier subscriptions
  (`SubscribeAsync`/`SubscribeBreakpointAsync`), and per-element `ResizeObserver` observation
  (`ObserveElementAsync`). Subscriptions return an `IAsyncDisposable` token - no observer interface to
  implement, no `DotNetObjectReference` to create, no subscription id to track: the service owns a single
  JS listener shared across all subscribers. Registered by `AddFlare()`; returns a configured fallback
  during prerender. New Gallery demo (EN + RU).
- **`Xxl` breakpoint** (default 2560px and up) on the shared breakpoint scale: `Breakpoint.Xxl` plus a
  matching `FlareCol.Xxl` column span, extending the responsive grid past the previous five-tier ceiling.

### Changed
- **`FlareMediaQuery`, `FlareLayout` and `FlareDateTimePicker`** now observe the viewport through
  `IBrowserViewportService` instead of each exposing its own `[JSInvokable] OnBreakpointChanged` callback -
  one shared, throttled resize listener instead of a per-component JS round-trip.

## [0.1.2] - 2026-07-07

### Added
- **Bottom-sheet dialogs.** `DialogOptions.Position` (`Center`/`Bottom`), `DialogOptions.ShowGrabber`,
  and `IDialogService.ShowSheetAsync<T>` present the same imperative component-dialog contract (typed
  parameters + cascaded `FlareDialogInstance` + `DialogResult`) as a slide-up bottom sheet: full-width,
  rounded top corners, grabber handle, safe-area padding. New `DialogOptions.PanelClass`/`ScrimClass`
  also let an app skin a specific dialog (e.g. glass) without global CSS. New Gallery demo (EN + RU).
- **`ColorScheme.OnSurfaceVariant2`** - a third, fainter neutral on-surface text tone below
  `on-surface-variant` for tertiary text (footnotes, counts, captions). Exposed as
  `FlareColor.OnSurfaceVariant2`, `Colors.OnSurfaceVariant2`, the `--flare-color-on-surface-variant2`
  variable and the `.flare-color-on-surface-variant2` utility. The `2` suffix leaves room for a future
  `OnSurfaceVariant3`. All in-box themes and the MD3/Fluent reference packages supply a value.

### Changed
- **Flare no longer draws a loading splash; each app owns its own** (background + animation).
  `flare-bootstrap.js` now only applies the saved theme/palette/mode classes to `<html>` before first
  paint and fires a readiness signal - `window.hideFlareSplash()` dispatches a `flare:ready` event and
  fades out the app's own splash element if it is tagged `id="flare-splash"` / `[data-flare-splash]`.
  The built-in spinner, theme-colored backdrop and the `data-splash-light`/`data-splash-dark`
  attributes are removed (`data-splash-timeout` is kept, also readable as `data-ready-timeout`). Apps
  that relied on the built-in splash should add their own to `index.html` (see the getting-started
  guide); `FlareThemeProvider.ManageSplash` and `revealApp`/`RevealAppAsync` are unchanged in name.

## [0.1.1] - 2026-07-06

A small follow-up release: mouse-wheel control on the Slider, a theme-aware SignaturePad stroke color, and
two component bug fixes.

### Added
- **`FlareSlider.MouseWheel`** - opt-in mouse-wheel control. When enabled, hovering the slider and turning
  the wheel moves `Value` by one `Step` (scroll up increases, down decreases) and page scrolling is
  suppressed over the track. In range mode a plain wheel moves the low handle (`Value`) and `Ctrl`+wheel
  moves the high handle (`ValueEnd`); neither handle can cross the other. New Gallery demo (EN + RU).

### Changed
- **`FlareSignaturePad.StrokeColor` is now a `FlareColor`** (was a raw CSS string). It accepts a semantic
  role (`FlareColor.Primary`), a custom color (`FlareColor.Custom("#e53935")` or the implicit string
  conversion) or a dynamic color, and defaults to `FlareColor.OnSurface`. Because a `<canvas>` cannot
  resolve CSS variables, the color is now resolved against the live theme before it is applied as the
  stroke style - so a role/token-based stroke actually renders (the previous `var(--flare-*)` default was
  passed straight to the canvas and silently fell back to black). The basic Gallery demo now strokes in the
  primary color.

### Fixed
- **`FlareMaskedField` no longer drops the first character on a mask that starts with a literal.** With a
  mask like `+# (###) ###-##-##` or `(###) ###-####`, the leading literal (`+`, `(`) meant the first typed
  digit was discarded and nothing appeared; the leading literal is now auto-filled, so typing `7` renders
  `+7 (`.
- **`FlareRichTextEditor` toolbar buttons work again.** Every toolbar command threw
  `ReferenceError: dotNetRef is not defined` (an out-of-scope reference in `execCommand`) and applied no
  formatting; it now uses the per-editor .NET reference, so bold/italic/lists/headings/links apply and the
  content change is reported back.

## [0.1.0] - 2026-07-06

Flare's theming API reaches completeness with this release: every value the component CSS reads is now a
themeable `--flare-*` token, the token registry is audit-clean and fully in sync with the CSS, and no
Material Design opinion remains baked into the core. That milestone is also a repositioning. Flare is a
theme-agnostic Blazor component library - a token-driven engine for building your own design system,
where components ship with zero baked-in styling and every color, shape, size, and motion comes from a
theme you control through one semantic token API. Seven production-ready preset themes (MD3 Expressive,
MD3, MD2, Fluent UI 2, Aero, Liquid Glass, Visual Studio 2026) ship as independent, optional packages -
pick one to start instantly, or build a fully custom theme from scratch; the umbrella `Flare.Blazor`
package ships no theme of its own.

### Added
- **14 new component token families make previously hard-coded component geometry themeable.** `AppBar`,
  `Breadcrumb`, `DateTimePicker`, `Dropzone`, `Form`, `Layout`, `Link`, `Otp`, `Picker`, `Scrim`,
  `ScrollTop`, `Skeleton`, `Table`, and `TimePicker` gained token records, and 13 existing families
  (`alert`, `button`, `calendar`, `checkbox`, `radio`, `menu`, `nav`, `stepper`, `tabs`, `toc`, `toggle`,
  `tree`, `splitter`) were extended to cover values the CSS still read as literals. Every newly wired
  value equals the prior CSS fallback, so the shipped themes render identically - the change is that a
  custom theme can now override these too.
- **`cssaudit tokens` report plus a build gate.** A token analog of the existing class audit cross-checks
  every `--flare-*` token referenced in component CSS against the `Css.Tokens` registry and reports drift
  in either direction - `[T+]` a token used in CSS with no constant, `[T-]` a constant no CSS references,
  `[T~]` a theme-only token. The registry now audits fully in sync (`[T+]0 / [T-]0 / [T~]0`), and
  `CssAuditTests.CssTokens_Components_And_Themes_StayInSync` fails the build if that drifts.
- **Extended the semantic motion scale** with `short3` / `short4` durations, so components that needed an
  intermediate duration reference a scale token instead of a literal.

### Changed
- **`Flare.Css.Tokens` uses direct `--flare-*` string literals** instead of the `Vars.Flare` prefix
  indirection, and the breakpoint variables were aligned from `--flare-bp-*` to `--flare-breakpoint-*` to
  match the rest of the naming scheme.
- **Every wired token family was reconciled to CSS reality under the theme-agnostic mandate.** Where a
  per-component token merely forwarded a shared role, typescale, spacing, motion, or elevation token, the
  pass-through duplicate was deleted and the CSS now references the shared token directly; only genuine
  component-specific geometry was kept and wired. Families touched: `Avatar`, `Tooltip`, `DataGrid`,
  `Popover`, `Dialog`/`Drawer`/`Snackbar`, `Progress`, `Switch`, and `Input`.
- **Theme-private token names moved into the theme projects.** Names that only a specific theme consumes
  no longer live in the core, keeping the core registry limited to the shared, theme-agnostic surface.

### Fixed
- **The Liquid Glass iOS switch style now applies.** Its tokens were wired but never reached the rendered
  switch; reconciling the `Switch` token family connected them, so the Liquid Glass theme's switch renders
  as intended.

## [0.0.11] - 2026-07-06

The largest release since the token mandate landed. Flare's core is now fully theme-agnostic: the
Material Design 3 and FluentUI2 baselines were extracted into their own reference token packages, and the
core ships zero visual defaults - a theme must supply every value or the components render unstyled by
design. Alongside that, the Select family was rebuilt on a headless C# core, the field components
converged on one shared chrome, and a large CSS deduplication pass removed the last of the re-baked
Material literals.

### Added
- **Baseline token packages `Flare.Theme.MaterialDesign3.Tokens` and `Flare.Theme.FluentUI2.Tokens`.**
  The MD3 and Fluent baselines that used to live inside the core are now standalone reference packages. A
  theme derives from one of them (`<Ref>.Design with { ... }`) instead of inheriting a baked-in core
  baseline, so the two shipped themes are now genuinely independent rather than one being the default the
  other patches.
- **`FlareThemeBuilder` takes a base `DesignTokens`.** The builder now derives a theme from an explicit
  reference package's tokens; there is no longer an implicit MD3 baseline underneath every theme.
- **Headless Select core - `ComboboxState` / `ListCollection` / `SelectionManager`.** The selection,
  filtering and open/close logic now lives in plain C# with no DOM dependency, so it is unit-testable on
  its own and shared by every select-family shell.
- **`FlareSelect` / `FlareMultiSelect` uncontrolled use.** Both work without `@bind-Value` (they hold
  their own selection when no binding is supplied), and `FlareMultiSelect` now implements
  `IFlareMultiField` and participates in `EditContext` validation.

### Changed
- **The core is now fully theme-agnostic - roughly 28 component token records are `required`.** Spacing,
  Card, DataGrid, Switch, Progress, Input, Menu, Slider, Dialog, Button, Nav, Tabs, Alert, Badge, Chip,
  Radio, Fab, Checkbox, ToggleButton, Drawer, Snackbar, Tooltip, Popover and Avatar records, plus
  `CornerRadius` and the `ColorScheme` shadow set, no longer carry literal defaults. A theme must supply
  every value; a guard test fails the build on any re-introduced literal default. Components render
  unstyled without a theme by design - the shipped themes are unaffected.
- **The Select family is now thin shells over the headless core.** `FlareSelect` and `FlareMultiSelect`
  are UI-only wrappers around `ComboboxState` and friends. Search moved into the trigger field (you type
  where the value shows, not in a separate box) and the keyboard/ARIA contract was hardened.
- **The field family shares one `FlareFieldBase` and one visual chrome.** Text, Password, Numeric, Masked,
  TextArea, DatePicker, TimePicker, DateTimePicker, TagField, Autocomplete, Select and MultiSelect all
  render the same label/helper/error, input well, and size + disabled/error state classes.
  `FlareInputControl` was renamed `FlareTextInput`.
- **Large CSS deduplication.** Shared `flare-input` / `flare-picker` / `flare-listbox` families back all
  the field and picker components; tabs and linktabs share one pill track; the DataGrid bars, nav icons
  and modal scrim were consolidated. `FlareButtonGroup` is now theme-agnostic via `ButtonGroupTokens`, the
  FAB adopts the shared `flare-btn` chrome, and hover state layers follow `--flare-state-hover-opacity`
  instead of per-component opacities.
- **Concrete theme names purged from the core** and roughly 100 remaining component literals promoted to
  themeable tokens, so the values that were previously hardcoded can now be retargeted by a theme.
- **ASCII-only source mandate completed.** Cyrillic comments were translated to English and stray UTF-8
  BOMs were stripped across the codebase.

### Fixed
- **A batch of accessibility and cross-theme bugs.** The `FlareBottomNav` Fluent pill regression,
  disabled-item keyboard safety, dialog ARIA naming, the layout drawer's modal semantics, and the
  password-field eye icon were all corrected. `CssAudit` now also reports duplicate token constants.

## [0.0.10] - 2026-07-04

A quality and hardening release driven by a full component review against Flare's
theme-agnostic token mandate (Flare exposes tokens; themes own the values). It fixes a
batch of confirmed accessibility and cross-theme bugs and begins removing Material Design
opinion that had leaked into the core.

### Fixed
- **`FlareBottomNav` no longer renders a Material pill under FluentUI2.** The active-item
  indicator baked its own MD3 tokens with no theme override, so the bottom bar showed an
  MD3 pill even under Fluent (which flattens the shared nav indicator). Its indicator now
  defaults to the shared `--flare-nav-*` tokens, so a theme's nav override reaches it too.
- **Disabled items are keyboard-safe.** A disabled `FlareBottomNavItem` / `FlareLinkTab`
  kept a live `href`, stayed in the tab order and was activatable by keyboard. They now
  suppress the `href` and emit `aria-disabled="true"` + `tabindex="-1"`.
- **`FlareLinkTabs` is a navigation landmark, not a `role="tablist"`.** A tablist owning
  plain `<a>` links (no `role="tab"`, no keyboard contract) is invalid ARIA that a screen
  reader cannot map. It now renders a `<nav>` (with an optional `AriaLabel`) and relies on
  the anchors' existing `aria-current="page"`.
- **Header-less dialogs have an accessible name.** `FlareDialog` only rendered its title
  `<h2>` when `Title` was set but always pointed `aria-labelledby` at it, leaving a dangling
  reference (now a common case via the component-dialog API). `aria-labelledby` is emitted
  only when there is a title; a new `AriaLabel` parameter (surfaced through
  `DialogOptions.AriaLabel`) names header-less dialogs.
- **The temporary layout drawer is a proper modal.** An open `Temporary`/mobile-overlay
  `FlareLayoutDrawer` gained a focus trap, Escape-to-close, `role="dialog"` + `aria-modal`
  and body-scroll lock; a closed push drawer is now `inert` so its collapsed links leave the
  tab order instead of sitting focusable under `aria-hidden`.
- **`FlareCard` explicit `Elevation` no longer defeats the hover lift.** It set an inline
  `box-shadow` that outranked the `:hover` rule; the level is now applied through the
  `--flare-card-elevation` variable so a clickable card still lifts on hover.
- **`FlareAvatar` re-renders when only `FallbackContent` changes**, and
  **`FlarePasswordField` forwards `Class`/`Style`** to its inner field.

### Changed
- **The layout drawer no longer collides with the standalone `FlareDrawer` over
  `--flare-drawer-width`.** Its geometry tokens were renamed to `--flare-layout-drawer-width`
  / `--flare-layout-drawer-rail-width`.
- **`FlareColorPicker` chrome uses semantic color tokens** (`outline-variant` / `surface` /
  `outline`) instead of the mode-blind `#ccc` / `#fff` / `rgba(...)` literals, so it adapts
  to the active theme and light/dark mode.
- **Token records stop shipping hard theme literals where a scale exists:** Alert/Badge
  radius now reference the shape scale and the `Switch` thumb shadow references
  `--flare-elevation-1` (the same MD3 shadow, via the theme's elevation + shadow-color
  tokens). No visual change under the shipped themes.
- **~900 dead literal fallbacks were stripped from the component CSS** (e.g.
  `var(--flare-spacing-4, 0.5rem)` -> `var(--flare-spacing-4)`). Those scale tokens are
  always emitted, so the fallbacks were dead code that re-baked the Material values a second
  time; removing them has no visual effect.

## [0.0.9] - 2026-07-01

A bug-fix release. `FlarePasswordField` never propagated the typed value to a consumer's
`@bind-Value`, so it silently broke every login/registration form bound to it; this is fixed, and the
component now exposes the `FlareField` parameters most forms need.

### Fixed
- **`FlarePasswordField` two-way binding now works.** The inner field was bound with
  `@bind-Value="Value"`, which only assigned the component's local `Value` field and never invoked
  `FlarePasswordField`'s own `ValueChanged` - so a consumer's `<FlarePasswordField @bind-Value="model.Password" />`
  never received the typed value and `model.Password` stayed at its initial value. The inner change is
  now propagated to the component's `ValueChanged`, so `@bind-Value` behaves as expected.

### Added
- **`FlarePasswordField` typed pass-through parameters.** In addition to the existing ones, the
  component now surfaces `Immediate` (commit on every keystroke) and `DebounceInterval`, `Variant`
  (Filled/Outlined), `FullWidth`, `Margin`, and `For` (validation accessor), forwarded to the inner
  `FlareField` so a password field behaves like a text field. `Required` now emits the native
  `required` attribute on the input.
- Gallery: a **Live two-way binding** demo on the Password Field page that binds a `FlarePasswordField`
  to a field and echoes the bound value on every keystroke (the case that would have caught the bug).

## [0.0.8] - 2026-07-01

Migrating a real application (PlaylistShared) from MudBlazor to Flare surfaced a batch of small,
generally-useful API gaps in existing components. This release closes them. Every addition is purely
additive and backward-compatible.

### Added
- **`FlareIconButton`** - a dedicated icon-only button. A thin wrapper over `FlareButton` that renders
  an `Icon` (or custom `ChildContent`) as the button's leading icon with no label, so the square
  icon-only treatment applies automatically. Replaces the verbose
  `<FlareButton><LeadingIcon><FlareIcon/></LeadingIcon></FlareButton>` idiom. Defaults to the
  `Text` ("standard") variant and forwards `Variant`/`Size`/`Color`/`Shape`/`Disabled`/`Loading`/
  `Href`/`Target`/`AriaLabel`/`OnClick`.
- **`FlareCollapse`** - a standalone expand/collapse container for a single region (unlike
  `FlareAccordion`, which is a panel group). Driven by `@bind-Expanded`, or by an optional built-in
  toggle `Header` / `HeaderContent`. The region animates its height open/closed. New
  `flare-collapse*` classes and a Gallery page.
- **`FlareChip.Variant`** (new `ChipVariant`: `Outlined` (default) / `Filled` / `Elevated`). The
  existing `Elevated` boolean is now shorthand for `Variant="ChipVariant.Elevated"`. New
  `flare-chip--filled` / `flare-chip--outlined` classes.
- **`FlareAvatar.FallbackIcon`** (Material Symbols name, default `person`) and **`FallbackContent`**
  (`RenderFragment`) for the no-image/no-text case, replacing the previously hard-coded icon.
- **`FlareField.Error` / `FlareField.Invalid`** - force the invalid visual state (and `aria-invalid`)
  without requiring an `ErrorText` message. Inherited by `FlareTextField`.
- **`FlareField.FullWidth`** (default `true`; `false` sizes the field to its content) and
  **`FlareField.Margin`** (new `FieldMargin`: `None` / `Dense` / `Normal`), inherited by `FlareTextField`.
- **`FlareStack.StretchItems`** (every child shares the main axis equally) and **`StretchFirst`**
  (only the first child grows to fill the remaining space).
- **`FlareMenuItem.Target`** (for an external `Href`; `_blank` adds `rel="noopener noreferrer"`) and
  **`IconColor` / `LeadingIconColor`** to tint the leading icon.
- **`FlareToggleGroup.Size` / `Color` / `Disabled`** cascade to every child `FlareToggleButton`
  (set once on the group). `FlareToggleButton` gains a `Color` parameter that tints its selected state.
- **`FlareCard.Elevation`** (nullable `int`, 0-5 on the MD3 elevation scale, clamped) overrides the
  variant's shadow; `Elevation="0"` is flat.
- **`FlareSelect` declarative options** - populate a select with native
  `<option value="..">Label</option>` child markup as an alternative to the `Items` collection.
- **`ISnackbarService.Show(string, SnackbarOptions)`** - an options overload carrying per-message
  severity/timing/action plus a per-message `CssClass` and a `CloseAfterNavigation` flag (the snackbar
  is dismissed automatically on the next route change). New `SnackbarOptions` type; `SnackbarMessage`
  gains `CssClass` and `CloseAfterNavigation`.
- **`FlareLink.Typo`** - apply a `TypographyScale` to the link text (otherwise it inherits the
  surrounding typography).
- Gallery: new demos for each of the above (Chip variants, Avatar fallback, Card elevation, Stack
  stretch, Menu icon color & external links, Toggle group cascade, Field error state, Field width &
  margin, Link typography, Snackbar options, a "shown only in a band" `FlareHidden` example), a new
  Collapse page, and the Icon Button demos rebuilt on `FlareIconButton`.

### Notes
- `FlareHidden` already supported showing an element only within a breakpoint band via
  `Only` + `Invert`; this is now demonstrated on the Responsive page. `FlareSlider` already exposes a
  `Vertical` parameter, so no separate vertical-bar component was added.

## [0.0.7] - 2026-06-30

This release adds a **generic component-dialog service** - render any Blazor component as a modal and
await a typed result, instead of the inline `@bind-Visible` plumbing previously required - and makes
**`FlareStepper` able to drive wizards with bespoke navigation**: the active step can be controlled
and observed from outside the component, the built-in Back/Next buttons can be replaced wholesale with
custom controls, and individual steps can be marked skippable, so a wizard that previously had to be
hand-rolled in HTML (custom arrow buttons, wheel-scroll navigation) can be expressed with
`FlareStepper` directly. Existing steppers are unaffected.

### Added
- **`IDialogService.ShowAsync<TComponent>` / `Show<TComponent>`** - open any component as a modal
  dialog body and await its outcome. `ShowAsync` returns a `Task<DialogResult>`; `Show` returns a
  `DialogReference` whose `Result` can be awaited and which can also close the dialog from the caller
  side. The body component receives a cascaded `FlareDialogInstance` to close itself
  (`Dialog.Close(value)` / `Dialog.Cancel()`), and the dialog is rendered through the existing
  `FlareDialogProvider` (a `DynamicComponent` host) with the same visuals, sizing, scrim, focus-trap
  and Escape handling as `FlareDialog`.
- **`DialogParameters`** - a fluent bag (`Add(name, value)`) binding values to the body component's
  `[Parameter]`s; **`DialogResult`** (`Ok(payload)` / `Cancel()` with `Cancelled` and a typed
  `GetData<T>()`); and **`DialogOptions`** (`Size`, `CloseOnScrimClick`, `CloseOnEsc`, `Divider`).
- Gallery: a new **Component dialog** demo on the Dialog page (an edit-profile dialog that receives
  initial values and returns an edited model).
- **`FlareStepper.ActiveIndex` two-way binding** (`@bind-ActiveIndex`, backed by the new
  `ActiveIndex` parameter and `ActiveIndexChanged` callback). The component writes the new index out
  on every navigation and adopts an externally assigned value (e.g. from a consumer's own controls)
  on the next render, so the active step can be controlled and observed externally. An out-of-range
  value is clamped to the registered step count. (`ActiveIndex` was previously a read-only property;
  reading it still works.)
- **`FlareStepper.ActionContent`** (`RenderFragment<StepperContext>`) - optional navigation content
  rendered in place of the built-in Back/Next buttons, letting consumers render bespoke controls
  (custom icon buttons, wheel/keyboard navigation, ...). The new `StepperContext` exposes the active
  position (`ActiveIndex`, `Count`, `IsFirst`, `IsLast`, the current `Step`) plus the same navigation
  operations the built-in controls use (`NextAsync`, `BackAsync`, `GoToAsync`, `CanGoTo`), each of
  which still runs the `OnStepChanging` guard. When `ActionContent` is not supplied the built-in
  buttons render exactly as before.
- **`FlareStep.Skippable`** - allows forward navigation (a step-indicator click or `GoTo`) to jump
  past the step in a linear stepper without it being completed. In a linear stepper a forward jump is
  permitted only when every step skipped over is `Skippable`; the immediately next step is always
  reachable. No effect in a non-linear stepper, where every step is already reachable.
- Gallery: two new Stepper demos - **Custom navigation & wheel scroll** (arrow icon buttons plus
  mouse-wheel navigation via `ActionContent` and `@bind-ActiveIndex`) and **Bound index & skippable
  step** (external buttons driving the stepper through the binding, with a skippable optional step).

### Changed
- **`DialogSize` moved from the `Flare.Components` namespace to `Flare.Abstractions`** (it is now a
  shared dialog contract used by `DialogOptions`). Code that imports both namespaces (the usual setup)
  is unaffected; code that referenced `Flare.Components.DialogSize` by its full name should update the
  namespace. The existing `ConfirmAsync` / `AlertAsync` helpers are unchanged.

## [0.0.6] - 2026-06-30

This release reworks the **layout and navigation API** - a breaking change - and adds a large batch
of component features and accessibility improvements. The single-drawer `FlareLayout` is replaced by
a composition model where each `FlareLayoutDrawer` owns its own state, enabling multi-drawer
(two-pane) layouts.

### Added
- **`FlareLayoutDrawer` - a self-owned, composable layout drawer.** Each drawer owns its open state
  via `@bind-Open` and registers a grid track with the parent `FlareLayout`. New parameters: `Variant`
  (`Persistent` / `Mini` / `Temporary` / `Responsive` / `Permanent`), `Anchor` (`Left` / `Right`),
  `Width`, `RailWidth`, `HoverExpand`; plus `IsOpen`, `IsCollapsedRail`, `SetOpenAsync`, `ToggleAsync`.
  Compose two drawers for a two-pane (rail + section) layout. A collapsed `Mini` rail can hover- or
  focus-expand to full width as a floating overlay without reflowing the content. New
  `flare-layout-drawer--{persistent,mini,temporary,responsive,permanent,end,floating,hover-expand}`
  classes and a `--flare-layout-appbar-height` (64px) token.
- **`FlareNavMenu.Mode`** (new `NavMenuMode` enum: `Full`, `Rail`, `RailLabeled`). `RailLabeled`
  renders an MD3 navigation rail (icon with a stacked caption); `Mode` takes precedence over the
  `Rail` flag and the drawer-driven auto-rail. New `flare-nav-menu--rail-labeled` class.
- **`FlareDateRangePicker` inline calendar mode and date constraints.** A new `Mode`
  (`DateRangePickerMode.Fields`, default / `Calendar`): Calendar mode is a single inline range
  calendar - click the start day then the end day, with the days between highlighted and a live hover
  preview while choosing the end. New `Min` / `Max` / `IsDateDisabled` constraints apply in both modes.
  New `flare-daterangepicker__calendar` / `__day--start` / `__day--end` / `__day--in-range` classes.
- **`FlarePopover.Trigger`** (new `PopoverTrigger` enum: `Manual`, default / `Click`). `Click` toggles
  the popover from its anchor with no extra wiring. While open, the popover now traps `Tab` focus
  inside the panel and restores focus to the trigger on close.
- **`FlareStepper.OnStepChanging`** - an async navigation guard (`Func<StepperChange, Task<bool>>`)
  run before the active step changes on any Next/Back/click; return `false` to veto the move (e.g.
  per-step validation). The new `StepperChange` readonly record struct carries the `From`/`To` indices,
  so a handler can allow backward moves while validating forward ones.
- **`FlareAccordionPanel.OnBeforeToggle`** - an async guard (`Func<bool, Task<bool>>`) run with the
  proposed expanded state before a panel toggles; return `false` to block it (e.g. confirm before
  collapsing a panel with unsaved edits).
- **Relevance-ranked filtering on `FlareAutocomplete` and `FlareMultiSelect`** - new `Fuzzy` and
  `RankFunc` parameters. `Fuzzy=true` ranks matches best-first via the new scorer (so typing "lo"
  surfaces "London" above "Los Angeles"); `RankFunc((item, query) => score)` supplies a custom scorer
  (positive scores only, best-first). Both apply only when a query is present (and are ignored when a
  `SearchFunc` owns the ordering).
- **`FlareSearch`** - a new public static relevance-scoring utility. `Score(text, query)` returns a
  banded `0..1000` score (exact > prefix > word-start > substring > subsequence), and
  `Rank(items, score)` keeps positive scores ordered best-first - usable to build custom `RankFunc`
  delegates.
- **`FlareFormBuilder` two-way model binding** - a new `ModelChanged` callback enables `@bind-Model`,
  so resetting the form (which swaps in a fresh model instance) updates a bound parent field instead
  of being overwritten by a stale reference on the next render.
- **WCAG contrast tooling.** `FlareColorCustomizer` shows a live contrast preview for the selected
  primary color (an "Aa" sample of the auto on-color, the numeric ratio, and an AA/AAA/AA-Large/Fail
  badge), gated by the new `ShowContrast` parameter (default `true`). New
  `Flare.Theming.ColorMath.WcagLuminance(hex)` and `ColorMath.ContrastRatio(a, b)` helpers.
- **Gallery:** a Settings -> Navigation tab to choose a labeled vs icon-only collapsed rail (persisted
  via a new `RailLabelService`, restored before first paint); new demos for Autocomplete fuzzy ranking,
  the DateRangePicker inline calendar and built-in-plus-custom presets, the Stepper async guard, and
  the `FlareColorCustomizer` on the Color page. All new strings localized (EN + RU).

### Changed
- **Layout / navigation API redesigned (breaking).** `FlareLayout` is now a composition-only shell:
  place a `FlareLayoutAppBar`, one or more `FlareLayoutDrawer`, and a `FlareLayoutContent` as
  `ChildContent` instead of the old `<AppBar>` / `<Drawer>` / `<Content>` slots, and the layout no
  longer owns drawer state. `FlareLayoutContext` (now `sealed`) is rewritten from a single-drawer
  holder into a multi-drawer registry/coordinator (`Register`/`Unregister`, `GridTemplateColumns`,
  `PrimaryDrawer`, `TogglePrimaryAsync`, `AnyOverlayOpen`, `CloseOverlaysAsync`); the shell is one CSS
  grid driven by a published `--flare-layout-cols` variable. `FlareLayoutAppBar.DrawerToggle` now
  toggles the layout's primary drawer (the first non-temporary start drawer). See **Removed** for the
  dropped members; the Gallery and both samples were migrated to the new API.
- **`FlareMenu` keyboard focus and screen-reader support.** An open menu now focuses its panel (so the
  arrow / Home / End / Tab handler actually receives keys), starts the active item on the first enabled
  item, and exposes it via `aria-activedescendant` (each item now has a stable id). Previously the
  panel was never focused, so keyboard navigation did not start at all.
- **`FlareDateRangePicker.DefaultPresets`** is now a public static property (was a private field), so
  callers can keep the built-in quick-ranges and append their own:
  `Presets="[.. FlareDateRangePicker.DefaultPresets, .. myPresets]"`. It is rebuilt on each access so
  the localized labels follow the current culture.
- **Performance.** `IThemeService.Themes` / `Palettes` now return cached read-only snapshots (rebuilt
  only on registration or a dynamic-palette change) instead of allocating a fresh list per read, and
  `FlareComponentBase.BuildCssClass` has a fast path that returns the root class directly when there
  are no modifier classes and no user `Class` - cutting per-render allocations.

### Fixed
- **`FlareAccordion` two-way binding stays in sync** when a sibling auto-collapses in single-expand
  mode: the collapsed panel now raises `ExpandedChanged(false)` (and auto-collapse is skipped when it
  is already collapsed), so a parent bound via `@bind-Expanded` no longer desyncs.
- **`FlareListItem` clickable items are keyboard-operable** - a clickable item (`role="button"`,
  `tabindex="0"`) now activates on Enter and Space, satisfying WCAG 2.1.1.
- **`FlareDatePicker` arrow-key navigation skips disabled dates** (`Min` / `Max` / `IsDateDisabled`)
  instead of landing on them, and stops rather than looping when an entire range is disabled.
- **`FlareDateRangePicker` (Fields mode) can no longer produce a start later than the end** - the inner
  pickers' bounds are clamped to the linked value.
- **`FlareCarousel` autoplay honors a changed `AutoPlayIntervalMs`** at runtime (the timer is recreated
  when autoplay turns on or the interval changes).
- **`FlareDataTree` caches lazily loaded children** across collapse/expand, avoiding a redundant
  `ChildrenProvider` call on every re-expand.
- **`FlareAutocomplete` returns focus to the input** after the clear button is used.

### Security
- **`FlareRichTextEditor` restricts inserted link URLs to a safe scheme allowlist** (relative,
  fragment, `http(s)`, `mailto`, `tel`), blocking `javascript:` / `data:` / `vbscript:` links that
  would otherwise be stored XSS in the edited HTML; an unsafe or empty URL leaves the link input open
  for correction instead of inserting.
- **`FlareImage` sanitizes its composed inline style** (`AspectRatio` / `BorderRadius` / `Style`)
  through `CssValidator.StripDangerous`, consistent with the other style-injecting components.

### Removed
- **Breaking - `FlareLayout` slot and single-drawer API.** Removed the `AppBar`, `Drawer`, `Content`,
  `ContentClass`, `ContentStyle`, `ContentMaxWidth` and `ContentAlignment` slot parameters,
  `DrawerOpen` / `DrawerOpenChanged`, and `MiniRail`; and `FlareLayoutContext.DrawerOpen` / `MiniRail`
  / `RailCollapsed` / `ToggleDrawer`. Migrate to the composition API (`FlareLayoutDrawer` with
  `@bind-Open` + `Variant`).
- **Breaking - old layout CSS classes.** Removed `flare-layout--drawer-open`, `flare-layout__body`,
  `flare-layout__main`, `flare-layout--mini-rail` and the matching `Css.Classes.Layout.DrawerOpen` /
  `Body` / `Main` / `MiniRail` constants; the shell now uses `flare-layout--mobile` / `--scrim-open`
  and the `flare-layout-drawer--*` variant classes.

## [0.0.5] - 2026-06-28

### Added
- **`FlareNavMenu` framed layout** with new `Header` / `Footer` slots. Setting either slot pins that
  region while `ChildContent` scrolls between them, filling the menu's container height; the pinned
  regions hold ordinary nav items so they still collapse to icons in a mini-rail. New
  `flare-nav-menu--framed` / `__header` / `__scroll` / `__footer` / `__meta` CSS classes.
- **`IVersionCheckService.CurrentVersion`** - the version the app is currently running (in
  service-worker mode the first version read from the deployed assets manifest, otherwise the
  configured `CurrentVersion`), distinct from `LatestVersion`, which is only set once a newer build
  is detected.
- **Snackbar update-in-place**: a new `ISnackbarService.Show(SnackbarMessage)` overload that
  preserves the message `Id`, an `Update(SnackbarMessage)` method and `OnUpdate` event that replace
  a shown snackbar in place (keeping its position in the stack), and a `SnackbarMessage.ShowProgress`
  flag that renders an indeterminate progress bar below the message - e.g. morphing a "new version
  available" toast into an "updating..." one. New `flare-snackbar--with-progress` / `__progress`
  classes.

### Changed
- **Ribbon command heights aligned** (`Flare.Components.IDE`): icon-only, icon + label and dropdown
  commands now all stretch to the group height instead of floating at their intrinsic sizes; large
  commands stack the icon over the label via `.flare-btn__label`, and the dropdown caret no longer
  inherits the large icon size.
- **Gallery: IDE components split into their own pages** (Backstage, DocumentTabs, FormulaBar,
  MenuBar, PropertyGrid, QuickAccessToolbar, Ribbon, SheetTabs, StatusBar, ToolPanel, Toolbar) with
  focused per-component demos, replacing the single combined IDE page.
- **Gallery: new Settings page** consolidating the design-system / palette / mode theme switcher and
  the language toggle, reachable from the nav menu, which now also surfaces the running Gallery
  version.

### Fixed
- **A PWA on-demand update no longer lands back on the old version.** The service worker now calls
  `clients.claim()` on activate, and `flare-version-check.js` drives the pending worker through
  `skipWaiting` and reloads on the single resulting `controllerchange` instead of a fixed 10s timer -
  so the reload is always served by the new worker and never falls back onto the stale cache (the
  cause of the "dev"/previous version that previously needed a hard reload). The samples'
  `service-worker.published.js` adds the matching `clients.claim()`.
- **No stray focus ring on the page heading after navigation.** Blazor's `FocusOnNavigate` focuses
  the page heading (`tabindex="-1"`) after every navigation, which Chrome's `:focus-visible`
  heuristic painted a ring on even though it was not a keyboard interaction; the ring is now
  suppressed on focused `h1`-`h6[tabindex="-1"]` headings, while real interactive controls keep
  theirs.

## [0.0.4] - 2026-06-27

### Changed
- **Anti-FOUC splash is now revealed automatically by `FlareThemeProvider`** (new `ManageSplash`
  parameter, default `true`). The provider waits for the theme stylesheets (`load` event) and the
  document's web fonts (`document.fonts.ready`), then fades the bootstrap splash out after the first
  themed frame - so apps no longer flash unstyled content and no longer need to call
  `window.hideFlareSplash()` by hand. `IThemeJsService.EnsureStylesheetAsync` now resolves only once
  the stylesheet has loaded; new `WhenFontsReadyAsync` / `RevealAppAsync`. A safety timeout in
  `flare-bootstrap.js` (overridable via `data-splash-timeout`) reveals the page even without the
  provider.
- The Gallery and Legacy samples drop their hand-written `hideFlareSplash()` wiring; the splash is now
  revealed by `FlareThemeProvider` out of the box.

### Fixed
- **A PWA update no longer crashes the render when a stale service-worker cache serves a previous
  `flare-theme.js`.** Because `_content/.../js` modules load at a fingerprint-free URL (unlike the
  fingerprinted framework files), an updated build's C# could call `whenFontsReady`/`revealApp` on an
  older cached module and throw a `JSException` in `OnAfterRenderAsync`. `FlareThemeProvider` now
  catches it and falls back to the global `window.hideFlareSplash()`, so the app is never crashed or
  stranded during an update; the full fonts-ready + framed-fade path resumes once the new worker
  activates.

## [0.0.3] - 2026-06-27

### Added
- **Two new design systems**: **Material Design 3** (baseline, non-Expressive) and **Material Design 2**.
  Flare now ships 7 themes (MD3 Expressive, MD3, MD2, Fluent UI 2, Aero, Liquid Glass, Visual Studio 2026).
- **Dynamic Color palette** (`Palette.DynamicId`): an opt-in palette generated at runtime from the
  OS/browser accent color (Windows/macOS accent, Android Material You via the CSS `AccentColor`
  system color) through the *active theme's* generator, so it adapts to every theme. Enabled via
  `FlareOptions.UseDynamicPalette` / `DynamicPaletteFallbackSeed`; falls back to a seed where the
  accent is unavailable. New `IThemeService.ApplyDynamicPaletteAsync` / `IsDynamicPalette` and
  `IThemeJsService.GetAccentColorAsync`.

### Changed
- **Token model cleanup**: common component tokens that themes set through the `Extended` bag now
  have typed homes - new `NavTokens`, plus added `InputTokens` hover, `MenuTokens` group/island and
  `ProgressTokens` track/stop/wave fields. All themes set these via typed `DesignTokens` records;
  `Extended` is reserved for genuinely theme-specific keys.
- **Fluent UI 2** typography and motion aligned to the official Fluent 2 design tokens (Semibold
  heading ramp, real `fontSize`/`lineHeight`/duration values; emphasized easing = `curveEasyEase`).
- Solution folders split into `src/Core` and `src/Themes`.

### Fixed
- **MD3 Expressive** palette aligned to the updated spec: light-mode `on-*-container` roles now use
  tone 30 (were tone 10), in both the static palette and the tonal generator.
- **Visual Studio 2026** connected document tabs - the active tab now takes the editor surface color
  (was a lighter floating gray in dark mode).
- Gallery home "design systems" stat is now bound to the registered theme count (was hardcoded).

## [0.0.2] - 2026-06-27

### Architecture
- Rebuilt as a clean onion / ports-and-adapters stack with 5 rings - `Flare.Abstractions`
  (contracts + design-token model + CSS registry), `Flare.Theming` (engine), `Flare.Infrastructure`
  (JS-interop/storage/feedback adapters), `Flare.Components` (UI only) and `Flare.Blazor` (composition
  root). Dependencies point strictly inward; `Flare.Components` no longer ships service implementations,
  and the old `Flare.Core` grab-bag was retired. Namespaces realigned to the rings.

### Added
- **Multi-targeting**: the libraries are .NET 10-first but now also target **net8.0** and **net9.0**
  (per-TFM ASP.NET Core versions; no net10 regression - identical code).
- **`ITheme.Derive(...)`** to tweak a built-in theme by composition instead of subclassing.
- **Id constants** on every theme (`<Theme>.ThemeId`) and palette set (`<Palettes>.<Name>Id`) for
  string-free theme/palette switching.
- **`[CssVar]`** attribute linking every token value to its `--flare-*` name (guarded by a drift test),
  and typed `Vars.Var(Css.Tokens.*)` token values instead of magic `var(--flare-*)` strings.

### Changed
- Compound components (Tree, Menu/SubMenu, FAB) standardised on a single typed cascading context.

### Fixed
- `FlareColorModeToggle` instances now stay in sync: the toggle reflects the live theme mode, so
  switching the mode anywhere updates every toggle.

## [0.0.1] - 2026-06-23

Initial public release of Flare - a production-ready Blazor component library for .NET 10.

### Components
- 100+ components across inputs, layout, navigation, data display, feedback, display and utilities,
  all inheriting `FlareComponentBase` with unified `Class`/`Style`/attribute forwarding.
- Inputs: Button, Input, TextArea, Checkbox, Switch, Radio/RadioGroup, Select, MultiSelect,
  Autocomplete, NumericField, Slider (single + range, sizes XS-XL, vertical, steps, value bubble),
  Rating, ColorPicker, TagInput, InputMask, PasswordInput, DatePicker, TimePicker, DateRangePicker,
  ToggleButton, ButtonGroup, SplitButton, FileUpload, FormBuilder/Form with `DataAnnotationsValidator`.
- Select & MultiSelect share a fully themed popover dropdown (rounded surface, elevation, grouping,
  keyboard navigation) via the shared `flare-listbox` styles, with an `ItemTemplate` for custom
  option markup.
- Layout: Stack, Grid/Col, Container, Hidden, Card, Paper, Divider, and the Layout/AppBar/Drawer set.
- Navigation: AppBar, NavMenu/NavGroup/NavLink (nested, auto-expanding), Tabs, Accordion, Breadcrumb,
  Pagination, Stepper, Drawer.
- Data display: DataGrid, Table, VirtualList, InfiniteScroll, TreeView, VirtualTree, List, Timeline,
  Chart (SVG line/bar/pie/donut, no dependency), Calendar, Carousel, Kanban, Transfer.
- Feedback: Dialog/DialogProvider, MessageBoxProvider, Alert, Snackbar, Progress, Skeleton, Tooltip,
  Overlay, EmptyState.
- Display: Typography (FlareText), Avatar/AvatarGroup, Badge, Chip/ChipGroup, Icon, Image, Link,
  Popover, Highlighter, ScrollTop.
- Utilities: Menu/MenuItem, SpeedDial, DropZone.
- Full XML documentation on every public type and `[Parameter]` for IntelliSense.

### Theming
- Runtime theme switching across three independent axes - design system, color palette, and
  light/dark/auto mode - with no page reload and no flash of unstyled content.
- Five design systems shipped as independent packages: Material Design 3 Expressive, Fluent UI 2,
  Aero, Liquid Glass, and Visual Studio 2026. The umbrella `Flare` package ships no theme of its own.
- 28 built-in palettes across the themes; each palette carries light + dark (and optional
  high-contrast) color schemes.
- Class-toggle delivery (default) swaps theme classes on `<html>`; CSS-variable injection available
  as a fallback (`ThemeDelivery`).
- Auto dark mode tracks `prefers-color-scheme`; a one-line bootstrap script applies the saved
  selection before first paint.
- Palette generation from a seed color (`Palette.FromColors`, `IThemeService.GeneratePalette`) using
  each design system's color rules (MD3 tonal / Fluent ramp).
- Runtime customization: `CustomizeColors`, `CustomizeDesign`, and per-token overrides; RTL and
  high-contrast support via CSS logical properties. `FlareThemeBuilder` and JSON theme serialization.

### Color API
- Unified `FlareColor` color parameter on every color-aware component: a semantic role
  (`FlareColor.Primary`) maps to a cached theme class, or any custom value
  (`FlareColor.Custom("#E91E63")`) emits sanitized inline tokens. Implicit conversion from `string`.

### DataGrid
- State-driven architecture (`DataGridState<T>` + `DataGridCommands` + single-pass pipeline + cache).
- Multi-sort, column resize/reorder, row reorder, row selection, type-aware inline editing, batch
  editing, and Excel-like cell selection (keyboard active cell, range selection, Ctrl+C/Ctrl+V).
- Type-aware columns (`ColumnDataType`, `Format`, alignment), per-column sort/filter strategies, and
  stable column identity used consistently for sort/filter/visibility/order/persistence.
- Filtering: auto filter row and Excel-style header filter menu (searchable distinct-value list),
  standalone AND/OR filter builder, named filter presets, debounced quick filter, and value-aware
  comparison for numbers/dates/times.
- Grouping with footer aggregates, column bands (nested), composite columns (banded / card),
  tree-grid, conditional row/cell formatting, virtualization, interactive column visibility.
- `IQueryable`/EF Core server-side translation (`DataGridQuery`) and exporters for
  CSV/TSV/JSON/Markdown/Excel (.xlsx)/PDF (dependency-free), with a pluggable `IDataGridExporter`.
- Keyboard navigation, ARIA grid roles, and localized (EN/RU) screen-reader announcements.

### Packages
- Core: `Flare.Core`, `Flare.Components`, `Flare` (umbrella, `AddFlare`/`AddFlareTheme`/`AddFlarePalette`).
- Themes: `Flare.Theme.MaterialDesign3Expressive`, `Flare.Theme.FluentUI2`, `Flare.Theme.Aero`,
  `Flare.Theme.LiquidGlass`, `Flare.Theme.VisualStudio`.
- Optional component families: `Flare.Components.Carousel`, `.Kanban`, `.Transfer`, `.QrCode`,
  `.RichTextEditor`, `.Media` (SignaturePad, VideoPlayer, FileUpload), `.IDE` (Ribbon, DocumentTabs,
  ToolPanel, Splitter, StatusBar, MenuBar).
- `Flare.Icons` (~10,700 Material Symbols paths, recommended to lazy-load in WASM apps).

### Services
- `IVersionCheckService` (`AddFlareVersionCheck`) - a headless service that polls for a newer app
  version on a configurable interval and raises `NewVersionAvailable`; the consumer decides how to
  surface it (e.g. a toast). Renders nothing itself. Built-in `UseServiceWorker` mode registers the
  service worker (`ServiceWorkerPath`), reads the deployed version from the Blazor PWA assets
  manifest, and applies a waiting update via `ApplyUpdateAsync` - so apps need no service-worker
  registration/update JS of their own. Or supply your own `CheckForLatestVersion` probe (e.g. a
  `version.json` poll) for the non-PWA case.

### Tooling & platform
- Targets .NET 10. Blazor Server, WebAssembly and SSR compatible; JS interop is prerender-guarded.
- No Bootstrap or third-party CSS - all styles use `var(--flare-*)` tokens.
- Interactive Gallery PWA with EN/RU switching, syntax-highlighted examples, and a live theme
  switcher; Docker-ready (`docker compose up --build`).
- NuGet packaging with SourceLink, symbol packages, and MinVer-derived versioning.
