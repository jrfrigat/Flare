# Mobile: verify and finish the small-screen story

**Status: OPEN - all six sweep items measured; twelve defect classes fixed, two demos remain.**
Everything in "Measured" was read off a real 375x812 viewport against the Release build, not inferred
from the CSS.

Flare has the machinery for this already - `IBrowserViewportService`, `Breakpoint`, `FlareMediaQuery`,
`FlareLayout`'s mobile bound, the `Hidden.BelowMd` utilities - and several components use it well
(`FlareDateTimePicker` subscribes to breakpoints; `FlareLayout` closes floating drawers on navigation).
What is missing is evidence that the whole library behaves on a phone, and three findings say it does not.

## The reference: how Material's own docs site does it

Measured on `m3.material.io/components/buttons/guidelines` at 1138px and at 375px.

| | Desktop | Mobile (375) |
| :-- | :-- | :-- |
| Primary nav | 88px icon rail, always visible, pushes the article | **removed from the DOM**; reached from a `menu` icon button in the top bar, which opens a modal drawer |
| Second level | sticky horizontal tab list across the top of the article (Overview / Specs / Guidelines / Accessibility) | the same list, full width, scrolled horizontally |
| Content | fills the space beside the rail | full bleed, `scrollWidth == innerWidth` - no horizontal page scroll |

Two things worth taking from it. **They never stack two vertical panels**: the second level is tabs over
the article, not a second column, so the narrow case has only ever one panel to place. And **the mobile
drawer is modal and transient** - it exists to be dismissed, so selecting from it closes it.

## Measured

### 1. The gallery's section drawer could not be dismissed on a phone - FIXED

The gallery stacks a 5.5rem rail and a 17rem section column; below the Md bound `FlareLayout` floats both
over the content instead. The section drawer was given `Open` one-way, with no `OpenChanged`, so every
close request the drawer raised - scrim tap, Escape, and `FlareLayout`'s own "navigating away closes any
floating drawer" - was raised into nothing, and the next render re-supplied `Open=true`. Measured before:
the panel sat at `x=0` covering a 375px viewport, and stayed at `x=0` after both a scrim click and Escape.

