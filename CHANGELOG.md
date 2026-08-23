# Changelog

All notable changes to Flare are documented here. This project adheres to
[Semantic Versioning](https://semver.org/).

## [0.18.1] - 2026-08-23

### Fixed
- **136 components in the API reference described themselves as "Base class for all Flare components".**
  `FlareButton`'s page said it, and so did 135 others. A component written as a `.razor` file has no
  type-level XML doc unless someone writes a code-behind partial to carry one - 8 of 183 did - and the
  doc reader, finding nothing, walked up the inheritance chain and used `FlareComponentBase`'s summary.
  Inheriting a doc is right for a MEMBER, where the base's text describes the very thing being shown;
  a type is not its base class, and the result was a reference that introduced almost every page with a
  sentence about something else. Type docs no longer fall back to a base unless an explicit
  `<inheritdoc/>` asks for it, so a component with nothing of its own now shows nothing - honest, and
  the only signal that it still owes a summary. Twenty components got theirs written in this release;
  the rest are tracked.
- **Twenty public components were missing from the API reference.** `Flare.ApiDocGen` decided what to
  document by walking for `FlareComponentBase` in the inheritance chain, so every component that sits
  directly on `ComponentBase` - because it needs none of the theme cascade - was invisible to it. That
  was not a short list of oddities: `FlareRadio`, `FlareColumn` and its band/row siblings, `FlareStep`,
  `FlareZone`, `FlareMeterSegment`, `FlareMonthGrid`, `FlareShortcuts`, the three DataGrid parts, and
  every provider and root component - `FlareThemeProvider`, `FlareThemeScope`,
  `FlareMessageBoxProvider`, `FlareConfirmDialogProvider` - including ones the setup guide tells people
  to place by hand. Discovery now follows the library's own naming convention instead: a public
  `ComponentBase` subclass whose name is `Flare`-prefixed, which keeps the internal composition helpers
  next to them (`DataGridExport`, `QueryConditionEditor`) out on the same rule that keeps them out of
  user code. The reference goes from 161 to 181 components, and `FlareThemeProvider.IconMorph` - the
  switch that turns 0.18.0's icon transitions on library-wide - is documented where a reader looks for
  it.

## [0.18.0] - 2026-08-22

### Added
- **Icons can transition when they change.** `FlareIconView` gains `Morph`
  (`Fade`/`Scale`/`Rotate`): the outgoing and incoming glyphs share one grid cell and trade places
  instead of the glyph being repainted on a single frame. It works for any pair of icons from any
  provider, and it is pure CSS - a keyed slot per glyph is what makes the browser INSERT the incoming
  node rather than patch the path data into the existing one, and an animation fires on insertion
  without anything imperative. There is no teardown step at all: `animation-fill-mode: both` parks the
  outgoing glyph at zero opacity and the next swap recycles it, so the component holds no timer, no
  animation-end callback and no JS, and the slot count is bounded at two. Off unless asked for - with
  no mode in effect the view renders exactly what it always did, with no wrapper and no change
  tracking.
- **One switch turns icon transitions on library-wide.** `FlareThemeProvider.IconMorph` (or a plain
  `CascadingValue<FlareIconMorph?>` for one region) sets the default for every `FlareIconView` beneath
  it that has no `Morph` of its own - Flare's own chrome included, from an expander's chevron to a
  select's caret, since all 95 icon call sites in the library already route through the view. An
  explicit `Morph` still wins, `None` included, so a single icon can opt out of a scope that is on.
  This is deliberately an app decision rather than a theme one: the theme owns how a swap MOVES,
  while whether icons transition at all is a statement about the app's character.
- **`FlareMorphIcon`: the outline itself interpolates.** A second, narrower kind of morph - one
  `<path>` element stays in the document while its geometry flows, which is the effect people usually
  picture. It uses the CSS `d` property, so it costs no JavaScript, and the geometry is emitted as the
  `d` attribute as well, so a browser without that property still draws the icon and simply lands the
  change in one frame. It only works between outlines drawn against each other - path interpolation
  requires the same command list on both sides - so `FlareMorphIcons` ships pairs authored that way
  (`Plus`/`Minus`, `ChevronDown`/`ChevronUp`), padded with degenerate segments, and a guard test
  compares their command lists because a mismatched pair does not fail loudly, it stutters.
  `FlareIconView` recognises the type and stands its cross-fade down even when a mode is on:
  cross-fading would replace the very element whose geometry is being interpolated.
- **Four theme tokens for the motion.** `IconTokens` adds `--flare-icon-morph-duration`,
  `--flare-icon-morph-easing`, `--flare-icon-morph-scale` and `--flare-icon-morph-rotate`. The
  easing times the MOVEMENT only - the cross-fade underneath rides the theme's standard easing,
  because a spring overshoots and an opacity driven past its endpoint finishes about a third of the
  way in, which turns the hand-off between two glyphs into a pop. A theme parks the duration to keep
  icon swaps instant everywhere, or parks the two geometry tokens to make `Scale` and `Rotate` plain
  cross-fades.

### Changed
- **BREAKING for custom themes: `DesignTokens` gains `Icon`.** `IconTokens` is `required` like every
  other component record, so a custom theme will not compile until it answers the four tokens above.
  Material Design 3 rides its fast spring; Fluent UI 2 takes a short decelerated fade with both
  geometry axes parked, because that language has no icon overshoot in it. The themes derived from
  Material inherit its values, and because those values reference the motion scale rather than
  literals, each one resolves through its own springs.
- **`FlareIconView.Morph` is `FlareIconMorph?`, not `FlareIconMorph`.** Unset now means "inherit the
  scope" rather than "none". Call sites that pass a value are unaffected.

### Fixed
- **Three chips in the Gallery rendered with no icon at all.** `ChipGroupIconDemo` still used the
  pre-migration idiom `<FlareIconView>check_circle</FlareIconView>`. `FlareIconView` takes a typed
  `FlareIcon` and has no `ChildContent`, and the Razor compiler drops content passed to a component
  that cannot accept it **without a diagnostic** - so the build stayed green while the icons silently
  did not exist. Swept the rest of the Gallery for the same shape; this was the only one.
- **The API reference had been missing `FlareQueryBuilder` and `FlareQueryEditor`.** `Flare.ApiDocGen`
  probes its own output directory, and its project did not reference `Flare.Components.Query` - the
  exact failure the comment above that reference list warns about. The two components are documented
  again, along with two `FlareTagField` parameters whose docs had drifted.

### Toolchain
- **The test run could report green from a stale binary.** xunit.v3 4.0.0 builds test projects as
  Microsoft.Testing.Platform applications, and the VSTest bridge `dotnet test` used to rely on is gone
  on the .NET 10 SDK, so the runner is selected in `global.json` now. MTP also takes `--report-trx`
  rather than VSTest's `--logger`, which it ignores silently.
- **The SDK floor is pinned at 10.0.400** (`global.json`, `rollForward: latestFeature`). The Gallery's
  source generator references `Microsoft.CodeAnalysis.CSharp`, and a generator may not reference a
  Roslyn newer than the compiler loading it: the SDK drops the analyzer with CS9057 - a warning - and
  the build then fails far away with CS0103 on the types that quietly stopped being generated. CI
  floats to the newest SDK and so could never reproduce it.
- Every build warning is cleared, and the build is warning-free across all target frameworks.

## [0.17.0] - 2026-08-16

### Changed
- **BREAKING for custom themes: the selected state is a layer, like every other state.** `StateTokens`
  gains `SelectedLayer` and `SelectedHoverLayer`, `DesignTokens` gains `Touch`, and
  `DataGridTokens.RowSelectedHoverPct` is replaced by `RangeLayer` - all `required`, so a custom theme
  will not compile until it answers them. Selection was the last interaction state the core still
  computed on every theme's behalf, mixing the primary colour at the selected opacity in four
  stylesheets. Now the core says where the paint goes and the theme says what it is, and the pairing a
  selected row makes with hover gets its own token for the same reason focus-while-hovered did: two
  translucent washes stacked is not what either language means by "the selected row you are pointing
  at". Under the in-box themes nothing moves - the DataGrid's selected row was measured before and
  after and composites to the same colour, because mixing into `transparent` over a surface backdrop
  and mixing into `surface` are the same arithmetic. It now also paints correctly over a stripe or a
  group tint, which the old "mix into surface" could only guess at.
- **Three states that were reading the wrong axis.** The tab's close button and the breadcrumb's
  expander painted their HOVER from the selected opacity - darkening by the amount the design language
  reserves for "chosen" - and the listbox's keyboard-highlighted option painted an accent wash from it,
  above the layer rather than through it. They read the hover and focus layers now. Under MD3 the
  close button and expander lighten from 12% to 8%, and the highlighted option changes from primary at
  12% to the content colour at 10%: the highlight is roving focus, which is neither an accent nor a
  selection.
- **The vertical tab's active wash is the theme's, not the core's.** It was a literal 8% of primary; it
  is the selected layer now, which under MD3 reads 12%. A theme could not previously retune it at all.
- **Aero and Liquid Glass stopped switching the core's state layer off.** Both suppressed it with
  `opacity: 0 !important` on the button's `::before` - the theme fighting the core - where the honest
  form is to name the paint: the layer tokens are `transparent` there now. The old declaration also
  outranked the DISABLED layer, so it had been deciding that too; both themes park `DisabledLayer` at
  `transparent`, so nothing moves on screen.

### Added
- **`TouchTokens.TargetMin` - the minimum a control presents to a finger.** Read only inside the core's
  `@media (pointer: coarse)` rules, where the square icon-sized controls take it as a minimum size.
  Measured at 375px beforehand: 36 of 36 interactive controls on the DataGrid page were shorter than
  44px. The CONTROL grows rather than a transparent hit area drawn over it - the usual advice, and
  wrong here, because each of these sits in a row of its own kind and an expanded target overlaps its
  neighbours, with the later sibling silently winning the tap. The reflow is confined to devices whose
  primary pointer is coarse; a laptop with a touchscreen reports `fine`.
- **Every theme package documents its design system's component mapping.** Each
  `Flare.Theme.*/README.md` now carries a table taking that design system's own vocabulary to the Flare
  component and the parameter that selects it - Material's "common buttons" to
  `FlareButton Variant=...`, Fluent's Dropdown-vs-Combobox split to `FlareSelect` and `FlareCombobox`,
  Aero's Win32 common controls, Visual Studio's shell parts to the IDE family. `Flare.Theme.MaterialDesign3`
  and `Flare.Theme.MaterialDesign2` had no README at all and now have one, so every theme package
  arrives on nuget.org describing itself. Each also states what the theme changes beyond colour, which
  is the part a token list cannot show.
- **The gallery serves those mappings at `/themes/{id}`.** The README file itself is embedded rather
  than copied, so the page and the package description are the same text and cannot drift; the page
  strips the install block, renders the rest with `FlareMarkdown`, and offers to switch to the theme
  you are reading about. The nav lists one entry per registered theme that ships a README, so adding a
  theme package adds its page without editing a list. Each README has a Russian translation beside it
  (`README.ru.md`), served by UI culture with a fall back to English, the same way the changelog works.
- **A guard against a core fallback that decides how a component looks**, closing the last open item of
  the core/theme decoupling work. `CoreCssFallbackTests` reads every `var(--flare-x, <fallback>)` in core
  CSS and requires the fallback to be an identity value. The plan had assumed this needed an allowlist of
  the 29 legitimate structural reads; measured, it does not - a per-instance var falls back to `1`,
  `auto`, `0deg` or a chain of other vars, and only a fallback that names a COLOUR is a design decision.
  One rule, no list, and a new structural fallback passes on its own merits.
  - It found two offenders immediately: **the switch's focus halo was core opinion no theme could
    reach.** `--flare-switch-focus-shadow-off`/`-on` were registered nowhere and set by nobody, so the
    halo's size and colour were baked into `switch.css`. They were invisible to CssAudit because
    `--flare-switch-focus-shadow` IS a const and the audit counts a token as declared when any const is a
    segment prefix of it. Both are now `required` members of `SwitchTokens`, set by both theme bases.
- **A guard against suppressing a state layer.** `StateLayerModelTests` already refused
  `opacity: 1 !important` in a theme; it now refuses `opacity: 0 !important` too, and all three checks
  in that file strip CSS comments before scanning. The first run reported three offenders, every one of
  them a paragraph explaining why the old form was wrong - a guard that forbids documenting the
  anti-pattern is not one worth having.

### Fixed
- **A tag chip forced white label text.** `FlareTagField` wrote a caller's `ChipColor` straight onto
  `background-color` and pinned the label to `#fff` - the last hardcoded colour in any component's
  markup, and the core deciding a foreground it cannot know is legible: a pale tag rendered white on
  white. The colour now goes through the chip's own custom-colour contract (`--fc-main` fills,
  `--fc-on` labels), so an unset label colour falls back to the theme's on-colour, and the new
  `ChipTextColor` callback lets the caller - the only one who knows what colour it picked - say
  otherwise.
- **The DataGrid clipped its own columns on a phone.** At 375px the grid measured 600px inside a 343px
  container with `overflow-x: visible` all the way up, so the columns past the viewport were not off to
  the side - they were cut off and unreachable by touch. The small-screen rule put its minimum width on
  `.flare-datagrid`, the outer flex column, instead of on the table that scrolls inside
  `.flare-datagrid__wrapper`: the scroller grew with the component and had nothing left to scroll. The
  minimum is `max-content` on the table now, so a narrow grid still fits without a scrollbar and a wide
  one scrolls by exactly what it needs. Measured after: 293px visible, 394px scrollable.
