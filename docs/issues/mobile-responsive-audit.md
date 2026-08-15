# Mobile: verify and finish the small-screen story

**Status: OPEN, opened 2026-08-15.** One instance found and fixed (the gallery's section drawer, below);
the audit behind it has not been done. Everything in "Measured" was read off a real 375x812 viewport
against the Release build, not inferred from the CSS.

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

### 2. The DataGrid overflows its container with no scroller - OPEN

At 375px, on `/components/datagrid`: `.flare-datagrid` measures **600px inside a 343px container**, and
every ancestor up to the layout has `overflow-x: visible` (`scrollWidth` 624 vs `clientWidth` 343). The
page itself does not scroll horizontally, so the columns past 343px are not merely off to the side - they
are **clipped and unreachable by touch**.

A wide grid on a narrow screen needs a decision, not a media query: horizontal scroll inside the grid's
own frame is the cheap answer; column priority (drop low-priority columns first) or a stacked card
rendering per row is the good one. `FlareTable` needs the same call and should get the same answer.

### 3. Touch targets are below the 48dp Material asks for - OPEN

On the same page, **36 of 36** interactive controls under `#gallery-main` are shorter than 44px:
pagination buttons at 36x36, and so on down the page. WCAG 2.5.8 (AA) sets the floor at 24x24, so this
is not a violation - but Material's own guidance is 48x48, iOS asks for 44x44, and 36px is under both.

This is a token question rather than a CSS one: the min target is a per-size ramp, and the sizes are
chosen for a mouse. Options are a coarse-pointer bump (`@media (pointer: coarse)`) applied through the
size tokens, or a documented "use the larger size on touch" rule. It must not become a core literal.

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
