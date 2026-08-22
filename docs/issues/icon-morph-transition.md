# FlareIconView: animate the transition when the icon value changes

**Status: CLOSED. Everything below shipped.** The cross-fade mechanism and its four tokens, the scope
that turns it on library-wide, and `FlareMorphIcon` - real outline interpolation for authored pairs -
are all in. The one thing deliberately NOT built is the View Transitions mode, and the reason is at the
bottom: it is a decision, not a gap.

Today `FlareIconView` swaps its glyph with no transition at all. `Value` changes, Blazor diffs the
`<svg>` in place, the new path data replaces the old one on the same DOM node, and the change lands on
a single frame. For a state-carrying icon - play/pause, menu/close, visibility on/off, the check that
appears when a copy succeeds - that instant swap is the one place Flare reads cheaper than the design
languages it targets. Material's own icon guidance treats a state change as a *transition*, not a
repaint.

This issue records what "morphing" can actually mean in a browser, which of those meanings are
reachable without betraying the conventions (theme-agnostic tokens, minimum JS, no per-render cost when
the feature is off), and what was built.

---

## What "morph" can mean, and what each option costs

### A. True path interpolation (`d` interpolation) - REJECTED

Interpolating one `d` string into another is the literal reading of "morph", and it is the one that
does not work for an icon *set*.

SVG path interpolation - whether driven by the CSS `d` property, by SMIL `<animate attributeName="d">`,
or by Web Animations - requires the two paths to be **structurally identical**: the same list of
commands in the same order, differing only in coordinates. When they are not, the UA does not
interpolate; it swaps discretely at the halfway point, which is the instant swap we started with plus a
delay.

Flare's catalog is derived from Material Symbols. `FlareIcons.Home` is one `M ... v ... h ... z` outline;
`FlareIcons.Menu` is three bars. There is no coordinate-only relationship between arbitrary pairs, and
there cannot be, because the caller picks both ends of the swap at runtime. Browser support for the CSS
`d` property is also uneven, but that is the second reason, not the first - even with universal support
this approach animates only hand-authored pairs.

There is a real, narrower version of this, and it is what `FlareMorphIcon` became (phase 3 below): a
**pair authored to interpolate**, drawn against each other so their command lists match. That is an
icon-artwork feature rather than a transition feature - it does not generalize to the catalog, which is
why it is a separate icon type instead of a mode of the mechanism chosen here.

### B. Runtime path normalization (flubber / polymorph in JS) - REJECTED

Libraries exist that normalize two arbitrary paths to a common command list and then interpolate. They
work, and the result is the "liquid" morph people picture. The cost is a JS dependency doing real
geometry work on the UI thread for every swap, plus a per-frame `d` rewrite that defeats compositing -
against both the minimum-JS convention and the performance mandate, for an effect most callers would
use on a two-state toggle where the crossfade reads the same.

### C. View Transitions API - REJECTED as the default, revisit as an opt-in

`document.startViewTransition()` would give a compositor-driven crossfade with no extra DOM. Three
things disqualify it as the default mechanism: it needs a JS interop hop on every value change; the
transition is **document-scoped**, so two icons changing in the same frame contend for one transition
and the second call is dropped; and each participating element needs a unique `view-transition-name`,
which means generating and tracking one per component instance. Worth revisiting as an opt-in mode once
the API's scoped form is broadly available - it is strictly better *when* it applies.

### D. Cross-fade / scale / rotate between the outgoing and incoming glyph - CHOSEN

Render the old icon and the new icon in the same box, animate one out and the other in. This is what
Material's icon-transition guidance actually specifies, it is what a designer means nine times out of
ten by "morph", it works for *any* pair of icons from *any* provider (inline SVG, icon font, a
third-party pack), and it is pure CSS: two keyframe animations on a keyed element, zero JS, zero
interop, compositor-only properties (`opacity`, `transform`).

---

## The mechanism as built (phase 1)

**Off by default.** With no mode set here and no scope above (phase 2), `FlareIconView` renders exactly
what it rendered before this issue - the icon element with no wrapper, no extra state, no change
tracking. The morph machinery costs nothing until it is asked for. This matters because `FlareIconView`
is not a leaf used once per page; it is the rendering path behind a large part of the library's chrome.

**When a mode is in effect**, the view renders a wrapper:

```
<span class="flare-icon-morph flare-icon-morph--scale">
  <span class="flare-icon-morph__slot flare-icon-morph__slot--exit">  <-- outgoing, retained
    <svg class="flare-icon ..."/>
  </span>
  <span class="flare-icon-morph__slot flare-icon-morph__slot--enter"> <-- incoming
    <svg class="flare-icon ..."/>
  </span>
</span>
```