- **The gallery's section drawer could not be closed on a phone.** Below the Md bound every drawer
  floats over the content, and the section column was given `Open` one-way with no `OpenChanged` - so
  the scrim tap, Escape, and the layout's own "close floating drawers on navigation" were all raised
  into nothing, and the next render put it straight back. It covered a 375px viewport with no way out.

## [0.16.1] - 2026-08-15

### Fixed
- **Every relative link rendered as `about:blank`.** The href guard allow-listed URL *shapes* - a leading
  `/` or `#`, or an `http`/`https`/`mailto`/`tel` scheme - so an internal link written the way the Blazor
  project template writes one (`href="counter"`, `href=""` for home) failed the check and was swapped for
  `about:blank`. It went unnoticed while every link in the gallery began with `/`, and a leading slash is
  exactly what an app cannot use once it is hosted under a sub-path, because it resolves against the
  origin and ignores `<base href>`: serving the gallery from `/Flare/` killed 112 of its 140 links at
  once. The guard now blocks by SCHEME instead of by shape - a relative reference is always safe, an
  absolute URL only when its scheme cannot run script - and it sees through what a shape allow-list
  never had to consider: leading whitespace, `JaVaScRiPt:`, and a tab dropped inside the word
  `javascript`, which a browser strips before it parses the scheme. `FlareMenuItem`,
  `FlareBottomNavItem` and `FlareNavLink` each carried a private copy of the old check; all six
  link-bearing components share `CssValidator` now. `IsImageSrcSafe` gets the same model, still refusing
  `data:` for anything that is not an image.

### Changed
- **GitHub Pages serves the gallery; the docfx site is gone.** docfx turned the XML docs into roughly
  eight thousand HTML pages that nothing in the repo ever linked to, and a component library's site
  should be its components: the gallery renders every one of them live, in every theme, and already
  carries the generated API reference and the changelog. `docs.yml` becomes `pages.yml` and publishes
  the WASM app; `docfx.json`, `filterConfig.yml`, the docfx landing page, its tocs and its template go
  with it. The markdown guides under `docs/` are no longer built into HTML - they stay in the repo,
  which is where the README already pointed at them. Deploys drop from 119 MB to 18 MB.
- **The gallery's own links are base-relative.** A project Pages site is served from `/Flare/`, so the
  navigation, the home page cards, the API links and the search index drop their leading slash and the
  home link becomes `""`. The same build now serves correctly from a site root (the Docker image) and
  from a sub-path.

## [0.16.0] - 2026-08-15

### Changed
- **BREAKING visually under MD3 Expressive: the button size ramp is the spec's.** Heights were
  32/40/48/56/64dp against a spec of 32/40/56/96/136dp, which made large and extra-large near-duplicates
  of medium and quietly threw away the size axis Expressive exists to offer - an extra-large button is a
  display-scale control for one hero action, not a slightly bigger button. Leading and trailing space
  (12/16/24/48/64dp) and the icon-label gap (8/8/8/12/16dp) move with them. The ramp lives on the
  Expressive theme rather than the shared Material bundle, because baseline M3 has exactly one button -
  "small", 40dp - and keeps its own gentler steps.
- **The button no longer reshapes on hover, which is what let the press be seen.** Material's shape-morph
  section has one trigger - "when pressed, buttons can morph to become more square" - and its corner
  table has three rows: round, square, pressed. Flare's Expressive theme also shrank the corners to a
  third of the height on hover, and that did more than add an unspecified state: a medium button
  travelled 24px to 16px on hover and then only 16px to 12px on press, so the one shape change Material
  asks for was reduced to the last quarter of its range and a pointer user never saw the rest. Corners
  now hold their resting shape until the press, and the press runs the full 28px to 12px.
- **BREAKING for custom themes: a toggle button is a button now, and the tokens follow.** `FlareToggleButton`
  renders a `FlareButton` carrying `flare-btn--selected` instead of a control of its own, so it takes the
  button's height, padding, typography, corners, focus ring and every variant - and a toggle dropped into a
  `FlareButtonGroup` is a segment on exactly the terms a plain button is. It gains `Variant`, `Shape`,
  `FullWidth`, `Typo` and `TrailingIcon` for free. `ToggleButtonTokens` drops from thirty members to four,
  losing the twenty-six that
  restated what `ButtonTokens` already said (heights, paddings, gap, rest and selected radii, rest colours,
  disabled opacity) and keeps only what the segmented container adds: its border, its two corner radii and
  its divider. A bespoke theme setting the old members will not compile; the values it wants are the
  button's. The `flare-toggle-btn` class family is gone from the DOM with them.
- **BREAKING for custom themes: thirty-nine new required token members across the button and the group.**
  `ButtonTokens` goes from 39 members to 62 and `ButtonGroupTokens` from 6 to 20.
  `ButtonTokens` gains `SelectedRadiusXs..Xl`, `SelectedRadiusSquare`, `SelectedBg` and `SelectedColor`;
  `ButtonGroupTokens` replaces `StandardGap` with a five-step ramp, `ConnectedInnerRadius` with one, adds
  `ConnectedPressedRadiusXs..Xl` and `ConnectedSelectedRadius`, and keeps `ConnectedGap`,
  `ConnectedOverlap`, `ConnectedOuterRadius` and `ZActive` as single values. Which families ramp is not
  arbitrary: a capsule is half the segment's own height and one token spells it at every size, while
  Material's interior corners and standard gaps are ramps no arithmetic on a height reproduces - and one
  of them tightens as the buttons grow, which no default would have guessed.
- **Selection changes shape, in the direction the shape decides.** Material states it as a swap - a
  selected button goes round to square, or square to round - so `flare-btn--selected` tightens a round
  button to the per-size selected corner and opens an explicitly square one out to whatever the theme
  calls its square-selected radius. Both directions are tokens rather than arithmetic, which is what lets
  Fluent UI 2 and Visual Studio answer selection with a repaint and no movement at all while Material
  Expressive travels the full swap.
- **The Expressive button group carries the spec's per-size numbers.** Standard gaps ramp 18/12/8/8/8dp,
  connected keeps a 2dp seam at every size, interior corners run 8/8/8/16/20dp and tighten to 4/4/4/12/16dp
  under a press. Those pressed corners used to be literals in the theme's stylesheet backed by an
  `!important`; they are tokens the base stylesheet reads now, and dropping the `!important` is what lets a
  pressed or selected segment be seen at all when the pointer is already over it.
- **A collapsed button group folds into a menu.** The segments that no longer fit used to appear in a
  bare popover panel; they now open from `FlareMenu`, which brings the surface, the backdrop that
  closes on an outside click, and the escape and focus handling a dropdown is expected to have. What
  is inside has not changed and is the point of the design: the panel holds the same buttons the bar
  declared, so a folded button keeps its own handlers and state because it *is* the same component.
  The hide rule that decides which of the two places each segment appears in is now scoped to the
  group, so it wins on specificity rather than on which stylesheet the bundle happens to load last.
- **The overflow ellipsis turns with the group.** A row folds into a vertical ellipsis and a column
  into a horizontal one - the dots run across the bar they fold, which is the convention an app bar
  and a navigation rail already follow.

### Added
- **A toggle button's colour now follows its variant, which is what Material's second colour table says
  it should.** "The default and toggle buttons use different colors", and every variant lands somewhere
  its own default never goes when selected: elevated fills with the accent, filled returns to it, tonal
  steps down from the container to the tone itself, and outlined inverts the surface. Ten new required
  `ButtonTokens` members carry it - four selected pairs plus one UNselected pair, because filled is the
  one variant that also differs before anything is chosen: a filled toggle at rest is a neutral
  container, not the primary fill a filled button is, or every option in a row would read as already
  selected. That distinction is keyed off `aria-pressed`, the attribute that already answers "is this a
  toggle?", so a command button cannot match it.
- **A per-size outline width.** `md.comp.button.<size>.outlined.outline.width` ramps 1/1/1/2/3dp - a
  stroke that reads as a hairline beside a small label is a thread beside a 32pt one - and the core had
  it hardcoded at 1px, which was both a spec miss and geometry the core is not entitled to own. Five new
  required `ButtonTokens` members; the width is reserved on every variant, not only the outlined one, so
  switching variant still never shifts layout.
- **`FlareToggleButton.OnLabel`, so the words can change with the state and not just the icon.** A toggle
  whose two states are different verbs - Follow and Following, Mute and Unmute - says what will happen
  and what already has, which one label cannot. Left null the label stays put across both states, which
  is what a toggle that signals itself with colour and shape wants.
- **`FlareMenu.FreeContent`, for a panel that holds content rather than menu items.** A menu panel
  keeps the focus on itself and moves a highlight over its items, which is the only way items that
  cannot take focus can be reached - and exactly wrong for content that is focusable in its own
  right, where swallowing Tab, Enter and Space leaves everything reachable by nothing but Escape.
  Setting `FreeContent` announces the panel as a group instead of a menu and lets those keys through.
  A button group's overflow is the first caller: its folded segments are buttons, not menu items.
  Menus of `FlareMenuItem` are untouched - the roving highlight, the panel role and the keys it
  claims are all as they were.

### Fixed
- **An icon-only toggle button was not square.** `FlareButton` decides "icon-only" by its `ChildContent`
  being null, and the rebuilt toggle handed its label over as markup between the tags - which compiles to
  a fragment that renders nothing but is not null. So a toggle with only an icon kept a full-width
  container, an empty label span and a gap beside its glyph: 78px wide where it should have been a 56px
  square. The label is passed as a parameter now, and a null label is genuinely null.
- **A selected toggle was invisible in four themes at one variant.** Giving every variant the same
  selected paint works only if no variant already rests there - and the tonal button rests on exactly the
  `secondary-container` those themes had chosen for "on", so a selected tonal toggle was pixel-identical
  to an unselected one in Fluent UI 2, Visual Studio, Aero, Liquid Glass and Material 2. Selection in
  those languages is the accent now, whatever the variant, and the filled toggle starts from a neutral
  container rather than from the accent it ends on. All four variants read distinctly in each theme.
- **Pressing the first or last button of a standard group made the group itself wider.** The press trade
  is meant to be a trade: the pressed segment takes space and the segments beside it give exactly that
  much up, so the row's width never changes and nothing around it moves. It only balanced in the middle
  of a row - at either end there is one neighbour rather than two, and the group grew by the step the
  missing neighbour never paid. Measured on a medium group: 304.4px at rest, 310.4px on a first or last
  press. The pressed segment now takes a step from each side that HAS a neighbour to take it from, so an
  end button expands inward and the row measures 304.4px in every position. The same correction applies
  to a vertical group's height, and to a collapsed group, where the last visible segment is followed by
  the overflow control rather than by a segment - a case no first/last-child rule could have described.
