# Mobile: verify and finish the small-screen story

**Status: OPEN - two of the six sweep items are now measured and clean; four remain.**
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

### No horizontal page scroll - CLEAN, and the first two attempts at measuring it were not

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
- **The gallery itself** is the harness, so its own layout has to be right first: the two-drawer model
  above, the "On this page" rail (already hidden below Md), demo code blocks, and the settings page.

## Done-when

Every gallery page passes the no-horizontal-scroll check at 375px in both themes; every overlay can be
opened and dismissed with touch alone; the DataGrid/table decision is made and implemented; the touch
target answer is a token, not a literal; and a guard exists for whichever of these can be asserted
without a browser.