Both slots sit in the same `inline-grid` cell (`grid-area: 1 / 1`), so the wrapper is the size of one
glyph and nothing reflows during the swap.

**Why a wrapper span per slot rather than animating the icon element itself.** Two reasons, and the
second is the one that decides it. First, `FlareIcon.Render()` is the provider's markup - a `<svg>` for
`FlareSvgIcon`, a `<span>` for a Symbols pack - so the view cannot rely on the shape of what it wraps.
Second, `@key` is a Razor construct that applies to an element in markup, not to a `RenderFragment`;
without a keyed host element Blazor's sequence-based diff would **reuse the existing DOM node** and
patch the path data into it, which is precisely the instant swap. The keyed slot forces node
replacement, and node insertion is what makes a CSS animation fire on the incoming glyph without any
imperative trigger.

**Change detection is record equality.** `FlareIcon` is a record, so `FlareSvgIcon { Data = X }` equals
another instance with the same data. Comparing the incoming `Value` against the retained previous one
therefore tests *content*, not identity: re-passing the same icon from a re-render does not start an
animation, and two independently constructed but identical icons correctly count as no change.

**There is no cleanup step, and that is the part worth reading.** The obvious implementation removes the
outgoing slot when its exit animation reports done, which means an `animationend` handler. Blazor has no
built-in binding for that event - it is not in the framework's `EventHandler` set - so it would take a
custom `[EventHandler]` registration (assembly-wide, and a name collision with any other library that
registers the same event), or a JS `registerCustomEventType` call to get the animation name, plus a timer
fallback because `animationend` never fires inside a `display: none` subtree.

None of that is necessary. `animation-fill-mode: both` parks the outgoing glyph at the *end* of its exit
animation - fully transparent - so leaving it in the DOM costs one invisible element that occupies the
same grid cell it already occupied and answers no pointer. The **next** swap recycles it: the keyed
current slot becomes the outgoing slot, its class flips from `--enter` to `--exit`, the animation name
changes and the exit runs again. The teardown a cross-fade normally needs is exactly the work the next
render already does, so the component holds no timer, no callback, no JS and no post-animation state, and
the slot count is bounded at two forever.

The one thing this does owe: the parked ghost is `aria-hidden` in the markup, because an icon carrying an
`AriaLabel` would otherwise be announced twice - transparent is not hidden to a screen reader.

**Reduced motion** zeroes the duration rather than removing the animation. `animation: none` would
un-park the ghost and paint it over its own replacement, which is the failure the fill mode exists to
prevent.

### Two things that were wrong first, and are worth not repeating

**The two slots must come from ONE source element.** The first version rendered the outgoing slot from an
`@if` branch and the incoming one from a separate element below it. Keyed diffing matched the nodes
correctly, but Blazor diffs *attributes* by their sequence number, and two source elements carry two sets
of them: the retained node was handed the other branch's attributes, ending up with `aria-hidden="true"`
and **no `class` attribute at all**. No exit class means no exit animation, and `animation-fill-mode`
never parks anything - so the outgoing glyph sat at full opacity on top of its replacement. Rendering
both slots from a single element inside a two-iteration `for` fixes it, because one source element has
one sequence.

**bUnit could not see that.** All twelve unit tests passed against the broken version: bUnit renders its
markup from the render tree, where the class was always present, so the bug lived entirely in the
DOM-patching step the browser performs. It was found by reading `outerHTML` off the running Gallery. A
component whose whole behaviour is "the DOM node is replaced rather than patched" has to be verified in a
browser; a render-tree assertion is checking the wrong layer.

**A spring easing must not drive opacity.** With one animation per slot, the morph curve timed the fade
as well as the movement - and the theme's spring reaches an eased fraction of 1 about a third of the way
in. Measured at 150ms of a 300ms swap: the outgoing glyph was already fully transparent while its
rotation was still mid-flight. The swap read as a pop followed by a spin. The fade and the transform are
separate animations now, of the same length, on different curves.

### The tokens

Four, all `required` on a new `IconTokens` record, none with a value in core:

| token | what the theme decides |
| :-- | :-- |
| `--flare-icon-morph-duration` | how long the swap takes; a theme parks it at `0s` to make icon swaps instant everywhere |
| `--flare-icon-morph-easing` | the curve of the MOVEMENT; a spring here is what makes `Scale` read as Expressive rather than as a fade. The cross-fade underneath rides `--flare-motion-easing-standard` instead, for the reason above |
| `--flare-icon-morph-scale` | how far the glyph is scaled at the far end of the `Scale` mode |
| `--flare-icon-morph-rotate` | the angle the glyph travels through in `Rotate` mode |