- **Pressing a toggle segment grew its neighbours instead of shrinking them.** A standard group trades
  width on a press: the pressed segment takes a step and the segments beside it give one up. The trade is
  written against the button's own padding token, and a toggle button was not a button - it carried a
  padding token of its own - so the rule reached it holding a value from the wrong family and the
  neighbours jumped outward. Measured after the rebuild: pressing a medium segment takes it from 82.9px to
  94.9px (the spec's 15%), its neighbour gives up 6px, and a segment that is not adjacent does not move.
- **A selected segment of a connected group did not go round.** Material makes it fully round - "selected
  inner corner size 50%" - but no rule said so, and the theme's hover capsule carried an `!important` that
  no non-important rule could have outranked anyway. A selected medium segment now measures 24px on all
  four corners against its own 48px height.
- **A selected item of a segmented `FlareToggleGroup` lost its fill.** The container clears its segments'
  backgrounds so the group reads as one object, and that rule outranks the button's selected paint on both
  specificity and load order; the selected item is repainted at the container's own weight again.

## [0.15.0] - 2026-08-09

### Changed
- **BREAKING for custom themes: five required token members added, one removed, one changed
  meaning.** `SliderTokens.DisabledActiveColor` and `DisabledInactiveColor`,
  `TabsTokens.SecondaryIndicatorThickness` and `SecondaryActiveColor`, and
  `StripeTokens.Background` (a new top-level `DesignTokens.Stripe`) are all new, and a bespoke
  theme - one not deriving from an in-box theme - must set them or it will not compile.
  `TableTokens.StripeOpacity` is gone, replaced by that shared stripe. **`ButtonGroupTokens` renamed
  its four geometry members** to `Connected*` and added `StandardGap`, and the four CSS variables
  moved with them (`--flare-btn-group-gap` is now `--flare-btn-group-connected-gap`, and so on) -
  the old names described one of two models while claiming to describe the component.
  **`TabsVariant` renumbered:**
  `Secondary` was inserted next to `Primary` where it belongs rather than appended, so `Text`,
  `Tonal`, `Filled` and `Outlined` moved from 3-6 to 4-7. Markup naming the members is unaffected;
  anything that persisted the *numeric* value - a stored setting, a query string - reads back one
  variant off. `CardTokens.StateLayer` now
  holds the **whole** hover layer rather than a colour the core mixed at a fixed 8%, so a custom
  theme setting a bare colour there will paint it opaque: point it at `var(--flare-state-hover-layer)`
  for the previous look, or name a translucent value of its own. Anything built with `with` from an
  in-box theme is unaffected.
- **One stripe token instead of three answers.** A striped table, a striped data grid and a striped
  description list all mean the same thing, and painted it three different ways: the table mixed
  `on-surface` at `--flare-table-stripe-opacity`, the description list mixed the same thing with the
  4% hardcoded, and the data grid stepped to `surface-container-low` - a different mechanism
  entirely. Only the table's was themeable at all. All three now read `--flare-stripe-bg`, which
  holds the whole paint rather than an opacity, because the two in-box answers to "what is a stripe"
  differed in more than strength. The table and the description list are pixel-identical; the data
  grid's stripe moves onto the wash, which measures **1 unit per channel in dark and at most 4 in
  light** - the two answers were nearly the same colour by different routes, which is the argument
  for there being one.
- **Seven hovers that invented their own paint now use the theme's.** The calendar's month arrows, a
  chip's close button, a dialog's close button, a chip's remove button in a multi-select or tag
  field, a snackbar's close button, the date picker's month and year buttons, and a picker day each
  mixed their own colour at their own percentage - anywhere from 8% to 15% for what is one concept -
  so hovering two affordances a few pixels apart could read differently, and no theme could change
  any of it. All seven read `--flare-state-hover-layer` now. Two of them, the date picker's
  month/year buttons and the picker day, mixed **primary**: picking a month lit up in the accent
  while the day beside it lit up neutral, an accent hover no spec asks for. They are neutral now,
  like the rest of the library.
- **`CardTokens.StateLayer` holds the layer, not a colour.** Same shape as the seven above, but the
  card had a token already - it just named the colour while the core kept the 8%. Both halves are
  the theme's now, which is what the token was for.
- **The virtualized tree reads the tree's selection tokens.** `TreeTokens.SelectedBg` and
  `SelectedColor` already held `color-mix(primary 16%, transparent)` and `primary` - the exact two
  values the virtual tree had written into its own stylesheet. So restyling a tree restyled one of
  them and not the other, and the two would have drifted the first time a theme touched either. Same
  pixels in every in-box theme; one token now reaches both.

- **Table and DataGrid row hover moved onto the state-layer model.** These were the last two core
  stylesheets computing an interaction state themselves - `color-mix(on-surface x hover-opacity,
  surface)`, i.e. core deciding what hover *means* for every theme - and they had been left behind on
  the strength of an assumption that turned out to be wrong. The assumption was that the paint has to
  stay on the cells, so each cell would need a `::before` layer and therefore a stacking context of
  its own, which would trap any popover opened from inside a cell - including Flare's own inline-edit
  dropdowns. A row needs none of that. It has one background and one `currentColor`, so the paint
  moved up to the `<tr>` and is simply `var(--flare-state-hover-layer)`: no layer, no isolation, and
  one tint per row instead of one per cell, which is what a row whose cells a consumer had coloured
  used to get. The striped rule moved to the row with it, because a cell background is the one thing a
  row-wide paint cannot get above. `StateLayerModelTests` no longer carries an allowlist.
- **A frozen DataGrid column highlights in the same colour as the row it belongs to.** It is the one
  cell that cannot take the row's paint - it is sticky, so it needs an opaque background of its own or
  the rows scrolling underneath read through it - and it used to be repainted on hover from `surface`
  while its idle state came from `surface-container`, so the pinned column shifted tone family
  whenever the pointer crossed its row. It now carries the same layer over its own background. This is
  the only cell in either component that takes one, and being sticky it was already a stacking
  context, so nothing new is isolated.

### Added
- **`FlareButtonGroup.Collapsible` - the segments that no longer fit fold into a "..." at the trailing
  end, and unfold when the room comes back.** Whether something fits is a question only the browser
  can answer, so this is the one part of the group measured in script rather than decided in CSS; a
  `ResizeObserver` reports it and the component is told how many folded. The fold is written as a
  `data-` attribute rather than a class or an inline style, because those two are Blazor's to rewrite
  and a decision written into either would be silently undone on the next render - and it is
  re-applied after every render anyway, which covers a segment being replaced outright.
  The overflow panel holds a second copy of the same content: with `ChildContent` an opaque fragment
  the group cannot relocate a button it never enumerated, but it can render the fragment twice and let
  the measurement show each item in exactly one of the two places. Nothing is on screen twice, and a
  folded button keeps its own handlers and state because it *is* the same component, declared once.
  Suits the standard model, which hugs its buttons and so can run out of room.
- **A button group's segments can be toggle buttons.** `FlareToggleButton` inside a `FlareButtonGroup`
  now gets the group's geometry - the seam, the shared corners, the raised z-index under the pointer -
  where before it was invisible to every one of those rules and rendered as a loose button in a row.
  A connected group of toggles is what Material means by a group that "helps people select options,
  switch views, or sort elements". The group's selectors say `:is(.flare-btn, .flare-toggle-btn)`,
  which costs the same specificity they already had, and they became CHILD selectors in the process,
  so the overflow panel's own buttons are not mistaken for segments.
- **`FlareButtonGroup.Connected` - the second of Material's two group models, and a theme now dresses
  both.** Until now Flare had one button group that behaved like both at once, and `ButtonGroupTokens`
  described one geometry, so whichever look a theme chose, the other model wore it. Fluent UI 2's
  bundle is written as the segmented control, so a *standard* Fluent group rendered joined - which is
  the opposite of what standard means.

  The models differ in what a press does: a standard group's segments respond to each other, so
  pressing one changes its width and its neighbours give up the space; a **connected** group's
  segments are independent, and pressing one changes only its shape. Flare applied the width trade to
  every group, which is wrong for half of them.

  `ButtonGroupTokens` now describes both - **unevenly, on purpose**. Connected keeps the whole seam
  vocabulary (`ConnectedGap`, `ConnectedOverlap`, `ConnectedOuterRadius`, `ConnectedInnerRadius`),
  because it is one control whose shape the group owns. Standard gets `StandardGap` and nothing else,
  because it is separate buttons standing together: every corner is the button's own, which is also
  what lets a standard group's buttons keep their own shape and their own hover and press morphs.
  Corner tokens for standard would describe a shape the model does not have. Measured: an Expressive
  standard group is three 24px pills 8px apart, its connected group a 2dp-seamed bar with capsule ends
  and 8dp interior corners; a Fluent standard group is three 4px buttons 8px apart, its connected group
  the joined control with a -1px seam.

  Standard stays the default, so existing markup is unchanged, and the rendered group names its model
  (`flare-btn-group--standard` / `--connected`) rather than one being the absence of the other.
- **`TabsVariant.Secondary` - the tab level below `Primary`.** A full-width indicator instead of one
  that hugs the label, thinner, and an active label in the content colour rather than the accent, so
  a secondary strip nested under a primary one reads as subordinate instead of competing with it.
  Both differences are tokens - `TabsTokens.SecondaryIndicatorThickness` and `SecondaryActiveColor` -
  because how far apart a design language sets its two tab levels is its answer, not the core's.
  Measured against the MD3 spec: primary active `#6750A4` at 3dp, secondary active `#1D1B20` with a
  2dp `#6750A4` indicator, all matching.
- **A theme states how a disabled slider looks.** `SliderTokens.DisabledActiveColor` and
  `DisabledInactiveColor`. The filled track, the thumb and the rail were three fixed fades of
  `on-surface` written into the core - one design language's disabled recipe - which is why Fluent
  UI 2 shipped a whole `slider.css` doing nothing but forcing its flat greys back over them with
  `!important`. That file is now four lines: three of its five rules became token assignments and
  every `!important` in it is gone. What stays is the disabled label colour, for the reason recorded
  in the issue - no CSS value means "leave this as painted", so a token for it would have to name a
  colour, which is the theme's decision to make.

### Fixed
- **A pressed button group's neighbours give up the space the pressed segment takes.** The Material
  Expressive press grew the pressed segment and left the rest alone, so the group got wider on every
  press and the neighbours were displaced rather than compressed. The token table has one row for
  this - `pressed width multiplier` 15% - and on its own it is not a complete description: a standard
  group hugs the buttons inside it and is meant to animate without disturbing the layout around it,
  so the 15% has to come from the segments on either side. Each neighbour now gives up half a step
  per side, which is exactly what the pressed one gains, so a group with a neighbour on both sides
  holds its size to the pixel. At the ends only one neighbour can pay and the group gives a little,
  which is how an elastic row behaves anyway.
- **The pressed corner is the group's own, not the lone button's.** A grouped segment tightens to
  `md.comp.button-group.connected.<size>.pressed.inner-corner-size` - 4/4/4/12/16dp - where this had
  been borrowing the button family's pressed corner (8/8/12/16/16dp), so every size below large
  under-tightened.
- **A vertical button group presses along its own axis.** The grow was side padding regardless of
  direction. In a column the cross axis is horizontal and its items stretch to the widest, so
  pressing any segment widened the entire stack instead of lengthening the one pressed. The spec's
  "width multiplier" is a main-axis measurement, and in a column that is the height; a vertical group
  now trades height, and its width no longer moves. Measured: pressing the middle segment of a
  three-button column takes it from 48px to 60px while its neighbours go to 42px and the column's
  height and width both stay put.
- **An inactive tab is no longer as dark as the active one under Material.** The theme set the
  inactive label to `on-surface`, where MD3 specifies `on-surface-variant` at rest for both tab
  levels and `on-surface` only under hover or focus. On a primary strip the accent hid it; on the new
  secondary strip, where the label colour is nearly the whole distinction, it would have been
  obvious. The audit had marked the primary tab faithful, which it was on every axis but this one.
- **A grouped table's group header hovers in its own colour again.** `.flare-table--hover tbody
  tr:hover td` reaches three elements deep and outranked `.flare-table__group-row:hover td`, so in a
  hoverable table - the default - a group header showed the ordinary row wash instead of the
  `surface-container-high` step written for it. The rule was never wrong; it just never won. With the
  row wash on the row, the group's own opaque cell sits above it and the rule applies.
- **A group header in a striped table keeps its group colour.** Same shape of defect: the stripe was
  two classes deeper than the group tint and both painted on the cell, so an even-numbered group
  header rendered as a striped data row. The stripe is on the row now, the group tint on the cell, and
  they no longer compete.

## [0.14.0] - 2026-08-09

### Added
- **`ListTokens`, `AccordionTokens` and `CollapseTokens`.** These three components had no token record
  at all: they were assembled out of primitives - raw spacing steps, literal `3.5rem`/`4.5rem` row
  heights, a `0.875rem` chevron, a magic `2000px` open ceiling - and both the list and the accordion
  read the **card's** radius, so a theme could not reshape a card without reshaping them too. Every
  value a design language could have an opinion about is now a `--flare-list-*`, `--flare-accordion-*`
  or `--flare-collapse-*` token, 48 in all. The in-box themes set them to what the core used to paint,
  so nothing changes appearance; what changes is that it can now be changed. Accordion and collapse
  keep separate records on purpose - one is a filled section inside a bordered container, the other a
  transparent standalone control, and a theme is entitled to size and weight them differently.

### Changed
- **List rows, expandable headers, tabs, tree rows and the DataGrid's chrome use the state-layer
  model.** Their hover was computed in core as `color-mix(on-surface x hover-opacity, ...)` - core
  deciding what hover *means* for every theme, which is why a language with a discrete neutral fill
  had to override the whole rule with `!important`. They now carry a `::before` layer painted from
  `--flare-state-*-layer`, like the button and the menu item; list rows and expandable headers also
  gain focus, pressed and focus-while-hovered states they did not have. Three theme overrides moved
  from repainting the element to naming the layer: Fluent UI 2's `!important` tab wash, Aero's
  gradient tab hover and Visual Studio's tab wash - Aero's being the proof that the token takes a
  gradient as readily as a colour. The table's and the DataGrid's **row** hovers are deliberately
  unchanged: they paint on the cells, whose content is whatever the consumer put there, and a layer
  over a bare text node needs a per-cell stacking context that would change how a popover inside a
  cell stacks. Tracked as an issue.
- **Nine more surfaces follow: nav links and group headers, pagination buttons, stepper navigation,
  the colour-mode toggle, virtual-tree rows and their toggles, calendar and time-picker cells, the
  field clear button and the numeric stepper.** Four more theme overrides became token assignments -
  Aero's glass pagination and calendar hovers, Liquid Glass's tinted ones - and two of those shed an
  `!important` with the move. The stepper's filled button lost its hand-written
  `color-mix(primary 92%, on-primary)` entirely: the shared layer is content-coloured, so it tints
  with the button's own label without a rule of its own.
- **The current page and the current nav destination no longer light up under the cursor**, which
  they briefly would have: the rules that used to suppress them were a class less specific than the
  rule that turns the layer on, so they lost regardless of order. The layer now excludes them instead
  of switching off afterwards. Two more `!important`s went with it.
- **The sweep finishes: listbox options, the eyedropper, the date picker's month label and range
  presets, the confirm-dialog and message-box buttons, the scroll-to-top button and the snackbar
  action.** Every core stylesheet except two now leaves the paint to the theme, and the Fluent UI 2
  theme's stylesheet contains no `!important` at all. The confirm dialog and the message box each lost
  one of their two hover rules: the layer is content-coloured, so cancel tints with its primary label
  and confirm with its on-primary label over its own fill - which is what the two hand-written
  `color-mix`es computed. Only the table's and the DataGrid's **row** hovers remain, for the reason
  above.
- **A hover on a list-box option is a neutral wash rather than a primary tint** in Material-lineage
  themes (also the eyedropper and the clickable month label). Core was mixing `primary` there - an
  accent hover no spec asks for, that no theme could change, and that made a select option hover in a
  different colour from the menu item beside it. A useful side effect: the keyboard-active row keeps
  its accent fill, so the two are now told apart at a glance instead of both reading violet.
- **BREAKING for custom themes: four more required token members.** `CheckboxTokens.DisabledOpacity`,
  `RadioTokens.DisabledOpacity`, `MenuTokens.ItemDisabledOpacity` and
  `ToggleButtonTokens.DisabledOpacity`. Each replaces a fade the core used to apply on every theme's
  behalf, continuing the button's treatment from 0.13.0: a language that signals disabled by fading
  sets the shared state value, one that repaints in a flat palette sets `1` and carries the change in
  its own stylesheet. That removed four `opacity: 1 !important` overrides from the Fluent UI 2 theme.
  A bespoke theme must set the four or it will not compile; in-box themes and anything built from
  them with `with` are unaffected and none of the six changes appearance.
- **BREAKING for custom themes: `DesignTokens` gains `List`, `Accordion` and `Collapse`.** Required,
  like every other component record. A bespoke theme must supply all three; anything built from an
  in-box theme with `with` inherits them.
- **BREAKING for custom themes: eight more required token members, and no theme forces opacity any
  more.** `NavTokens.LinkDisabledOpacity`, `TabsTokens.TabDisabledOpacity` and `ScrollDisabledOpacity`,
  `PaginationTokens.BtnDisabledOpacity`, `LinkTokens.DisabledOpacity`,
  `BottomNavTokens.ItemDisabledOpacity`, `InputTokens.DisabledOpacity` and
  `SliderTokens.DisabledOpacity`. With these the disabled model is finished: **every
  `opacity: 1 !important` is gone from every in-box theme**, where before Fluent UI 2 needed twelve
  selectors' worth of them to undo a fade the core applied on its behalf. What is left in the theme is
  the disabled foreground colour, which cannot become a token - no CSS value means "leave this as
  painted". The tab and its overflow scroll button get separate members on purpose: a language may
  mute a spent affordance more heavily than an unavailable destination.

- **A guard against the state model sliding back.** `StateLayerModelTests` fails if a core stylesheet
  mixes an interaction state from `--flare-state-hover-opacity` again, if an in-box theme forces
  `opacity: 1 !important` to undo a core fade, or if the two-file allowlist for the table and DataGrid
  rows goes stale. The old form is the kind of thing that returns by imitation - the next person adding
  a hover copies the rule beside it - and nothing else would catch it: the CSS is valid, CssAudit sees
  well-formed token names, and it renders correctly under the one theme it was written for.

### Fixed
- **The chevron on a combobox, select or multi-select opens and closes the list.** On
  `FlareAutocomplete` it did nothing at all - it was a decorative `<span>` with no handler, and only
  focusing the input opened the list, which is why the dead affordance went unnoticed. On a
  **searchable** select or multi-select the container's click handler could only ever open, so the
  chevron could not close what it had opened while visibly pointing up. It is a real `<button>` in all
  three now, with `aria-expanded` and a label, kept out of the tab order (the field is the tab stop)
  and with `mousedown` suppressed so clicking it never pulls focus out of the input.
- **The wavy ring flows.** Its crests now travel round the indicator while the arc stays where the
  value put it. The flow rotates the path a full turn and walks `stroke-dashoffset` by the matching
  arc length to cancel it - which is what the first attempt did, and it came apart because the dash
  array was a single window rather than a repeating pattern: its period shared no common measure with
  the path, so the sweep dragged the visible arc onto the trailing gap and it changed length and
  position every frame. The array is now two values summing to exactly the path length, so the pattern
  repeats once per lap and the sweep lands back on itself. Rotation is linear in angle and arc length
  is not, so the arc's endpoints breathe by about a quarter of a percent of the circumference - a
  fraction of a pixel, and it cancels each wave rather than accumulating. One cycle is one lap, so the
  ring and the linear bar pulse at the same rate off the same `wave-speed` token. Honours
  `prefers-reduced-motion`, which stills the crests and keeps the gap.
- **`FlareProgress.Wavy` and the circular gap were being ignored by every theme.** Both are computed in
  C# rather than in CSS, because an SVG path has to be built from numbers - and the reader looked only
  in `DesignTokens.Extended`, which those values left when they became typed `ProgressTokens` members
  during the token-mandate work. Every lookup fell through to its fallback, so `Wavy` drew a flat bar
  and the ring drew no break between the indicator and the track. The reader now asks the same
  flattened design the emitted CSS is built from, so what the component computes and what the
  stylesheet paints cannot disagree again. CssAudit could not have caught this: every name existed and
  was in sync - a guard test now checks that each token the component looks up is actually there.
- **Pressing a button, a split button or a group segment now shows its morph.** The corners and the
  group's width grow rode the same 300ms spring as the hover morph, and an ordinary click holds
  `:active` for a fraction of that - measured at about two frames. The shape moved a hair and sprang
  back, so a press looked exactly like a hover, which is what it was: only the hover morph was ever
  visible. Entering the pressed state now uses a short duration so it arrives while the finger is
  still down; leaving it keeps the spring, which is where the overshoot belongs.

## [0.13.0] - 2026-08-08

### Changed
- **BREAKING for custom themes: six more required token members.** `StateTokens.FocusHoverLayer`,
  `ButtonTokens.DisabledOpacity` and `DisabledLayer`, and `RadioTokens.FocusOutline`,
  `FocusOutlineOffset` and `FocusShadow`. A bespoke theme - one not deriving from an in-box theme -
  must set them or it will not compile, which is the mandate working rather than an oversight: each
  replaces something the core used to decide on every theme's behalf. In-box themes and anything
  built with `with` from them are unaffected, and none of the six in-box themes changes appearance
  because of them.

### Added
- **A theme states how a disabled button looks, instead of forcing it.** `ButtonTokens` gained
  `DisabledOpacity` and `DisabledLayer`. A design language that signals disabled by fading sets a
  fraction and parks the layer at transparent; one that repaints in a flat palette stays fully opaque
  and puts its fill in the layer. Both sides are real values - parking a token at `initial` is banned
  here, and for good reason. The repaint is a layer rather than a background colour precisely because
  transparent is then a genuine no-op, where overriding the element background has none. FluentUI2's
  disabled button drops its forced opacity and container repaint; its foreground and stroke stay in
  theme CSS, because neither `color` nor `border-color` has a value meaning "leave this alone".
- **Pressing a segment in a button group widens it, so its neighbours flinch.**
  `md.comp.button-group.standard.<size>.pressed.item.width.multiplier` asks for 15%; the spec says
  nothing about the neighbours because it does not have to - they move because layout moves them.
  That is also why it had to be a layout property: a transform would leave them exactly where they
  were and stretch the label besides. Measured on a live group, the pressed segment grows to the
  spec figure and the neighbour overshoots its resting place by a pixel before settling, which is
  the flinch. Side padding stands in for the width multiplier, since a percentage of an item's own
  width is not something CSS can name without measuring each segment in script.
- **A theme can now state its own interaction model for the button, instead of overriding the core.**
  `StateTokens` gained `FocusHoverLayer` - the layer painted while an element is hovered AND focused,
  the one pairing the design languages genuinely disagree about (pressed outranks both everywhere
  else). A language whose focus is a fill resolves it to focus; one whose focus is a stroke resolves
  it to the hover fill so the ring and the fill coexist. FluentUI2 uses this to drop the
  `opacity: 0 !important` blocks that used to suppress the core state layer before repainting the
  button underneath: each variant assigns the layer tokens instead. Identical pixels - the black
  overlay and the old darkened repaint agree to 0.0001 per channel - with the difference now living
  in token values, which is the point.
  The menu item followed in the same release, which leaves the FluentUI2 theme with no state-layer
  suppressions at all. List, tab and listbox items keep a one-line hover override each: they paint
  their hover straight onto the element rather than through a state layer, so there is no token to
  assign - recorded in the issue as an accepted coupling rather than swept.

- **`FlareSlider.HandleOnHover` turns the slider into a media scrubber.** A seek bar was expressible
  already - a zone paints the buffered range, `MouseWheel` seeks - but the handle sat there at rest,
  so it never looked like one and apps hand-rolled a raw `<input type="range">` instead. The handle
  now stays hidden until the control is hovered, focused or dragged. Only its paint is hidden: the hit
  area, the tab order and the reported value are untouched, and where the pointer cannot hover the
  handle stays visible rather than leaving a bar nobody can grab.

### Fixed
- **The button and menu item labels sit above their state layer.** The layer is an absolutely positioned
  `::before`, so it painted over the label; that only ever went unnoticed because every state layer
  so far was translucent. A theme whose states are opaque fills would have covered its own text.
- **The radio ring reads focus tokens, like every other selection control.** Its focus indicator was
  a literal in the core stylesheet - 2px primary - while the checkbox and the switch drew theirs from
  tokens at 3dp secondary, so the family disagreed with itself and no theme could reach the radio to
  say otherwise. `RadioTokens` gained `FocusOutline`, `FocusOutlineOffset` and `FocusShadow`,
  mirroring the checkbox, and each in-box theme now gives its radio the same focus it already states
  for its checkbox.
- **The clipboard's copied tick is visible on a filled button.** The copied state tinted the whole
  control primary, including its foreground - so on a filled primary copy button the tick was drawn
  primary on a primary container and could not be seen at all. The tint now skips the filled variant,
  where the foreground belongs to the fill and the icon swap is the confirmation on its own. This was
  specific to `FlareClipboard`: it is the only component that repaints the foreground of a button root.
- **The split button and the button group morph on press, not only on hover.** Press was listed
  alongside hover and aimed at the same capsule, so pressing a half or a segment the pointer was
  already over changed nothing. It could not fall through to the button's own pressed corner either,
  because the hover capsule carries `!important` and outranks it whatever its specificity. Both now
  tighten to the button family's per-size pressed corner - 8dp at xs and sm, 12dp at md, 16dp at lg
  and xl - the same ramp a single button uses.

## [0.12.1] - 2026-08-08

### Changed
- **BREAKING: the shape morph is a theme token, and `PressMorph` is gone.** Whether a component
  reshapes its corners as you interact with it is a property of a design language, not of a call
  site - MD3 Expressive specifies it, Fluent 2 specifies the opposite - so a component parameter
  meant no theme could deliver its own behaviour without every usage opting in. `ShapeTokens` gained
  `MorphDuration` and `MorphEasing`: one pair for the whole library, because reshaping on interaction
  is a single statement a design language makes. A theme that reshapes gives the travel a duration,
  one that does not parks it at an instant. The `PressMorph` parameter on `FlareButton`,
  `FlareIconButton` and `FlareFileUploadButton`, the `flare-btn--morph` class, and the core's
  pressed-corner values that came with it are all removed - those radii were a theme opinion living
  in the core. The button, the toggle button, the split button and the button group all read the new
  pair, so under MD3 Expressive they now morph as its spec always said they should; the other five
  in-box themes are unchanged.
- **BREAKING for custom themes: the toggle button's rest radius is per size.**
  `ToggleButtonTokens.Radius` is replaced by `RadiusXs` through `RadiusXl`. A theme whose toggle
  button is a capsule has to express that as half the size's own height, and a single token cannot:
  it resolves once at the document root, where no size is in scope. Themes deriving from an in-box
  theme are unaffected.

### Fixed
- **A capsule that animates is half the height, not 9999px.** MD3 Expressive's button and toggle
  button expressed their rest shape through the full end of the shape scale. The browser clamps a
  radius that large when it paints, so the pill looked right standing still - but once those corners
  animated, nothing changed on screen until the interpolated value fell under the clamp, in the last
  thousandth of the duration. The morph read as a pause and then a jump. Both now use half the size's
  own height: the same pill, interpolating through values that are actually painted.
- **The browser no longer draws its own stepper inside a number field.** It appeared regardless of
  `FlareNumericField.ShowStepper` - two steppers side by side with it on, one the parameter said
  should not exist with it off. Three components render a number input and only the colour picker had
  suppressed it, so the DataGrid's numeric filter and the date-time picker's hour and minute boxes
  carried it too. Nothing is lost: the arrow keys already step the value.
- **A field's prefix, suffix, clear button and stepper stay inside the field.** A form control's
  `min-width: auto` resolves to its intrinsic width - about twenty characters - so as a flex item it
  refused to shrink and pushed whatever followed it past the field's edge, which is how a `kg` suffix
  ended up outside a numeric field while a shorter `%` happened to fit.

## [0.12.0] - 2026-08-08

### Added
- **Spring motion, which a cubic-bezier cannot express.** `MotionTokens` gained three spatial spring
  easings (fast / default / slow) and a matching duration for each. A cubic-bezier is monotone by
  construction, so it cannot describe a settling overshoot at all - which is why a design language built
  on springs previously rendered without any. Each easing must be used with its own duration: a spring is
  a shape *plus* the time it takes to settle, and pairing it with a different duration truncates the curve
  into a snap. MD3 ships the real curves, sampled from a damped harmonic oscillator at the Expressive
  scheme's parameters (a pronounced overshoot on the fast rung, restrained on the other two). They are read
  by the switch handle's travel and grow-on-press, the button's opt-in press morph, and the split button's
  seam morph and caret rotation. Colour deliberately stays on the plain ramp, since a spring on a colour
  overshoots past the target tone and clamps there instead of settling into it.
- **`FlareChip.Disabled`.** The chip was the one component with no way to switch it off - no parameter, no
  class, no styling - so a filter or choice chip that needed to show an unavailable option had to not
  render it. A disabled chip dims, leaves the tab order, reports `aria-disabled`, and fires none of its
  click, keyboard or close callbacks. Inside a `FlareChipGroup` it also stops taking part in selection, so
  one that is already selected stays selected and cannot be cleared by clicking it.
- **A reduced-motion guard on the switch**, matching the one the button's press morph already had.

### Changed
- **BREAKING for custom themes: `MotionTokens` grew six required members** (three spring easings, three
  spring durations). A bespoke theme - one not deriving from an in-box theme - must set them or it will not
  compile. In-box themes and anything built with `with` from them are unaffected. A theme whose language
  does not bounce is expected to answer that plainly by pointing the spring easings at its ordinary curve;
  that is what four of the six in-box themes do, and none of them changes visually.
- **`FlareQrCode` encodes up to 2953 bytes instead of 76.** See below - the cap was low enough to count as
  a defect rather than a limit.

### Fixed
- **`FlareQrCode` no longer refuses ordinary payloads.** The encoder stopped at symbol version 4, so
  anything past 76 bytes at correction level L (60 at M) drew "Value too long" instead of a code - under
  the length of a plain vCard and under many everyday URLs, which made the component unusable for its two
  most common jobs. It now covers the whole ISO/IEC 18004 range, versions 1 to 40, choosing the smallest
  symbol that fits: 2953 bytes at level L, 1273 at H. The message it shows when a payload genuinely does
  not fit is localized now, too.
- **Encoding a QR code no longer repeats on every parameter change.** It evaluates all eight mask patterns
  over the whole symbol, which is worth skipping when neither the payload nor the correction level moved -
  and at the larger versions now reachable, that is no longer a small saving.

## [0.11.0] - 2026-08-07

### Added
- **`Flare.Components.Query` - a visual query designer and a query text editor.** Two components over
  the [Querio](https://www.nuget.org/packages/Querio/) query model, which lives in its own repository and
  ships as its own package:
  - **`FlareQueryBuilder`** composes sources, joins, columns, aggregates, date truncation, grouping,
    nested AND/OR conditions, sorting and paging against a caller-supplied `QuerySchema`. It **builds a
    query and never runs one** - the output is a serializable `QuerySpec` that the consumer renders to
    SQL, translates to `IQueryable`, or posts to an API. What it offers is narrowed by an
    `IQueryCapabilities`, so a target that cannot do percentiles never lists them.
  - **`FlareQueryEditor`** writes the same query as text, with completion as you type (including
    navigation through foreign keys, `[r].[apiKeyId].[ownerId].[name]`, which no SQL dialect has) and
    every problem underlined in place rather than only the first.
  - **`QueryBuilderLabels`** holds every caption the designer shows in one record, so a host translates
    the whole set in a single assignment instead of parameter by parameter.
- **`FlareCodeBlock` became editable, and can advise.** `SuggestionProvider` supplies completions
  asynchronously, `Markers` underlines errors and warnings at exact offsets, `MaxSuggestions` caps the
  list, and `Wrap` soft-wraps long lines instead of scrolling them off the side.
- **Splitter tokens.** `SplitterTokens` - gutter thickness, grip thickness/length, idle and hover
  colour, centre-icon size and colour - so a theme owns how a splitter looks.

### Changed
- **BREAKING for custom themes: two token records grew required members.** `DesignTokens` gained
  `Splitter`, and `ToggleButtonTokens` gained `RadiusSelectedXs` / `RadiusSelectedXl`. A bespoke theme
  (one not deriving from an in-box theme) must set them or it will not compile. In-box themes and
  anything built with `with` from them are unaffected. This is the token mandate being enforced, not
  widened: those values previously came from core CSS, where no theme could reach them.
- **A dropdown now sizes to its content.** Anchored panels take the anchor's width as a minimum rather
  than as the width, grow to `max-content`, and stop at the viewport - so a select whose values are
  longer than its field no longer truncates them.
- **FluentUI2's toggle button no longer morphs its shape at the xs and xl sizes.** The theme states that
  selection changes colour only, and those two sizes were the ones it could not reach, so they had been
  rendering a shape morph against that intent.
- `Microsoft.AspNetCore.Components.Web` now floats to the newest patch of each target's major
  (`8.0.*` / `9.0.*` / `10.0.*`), rather than pinning net10.0 to one patch in two places at once.

### Fixed
- **Text and colour in the code editor no longer drift apart.** The editor is an invisible textarea over
  a coloured `<pre>`; the textarea soft-wrapped and the `<pre>` did not, so on any line after a wrap the
  caret landed on the wrong character and a selection highlighted text other than the one shown. Both
  layers now agree on wrapping, font, line height and scroll position.
- **The split button's trigger is square at every size, so MD3 Expressive draws it round.** Its width token
  is declared on `:root`, where the per-size button height does not exist, so "width = button height"
  resolved once against the medium fallback and inherited 48px to every size. At the large size that is a
  48x56 box, on which a half-height radius draws a stadium rather than a circle. The width now comes from
  the element's real height; a theme that wants a fixed-width trigger still sets one and is unaffected.
- **An icon inside a coloured button is no longer painted in the button's own fill.** A `Color` sets the
  role tokens on the button element, both inherit into its content, and an icon paints itself the role's
  main colour - which on a filled button is exactly the background beneath it, so
  `<FlareButton Variant="Filled" Color="Primary">` drew any icon invisibly. This is what hid the split
  button's caret. Icons in a button now take the button's own foreground, as part of its label.
- **A menu opened with the pointer no longer highlights its first item.** The highlight said "this one is
  selected" to someone who had just clicked somewhere else. A keyboard opening still starts on the first
  item, and the first arrow key after a click lands on the first item rather than skipping it.
- **The splitter and the toggle button's end sizes are the theme's again.** Seven splitter constants had
  no token record behind them, and the toggle's selected radius was declared for sm/md/lg while the CSS
  also read xs and xl, so those values shipped from core CSS where a theme could not change them. A
  guard test now fails the build on any token constant no theme can set.

## [0.10.0] - 2026-07-19

### Added
- **A polymorphic icon system: any icon provider fits any icon slot.** `FlareIcon` is an abstract icon
  descriptor; concrete providers carry their own options and drop into any parameter typed `FlareIcon`.
  `FlareSvgIcon` (inline SVG path/markup) is the only provider in core; the rest ship as opt-in packages so
  core depends on no third-party icon set:
  - `Flare.Icons.MaterialDesign3.Symbols` - `FlareMaterialDesign3Icon` (Material Symbols variable font:
    `Fill`/`Weight`/`Grade`/`OpticalSize`/`Family`).
  - `Flare.Icons.MaterialDesign2.Symbols` - `FlareMaterialDesign2Icon` (classic Material Icons webfont:
    Filled/Outlined/Round/Sharp/TwoTone).
  - `Flare.Icons.MaterialDesign2.Svg` - the full Material Icons (filled) set as inline SVG (`MaterialDesign2Icons`
    catalog, 2122 icons), generated by `tools/MaterialIconGen`.
  - `Flare.Icons.MaterialDesign3.Svg` - the full Material Symbols (rounded) set as inline SVG
    (`MaterialDesign3Icons` catalog, Regular + Filled, 3894 icons), generated by `tools/MaterialSymbolsGen`.
  - `Flare.Icons.FluentUI.Svg` - the full Fluent UI System Icons set as inline SVG (`FluentUIIcons` catalog,
    Regular + Filled, Size 24, ~5000 icons), generated by `tools/FluentIconGen`. Values are plain
    `FlareSvgIcon`, the same type the Material SVG packages use (no Fluent-specific icon type).
  - `Flare.Icons.FontAwesome.Symbols` - `FlareFontAwesomeIcon` (Font Awesome webfont: Solid/Regular/Light/
    Thin/Duotone/Brands).

  Render one standalone with `<FlareIconView>`. Each icon is its own static member and the SVG packages are
  marked `IsTrimmable`, so a trimmed (Blazor WebAssembly) publish downloads only the icons the app actually
  references - not the whole catalog - even under partial trim mode.
- **A built-in, dependency-free SVG icon set (`FlareIcons`).** 96 ready `FlareSvgIcon` members (`Home`,
  `ChevronLeft`, `ExpandMore`, `Close`, ...) rendered as inline SVG - no icon font, no network request, no
  FOUT, theme-agnostic. This is Flare's own set and it backs the default component chrome. Reference a member
  directly (`FlareIcons.Home`); `FlareIcons.All` / `FlareIcons.Find(id)` enumerate the set by id.

### Changed
- **BREAKING: the icon value types + catalogs moved to a new `Flare.Icons` package and namespace.**
  `FlareIcon`, `FlareSvgIcon` and the built-in `FlareIcons` set now live in a small **`Flare.Icons`** package
  (namespace `Flare.Icons`), and every provider catalog/type (`MaterialDesign2Icons`, `MaterialDesign3Icons`,
  `FluentUIIcons`, `FlareMaterialDesign3Icon`, ...) is in the `Flare.Icons` namespace too. The provider
  packages now depend on the tiny `Flare.Icons` (which needs only `Flare.Theming`) instead of the whole
  `Flare.Components`, so icons can be used with a far lighter footprint. Migration: add `using Flare.Icons`
  (or `@using Flare.Icons`). `FlareIconView` stays in `Flare.Components`.
- **BREAKING: the `<FlareIcon>` component is replaced by `<FlareIconView>` plus the `FlareIcon` descriptor.**
  `FlareIcon` is now the abstract icon-value type, so the standalone renderer is `<FlareIconView>`. It takes a
  typed `Value` (`<FlareIconView Value="@FlareIcons.Home" />`) plus `Size`/`Color`/`AriaLabel` overrides -
  there is **no `Name`/`Icon` string lookup and no implicit `string`->`FlareIcon` conversion** (a name lookup
  would defeat SVG-package trimming). Reference icons by their typed member everywhere.
- **BREAKING: `FlareIconButton.Icon` is now a `FlareIcon`, not a Material-name string**, so any provider
  works in an icon button. Pass a typed value: `Icon="@FlareIcons.Settings"`.
- **BREAKING: `FlareIcons.*` members are now `FlareSvgIcon` values, not name strings** - they render inline
  SVG. `FlareIcons.All` (icon ids) and `FlareIcons.Brands.FlareLogoShort` are unchanged.
- **Component chrome no longer forces the Material Symbols font.** Every default icon Flare draws itself -
  tree/submenu/nav chevrons, dialog and tab close, scroll-to-top and stepper marks, data-grid sort/filter/
  group/tree toggles, file-upload and split-button glyphs, and more - now renders as built-in SVG, so a
  non-MD3 theme (or an app that never loads the Material Symbols font) shows correct chrome with no external
  icon dependency and no FOUT. Zero raw `material-symbols` spans remain in the components.
- **BREAKING: icon parameters across components become `FlareIcon?` (any provider).** `FlareMenuItem`,
  `FlareSubMenu`, `FlareTreeItem`, `FlareNavGroup`, `FlareTimelineItem`, `FlareSplitter` (`Icon`/`HoverIcon`),
  `FlareFloatingActionMenuItem`, `FlareAvatar` (`FallbackIcon`), `FlareSlider` (`StartIcon`/`EndIcon`) and
  `DataGridTreeConfig` (`CollapsedIcon`/`ExpandedIcon`), and the optional `Flare.Components.IDE` package's
  ribbon/backstage/document-tab icons now take a `FlareIcon` rather than a Material-name string. Pass a typed
  value (`Icon="@FlareIcons.Home"`). No raw `material-symbols` spans remain anywhere in `src/`.

### Fixed
- **The DataGrid export menu, date/time picker triggers and combobox spinner show their icons again.** These
  chrome icons referenced Material Symbols ids (`data_object`, `grid_on`, `picture_as_pdf`, `table`,
  `calendar_month`, `calendar_clock`, `progress_activity`, `fullscreen`) that were not in the built-in
  `FlareIcons` set, so after core stopped falling back to the Material font they rendered empty. The eight are
  now part of the built-in set (`FlareIcons` is 84 -> 92), so the default chrome is whole again without any
  icon-font dependency.
- **A snackbar error now interrupts a screen reader; success/info/warning still wait their turn.** The
  provider was one `aria-live="polite"` container, so every notice - errors included - was announced
  politely and could be missed. A stack mixes politeness levels and a container can only carry one, so the
  live semantics move onto each toast: `role="alert"` (assertive) for `Error`, `role="status"` (polite) for
  the rest, `aria-atomic` so the whole toast is read. No API change - `ISnackbarService.Show(...)` is
  unchanged; this is the transient-notification service the `toast-snackbar` request asked for, now with the
  assertive-error behaviour it wanted.

## [0.9.0] - 2026-07-17

### Changed
- **BREAKING: `BadgeTokens` gains a per-size ramp.** `MinWidth`, `Height`, `DotSize` and `PaddingX` become
  `*Xs`/`*Sm`/`*Md`/`*Lg`/`*Xl`, and the indicator's font size joins them as `LabelSize*` - the same shape
  `ButtonTokens` already uses.

  It was one set. The theme named the default size and `badge.css` hardcoded the other four in literals, so
  four of the five sizes were core's opinion and no theme could reach them - a badge simply could not be
  resized by a theme. Measured after: xs/sm/md/lg/xl still paint 12/14/16/20/24px with a 4/5/6/8/10px dot
  and a 9/10/11/12/14px label, identical to the literals they replace; and setting
  `--flare-badge-height-xs` from a theme now moves the box, which it never did before.

  Migration: `Badge = ... with { Height = "1rem" }` -> name the step you mean (`HeightMd = "1rem"`), or all
  five. `Radius`, `Offset` and `DotOffset` are unchanged - they do not vary by size.
- **BREAKING: `SwitchTokens` gains a per-size ramp.** The eight geometry members - `TrackWidth`,
  `TrackHeight`, `ThumbOffSize`, `ThumbOnSize`, `ThumbPressedOffSize`, `ThumbPressedOnSize`, `ThumbOffLeft`,
  `ThumbOnLeft` - each become `*Xs`/`*Sm`/`*Md`/`*Lg`/`*Xl`, the same shape `ButtonTokens` uses.

  They were single. The theme named the md size and `switch.css` hardcoded xs/sm/lg/xl in literals, so four
  of the five sizes were core's opinion and no theme could reach them. Measured after: every size paints
  exactly as before under Material (track 34/40/52/64/76px across xs..xl), Fluent keeps its own compact md
  thumb, and setting e.g. `--flare-switch-track-width-lg` from a theme now resizes the lg switch, which it
  never could.

  Migration: a theme building `SwitchTokens` names all five steps per property (or derives via `with` and
  overrides the md step, as Visual Studio and Liquid Glass do). Positions (`ThumbOffLeft`/`ThumbOnLeft`) are
  ramped too, so a theme controls the thumb's inset per size rather than inheriting a fixed formula.
- **BREAKING: the field's border tokens now carry a colour, not a border shorthand.**
  `InputTokens.OutlinedBorder`, `FilledBorderBottom` and `HoverBorderBottom` become `BorderColor`,
  `BorderBottomColor` and `HoverBorderBottomColor` (`--flare-input-border` -> `--flare-input-border-color`,
  and likewise for the other two). A theme sets `BorderColor = "var(--flare-color-outline)"` now, not
  `"1px solid var(--flare-color-outline)"`; `none` becomes `transparent`.

  The width moved into the component CSS as `border: 1px solid transparent`, reserved on every variant. That
  is what fixes the bug below, and it takes the `1px solid` literals out of the variant classes so the token
  mandate holds. Migration: drop the `1px solid` from your three values (and turn `none` into
  `transparent`).

### Fixed
- **The Fluent theme's switch sizes were out of order.** Fluent overrode only its md switch to a compact
  thumb but let xs/sm/lg/xl inherit the Material ramp, so the on-thumb went 15/18/**14**/30/36px across
  xs..xl - md smaller than sm, and lg/xl a big Material ball on a Fluent switch. Fluent has its own compact
  ramp now (2:1 track, small thumb, scaled monotonically around its 40x20 md): 10/12/14/17/20px, md
  unchanged. Visual Studio, which derives from Fluent, follows it and is monotonic too.
- **A field changed height between its filled and outlined variants.** Filled drew a bottom border only,
  outlined drew all four - and since the field's height is content-driven, the extra top+bottom border made
  an outlined field 1px taller (measured 52px vs 53px under Material). The border width is reserved on every
  variant now, so both are the same height (measured 53px/53px under Material, 47px/47px under Fluent); only
  the colour differs, so filled still shows a bottom bar and outlined a full box.
- **A data-grid filter editor looked outlined but kept the theme's own focus treatment.** `datagrid.css`
  copied four of the six declarations that `.flare-input-variant--outlined` makes and dropped the two focus
  ones, so under a filled theme a filter editor drew an outlined box with a bottom-bar focus. The filter row
  and the advanced builder wear the variant class now instead of copying it, so they get all six - measured,
  the focus ring on a filter editor matches a real outlined field. No visual change under an already-outlined
  theme; the copied box geometry is byte-for-byte the variant's.
- **A theme could style a normal menu but never a dense one.** `menuitem.css` hardcoded the dense item's
  block padding and gap, so `FlareMenu.Dense` was core's opinion outright. `MenuTokens` gains
  `ItemPaddingBlockDense` and `ItemGapDense`; measured after, dense still paints 6px/8px and the theme can
  now move it.
- **The layout's geometry was a core default, not the theme's.** `layout-shell.css` opened with a `:root`
  block setting `--flare-layout-drawer-width`, `-drawer-rail-width`, `-appbar-height`,
  `-appbar-height-dense` and `-appbar-bg` to literals. That is a default baked into core, which the token
  mandate forbids - and a `:root` declaration can out-rank the theme's own block, so the theme's value need
  never render.

  Two of the five had no route out of core at all: `-appbar-height-dense` and `-appbar-bg` had a name in the
  registry, no `LayoutTokens` member and nothing emitting them, so the core literal was their only source
  and **no theme could change them**. Both are proper `LayoutTokens` members now (`AppBarHeightDense`,
  `AppBarBg`), wired through `CssVarMap` and supplied by the reference packages. Values are unchanged.

### Added
- **A guard against the same thing coming back**: `DeadFallbackTests.CoreCss_DoesNotDeclareATokenTheThemeSupplies`.
  Its sibling already banned a core *fallback* on a theme token; this bans a core *declaration* of one, which
  is the more dangerous half - a fallback that never renders is dead code, but a declaration nearer the
  element than the theme's silently wins.

  It follows the mandate's own line: pointing a token at another semantic token is fine, a hardcoded `16px`
  is not. Its first run found more than the layout shell - `badge.css`, `switch.css` and `menuitem.css`
  hardcode geometry over the theme's, and `datagrid.css` re-declares what `.flare-input-variant--outlined`
  already says. Those are named in the test as known debt so the guard holds every other stylesheet clean
  while they come out one at a time.

## [0.8.0] - 2026-07-17

### Changed
- **BREAKING: file upload moved from `Flare.Components.Media` to `Flare.Components`.** Uploading a file is
  not a media concern - it takes any file - and it is wanted often enough that an extra package reference to
  reach it is friction. `FlareVideoPlayer` and `FlareSignaturePad` stay; they are genuinely media.

  Nothing changes in your markup: `Flare.Components.Media` already declared
  `RootNamespace=Flare.Components`, so the types were `Flare.Components.FlareFileUpload*` all along. Drop the
  `Flare.Components.Media` reference if upload was the only thing you used it for.

  The move paid for itself. CssAudit only covers `Flare.Components`, so `fileupload.css` had never been
  audited: it carried 11 literal token fallbacks, two of which gave the **same** token different values
  (`--flare-typescale-body-small-size` as `0.75rem` on one line, `0.875rem` on another). All stripped.
- **BREAKING: `FlareDropZone` is gone, folded into `FlareFileUploadZone`.** They were the same component
  written twice - same hidden input, same drag state, same default upload glyph, label and accept hint - in
  two different packages, and demoed side by side on the same Gallery page.

  `ChildContent` and `MaxFileSize` came across, so nothing DropZone did is lost. `/components/dropzone` is
  gone from the Gallery; the file upload page covers both forms.

  Migration: `<FlareDropZone OnFilesDropped="X" />` -> `<FlareFileUploadZone OnFilesChanged="X" />`. Two
  defaults change with it, both deliberately: `Multiple` is now `false` (the field's own default, and the
  HTML one) rather than `true`, and **`MaxFileSize` is unlimited** rather than 10MB. That 10MB cap silently
  discarded the user's file with no explanation; a cap now has to be asked for.

  The tokens follow the component: `DropzoneTokens` -> `FileUploadTokens`, and `--flare-dropzone-*` ->
  `--flare-file-upload-*`. Three knobs that were literals in the CSS are now tokens too -
  `--flare-file-upload-zone-min-height`, `--flare-file-upload-zone-radius` and
  `--flare-file-upload-file-icon-size`. Derived themes inherit all of it; only the two reference packages
  name the values.
- **BREAKING: `FlareFileUpload` is replaced by `FlareFileUploadZone` and `FlareFileUploadButton`**, and the
  `FileUploadVariant` enum is gone.

  One component could not be both. Its `Variant` already meant "drop zone or button", so it had no room for
  the `Variant` every other button takes - and `Size`/`Color` would have been visible but silently dead
  whenever the drop zone was chosen. A zone is a region that owns its footprint; a button is a control in a
  row, and a row has a size, a variant and a colour. They are now two components, sharing the input, the
  accept/multiple/limit rules and the file list through a common base.

  `FlareFileUploadButton` takes `FlareButton`'s own vocabulary - `Variant`, `Size`, `Color`, `OnColor`,
  `Shape`, `PressMorph`, `Typo`, `FullWidth`, `Loading`, `LoadingText`, `LoadingTemplate`, `LeadingIcon`,
  `TrailingIcon`, `ChildContent`, `AriaLabel` - under the same names, types and defaults. Measured: it now
  matches a `FlareButton` beside it at every size (32/40/48/56/64px for xs..xl); it used to be one fixed
  height that matched nothing.

  `ButtonText` is now `Text`. The old name is not kept as an alias: the tag has to be rewritten anyway
  (there is no `FlareFileUpload` left to migrate from), so a deprecated parameter would ship on a brand-new
  component's first day - the same reasoning that removed the `FlareSliderZone` shim in 0.7.0.

  It does not take `OnClick`: the click only opens the picker, while `OnFilesChanged` carries the files and
  arrives only once the user confirms. `Href`/`Target`/`Rel`/`Type` are likewise absent - it opens a picker,
  it does not navigate or submit.

  Migration: `<FlareFileUpload ... />` -> `<FlareFileUploadZone ... />`;
  `<FlareFileUpload Variant="FileUploadVariant.Button" ButtonText="Import" />` ->
  `<FlareFileUploadButton Text="Import" />`. `Accept`, `Multiple`, `Disabled`, `MaxFiles` and
  `OnFilesChanged` are unchanged on both. `DropText` stays on the zone; `ButtonText` is now `Text`.

### Fixed
- **`FlareFileUploadButton` sat a fraction lower than the buttons beside it.** Its label is an inline-level
  box, so inside the wrapping `<div>` it joined a line box and picked up that line's strut - half-leading
  and descender space from the wrapper's own font - leaving the wrapper taller than the button it holds and
  nudging the whole control down. It only surfaced where the button is short enough for the strut to matter,
  which is why it looked fine under Material (48px medium) and showed under Fluent (24px small). The wrapper
  is a flex container now, so the label is a flex item and there is no line box to inherit from. Measured
  across 16-96px button heights: the wrapper is exactly the button's height at every one.
- **`FlareFileUploadButton` had no hover, no press and no focus ring.** Its hidden file input was stretched
  over the whole trigger, so the pointer landed on the input and never on the label wearing the button
  classes - everything `.flare-btn` draws through `:hover` / `:active` was unreachable, and beside a real
  `FlareButton` it looked inert. The overlay was never needed here: `<label for>` opens the picker on its
  own. The input is now hidden out of the way, and keyboard focus (which still lands on it) paints the ring
  on the button the user sees. The drop zone keeps its overlay - that is what carries drag-and-drop.
- **The drop zone's hover was dead too.** `FlareDropZone` hung `:hover` on its root, an ancestor of the
  overlaying input, so hovering the input lit it. Folding it into `FlareFileUploadZone` moved that rule onto
  the label - the input's *sibling*, which the pointer never reaches. Hover hangs off the wrap again.
- **`.flare-btn` no longer borrows its look from being a `<button>`.** It leaned on the UA stylesheet for
  `text-align: center` and the block-padding reset, so the same classes on a `<label>` - which
  `FlareFileUploadButton` has to use - rendered left-aligned with different padding. Both are declared now.
- **`FlareClipboard.OnCopied` no longer waits for the confirmation animation.** It was raised after the
  two-second "copied" feedback, so a caller was told the copy had succeeded two seconds late - the callback
  was locked behind a purely decorative delay. It now fires as soon as the text is on the clipboard.

  Two neighbours went with it: a second click's confirmation is no longer cleared early by the first click's
  timer still counting down, and the feedback length is now `FeedbackDurationMs` rather than a hardcoded
  2000ms.
- **Icon sizes set by a theme now actually reach the icon.** A component that takes an icon as a fragment
  (`FlareButton.LeadingIcon`, `FlareDialog.Icon`, `FlareBottomNavItem.Icon`) wraps it in its own element and
  styled that wrapper's `font-size`. But `FlareIcon` sets `font-size` on itself, and a declaration on an
  element always beats what it would inherit - so the wrapper sized nothing and every such icon painted at
  the `title-large` baseline (22px) regardless of what the theme asked for.

  The size now travels as `--_flare-icon-size`, a custom property, which cascades **into** the icon instead
  of competing with it. Three sets of theme tokens that had never rendered come alive:
  `--flare-btn-icon-size-{xs..xl}`, `--flare-dialog-icon-size` and `--flare-bottom-nav-icon-size`.

  Visible change under the in-box themes: button icons ramp 20/20/24/32/40px with the button size (they were
  a flat 22px, and overflowed their own 20px box at `xs`), and the dialog and bottom-nav icons move 22px ->
  24px. All of those are the values the themes were already declaring. An icon with no host asking for a
  size still renders at the baseline, unchanged. Components that render their own raw icon glyph (menu item,
  tree, switch, stepper, timeline, dropzone, splitter) were never affected and are untouched.

  A theme can now size any icon from any ancestor by setting `--_flare-icon-size`, without a specificity war.

### Added
- **`InputTokens.IconSize`** (`--flare-input-icon-size`) - the size of a field's leading/trailing icons. It
  also drives the expand toggle that `FlareDatePicker`, `FlareDateTimePicker` and `FlareTimePicker` put in
  the trailing slot: that toggle is the same field affordance, so it reuses the field's token rather than
  inventing one. In-box values: 24px (Material Design 3), 20px (Fluent UI 2) - each taken from its own spec.

  Those three pickers previously hardcoded `font-size:1.25rem` inline in their markup, which no theme could
  reach; that was the last inline dimension left in any component's markup. Field icons move 22px -> 24px
  under Material Design 3, and the picker toggles 20px -> 24px.
- **`IFlareButtonAppearance`** - the look and interactive state every button-shaped control shares
  (`Variant`, `Size`, `Color`, `Disabled`, `Loading`). `IFlareButton` now extends it, adding only `OnClick`,
  so a control whose action is not a plain press can still speak the button vocabulary. `FlareClipboard` and
  `FlareFileUploadButton` take the appearance; `FlareButton`, `FlareIconButton` and `FlareSplitButton` take
  the full contract.

  `FlareIconButton` and `FlareSplitButton` already matched it exactly and simply never declared it - now
  they do, so they cannot drift. `FlareClipboard` gains the `Disabled` and `Loading` it was missing.
- **`ButtonCssClasses`** - the `ButtonVariant`/`ButtonSize`/`ButtonShape` -> `flare-btn--*` mapping, now
  shared. `FlareButton` reads it, and so does `FlareFileUploadButton`, which cannot nest a `FlareButton`
  (a `<button>` inside its `<label for>` would not open the file picker) and so wears the classes directly.

## [0.7.0] - 2026-07-17

### Changed
- **BREAKING: `FlareSliderZone` is removed.** It had been an `[Obsolete]` alias since 0.3.0 - a thin subclass
  of `FlareZone` kept so existing `<FlareSliderZone Start End Color />` markup would still compile while
  callers moved over. 0.6.0 then dropped `SliderSize` and `FlareProgress.Thickness` outright, so keeping one
  deprecation shim while breaking its neighbours only made the surface harder to reason about.

  Migration: replace `<FlareSliderZone .../>` with `<FlareZone .../>`. Identical parameters and behaviour -
  the alias never did anything of its own - and `FlareZone` additionally works inside `FlareProgress`.

### Added
- **`FlareDrawer.ContentPadding`** - the content region's horizontal inset, on the shared `FlareSpacing`
  scale (the same steps `FlareStack.Gap` uses, so a drawer and a stack asking for the same step get the same
  value from the theme). `ContentPaddingValue` takes a raw CSS length for `FlareSpacing.Custom`.

  The drawer's content is full-bleed while its header is inset, so anything placed in a drawer ran to the
  panel edge and a form did not line up with the header above it. Every caller writing a form into a drawer
  was overriding `.flare-drawer__content` - a Flare-internal class - to fix it.

  The inset is opt-in rather than the new default, because the drawer cannot know what it holds and the two
  cases want opposite things: measured in MD3, a nav menu's item highlight sits flush at the panel edge (0px)
  and would be pushed to 24px by a default inset, while a form field sits at 0px and wants the header's 24px.
  `ContentPadding="FlareSpacing.Large"` gives the form case exactly the header's inset; the default leaves
  every existing drawer where it is. Flare's own drawer demo had been faking it with a hand-written
  `padding:1rem` and now just asks for the step.

### Removed
- **43 dead strings from the Gallery's resources**, left behind by the rewritten Getting Started page and the
  removed About page. They were still shipping: a resx string is embedded in the assembly *and* in the
  Russian satellite, so they cost download size in a WASM app for nothing. Also dropped
  `ChangelogService.LatestVersion`, a property nothing read.

### Fixed
- **BREAKING (visual): the drawer's width is the theme's again - 360px, not 280px.** `FlareDrawer` wrote
  `width:280px` inline on every render from a C# default, and an inline style beats the stylesheet - so
  `--flare-drawer-width` could never render. The theme shipped the spec-exact MD3 nav-drawer width (360dp)
  and the component overrode it with a hardcoded literal at every size. `Width` now defaults to `null` and
  only goes inline when a caller sets one; the resting width comes from the theme.

  Migration: pass `Width="280px"` on any drawer that must keep its old size.
- **The split button's corner ramp was emitted and then thrown away.** The per-size seam tokens ramp
  4/4/4/8/12px - exactly what MD3 Expressive specifies - but the theme's own `split-button.css` pinned every
  seam corner to `shape-small` (8px) with `!important`, so five tokens painted nothing and only `lg` was
  right by luck. The override existed for a real reason: a 9999px pill radius sharing an edge with a small
  corner makes the browser scale *both* toward zero, so the seam rendered square while `getComputedStyle`
  still reported its px. That fix (a `calc(height/2)` outer end) stays; it just no longer takes the seam
  with it. Measured after the change: 4/4/4/8/12px across xs..xl.
- **`InputTokens.FocusBorder` / `FocusBorderBottom` are removed.** Nothing read them. The focus indicator has
  been `--flare-input-focus-ring` + `--flare-input-focus-outline` for some time; these two were left behind by
  that change, so five themes were filling in values that went nowhere and a theme author tuning focus through
  them got silence. **Breaking for a custom theme that sets them** - delete the two lines; the ring/outline
  pair already carries the behaviour.
- **A hover popover closed under the cursor the moment you reached its content.** Any interactive panel - a
  volume slider, a menu, a form - was unusable: crossing the `Offset` gap between anchor and panel started the
  `HideDelay` countdown, and arriving inside the panel did not stop it, so the popover vanished under the
  pointer. Raising `HideDelay` only postponed that.

  The generation counter is the only cancellation there is, and `HandleMouseEnter` returned on its
  `|| Open` guard *before* bumping it - so the one case that must cancel a pending hide, re-entering an open
  popover, was exactly the case that never did. The guard was right; its position was not. `HideDelay`'s own
  documentation calls it "a short grace period so brief gaps between anchor and panel do not close it", which
  is the behaviour it could not deliver.
- **The same mistake, mirrored: a pointer passing straight over an anchor still opened the popover.** With a
  hover `Delay` set, leaving during the delay could not cancel the pending open, because while that delay runs
  the popover is still closed and `HandleMouseLeave`'s `!Open` guard also sat before the generation bump. So
  `Delay` did not prevent a pass-through from opening the panel - it only delayed it, onto a pointer that was
  already gone.

  Both handlers now bump the generation first and guard afterwards; the two regressions are pinned by tests.
- **Core docs stated a design system's measurements, and were already wrong.** `SwitchSize.Md` was documented
  as the "52x32dp" switch - Material Design 3's number - while FluentUI2 draws it 40x20, so the doc lied under
  a shipped theme. `FabSize` promised a "40dp / 56dp / 96dp container" for a component that has no size token
  at all: a FAB is sized by the padding around its glyph. `FlareBadge`, `FlareCheckbox` and `FlarePaper` did
  the same, and `FlareSwitch.Size` additionally claimed the enum had two members when it has five.

  This is 0.6.0's token-doc fix applied where it was still missing - the components' own `[Parameter]` and
  enum docs. A size step is a *label*: each theme maps it onto its own tokens, so a doc that pins a number to
  it is unowned prose that goes stale. The docs now say what a step is for and leave the geometry to the
  theme.
- **A guard so it cannot come back** (`CoreDocSpecUnitTests`): no core doc may quote a `dp` measurement.
  `dp` is the exact signal - a design-spec unit that appears in no C# and no CSS, so it can only turn up in
  the core when someone is quoting a design system. That precision is what lets the guard stay out of the
  way of numbers the core legitimately owns: a debounce (`Default: 300ms`), or an example of a value the
  caller passes (`e.g. "48px"`). Theme packages are out of scope - quoting the spec they implement is what
  their comments are for.
- **The Gallery's slider size demo was labelled in MD3's dp**, which is wrong under four of the seven themes -
  FluentUI2 in particular draws one slider geometry at every step, so the demo renders five identical sliders
  there while the labels claim 16dp through 96dp. The labels now name the step, and the demo says why the
  same step looks different per theme.

- **The Gallery's API pages still described the pre-0.6.0 progress API** - `Size` as an `int` of pixels, plus
  a `Thickness` parameter that 0.6.0 had removed - and listed no `Size` on `FlareMeter`. The generated API
  registry had not been regenerated after that change, so the reference documented an API the release no
  longer had. (Gallery only: the registry drives the demo site's API pages, not any shipped package.)

## [0.6.0] - 2026-07-17

### Changed
- **BREAKING: `FlareSlider`, `FlareProgress` and `FlareMeter` now share one size scale, `TrackSize`.** The
  three are one family - a meter already read the progress track tokens - but sizing them was three different
  stories: the slider took a `SliderSize` enum, progress took an `int` of pixels, and a meter had no size at
  all. Progress's `int` was the only pixel-valued `Size` in the library (every other component uses an enum),
  and it meant two things at once: a diameter for the circular variant, while the linear thickness lived in a
  separate `Thickness` parameter.

  - **`SliderSize` -> `TrackSize`.** Same members (`Xs`..`Xl`), same behaviour; replace the type name.
  - **`FlareProgress.Size` is now `TrackSize`, not `int`.** One step drives both variants - linear thickness
    and circular diameter + stroke.
  - **`FlareProgress.Thickness` is removed.** It set the linear height in pixels; `Size` covers it, and the
    theme owns the values.
  - **`FlareMeter.Size` is new**, reading the progress ramp so a meter and a bar at the same step match by
    construction rather than by two themes agreeing.

  Migration: `Size="40"` -> `Size="TrackSize.Md"` (the old default), `Size="24"` -> `TrackSize.Xs`,
  `Thickness="8"` -> the nearest step. **Nothing moves at the default**: every theme's `Md` is exactly what it
  drew before, verified against the rendered values in MD3 and Fluent.

  Progress and meter default to `Md`, not `Xs` like the slider: their ramp runs both ways, because an inline
  spinner in a table row needs to go finer than the resting size as much as a hero bar needs to go heavier.
  Flare's own `FlareDataGrid` proved the point - it was asking for 28px and 36px spinners, below the 40px
  default, and a scale that only climbed would have had nothing to offer it.

  The steps are labels, not measurements: each theme maps them onto its own ramp. Only two steps of each ramp
  are anchored in a spec (MD3 names a 4dp bar with an 8dp "thick", and a 40dp ring with a 52dp "thick";
  Fluent names a 2px bar with a 4px "large", and eight spinner sizes). The rest is each theme's own
  interpolation, and the token comments say which is which.
- **Fixed as a side effect: the `CircularSize` theme token never rendered.** `FlareProgress` wrote
  `width:{Size}px;height:{Size}px` inline on every circular spinner, and inline style beats the stylesheet -
  so `--flare-progress-circular-size` was dead and the core's hardcoded `40` overrode whatever a theme asked
  for. It went unnoticed because both reference themes happen to say 40px. The geometry is now entirely the
  theme's, and a test asserts the component writes no inline width/height.
- **Token docs no longer state what a theme sets the token to.** Core token records documented their members
  by quoting a value - `/// <summary>Gap xs token (0.25rem).</summary>` - across `ButtonTokens`, `FabTokens`,
  `MenuTokens`, `SplitButtonTokens`, `ToggleButtonTokens`, `InputTokens` and `SpacingTokens`. The core owns no
  value, so such a doc is unowned prose that drifts the moment either side is edited, and it had drifted:
  **27 of the 84 quoting docs contradicted the shipping MD3 theme** (that one said `0.25rem` while the theme
  used `0.5rem`; `MenuTokens.ItemHeight` said `0` while the theme used `3rem`). The summaries also restated
  the member name instead of explaining it. Each now says what the token is *for* - "Space between the icon
  and the label at the xs size" - and the value is left to the themes, which are the only place it is true.
  Naming a *special* value semantically is still fine and still done ("a theme with flat filled buttons parks
  this at `none`"), because that describes the token's own contract rather than one theme's taste.
- **BREAKING: every in-box theme class is now named after its package.** Two broke the rule the other five
  followed, and both were the only thing standing between a reader and a guessable name:
  - `Md3Theme` -> **`MaterialDesign3ExpressiveTheme`** (package `Flare.Theme.MaterialDesign3Expressive`).
    It also read like the *base* MD3 theme, which sits right next to it as `MaterialDesign3Theme`.
  - `Fluent2Theme` -> **`FluentUI2Theme`** (package `Flare.Theme.FluentUI2`).

  Both classes already lived in files with the new names (`MaterialDesign3ExpressiveTheme.cs`,
  `FluentUI2Theme.cs`), so only the type names were out of step. With these two fixed, the rule now holds
  with no exceptions: `AeroTheme`, `FluentUI2Theme`, `LiquidGlassTheme`, `MaterialDesign2Theme`,
  `MaterialDesign3Theme`, `MaterialDesign3ExpressiveTheme`, `VisualStudioTheme`.

  Migration: replace `new Md3Theme()` with `new MaterialDesign3ExpressiveTheme()`, and `new Fluent2Theme()`
  with `new FluentUI2Theme()`. Nothing else changes - same ids (`md3-expressive`, `fluent2`), same tokens,
  same palettes. The ids are deliberately untouched, so a user's saved theme choice survives the upgrade.

  The `Md3Palettes` / `Fluent2Palettes` classes are **not** renamed. Palette class names have no convention
  today (`Md2Palettes` and `Md3Palettes` are short, `AeroPalettes` and `VisualStudioPalettes` follow the
  package), and settling that is a separate decision - not one to smuggle in under a theme rename.
- **`Microsoft.Extensions.Localization` bumped to 10.0.10**, matching the rest of the ASP.NET Core 10.0.10
  packages picked up in 0.5.0.

### Added
- **A guard against token docs that state a theme's value** (`TokenDocLiteralTests`), completing the set that
  keeps theme opinion out of the core: one guard already forbids a literal default on the token member, one
  forbids a theme default hiding in a CSS fallback, and this one covers the prose. It fails a `[CssVar]`
  member whose summary carries a CSS dimension (`0.25rem`, `2px`), or a trailing `(<c>value</c>).` claim -
  the second rule catches the digit-free quotes like `(<c>var(--flare-elevation-2)</c>)` that a literal
  check alone misses. Mid-sentence references stay legal, since naming the selector a token applies at is a
  pointer, not a claim. Pointed at the whole token model rather than the records that were known to be
  wrong, it found violations in `SpacingTokens`, `RadioTokens` and `ButtonGroupTokens` that the manual pass
  had missed; those are fixed too.
- **A guard against a length token spelled without its unit** (`LengthTokenUnitTests`), which is what made the
  MD2 slider below invisible. For each in-box theme it fails any token whose value is a bare number while some
  `calc()` adds or subtracts it from a length or a percentage.

  The requirement is *derived from the consuming expression*, not declared on the token. Annotating every
  `[CssVar]` with a CSS type would have been hundreds of edits that then rot as the CSS moves; the calc sites
  already state it exactly, and reading them keeps the guard honest for calc sites nobody has written yet.
  Two details decide whether it works at all: it scans component `.razor` as well as stylesheets (the slider
  builds its geometry inline in C#, so a CSS-only scan misses the very case that shipped), and it resolves the
  private `--_gap` alias back to the theme token (matching on the token's own name finds nothing).
  Multiplication (`calc(-1 * var(--x))`) is deliberately out of scope - see the test for why.

### Fixed
- **The Material Design 2 slider had no visible rail - at every size, in every release that shipped the
  theme.** Only the handle drew; the track and the fill were both absent. The cause was one missing unit:
  the theme set `SliderTokens.Gap = "0"` where every other theme writes `"0px"`. `FlareSlider` builds each
  rail segment with `calc(100% - <pct> + var(--flare-slider-gap))`, and inside a `calc()` a bare `0` is a
  `<number>`, not a `<length>` - the "zero needs no unit" leniency only applies to a literal written straight
  into a property. So the sum was `<percentage> + <number>`, invalid at computed-value time, and the browser
  dropped the whole declaration: `right` fell back to `auto` and an empty absolutely-positioned box
  shrink-to-fits to width 0.

  The value was present, non-empty and not parked, so it passed every guard 0.5.0 added - those ask whether a
  value was supplied, and this is the question after it: whether the value is the CSS *type* the declaration
  substitutes it into. The debugging trail actively misleads, too: `getComputedStyle` reports
  `right: 464px` on the collapsed segment, which reads like a deliberate position but is only the used value
  implied by `width: 0`, while the rail height and fill colour both measure correct. The
  `LengthTokenUnitTests` guard above now fails any theme that spells one of these without its unit.
- **The docs shipped a `Program.cs` that no longer compiles, straight to NuGet.** The repo README still
  opened with `new Md3Theme()`, and it is the packaged readme for the seven packages that have no readme of
  their own (`Flare.Abstractions`, `Flare.Theming`, `Flare.Infrastructure`, `Flare.Theme.MaterialDesign3`,
  `Flare.Theme.MaterialDesign2`, and the two token packages) - so the rename above would have landed on
  nuget.org next to a quick-start that fails to build. Fixed in both languages.
- **Component counts were wrong everywhere, in two directions.** `Flare.Components` advertised
  **67 components** on NuGet while its own readme next to it said 100+; the real figure from the project's
  own `ApiDocGen` is **131** in that package (159 across all Flare packages), so the headline number
  undersold the library by half. Everything now reads `130+`, consistently: both readmes, `index.md`, the
  `api/` index, the getting-started and ai-agents docs (EN + RU), the two package descriptions, and the
  Gallery's home stat. The `+` form is deliberate - an exact count is what rotted into `67`.
- **Docs pointed at `Flare.Core`, a package that does not exist** (it was retired in 0.1.0 when the rings
  were split). The Gallery's install snippet named it as the home of the abstractions, and the CssAudit
  readme and tool output pointed at `Flare.Core/CssClasses.cs`; the file lives in `Flare.Abstractions`.
- **The Gallery's API pages were missing every component added since 12 July** (`FlareMeter`,
  `FlareMeterSegment`, `FlareZone`), because the generated API registry had not been regenerated since.
- **`FlareMeter` under-filled its track whenever the segment values summed to less than 1.** The raw value
  went straight into `flex-grow`, but flex only distributes *all* the free space when the grow factors sum
  to at least 1 (CSS Flexbox 1, 7.1.1); below that each segment takes only its own fraction and the bar
  stops early - in correct proportions, which is what made it easy to miss. That is not an edge case for a
  component fed raw measurements: it is the whole sub-1 range of any unit (fractions of a millisecond,
  ratios, bytes under a byte). A real 0.3955 ms call filled 40% of its track. The factors are now scaled to
  a fixed total inside the component, so raw values stay declarable as-is and the browser still does the
  division - one factor per segment, no rounding gaps.
- **`FlareMeter` announced values that `ShowValues` was hiding, at full round-trip precision.** The
  `aria-label` was built from every segment's value unconditionally, so a meter that deliberately hid its
  numbers still read them out - and the `"G"` default format meant a 0.0627 ms slice was announced as
  `0.06269999999999998`. Values in the label now follow `ShowValues`, exactly as the legend and tooltip
  already did, and `Format` defaults to a bounded `0.##`. Segments are joined with `"; "` so a decimal-comma
  culture does not use the same character for both separators.

  Note for callers that relied on the old default: values shown with no explicit `Format` now render as
  `0.##` rather than `"G"` round-trip precision. Pass `Format` to choose your own.

## [0.5.0] - 2026-07-16

0.4.0 moved the slider / pagination / rating ramps into the themes and banned "parking" a token at
`initial`. That made an old assumption safe at last, so this release finishes the job: **the component CSS
now carries no default that belongs to a theme, and a test keeps it that way.**

### Fixed
- **89 dead fallbacks removed from the component CSS** (`button`, `togglebutton`, `fab`, `a11y`,
  `datagrid`, `timepicker`, `datepicker`, `splitter`, `splitbutton`, `input`, `breadcrumb`). Each sat on a
  token every theme is required to supply - `var(--flare-btn-gap-xs, 0.25rem)` - so it could never render:
  dead code that quietly kept a theme's opinion inside the core, and let a theme get away with not
  supplying a value. Rendering is unchanged (that is what "dead" means); verified by measuring every button,
  toggle, FAB and toggle-group size against the theme's own values.

  Notably the sweep is **not** "strip every `var(--flare-*, ...)`": 38 fallbacks were deliberately kept.
  A `--flare-*` var is not automatically a theme token - `--flare-col-span` on a grid cell,
  `--flare-slider-length` on a vertical slider and the `--flare-ide-*` pane sizes are set by the CONSUMER,
  never by a theme, so their fallback is the real default. Removing those would have broken the grid.

### Added
- **A guard for each half of the mandate**, so "no defaults in the core" is now enforced rather than
  intended:
  - a fallback on a token the theme supplies fails the build (`DeadFallbackTests`) - it keys off the
    theme-emitted name set, so consumer-set vars keep their defaults;
  - no theme may park a token at `initial` (`ParkedTokenFallbackTests`), now covering **all seven** in-box
    themes rather than the two reference token packages, with a completeness check that fails if a new theme
    is not added to it.

  The pair is what makes either rule safe: parking is what made "the fallback is dead" a false premise in
  0.2.0, and that premise shipped broken geometry for three releases.

### Changed (theme authors)
- **A custom theme may no longer rely on a component default.** With the fallbacks gone, a theme that parks
  a token at `initial` (or leaves it empty) now renders nothing for that property instead of quietly getting
  the core's value. Supply a real value for every token; if it is size-dependent, set the per-size members.
  In-box and derived themes are unaffected - this only bites a theme that was leaning on the core.

## [0.4.0] - 2026-07-16

Two corrections, both about a value living in the wrong place.

**The zone model** (from 0.3.0): the slider / progress / meter band was unified into one `FlareZone` whose
meaning depended on its parent - so half its parameters were dead in any given host, and a mismatched one
was silently dropped. The mechanism stays shared; the two kinds of band are now separate types, each
carrying only what applies to it.

**Component geometry** (broken since 0.2.0): the slider, pagination and rating lost their sizing under
Material Design 3 - the default theme - because a theme cannot express a per-size ramp through a single
token, so it punted the values into the component CSS, where a later cleanup removed them as "dead". Every
size ramp now lives in the theme, one token per size, and a guard keeps it that way.

### Changed
- **BREAKING: `FlareZone` and meter parts are now separate types.** A zone and a meter part are not the
  same thing: a zone is an absolute range on a scale the **host owns** (`FlareSlider`'s Min..Max,
  `FlareProgress`'s 0-100), while a meter part is a weight that helps **define** the scale. So:
  - `FlareZone` keeps `Start`/`End`/`Color` and is for `FlareSlider` / `FlareProgress`. Its `Value` and
    `Label` parameters are **gone**.
  - New **`FlareMeterSegment`** (`Value`/`Label`/`Color`) is what `FlareMeter` takes.

  Migration: inside a `FlareMeter`, replace `<FlareZone Value="12.4" Label="DB" />` with
  `<FlareMeterSegment Value="12.4" Label="DB" />`. Slider and progress markup is unchanged.
- **A mismatched band now fails loudly.** Putting the wrong kind inside a host previously rendered nothing
  at all, with no explanation; it now throws with a message naming the host and the type it expects.

### Added
- **`FlareZoneBase` / `IFlareZoneHost`**: both kinds derive from one base and register through one host
  contract, so all three hosts keep sharing a single registration path and band model - the 0.3.0
  consolidation is intact, only the author-facing surface split.

### Fixed
- **`FlareSlider`, `FlarePagination` and `FlareRating` lost their geometry under Material Design 3**
  (the default theme), since 0.2.0. The slider's visual rail collapsed to **0px** at every size, pagination
  buttons lost their fixed size and size ramp, and the rating star lost its size ramp.

  The root cause was a token-model gap, not a missing fallback. A single `:root` token cannot hold five
  per-size values, so these themes "parked" their geometry tokens at `initial` - meaning "I do not override
  this; use the component's own per-size default" - which pushed the ramp into the component CSS as literal
  fallbacks. That contradicts the token mandate (the theme supplies every value; the core carries no
  defaults), and it broke outright when the 0.2.0 pass stripped those fallbacks as "dead code": they look
  dead because every theme emits the token, but `initial` is the guaranteed-invalid value for a custom
  property, so the fallback was the live path. Without it the substitution yields nothing and the whole
  declaration is invalid at computed-value time.

  Fixed properly: size-dependent geometry is now **one token per size** - `--flare-slider-track-height-xs`
  ... `-xl` (likewise track radius, handle height, the flanking `--flare-slider-icon-size-*`,
  `--flare-rating-size-*` and `--flare-pagination-size-*`), the shape `FlareButton` already used for its
  per-size gaps and heights. The theme emits all five and the component's size class only selects which to
  read, so the ramp lives in the theme, the component CSS holds no geometry values at all, and a theme can
  now express a real ramp instead of one flat value. The vertical slider's default length moved to
  `--flare-slider-length` for the same reason - a consumer still overrides it per instance with an inline
  value, which wins over the theme's. Rendering is unchanged in every theme.
- **`FlareSlider` zones ignored the track's shape: squared-off ends, no separation, and no gap at the
  handle.** A zone was painted as a raw rectangle - no radius, no notch inset, and never cut by the handle.
  So a zone reaching the track end covered the rail's rounded corner (the track looked square wherever
  zones reached the edge - i.e. always, for a full-scale gauge), adjacent zones touched with no separation,
  and a zone under the handle painted straight through the notch, filling the very gap the rail leaves.
  A zone is a band on the same rail as the active/inactive segments, so it is now cut by the same notches
  and shaped by the same theme tokens: an edge on the track's outer end keeps the full track radius, every
  interior edge takes the gap radius and is inset by the notch gap, and a zone spanning the handle renders
  as two spans with the gap between them. Zone separation therefore matches the active/inactive split
  exactly (2x`--flare-slider-gap`) in themes that define a notch gap - Material Design 3 / Expressive - and
  stays flush where the gap is 0 (FluentUI2, Visual Studio). No per-theme CSS.
- **A guard now pins the mandate**: a new test fails when a reference theme parks any token at `initial`
  instead of supplying a value. Name-level auditing cannot see this - every name is present and in sync -
  which is why it shipped in three releases.

### Changed (theme authors)
- **`SliderTokens`, `RatingTokens` and `PaginationTokens` gained per-size members.**
  - Replaced: `SliderTokens.TrackHeight` / `TrackRadius` / `HandleHeight` by `TrackHeightXs..Xl` /
    `TrackRadiusXs..Xl` / `HandleHeightXs..Xl`; `RatingTokens.Size` and `PaginationTokens.Size` by
    `SizeXs..Xl`.
  - New required members on `SliderTokens`: `IconSizeXs..Xl` (the icons flanking the track) and `Length`
    (a vertical slider's default length) - both previously hardcoded in the component CSS.

  A theme that wants one value for every size sets the same value five times. Parking a token at `initial`
  is no longer supported - supply a real value; a guard test enforces it.

  Themes that derive from the in-box reference themes via `with` are unaffected unless they override these
  members. A theme that constructs `SliderTokens` / `RatingTokens` / `PaginationTokens` directly must set
  the new members (they are `required`, so the compiler points at every one).

## [0.3.0] - 2026-07-16

A consolidation release around one idea: **a colored band on a track is one concept**. The slider's zones
grow into a single `FlareZone` shared by the slider, the progress bar and a new segmented `FlareMeter`, so
the three no longer each reinvent the same band. Plus a new part-to-whole meter, and hardening that keeps a
theme's token mismatch from breaking a control.

### Added
- **`FlareMeter`**: a segmented part-to-whole bar - how a whole divides into proportional colored parts
  (a request-timing breakdown, a storage quota, a pass/fail/skip test ribbon). Non-interactive. Parts are
  `<FlareZone Value="..." Color="..." Label="..." />` children, sized in proportion to their sum, with an
  optional legend (`ShowLegend`, `ShowValues`, `Format`). Its track reuses the `FlareProgress` linear-track
  tokens (height, rounded ends, resting background), so a meter and a progress bar line up visually.
- **One `FlareZone` for the whole track family**: the slider-only `FlareSliderZone` is generalized into a
  single `FlareZone` that works in `FlareSlider`, `FlareProgress` and `FlareMeter`. The host decides how a
  zone is read: a **scale** host (slider / progress) takes an absolute `Start`/`End` range on its own scale,
  while a **proportional** host (meter) takes a `Value` weight. `Color` and `Label` are shared. The new
  `IFlareZoneHost` contract lets any component host zones with one shared registration path.
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