The library was doing everything right; the sample dropped the callback. Fixed by wiring `OpenChanged`,
and by not following the route into an open panel while mobile (Material's drawer closes on selection).
Verified after: direct load leaves the panel at `x=-272` with the content at full width; tapping the rail
opens it; picking a component navigates AND closes it; scrim and Escape both close it. Desktop is
byte-identical - rail 0-88, panel 88-360, content from 392.

**This is the shape of the whole issue: the responsive primitives work, and the places that consume them
have not all been checked.**

### 2. The DataGrid clipped its own columns - FIXED

At 375px, on `/components/datagrid`: `.flare-datagrid` measured **600px inside a 343px container**, with
`overflow-x: visible` on every ancestor (`scrollWidth` 624 vs `clientWidth` 343). The page did not scroll
horizontally, so the columns past 343px were not off to the side - they were **clipped and unreachable
by touch**.

The cause was one selector naming the wrong element. The small-screen rule read:

```css
@media (max-width: 599px) {
  .flare-datagrid__wrapper { overflow-x: auto; }
  .flare-datagrid       { min-width: 600px; }   /* the OUTER element */
}
```

`.flare-datagrid` is the component's outer flex column and `__wrapper` is the scroller inside it, so the
minimum made the whole component 600px wide - the scroller grew with it and had nothing left to scroll.
The comment above the rule described the right behaviour all along. The minimum now sits on
`.flare-datagrid__table`, as `max-content` rather than a fixed width, so a narrow grid still fits without
a scrollbar and a wide one scrolls by exactly what it needs. Measured after: grid 295px in its container,
wrapper 293 visible / 394 scrollable.

`FlareTable` was checked at the same time and is structurally fine - `.flare-table-container` already
scrolls, and a table that squashes rather than scrolls keeps its content reachable.

### 3. Touch targets below every published minimum - FIXED for the square controls

On the same page, **36 of 36** interactive controls under `#gallery-main` were shorter than 44px:
pagination buttons at 36x36, and so on. WCAG 2.5.8 (AA) sets the floor at 24x24, so this was not a
violation - but it is under both Material's and Apple's guidance.

`TouchTokens.TargetMin` (`--flare-touch-target-min`) is now a theme token, read only inside the core's
`@media (pointer: coarse)` block, where the square icon-sized controls take it as a minimum size.

**The control grows rather than a hit area drawn over it**, which is the opposite of the usual advice
and deliberate: each of these sits in a row of its own kind, so an expanded target overlaps its
neighbours and the later sibling silently wins the tap - a pagination bar of 36px buttons with a 4px gap
would have every button stealing 4px from the one beside it. The reflow this causes is confined to
devices whose PRIMARY pointer is coarse; a laptop with a touchscreen reports `fine` and is untouched.

**Still open here:** the dense inline affordances - a chip's close icon (1.1em) and a tab's close icon
(1.25rem) - are excluded, because their hosts have no room to give and an oversized target would cover
the chip body or the neighbouring chip. Making those tappable is a spacing and layout question.

## Measured 2026-08-30, at 375x812 against the running Gallery

### No horizontal page scroll - the first THREE attempts at measuring it were wrong

Thirty-six pages - every one the list below names as risky, plus the shells, changelog, API browser and
settings - navigated at 375px, each scrolled through its full height so the deferred demos mount, then
checked for `documentElement.scrollWidth > clientWidth`. **No page scrolls sideways.** Node counts per
page ran 41 to 2469, which is the part that makes the result mean anything.

Two earlier passes produced the same clean answer and were both worthless, which is worth writing down
because the next person will reach for the same shortcuts:

- **Pass 1 navigated without scrolling.** The Gallery defers demo mounting on an IntersectionObserver, so
  every page measured was a heading and a paragraph. A clean sweep of empty pages.
- **Pass 2 patched IntersectionObserver to report everything visible.** That is the documented trick for
  this Gallery, and here it took the application down - the same incomplete fake records that break
  Blazor's `Virtualize`. `body` held ten nodes and the sweep reported 131 clean routes.

The measurement that counts scrolls the container in viewport-sized steps and asserts the node count
alongside the width. A sweep that cannot say how much DOM it measured is not evidence.

### The mobile keyboard - ONE REAL DEFECT, FIXED

`FlareMaskedField` rendered `type="text"` with no `inputmode`, and every one of its presets is digits:
Phone, Date, Time, IpAddress, CreditCard, Ssn. On a phone that is a full QWERTY keyboard for entering a
credit-card number. It now derives the hint from the MASK rather than the preset, so a custom all-`#`
mask gets it too, a mask with letter placeholders correctly keeps the full keyboard, and Phone gets
`tel` rather than `numeric` because the tel keypad carries +, * and #.

`FlareNumericField`, `FlareOtpField`, `FlareTimePicker` and `FlareTimeSpanPicker` were already correct.

### Touch targets on the dense chrome - EIGHT DEFECTS, FIXED

The earlier pass fixed the square controls and left "the dense inline affordances" open. Measured at
375px with a coarse pointer emulated, every interactive element under 44px - the lowest of the three
published minimums (Apple 44, Google 48, WCAG 2.2 AAA 44):

| Target | Was | Why it matters |
| :-- | :-- | :-- |
| `flare-chip__close` | **14px** | Deleting a chip |
| `flare-tabs__tab-close` | 20px | Closing a tab |
| `flare-input__arrow` | 22px | Opening a select |
| `flare-layout-appbar__toggle` | 36px | **The drawer toggle - primary navigation** |
| `flare-tabs__scroll` | 36px | Reaching an overflowed tab |
| `flare-picker__day` | 37px | Picking a date |
| `flare-btn` | 40px | Every small button |

Two techniques, because they are two different problems. Controls that ARE the target and have room -
the drawer toggle, the scroll arrows, a calendar day, a small button - grow to
`--flare-touch-target-min`. Icons inside dense chrome cannot: growing a chip's delete to 48px grows the
chip with it. Those get a centred `::after` that takes the touch and leaves the layout alone, so the
chip still measures 14px and its target measures 48.

Verified after the fix: drawer toggle 48, scroll arrows 48, calendar day 48, chip close 14px element
inside a 48px hit area.

### Gestures that a finger cannot perform - ONE FIXED, THE REST FILED

**Column resize was mouse-only.** `FlareDataGrid.initResize` listened for `mousedown`/`mousemove`, which
a touch drag never fires, and the handle was additionally hidden behind `:hover` - invisible AND inert.
It now runs on the shared `startDrag` pointer primitive that already backs the splitter, the resizable
container and the dialog move, and the handle stays visible where there is no hover to reveal it with.

The rest is a bigger finding and has its own file: **[the drag model](drag-model.md)**. Four components
implement drag-and-drop independently and three of them do not work on touch at all - tree reorder and
both DataGrid reorders are desktop-only, because native HTML5 drag-and-drop does not fire on touch.
`FlareTreeItem`'s drag handle is deliberately left hidden on coarse pointers until that lands: showing
it would advertise a gesture that cannot be performed.

### Overlays - MEASURED, no defects found

Dialog, menu and the time picker opened at 375px: each fits horizontally and vertically, each has either
a scrim or an action row of 56px controls, and none traps the reader. The listbox family could not be
driven from the sweep (its triggers mount lazily and the automation could not reach them reliably), so
**select, multi-select, autocomplete and the date picker panel remain unverified** rather than passing.

### The measurement itself was wrong - CORRECTED, and it hid thirteen defects

The sweep above read `documentElement.scrollWidth`. **The Gallery's document never scrolls.** Its scroll
container is `.flare-layout-content`, and every page that overflowed sideways did so inside that element,
where a document-level check cannot see it. A clean answer from the wrong element is not a clean page.

Re-run against `.flare-layout-content` over all 133 routes, scrolling each page in viewport-sized steps
so the deferred demos mount, and attributing the overflow to the widest element that is NOT inside some
inner scroller (a DataGrid wrapper scrolling its own columns is correct and must not be reported):

| Route | Overflowed by | What | Verdict |
| :-- | --: | :-- | :-- |
| `/components/button-group` | **281px** | `flare-btn-group` | library |
| `/components/cards` | 209px | a demo card at `min-width:34rem` | demo |
| `/components/date-range-picker` | 199px | two date fields side by side | library |
| `/components/buttons` | 109px | a row of five XL buttons | demo |
| `/components/fab-menu` | 91px | `flare-fab-menu__list--right` | demo, open |
| `/components/otp-field` | 73px | six OTP cells that never shrank | library |
| `/components/on-this-page` | 71px | a `1fr 14rem` demo grid | demo |
| `/components/charts` | 49px | the visually-hidden data table | library |
| `/components/tabs` | 42px | the tab bar's end zone | library |
| `/components/date-picker` | 39px | the calendar's own day cells | library, self-inflicted |
| `/components/toggle-button` | 38px | `flare-togglegroup` | library |
| `/components/bottomnav`, `/mobile-shells` | 25px | 360px phone-shell mocks | demo |
| `/components/tooltip` | 13px | a tooltip at the right edge | library, open |

Eleven of the thirteen are fixed and re-measured at 0. What they had in common is worth stating once,
because it is one mistake in several places: **a component that sizes to its content and is never told it
cannot exceed its container.** `.flare-input` (and every field root), `.flare-tabs`, `.flare-toc`,
`.flare-btn-group` and `.flare-togglegroup` now carry `max-inline-size: 100%`, and the flex items inside
them carry `min-inline-size: 0` so they can actually shrink once the cap bites. Where the content genuinely
cannot shrink, the model decides the answer: a standard button group **wraps**; a connected group and a
toggle group **scroll inside their own frame**, because a segmented control that wraps grows rounded
corners in the middle of the run.

Three were their own thing:

- **The calendar was my own doing.** The touch-target rule from the previous pass gave `.flare-picker__day`
  a 48px minimum. The day sits in a `repeat(7, 1fr)` track whose automatic minimum is the cell's, so seven
  columns demanded 336px plus gaps plus the week-number column - 360px of tracks inside a 295px panel, with
  the last column pushed outside the calendar. The rule is withdrawn and the reasoning recorded in
  `a11y.css`: the day can only reach the minimum if the panel grows with it, which is a responsive-panel
  change and not a minimum-size one. **A fix that makes a component overflow its own frame is not a fix.**
- **The chart's screen-reader table was 368px wide.** It carries the standard visually-hidden recipe -
  `position:absolute; width:1px; clip:rect(0,0,0,0)` - and that recipe fails twice on a `<table>`: automatic
  table layout treats `width` as a minimum, and a table is at least as wide as its `<caption>`. An
  absolutely positioned box still counts toward its container's scrollable area, so an element nobody can
  see gave the page 49px of horizontal scroll. Now `table-layout: fixed`, pinned to the origin, caption
  constrained. `.flare-visually-hidden` is pinned to the origin for the same reason.
- **The tab bar had a scroller it never used.** `.flare-tabs__bar` is `flex: 1 1 auto; overflow-x: auto`,
  but a flex item will not shrink below its content without `min-inline-size: 0`, so the bar kept its full
  tab run and pushed the end zone off the screen instead of scrolling. The header zones were `flex: 0 0 auto`
  and could not give either.

Still open, both demo-shaped: the FAB menu demo shows all four opening directions at once and the
right-opening one leaves a 375px screen by construction (a real app would not place it there), and a
tooltip anchored at the right edge overhangs by 13px, which is the collision engine shifting late.

### The Gallery's own chrome did not fit - TWO DEFECTS, ONE OF THEM A LIBRARY BUG

The app bar measured **519px of content in a 375px bar**: the search field was clipped and the GitHub link
sat entirely off-screen, unreachable. The search already declared `Class="@Css.Classes.Hidden.BelowSm"` and
the utility already had its media query - the class simply never reached the DOM.

**`FlareCombobox` did not forward `Class` to its root, and neither did the other twelve fields.** Every
field renders `FlareFieldChrome` as its root, and each one passed `Style` and the splatted attributes and
dropped `Class`. The parameter exists, compiles, appears in the API reference, and does nothing - on
`FlareField`, `FlareTextArea`, `FlareSelect`, `FlareMultiSelect`, `FlareCombobox`, `FlareTagField`,
`FlareNumericField`, `FlareMaskedField`, `FlareOtpField`, `FlareDatePicker`, `FlareDateTimePicker`,
`FlareTimePicker` and `FlareTimeSpanPicker`. Any consumer styling a field through a utility class got
nothing back. Fixed in all thirteen, with `FieldChromeForwardingTests` failing when a field drops any of
the three parameters that address its root.

Demo rows were the other half: `<FlareStack Row>` does not wrap, and 66 demo rows used it. They wrap now -
invisible on a desktop, and the difference between a usable and an unusable demo on a phone.

### Hover-only affordances - ONE DEFECT, the rest are feedback rather than affordances

Every `:hover` rule in the library that raises opacity or reveals an element, sorted into the two kinds.
Nearly all are **state layers** - a `::before` painting `--flare-state-hover-layer` - which are hover
feedback and need no touch equivalent. The genuinely hidden controls are four, and three of them fade
between a token opacity and 1 (chip close 0.7, snackbar close and tab close from their own tokens), so they
are visible without a pointer.

The fourth was invisible: **`.flare-doc-tabs__close` in the IDE package sat at `opacity: 0`** and appeared
only on hover, while still taking clicks - a close button a touch user cannot find and can hit by accident.
It is now hidden only inside `@media (hover: hover)`, and given a 48px hit area on coarse pointers.
`.flare-tree-item__drag-handle` stays hidden on touch deliberately: it advertises a gesture that
[the drag model](drag-model.md) has not made possible yet.

## What the audit still has to cover

Per component, at 375x812 and 768x1024, in at least MD3 Expressive and Fluent UI 2:

- **No horizontal page scroll.** `documentElement.scrollWidth === clientWidth` on every gallery page.
  Anything wider than the viewport scrolls inside its own frame or reflows - it never clips.
- **Overlays.** Dialog, message box, prompt, menu, popover, select/autocomplete listbox, date and time
  pickers: does each become full-screen or a bottom sheet where it should, and can each be dismissed by
  touch alone? The collision engine's flip/shift behaviour needs checking against the on-screen keyboard,
  which shrinks the visual viewport without a resize event.
- **Hover-only affordances.** Anything that only appears on `:hover` - row actions, close buttons that
  fade in, tooltips - has no touch equivalent. Enumerate them and decide per case (always visible on
  coarse pointers, long-press, or an explicit control).
- **Wide content.** DataGrid, table, tabs, stepper, breadcrumb, toolbar, kanban, code blocks, charts.
- **Text input.** Correct `inputmode`/`enterkeyhint` on the numeric, OTP, phone and search fields so the
  right on-screen keyboard appears.
- **The FAB menu and the tooltip**, the two remaining overflows above.

## Done-when

Every gallery page passes the no-horizontal-scroll check at 375px in both themes; every overlay can be
opened and dismissed with touch alone; the DataGrid/table decision is made and implemented; the touch
target answer is a token, not a literal; and a guard exists for whichever of these can be asserted
without a browser.