The last two are geometry, and the mandate is why they are tokens rather than numbers in the keyframes:
a `0.6` scale factor baked into `icon.css` would be core asserting how expressive the library is, which
is a theme's call. A theme that wants `Scale` and `Rotate` to be plain crossfades sets them to `1` and
`0deg` and never touches the modes.

`Fade` deliberately reads none of the geometry tokens - it is the mode that is only opacity.

---

## Phase 2 - DONE

**1. The premise was wrong, and measuring it was the whole fix.** This said "roughly a dozen components
call `icon.Render()` themselves rather than going through `FlareIconView`". They do not:
`grep -rn "\.Render()" src` returns three hits, two of which are inside `FlareIconView` and the third a
comment. All 95 icon call sites across `src` already go through the view, so every one of them inherits
the transition the moment a scope turns it on. Nothing to route, nothing to measure.

What DOES sit outside the system is a different backlog: the handful of components that emit a raw
`<span class="material-symbols-rounded">` instead of a `FlareIcon` at all (the DataGrid's sort arrow and
bool cell, the Carousel's chevrons, the RichTextEditor's toolbar). Those belong to the FlareIcon-system
de-hardcoding work, not here - they have no descriptor to transition between.

**2 and 3 turned out to be one thing.** The plan treated "state-driven icons inside Flare" as a
per-component decision - pick a mode for the expander chevron, another for the checkbox tick - and the
cascade as a separate convenience. That was backwards. Baking a mode into each component would assert
Flare's taste in exactly the place the token mandate says it must not, and it would leave an app that
wants no motion at all unable to say so. The scope is therefore the *only* mechanism, and internal
chrome gets its transition the same way an app's own icons do:

```razor
<FlareThemeProvider IconMorph="FlareIconMorph.Scale">   @* the whole library, chrome included *@
<CascadingValue TValue="FlareIconMorph?" Value="FlareIconMorph.Rotate">  @* or one region *@
```

`Morph` on the view became `FlareIconMorph?`: unset inherits the scope, and any explicit value wins -
including `None`, so a single call site can opt out of a scope that is on. Default behaviour is
unchanged: no scope, no transition, no wrapper.

The scope is a cascading parameter on `FlareIconView` rather than a field on the theme snapshot, and
that split is deliberate. The theme already owns how a swap MOVES (`--flare-icon-morph-*`); whether
icons transition at all is a statement about the app's character, and an app must be able to make it
without forking a theme.

## Phase 3 - `FlareMorphIcon` DONE, View Transitions REJECTED

**`FlareMorphIcon` ships**, and it is option A from the top of this file in the narrow form where that
option actually works: one `<path>` element that stays in the document while its geometry is
interpolated. The design landed simpler than this file predicted in two ways.

It holds **one** path, not a pair. A descriptor carrying `From` and `To` would have to be told which end
it is on, which is a second piece of state saying what `Value` already says; the caller flips between two
descriptors exactly as they do with any other icon, and the transition happens because the element is
retained rather than replaced.

And there is **no fallback branch**. The path is emitted twice - as the `d` attribute and as the CSS `d`
property - so a browser that implements the property transitions it and one that does not draws the
attribute and lands the change in a frame. No `@supports`, no feature detection, no JS. Verified in the
Gallery: mid-transition the computed `d` reads `M 11 10.3393 …`, an interpolated point between the plus's
`11 5` and the minus's `11 11`, so the outline genuinely flows.

`FlareIconView` recognises the type and stands its cross-fade down - even with a mode on - because
cross-fading would replace the very element whose geometry is being interpolated. The rule is stated on
the type rather than tracked in state: *a morph icon is never cross-faded*.

`FlareMorphIcons` holds the built-in matched pairs (`Plus`/`Minus`, `ChevronDown`/`ChevronUp`), drawn
with absolute line segments only and padded with degenerate segments so both sides of a pair share one
command list. A guard test compares the command lists, because a mismatched pair does not fail loudly -
it just swaps discretely half way through, which reads as a stutter rather than a bug.

**View Transitions stays rejected.** Nothing about the API changed while this was built, and the reason
from the top of the file is a correctness problem rather than a polish one: the transition is
document-scoped, so two icons changing in the same frame contend and the second call is dropped. An icon
transition that works only when a single icon changes at a time is not a mode worth offering. If scoped
view transitions become broadly available, this is worth revisiting as an additive mode - it would not
change anything built here.
