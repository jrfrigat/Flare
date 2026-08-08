# FlareProgress: the wavy RING does not flow (the linear bar does)

**Status: CLOSED in 0.14.0.** The crests travel and the arc stays where the value put it.

## What was wrong

The ring's flow was attempted with a CSS keyframe on the indicator:

```css
from { rotate: 0deg;   stroke-dashoffset: 0;   }
to   { rotate: 360deg; stroke-dashoffset: 100; }
```

The idea was to rotate the wavy path a full turn while sliding the dash window by the same amount, so
the two cancel and the visible arc stays put while the crests travel through it.

The idea was right; the dash it was applied to was not. The indicator's dash array was **a single
window**, not a repeating pattern:

```
stroke-dasharray = "0 <lead> <len> 100"      (pathLength=100)
```

Its period is `lead + len + 100`, which shares no common measure with the path, so sliding the offset
walked the window onto the trailing 100-unit gap: the visible arc changed length and position every
frame and periodically collapsed to a fragment.

Two smaller flaws sat underneath. `transform-box: fill-box` puts the rotation origin at the bounding
box centre of the wavy path, which is not the ring's centre. And the sweep cannot be written as
`from { stroke-dashoffset: var(--lead) }` / `to { calc(var(--lead) + 100) }` - those keyframes parse,
and read back correctly from `getKeyframes()`, but do not interpolate: the computed offset snaps to
the `to` value for the whole cycle.

## How it was fixed

- **Dash period = path length.** Two values summing to exactly 100, so the pattern repeats once per
  lap and an offset sweep of 100 lands back on itself. The window's start moved out of the array and
  into the offset.
- **A full turn, not `360/waves`.** The wave is n-fold symmetric, so the *shape* returns after
  `360/waves` - but the dash would only have slid `100/waves` by then. Only the whole turn closes both.
- **The sweep is a registered custom property.** `@property --_ring-sweep { syntax: '<number>' }`
  animates 0 -> 100 and `stroke-dashoffset` is `calc(var(--_ring-lead) + var(--_ring-sweep))`, so the
  offset recomputes each frame instead of snapping.
- **`transform-box: view-box` + `transform-origin: 22px 22px`** for the true centre.
- **One cycle is one lap**, so the duration is `wave-speed x ring-waves`: the ring and the linear bar
  then pulse at the same rate off a single theme token, with no new token for the ring.

## What "stays put" means, measured

Rotation is linear in angle; arc length is not, because the path runs slightly long over a crest and
short through a trough. Sampled across a cycle in the Gallery, the visible arc's endpoints move by
**0.77 deg at the start and 0.63 deg at the end** - about a quarter of a percent of the circumference,
a fraction of a pixel at the rendered size. It cancels every `360/waves` rather than accumulating, so
the arc breathes imperceptibly instead of drifting.

## Guards

`C_FlareProgressWavyTests` asserts the dash array has exactly two values summing to 100 (at 0/25/60/100)
and that `--_ring-lead` is published and negative. A regression would not throw or fail to render - it
would only come apart while animating.
